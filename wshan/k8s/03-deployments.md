# Deployments

## 1. Deployment란?
- Pod와 ReplicaSet을 관리하는 상위 레벨 리소스
- 애플리케이션의 배포와 업데이트를 관리
- 롤링 업데이트와 롤백 지원
- 스케일링 관리

## 2. Deployment의 주요 기능
- 선언적 업데이트
- 롤링 업데이트
- 롤백
- 스케일링
- 상태 모니터링

## 3. 기본 Deployment 예시
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: nginx-deployment
  labels:
    app: nginx
spec:
  replicas: 3
  selector:
    matchLabels:
      app: nginx
  template:
    metadata:
      labels:
        app: nginx
    spec:
      containers:
      - name: nginx
        image: nginx:1.14.2
        ports:
        - containerPort: 80
```

## 4. Deployment 전략
### 4.1 롤링 업데이트
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
  replicas: 3
  template:
    spec:
      containers:
      - name: nginx
        image: nginx:1.14.2
```

### 4.2 재생성(Recreate) 전략
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: nginx-deployment
spec:
  strategy:
    type: Recreate
  replicas: 3
  template:
    spec:
      containers:
      - name: nginx
        image: nginx:1.14.2
```

## 5. Deployment 관리 명령어
```bash
# Deployment 생성
kubectl apply -f deployment.yaml

# Deployment 목록 확인
kubectl get deployments

# Deployment 상세 정보 확인
kubectl describe deployment <deployment-name>

# Deployment 스케일링
kubectl scale deployment <deployment-name> --replicas=5

# Deployment 업데이트
kubectl set image deployment/<deployment-name> nginx=nginx:1.16.1

# Deployment 롤백
kubectl rollout undo deployment/<deployment-name>

# Deployment 상태 확인
kubectl rollout status deployment/<deployment-name>

# Deployment 이력 확인
kubectl rollout history deployment/<deployment-name>
```

## 6. 실습 예제
### 6.1 다중 컨테이너 Deployment
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: multi-container
spec:
  replicas: 2
  selector:
    matchLabels:
      app: multi-container
  template:
    metadata:
      labels:
        app: multi-container
    spec:
      containers:
      - name: nginx
        image: nginx:1.14.2
        ports:
        - containerPort: 80
      - name: redis
        image: redis:5.0.4
        ports:
        - containerPort: 6379
```

### 6.2 리소스 제한이 있는 Deployment
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: resource-limited
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: nginx
        image: nginx:1.14.2
        resources:
          limits:
            cpu: "0.5"
            memory: "512Mi"
          requests:
            cpu: "0.25"
            memory: "256Mi"
```

## 7. 문제 해결
### 7.1 일반적인 문제
- Deployment가 업데이트되지 않는 경우
  - 이미지 태그 확인
  - 리소스 제한 확인
  - 노드 리소스 확인

- Pod가 생성되지 않는 경우
  - ReplicaSet 상태 확인
  - 이벤트 로그 확인
  - 노드 상태 확인

### 7.2 디버깅 명령어
```bash
# Pod 이벤트 확인
kubectl describe pod <pod-name>

# Deployment 이벤트 확인
kubectl describe deployment <deployment-name>

# Pod 로그 확인
kubectl logs <pod-name>

# 이전 버전으로 롤백
kubectl rollout undo deployment/<deployment-name> --to-revision=2
``` 