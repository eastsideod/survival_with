# EKS 설정

## EKS 클러스터 생성

### 1. 사전 요구사항
- AWS CLI 설치 및 구성
- eksctl 설치
- kubectl 설치
- IAM 사용자 권한 설정

### 2. 클러스터 생성 명령어
```bash
eksctl create cluster \
  --name game-cluster \
  --region ap-northeast-2 \
  --node-type t3.large \
  --nodes 3 \
  --nodes-min 3 \
  --nodes-max 5 \
  --managed
```

### 3. 클러스터 구성 확인
```bash
# 클러스터 상태 확인
eksctl get cluster --name game-cluster

# 노드 상태 확인
kubectl get nodes

# 클러스터 정보 확인
kubectl cluster-info
```

## 노드 그룹 설정

### 1. 게임 서버용 노드 그룹
```yaml
apiVersion: eksctl.io/v1alpha5
kind: ClusterConfig
metadata:
  name: game-cluster
  region: ap-northeast-2
nodeGroups:
  - name: game-nodes
    instanceType: t3.large
    desiredCapacity: 3
    minSize: 3
    maxSize: 5
    labels:
      role: game-server
    taints:
      game-server: "true:NoSchedule"
```

### 2. 시스템용 노드 그룹
```yaml
  - name: system-nodes
    instanceType: t3.medium
    desiredCapacity: 2
    minSize: 2
    maxSize: 3
    labels:
      role: system
```

## 네트워크 구성

### 1. VPC 설정
```yaml
vpc:
  cidr: 10.0.0.0/16
  subnets:
    public:
      ap-northeast-2a: { cidr: 10.0.0.0/20 }
      ap-northeast-2b: { cidr: 10.0.16.0/20 }
      ap-northeast-2c: { cidr: 10.0.32.0/20 }
    private:
      ap-northeast-2a: { cidr: 10.0.64.0/20 }
      ap-northeast-2b: { cidr: 10.0.80.0/20 }
      ap-northeast-2c: { cidr: 10.0.96.0/20 }
```

### 2. 보안 그룹 설정
```yaml
securityGroups:
  - name: game-server-sg
    description: Game server security group
    rules:
      - type: ingress
        fromPort: 7000
        toPort: 8000
        protocol: tcp
        cidrBlocks: ["0.0.0.0/0"]
      - type: egress
        fromPort: 0
        toPort: 0
        protocol: "-1"
        cidrBlocks: ["0.0.0.0/0"]
```

## 스토리지 설정

### 1. EBS CSI 드라이버 설치
```bash
eksctl utils associate-iam-oidc-provider \
  --cluster game-cluster \
  --approve

eksctl create iamserviceaccount \
  --name ebs-csi-controller-sa \
  --namespace kube-system \
  --cluster game-cluster \
  --attach-policy-arn arn:aws:iam::aws:policy/service-role/AmazonEBSCSIDriverPolicy \
  --approve \
  --role-only \
  --role-name AmazonEKS_EBS_CSI_DriverRole
```

### 2. 스토리지 클래스 설정
```yaml
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: game-server-storage
provisioner: ebs.csi.aws.com
parameters:
  type: gp3
  iopsPerGB: "3000"
  throughput: "125"
volumeBindingMode: WaitForFirstConsumer
```

## 모니터링 설정

### 1. CloudWatch 에이전트 설치
```bash
# CloudWatch 에이전트 설치
kubectl apply -f https://raw.githubusercontent.com/aws-samples/amazon-cloudwatch-container-insights/latest/k8s-deployment-manifest-templates/deployment-mode/daemonset/container-insights-monitoring/cloudwatch-namespace.yaml

# 서비스 계정 생성
eksctl create iamserviceaccount \
  --name cloudwatch-agent \
  --namespace amazon-cloudwatch \
  --cluster game-cluster \
  --attach-policy-arn arn:aws:iam::aws:policy/CloudWatchAgentServerPolicy \
  --approve
```

### 2. Prometheus 연동
```yaml
apiVersion: monitoring.coreos.com/v1
kind: ServiceMonitor
metadata:
  name: eks-monitor
  namespace: monitoring
spec:
  selector:
    matchLabels:
      app: game-server
  endpoints:
  - port: metrics
    interval: 15s
```

## 문제 해결

### 1. 일반적인 문제
- 노드 연결 문제
  ```bash
  # 노드 상태 확인
  kubectl describe node <node-name>
  
  # 노드 로그 확인
  kubectl logs -n kube-system <node-problem-detector-pod>
  ```

- 네트워크 문제
  ```bash
  # 네트워크 정책 확인
  kubectl get networkpolicy --all-namespaces
  
  # VPC 흐름 로그 확인
  aws ec2 describe-flow-logs --filter "Name=resource-id,Values=<vpc-id>"
  ```

### 2. 성능 최적화
- 노드 자동 스케일링
  ```yaml
  apiVersion: autoscaling.openshift.io/v1
  kind: ClusterAutoscaler
  metadata:
    name: default
  spec:
    resourceLimits:
      maxNodesTotal: 10
    scaleDown:
      enabled: true
      delayAfterAdd: 10m
      delayAfterDelete: 10m
      delayAfterFailure: 3m
  ```

- 리소스 제한 설정
  ```yaml
  apiVersion: v1
  kind: LimitRange
  metadata:
    name: game-server-limits
  spec:
    limits:
    - type: Container
      max:
        cpu: "2"
        memory: "4Gi"
      min:
        cpu: "500m"
        memory: "512Mi"
  ``` 