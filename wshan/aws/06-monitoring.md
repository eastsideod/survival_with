# AWS 모니터링 설정

## 1. CloudWatch 설정

### CloudWatch 에이전트 설치
```bash
# CloudWatch 네임스페이스 생성
kubectl apply -f https://raw.githubusercontent.com/aws-samples/amazon-cloudwatch-container-insights/latest/k8s-deployment-manifest-templates/deployment-mode/daemonset/container-insights-monitoring/cloudwatch-namespace.yaml

# IAM 역할 설정
eksctl create iamserviceaccount \
  --name cloudwatch-agent \
  --namespace amazon-cloudwatch \
  --cluster game-cluster \
  --attach-policy-arn arn:aws:iam::aws:policy/CloudWatchAgentServerPolicy \
  --approve
```

### CloudWatch 에이전트 설정
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: cloudwatch-agent-config
  namespace: amazon-cloudwatch
data:
  cloudwatch-agent.json: |
    {
      "logs": {
        "metrics_collected": {
          "kubernetes": {
            "metrics_collection_interval": 60,
            "cluster_name": "game-cluster"
          }
        }
      }
    }
```

## 2. Prometheus 연동

### Prometheus 설치
```bash
# Prometheus 네임스페이스 생성
kubectl create namespace monitoring

# Prometheus 설치
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm install prometheus prometheus-community/prometheus \
  --namespace monitoring \
  --set server.persistentVolume.storageClass=game-server-storage
```

### ServiceMonitor 설정
```yaml
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: game-server-monitor
  namespace: monitoring
spec:
  selector:
    matchLabels:
      app: game-server
  endpoints:
  - port: metrics
    interval: 15s
```

## 3. Grafana 대시보드

### Grafana 설치
```bash
# Grafana 설치
helm repo add grafana https://grafana.github.io/helm-charts
helm install grafana grafana/grafana \
  --namespace monitoring \
  --set persistence.storageClassName=game-server-storage
```

### 대시보드 설정
```yaml
apiVersion: integreatly.org/v1alpha1
kind: GrafanaDashboard
metadata:
  name: game-server-dashboard
  namespace: monitoring
spec:
  json: |
    {
      "dashboard": {
        "title": "Game Server Dashboard",
        "panels": [
          {
            "title": "Game Server Count",
            "type": "graph",
            "datasource": "Prometheus",
            "targets": [
              {
                "expr": "count(agones_gameservers_count)",
                "legendFormat": "Total Servers"
              }
            ]
          },
          {
            "title": "Player Count",
            "type": "graph",
            "datasource": "Prometheus",
            "targets": [
              {
                "expr": "sum(agones_gameservers_players_count)",
                "legendFormat": "Total Players"
              }
            ]
          }
        ]
      }
    }
```

## 4. X-Ray 설정

### X-Ray 데몬 설치
```bash
# X-Ray 네임스페이스 생성
kubectl create namespace xray

# X-Ray 데몬 설치
kubectl apply -f https://raw.githubusercontent.com/aws-samples/amazon-xray-kubernetes/master/xray-daemon.yaml
```

### X-Ray 추적 설정
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: game-server
spec:
  template:
    spec:
      containers:
      - name: game-server
        image: game-server:latest
        env:
        - name: AWS_XRAY_DAEMON_ADDRESS
          value: xray-service.xray:2000
```

## 5. CloudTrail 설정

### CloudTrail 로그 설정
```bash
# CloudTrail 트레일 생성
aws cloudtrail create-trail \
  --name game-server-trail \
  --s3-bucket-name game-server-logs \
  --is-multi-region-trail

# 로그 파일 검증 활성화
aws cloudtrail update-trail \
  --name game-server-trail \
  --enable-log-file-validation
```

## 6. 문제 해결

### CloudWatch 로그 확인
```bash
# 로그 그룹 확인
aws logs describe-log-groups \
  --log-group-name-prefix /aws/containerinsights/game-cluster

# 로그 스트림 확인
aws logs describe-log-streams \
  --log-group-name /aws/containerinsights/game-cluster/application
```

### Prometheus 상태 확인
```bash
# Prometheus 서버 상태 확인
kubectl get pods -n monitoring -l app=prometheus-server

# ServiceMonitor 상태 확인
kubectl get servicemonitor -n monitoring
```

### Grafana 상태 확인
```bash
# Grafana 서버 상태 확인
kubectl get pods -n monitoring -l app.kubernetes.io/name=grafana

# 대시보드 상태 확인
kubectl get grafanadashboard -n monitoring
``` 