# Kubernetes 기본 개념

## 1. Kubernetes란?
- 컨테이너화된 애플리케이션의 배포, 확장, 관리를 자동화하는 오픈소스 플랫폼
- 구글에서 개발하여 CNCF(Cloud Native Computing Foundation)에 기증
- 컨테이너 오케스트레이션의 사실상 표준

## 2. 주요 특징
- 자동화된 배포와 스케일링
- 서비스 디스커버리와 로드 밸런싱
- 스토리지 오케스트레이션
- 자동화된 롤아웃과 롤백
- 시크릿과 설정 관리
- 컨테이너 상태 모니터링

## 3. 기본 아키텍처
### 3.1 Control Plane (마스터 노드)
- API Server: 클러스터의 외부 인터페이스
- Scheduler: 파드를 노드에 할당
- Controller Manager: 클러스터 상태 관리
- etcd: 클러스터 데이터 저장소

### 3.2 Worker Node
- kubelet: 노드에서 파드 실행 관리
- kube-proxy: 네트워크 프록시
- Container Runtime: 컨테이너 실행 엔진

## 4. 기본 리소스
- Pod: 가장 작은 배포 단위
- Deployment: 파드의 배포와 업데이트 관리
- Service: 파드 집합에 대한 네트워크 엔드포인트
- ConfigMap: 설정 데이터 저장
- Secret: 민감한 정보 저장
- Volume: 영구 스토리지

## 5. 기본 명령어
```bash
# 클러스터 정보 확인
kubectl cluster-info

# 노드 목록 확인
kubectl get nodes

# 파드 목록 확인
kubectl get pods

# 서비스 목록 확인
kubectl get services

# 리소스 상세 정보 확인
kubectl describe <resource-type> <resource-name>

# 리소스 생성
kubectl apply -f <yaml-file>

# 리소스 삭제
kubectl delete -f <yaml-file>
``` 