
# 📘 Kubernetes 이론 완전 정리 (초급 → 고급)

---

## 🟢 1단계: 컨테이너와 Kubernetes의 필요성

### ✅ 왜 컨테이너?
- 애플리케이션 + 실행 환경을 묶어서 어디서나 실행 가능
- `Docker`로 이미지 빌드 → 배포가 쉬워짐

### ✅ 컨테이너의 문제점
- 컨테이너가 많아지면 수동 관리가 불가능
- 자동 배포, 복구, 확장이 필요

### ✅ Kubernetes의 등장
- Google이 개발한 컨테이너 **오케스트레이션 플랫폼**
- 선언형으로 Pod 관리, 셀프힐링, 오토스케일링, 로드밸런싱 지원

---

## 🟡 2단계: Kubernetes 기본 개념

### 🔹 주요 오브젝트

| 오브젝트 | 설명 |
|----------|------|
| Pod | 컨테이너 1개 이상의 실행 단위 |
| Deployment | Pod를 관리 (롤링 업데이트, 복제 등) |
| Service | Pod에 대한 고정 IP 제공 |
| Namespace | 리소스를 격리하는 논리 공간 |
| ConfigMap / Secret | 설정과 민감 정보 관리 |
| Ingress | 외부 HTTP 트래픽을 내부 서비스로 라우팅 |

---

## 🔵 3단계: Kubernetes 핵심 아키텍처

### ✅ 구조 요약

```
사용자 (kubectl)
    ↓
API Server
 ├─ Scheduler
 ├─ Controller Manager
 └─ etcd
    ↓
Worker Node
 ├─ Kubelet
 ├─ kube-proxy
 └─ Pod (컨테이너)
```

### 🔹 Control Plane
- API Server: 명령 수신
- Scheduler: Pod 배치 결정
- Controller Manager: 상태 유지 관리
- etcd: 상태 저장소

### 🔹 Node
- Kubelet: Pod 실행 및 보고
- kube-proxy: 네트워크 라우팅
- Container Runtime: 컨테이너 실행기 (Docker, containerd)

---

## 🟣 4단계: 선언형 운영 & 컨트롤러 패턴

- 사용자는 **원하는 상태(Desired State)** 를 YAML로 선언
- 컨트롤러는 현재 상태를 계속 감시 → 차이가 있으면 자동으로 맞춤
- 예: Deployment에 `replicas: 3` → 항상 Pod 3개 유지

---

## 🟠 5단계: 리소스 상세 이론

### ✅ Pod
- IP, Volume, 설정 등을 공유
- 한 Pod에 여러 컨테이너 가능 (sidecar 패턴)

### ✅ Service
- ClusterIP, NodePort, LoadBalancer, ExternalName
- 내부 DNS (`my-svc.my-namespace.svc.cluster.local`) 자동 생성

### ✅ Ingress
- 여러 서비스에 도메인 기반 HTTP 라우팅
- Ingress Controller 필요 (nginx, traefik 등)

---

## 🔴 6단계: 상태 유지 리소스 & 스토리지

### ✅ Volume / PVC / PV
- Pod가 죽어도 유지되는 외부 저장소 사용
- PVC: 사용자가 요청
- PV: 실제 스토리지 제공자
- StorageClass: 스토리지 유형 정의

### ✅ StatefulSet
- 이름 보존, 순차 배포
- Pod별 PVC 자동 연결
- DB, Kafka 등 상태 있는 앱 배포 시 사용

---

## 🟤 7단계: 스케일링 & 오토스케일링

### ✅ HPA (HorizontalPodAutoscaler)
- CPU 등 메트릭 기준으로 Pod 수 자동 증가/감소

### ✅ VPA
- Pod 하나의 리소스 요청량 자동 조정

---

## ⚫ 8단계: Job & CronJob

| 리소스 | 설명 |
|--------|------|
| Job | 단발성 작업 (DB 백업 등) |
| CronJob | CRON 스케줄 기반 반복 작업 |

---

## 🟧 9단계: 보안 구성 요소

### ✅ 인증 / 인가
- Authentication: TLS, Token, OIDC 등
- Authorization: RBAC (Role, RoleBinding, ClusterRole)

### ✅ Pod 보안
- SecurityContext: 사용자 권한 설정
- PSA (Pod Security Admission): baseline, restricted 등급

### ✅ Secret & ConfigMap
- Secret은 base64 인코딩됨 (etcd 암호화 필요)

---

## 🟨 10단계: 고급 배치 제어

### ✅ Taint & Toleration
- 특정 노드에 특정 Pod만 배치 가능

### ✅ Affinity / Anti-Affinity
- Pod의 배치 위치 제어 (같이/떨어지게)

---

## 🟩 11단계: 네트워크 & 정책

### ✅ NetworkPolicy
- Pod 간 통신 제어 (방화벽 역할)
- 네트워크 플러그인 필요 (Calico, Cilium 등)

---

## 🟦 12단계: 모니터링 & 로깅

### ✅ Metrics 흐름
- Pod → kubelet → Metrics Server → Prometheus → Grafana

### ✅ 로그 흐름
- stdout → kubelet → Fluentd → Elasticsearch → Kibana (EFK)

---

## 🟥 13단계: 확장성

### ✅ CRD (Custom Resource Definition)
- 사용자 정의 리소스 타입

### ✅ Operator
- CRD + Controller = 앱 생애주기 자동화

---

## 🟫 14단계: Admission Controller & 정책

- 리소스 생성 전에 가로채어 수정/검증
- Validating & Mutating Webhook
- Kyverno, OPA Gatekeeper 등으로 정책 작성

---

## 🎯 종합 요약

Kubernetes는…

- 선언형 구조 (원하는 상태 → 자동 맞춤)
- 모든 리소스는 API 기반 (kubectl, REST)
- 다양한 리소스를 조합해 확장성과 자동화 제공
- 보안, 네트워크, 모니터링, 배포 자동화를 모두 아우르는 **완전한 플랫폼**

---

**다음은 실습, 클라우드 연동(EKS 등), CI/CD, GitOps로 확장하면 완성! 🚀**
