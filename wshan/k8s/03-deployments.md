# Deployments

## 1. Deployment란?
- Pod와 ReplicaSet을 관리하는 상위 레벨 리소스
- 애플리케이션의 배포와 업데이트를 관리
- 롤링 업데이트와 롤백 지원
- 스케일링 관리

## 2. ReplicaSet
### 2.1 ReplicaSet이란?
- 지정된 수의 Pod 복제본을 유지하는 컨트롤러
- Pod의 가용성과 확장성을 보장
- Pod 셀렉터를 통해 관리 대상 Pod 식별
- Deployment의 하위 리소스로 주로 사용

### 2.2 ReplicaSet의 주요 기능
- Pod 복제본 수 유지
- Pod 상태 모니터링
- 실패한 Pod 자동 교체
- 스케일링 지원

### 2.3 Selector와 Label 매칭
#### 2.3.1 Selector
- ReplicaSet이 관리할 Pod를 식별하는 방법
- Pod의 레이블과 매칭되는 규칙 정의
- matchLabels와 matchExpressions 두 가지 방식 지원

#### 2.3.2 matchLabels
- 정확한 레이블 키-값 쌍으로 매칭
- 단순하고 직관적인 매칭 방식
- 예시:
```yaml
selector:
  matchLabels:
    app: nginx
    tier: frontend
```
위의 경우 `app=nginx`와 `tier=frontend` 레이블을 모두 가진 Pod만 매칭

#### 2.3.3 matchExpressions
- 더 복잡한 매칭 조건 사용 가능
- 연산자: In, NotIn, Exists, DoesNotExist
- 예시:
```yaml
selector:
  matchExpressions:
    - {key: tier, operator: In, values: [frontend, backend]}
    - {key: environment, operator: NotIn, values: [dev]}
```

#### 2.3.4 ReplicaSet의 Pod 관리 방식
1. 레이블 기반 Pod 선택:
```yaml
# ReplicaSet 정의
apiVersion: apps/v1
kind: ReplicaSet
metadata:
  name: frontend
spec:
  replicas: 3
  selector:
    matchLabels:
      app: nginx
      tier: frontend
  template:
    metadata:
      labels:
        app: nginx
        tier: frontend
```

2. Pod 관리 로직:
   - ReplicaSet은 selector와 매칭되는 Pod만 관리
   - 예: `app=nginx`와 `tier=frontend` 레이블을 가진 Pod만 관리
   - 다른 레이블을 가진 Pod는 무시 (예: `app=nginx`, `tier=backend`)

3. 복제본 수 유지:
   - 매칭되는 Pod가 replicas(3)보다 적으면 새 Pod 생성
   - 매칭되는 Pod가 replicas(3)보다 많으면 과잉 Pod 삭제
   - 항상 정확히 3개의 매칭 Pod 유지

4. 실제 예시:
```yaml
# ReplicaSet이 관리하는 Pod
apiVersion: v1
kind: Pod
metadata:
  name: nginx-frontend-1
  labels:
    app: nginx
    tier: frontend  # ReplicaSet의 selector와 매칭

# ReplicaSet이 관리하지 않는 Pod
apiVersion: v1
kind: Pod
metadata:
  name: nginx-backend-1
  labels:
    app: nginx
    tier: backend   # ReplicaSet의 selector와 불일치
```

### 2.4 ReplicaSet 예시
```yaml
apiVersion: apps/v1
kind: ReplicaSet
metadata:
  name: frontend
  labels:
    app: guestbook
    tier: frontend
spec:
  replicas: 3
  selector:
    matchLabels:
      tier: frontend
  template:
    metadata:
      labels:
        tier: frontend
    spec:
      containers:
      - name: php-redis
        image: gcr.io/google_samples/gb-frontend:v3
```

### 2.4 ReplicaSet 관리 명령어
```bash
# ReplicaSet 목록 확인
kubectl get rs

# ReplicaSet 상세 정보 확인
kubectl describe rs <replicaset-name>

# ReplicaSet 스케일링
kubectl scale rs <replicaset-name> --replicas=5

# ReplicaSet 삭제
kubectl delete rs <replicaset-name>
```

## 3. Deployment의 주요 기능
- 선언적 업데이트
- 롤링 업데이트
- 롤백
- 스케일링
- 상태 모니터링

## 4. 기본 Deployment 예시
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

## 5. Deployment 전략
### 5.1 롤링 업데이트
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

### 5.2 재생성(Recreate) 전략
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

## 6. Deployment 관리 명령어
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

## 7. 실습 예제
### 7.1 다중 컨테이너 Deployment
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

### 7.2 리소스 제한이 있는 Deployment
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

## 8. 문제 해결
### 8.1 일반적인 문제
- Deployment가 업데이트되지 않는 경우
  - 이미지 태그 확인
  - 리소스 제한 확인
  - 노드 리소스 확인

- Pod가 생성되지 않는 경우
  - ReplicaSet 상태 확인
  - 이벤트 로그 확인
  - 노드 상태 확인

### 8.2 디버깅 명령어
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