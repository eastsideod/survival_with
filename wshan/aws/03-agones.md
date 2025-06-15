# EKS에 Agones 설치

## 1. 사전 요구사항

### IAM 역할 설정
```bash
# Agones 컨트롤러용 IAM 역할 생성
eksctl create iamserviceaccount \
  --name agones-controller \
  --namespace agones-system \
  --cluster game-cluster \
  --attach-policy-arn arn:aws:iam::aws:policy/AmazonEKSClusterPolicy \
  --approve
```

### 네임스페이스 생성
```bash
kubectl create namespace agones-system
```

## 2. Helm을 통한 Agones 설치

### Helm 저장소 추가
```bash
helm repo add agones https://agones.dev/chart/stable
helm repo update
```

### Agones 설치
```bash
helm install agones agones/agones \
  --namespace agones-system \
  --set agones.controller.serviceAccount.create=false \
  --set agones.controller.serviceAccount.name=agones-controller \
  --set agones.crds.install=true \
  --set agones.ping.http.enabled=true \
  --set agones.ping.udp.enabled=true
```

## 3. GameServer 설정

### 기본 GameServer 설정
```yaml
apiVersion: "agones.dev/v1"
kind: GameServer
metadata:
  name: simple-game-server
  labels:
    game-type: fps
    region: asia-northeast-2
spec:
  ports:
  - name: game-port
    portPolicy: Dynamic
    containerPort: 7654
    protocol: UDP
  - name: query-port
    portPolicy: Dynamic
    containerPort: 7655
    protocol: TCP
  template:
    spec:
      containers:
      - name: simple-game-server
        image: gcr.io/agones-images/simple-game-server:0.1
        resources:
          requests:
            memory: "64Mi"
            cpu: "500m"
          limits:
            memory: "128Mi"
            cpu: "1"
        env:
        - name: MAX_PLAYERS
          value: "16"
        - name: MATCH_TIMEOUT
          value: "300"
        - name: GAME_MODE
          value: "team-deathmatch"
```

### AWS 환경을 위한 GameServer 설정
```yaml
apiVersion: "agones.dev/v1"
kind: GameServer
metadata:
  name: aws-game-server
  labels:
    game-type: moba
    region: ap-northeast-2
    version: "v1.2.0"
spec:
  ports:
  - name: game-port
    portPolicy: Dynamic
    containerPort: 7777
    protocol: UDP
  - name: api-port
    portPolicy: Dynamic
    containerPort: 8080
    protocol: TCP
  - name: metrics-port
    portPolicy: Static
    containerPort: 9090
    protocol: TCP
  template:
    spec:
      serviceAccountName: game-server-sa
      nodeSelector:
        node.kubernetes.io/instance-type: "c5.xlarge"
      containers:
      - name: game-server
        image: <account-id>.dkr.ecr.ap-northeast-2.amazonaws.com/game-server:latest
        resources:
          requests:
            memory: "1Gi"
            cpu: "500m"
          limits:
            memory: "2Gi"
            cpu: "1000m"
        env:
        - name: AWS_REGION
          value: "ap-northeast-2"
        - name: AWS_DEFAULT_REGION
          value: "ap-northeast-2"
        - name: AGONES_SDK_GRPC_PORT
          value: "9357"
        - name: AGONES_SDK_HTTP_PORT
          value: "9358"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /ready
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
```

### 상태 저장 GameServer 설정
```yaml
apiVersion: "agones.dev/v1"
kind: GameServer
metadata:
  name: stateful-game-server
spec:
  ports:
  - name: default
    portPolicy: Dynamic
    containerPort: 7654
  template:
    spec:
      containers:
      - name: stateful-game-server
        image: gcr.io/agones-images/stateful-game-server:0.1
        volumeMounts:
        - name: game-data
          mountPath: /data
      volumes:
      - name: game-data
        persistentVolumeClaim:
          claimName: game-data-pvc
```

## 4. Fleet 설정

### 기본 Fleet 설정
```yaml
apiVersion: "agones.dev/v1"
kind: Fleet
metadata:
  name: simple-game-fleet
  labels:
    game-type: fps
spec:
  replicas: 3
  template:
    metadata:
      labels:
        game-type: fps
    spec:
      ports:
      - name: game-port
        portPolicy: Dynamic
        containerPort: 7654
        protocol: UDP
      - name: query-port
        portPolicy: Dynamic
        containerPort: 7655
        protocol: TCP
      template:
        spec:
          containers:
          - name: simple-game-server
            image: gcr.io/agones-images/simple-game-server:0.1
```

### AWS EKS에서의 다중 리전 Fleet 설정
```yaml
# 서울 리전 게임 Fleet
apiVersion: "agones.dev/v1"
kind: Fleet
metadata:
  name: game-fleet-ap-northeast-2
  labels:
    game-type: fps
    region: ap-northeast-2
spec:
  replicas: 10
  scheduling: Packed
  template:
    metadata:
      labels:
        game-type: fps
        region: ap-northeast-2
      annotations:
        cluster.autoscaler.kubernetes.io/safe-to-evict: "false"
    spec:
      ports:
      - name: game-port
        portPolicy: Dynamic
        containerPort: 7777
        protocol: UDP
      template:
        spec:
          serviceAccountName: game-server-sa
          nodeSelector:
            kubernetes.io/arch: amd64
            node.kubernetes.io/instance-type: c5.large
          affinity:
            nodeAffinity:
              requiredDuringSchedulingIgnoredDuringExecution:
                nodeSelectorTerms:
                - matchExpressions:
                  - key: topology.kubernetes.io/zone
                    operator: In
                    values:
                    - ap-northeast-2a
                    - ap-northeast-2b
                    - ap-northeast-2c
          containers:
          - name: game-server
            image: <account-id>.dkr.ecr.ap-northeast-2.amazonaws.com/fps-server:latest
            resources:
              requests:
                memory: "2Gi"
                cpu: "1000m"
              limits:
                memory: "4Gi"
                cpu: "2000m"
            env:
            - name: AWS_REGION
              value: "ap-northeast-2"
            - name: CLOUDWATCH_LOG_GROUP
              value: "/aws/eks/game-cluster/game-server"
```

### 자동 스케일링 Fleet 설정
```yaml
apiVersion: "autoscaling.agones.dev/v1"
kind: FleetAutoscaler
metadata:
  name: simple-game-fleet-autoscaler
spec:
  fleetName: simple-game-fleet
  policy:
    type: Buffer
    buffer:
      bufferSize: 2
      minReplicas: 3
      maxReplicas: 10
```

## 5. 네트워크 설정

### 로드 밸런서 설정
```yaml
apiVersion: v1
kind: Service
metadata:
  name: game-server-lb
spec:
  type: LoadBalancer
  ports:
  - port: 80
    targetPort: 7654
    protocol: TCP
  selector:
    agones.dev/gameserver: "true"
```

### 네트워크 정책 설정
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: game-server-policy
spec:
  podSelector:
    matchLabels:
      agones.dev/gameserver: "true"
  ingress:
  - from:
    - namespaceSelector:
        matchLabels:
          role: game-client
    ports:
    - protocol: TCP
      port: 7654
```

## 6. 모니터링 설정

### Prometheus 설정
```yaml
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: agones-monitor
  namespace: monitoring
spec:
  selector:
    matchLabels:
      app: agones
  endpoints:
  - port: metrics
    interval: 15s
```

### CloudWatch 설정
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: cloudwatch-agent-config
  namespace: agones-system
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

## 7. 문제 해결

### 일반적인 문제
```bash
# Agones 컨트롤러 로그 확인
kubectl logs -n agones-system -l app=agones-controller

# GameServer 상태 확인
kubectl describe gameserver <gameserver-name>

# Fleet 상태 확인
kubectl describe fleet <fleet-name>
```

### 성능 최적화
```yaml
apiVersion: v1
kind: ResourceQuota
metadata:
  name: game-server-quota
  namespace: default
spec:
  hard:
    pods: "50"
    services: "20"
    configmaps: "20"
    persistentvolumeclaims: "20"
``` 