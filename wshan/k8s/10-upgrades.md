# Kubernetes 업그레이드

## 1. 쿠버네티스 버전 관리
### 1.1 버전 체계
- Major.Minor.Patch 형식 (예: 1.28.3)
- Major: 주요 변경사항, 호환성 없는 변경
- Minor: 기능 추가, 호환성 있는 변경
- Patch: 버그 수정, 보안 패치

### 1.2 지원 정책
- 최신 3개의 Minor 버전 지원
- 각 Minor 버전은 약 1년간 지원
- 보안 패치는 최신 3개 Minor 버전에 적용

## 2. 업그레이드 전략
### 2.1 클러스터 업그레이드
#### 2.1.1 단계적 업그레이드
1. Control Plane 업그레이드
   - API 서버
   - Controller Manager
   - Scheduler
   - etcd

2. Worker Node 업그레이드
   - kubelet
   - kube-proxy
   - 컨테이너 런타임

#### 2.1.2 업그레이드 순서
1. etcd 업그레이드
2. Control Plane 컴포넌트 업그레이드
3. Worker Node 업그레이드
4. 애플리케이션 업그레이드

### 2.2 업그레이드 방법
#### 2.2.1 수동 업그레이드
```bash
# Control Plane 업그레이드
kubeadm upgrade plan
kubeadm upgrade apply v1.28.3

# Worker Node 업그레이드
kubeadm upgrade node
systemctl restart kubelet
```

#### 2.2.2 자동 업그레이드
- 클라우드 제공자의 관리형 쿠버네티스 사용
- 자동 패치 적용
- 예약된 업그레이드 설정

## 3. 애플리케이션 업그레이드
### 3.1 Deployment 업그레이드
#### 3.1.1 롤링 업데이트
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
        image: nginx:1.25.3
```

#### 3.1.2 재생성 업데이트
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: nginx-deployment
spec:
  strategy:
    type: Recreate
  template:
    spec:
      containers:
      - name: nginx
        image: nginx:1.25.3
```

### 3.2 StatefulSet 업그레이드
```yaml
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: web
spec:
  updateStrategy:
    type: RollingUpdate
    rollingUpdate:
      partition: 1
  template:
    spec:
      containers:
      - name: nginx
        image: nginx:1.25.3
```

## 4. 업그레이드 준비
### 4.1 사전 검사
```bash
# 클러스터 상태 확인
kubectl get nodes
kubectl get pods --all-namespaces

# API 서버 버전 확인
kubectl version

# etcd 상태 확인
kubectl get pods -n kube-system -l component=etcd
```

### 4.2 백업
```bash
# etcd 백업
ETCDCTL_API=3 etcdctl snapshot save snapshot.db

# 리소스 백업
kubectl get all --all-namespaces -o yaml > cluster-backup.yaml
```

## 5. 업그레이드 후 검증
### 5.1 기본 검증
```bash
# 노드 상태 확인
kubectl get nodes

# Pod 상태 확인
kubectl get pods --all-namespaces

# 서비스 확인
kubectl get services --all-namespaces
```

### 5.2 애플리케이션 검증
```bash
# Deployment 상태 확인
kubectl rollout status deployment/<deployment-name>

# Pod 로그 확인
kubectl logs <pod-name>

# 서비스 엔드포인트 확인
kubectl get endpoints <service-name>
```

## 6. 롤백 절차
### 6.1 클러스터 롤백
```bash
# etcd 복원
ETCDCTL_API=3 etcdctl snapshot restore snapshot.db

# Worker Node 롤백
kubeadm downgrade
systemctl restart kubelet
```

### 6.2 애플리케이션 롤백
```bash
# Deployment 롤백
kubectl rollout undo deployment/<deployment-name>

# StatefulSet 롤백
kubectl rollout undo statefulset/<statefulset-name>
```

## 7. 문제 해결
### 7.1 일반적인 문제
- 업그레이드 실패
  - 로그 확인
  - 이전 버전으로 롤백
  - 문제 해결 후 재시도

- 애플리케이션 오류
  - Pod 상태 확인
  - 로그 분석
  - 롤백 고려

### 7.2 디버깅 명령어
```bash
# 업그레이드 이벤트 확인
kubectl get events --sort-by='.lastTimestamp'

# 컴포넌트 로그 확인
kubectl logs -n kube-system <component-pod>

# API 서버 상태 확인
kubectl get --raw='/readyz?verbose'
``` 