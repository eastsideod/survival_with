# Kubernetes Best Practices

## 1. 리소스 관리
### 1.1 리소스 요청과 제한
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: frontend
spec:
  containers:
  - name: app
    image: images.my-company.example/app:v4
    resources:
      requests:
        memory: "64Mi"
        cpu: "250m"
      limits:
        memory: "128Mi"
        cpu: "500m"
```

### 1.2 Quality of Service (QoS)
- Guaranteed: requests = limits
- Burstable: requests < limits
- BestEffort: requests와 limits 없음

## 2. 레이블과 어노테이션
### 2.1 레이블 사용
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: nginx-deployment
  labels:
    app: nginx
    environment: production
    tier: frontend
spec:
  replicas: 3
  selector:
    matchLabels:
      app: nginx
  template:
    metadata:
      labels:
        app: nginx
        environment: production
        tier: frontend
```

### 2.2 어노테이션 사용
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: nginx-deployment
  annotations:
    description: "Production Nginx deployment"
    last-updated: "2023-01-01"
```

## 3. 상태 관리
### 3.1 Health Checks
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: goproxy
spec:
  containers:
  - name: goproxy
    image: k8s.gcr.io/goproxy:0.1
    ports:
    - containerPort: 8080
    readinessProbe:
      tcpSocket:
        port: 8080
      initialDelaySeconds: 5
      periodSeconds: 10
    livenessProbe:
      tcpSocket:
        port: 8080
      initialDelaySeconds: 15
      periodSeconds: 20
```

### 3.2 Pod Disruption Budget
```yaml
apiVersion: policy/v1
kind: PodDisruptionBudget
metadata:
  name: zk-pdb
spec:
  minAvailable: 2
  selector:
    matchLabels:
      app: zookeeper
```

## 4. 배포 전략
### 4.1 Rolling Updates
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: nginx-deployment
spec:
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0
  template:
    spec:
      containers:
      - name: nginx
        image: nginx:1.14.2
```

### 4.2 Blue-Green 배포
```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-app
  labels:
    app: my-app
spec:
  type: NodePort
  ports:
  - port: 80
    targetPort: 80
  selector:
    app: my-app
    version: v1.0.0
```

## 5. 모니터링과 로깅
### 5.1 리소스 모니터링
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: php-apache
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: php-apache
  minReplicas: 1
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 50
```

### 5.2 로깅 설정
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: counter
spec:
  containers:
  - name: count
    image: busybox
    args: [/bin/sh, -c,
            'i=0; while true; do echo "$i: $(date)"; i=$((i+1)); sleep 1; done']
```

## 6. 네트워킹
### 6.1 네트워크 정책
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: default-deny
spec:
  podSelector: {}
  policyTypes:
  - Ingress
  - Egress
```

### 6.2 서비스 디스커버리
```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  selector:
    app: MyApp
  ports:
  - protocol: TCP
    port: 80
    targetPort: 9376
```

## 7. 스토리지
### 7.1 영구 볼륨
```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: my-pvc
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 1Gi
  storageClassName: standard
```

### 7.2 스토리지 클래스
```yaml
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: standard
provisioner: kubernetes.io/aws-ebs
parameters:
  type: gp2
reclaimPolicy: Retain
allowVolumeExpansion: true
```

## 8. 보안
### 8.1 RBAC
```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  namespace: default
  name: pod-reader
rules:
- apiGroups: [""]
  resources: ["pods"]
  verbs: ["get", "watch", "list"]
```

### 8.2 Pod 보안
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: security-context-demo
spec:
  securityContext:
    runAsUser: 1000
    runAsGroup: 3000
    fsGroup: 2000
  containers:
  - name: sec-ctx-demo
    image: busybox
    securityContext:
      allowPrivilegeEscalation: false
```

## 9. 관리 명령어
```bash
# 리소스 사용량 확인
kubectl top nodes
kubectl top pods

# 이벤트 확인
kubectl get events

# 로그 확인
kubectl logs <pod-name>

# 리소스 상태 확인
kubectl describe <resource-type> <resource-name>
```

## 10. 문제 해결
### 10.1 일반적인 문제
- Pod가 시작되지 않는 경우
  - 이벤트 로그 확인
  - 리소스 제한 확인
  - 이미지 풀 정책 확인

- 서비스에 연결할 수 없는 경우
  - 엔드포인트 확인
  - 네트워크 정책 확인
  - 서비스 타입 확인

### 10.2 디버깅 명령어
```bash
# Pod 상세 정보 확인
kubectl describe pod <pod-name>

# 컨테이너 로그 확인
kubectl logs <pod-name> -c <container-name>

# 네트워크 연결 테스트
kubectl exec -it <pod-name> -- curl <service-name>:<port>
``` 