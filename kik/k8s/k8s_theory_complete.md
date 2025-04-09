
# 📘 Kubernetes 고급 이론 정리

---

## 🧭 1. 컨트롤러 작동 방식 (Controller Pattern)

Kubernetes는 “사용자가 원하는 상태(Desired State)”를 정의하면, 컨트롤러가 현재 상태(Actual State)와 비교해서 자동으로 맞춰준다.

### 🔁 컨트롤 루프 예시

```yaml
apiVersion: apps/v1
kind: Deployment
spec:
  replicas: 3
```

→ ReplicaSet Controller는 항상 Pod 수가 3개인지 확인하고 부족하면 자동 생성.

---

## 📦 2. etcd 구조

`etcd`는 Kubernetes Control Plane의 상태 저장소이다.

- 모든 클러스터 구성 정보와 상태 정보를 key-value로 저장
- 고가용성 클러스터 구성 가능
- 중요한 리소스 (Secret 포함) 저장 → 백업 필수

```bash
etcdctl snapshot save backup.db
```

---

## 🔧 3. CRD & Operator 설계

### ✅ CRD (Custom Resource Definition)
- 사용자가 직접 만든 리소스 타입 정의

### ✅ Operator
- CRD + 컨트롤러 조합
- 앱의 생애주기를 자동화 (예: MySQL Operator)

---

## 💽 4. Kubernetes 스토리지 구조

### 🔹 구성 요소
- Volume: Pod 내부 임시 디스크
- PersistentVolume (PV): 클러스터 제공 영구 저장소
- PersistentVolumeClaim (PVC): 사용자가 요청하는 볼륨

```yaml
resources:
  requests:
    storage: 1Gi
```

---

## 🔌 5. 쿠버네티스 API 확장성

모든 리소스는 REST API 기반으로 구성되며 다음과 같은 확장이 가능하다:

- CRD (리소스 확장)
- Admission Webhook (행위 제어)
- Aggregated API Server (기능 확장)

---

## 🔹 6. Namespace와 멀티 테넌시

- `Namespace`로 리소스를 논리적으로 격리
- RBAC, NetworkPolicy와 결합해 팀별 클러스터 분리 가능

---

## 🔹 7. Resource Request & Limit

```yaml
resources:
  requests:
    cpu: "250m"
  limits:
    cpu: "500m"
```

- Request: 스케줄링에 사용
- Limit: 실제 최대 사용량 제한

---

## 🔹 8. HPA & VPA

- HPA: 부하에 따라 Pod 개수 조절
- VPA: Pod의 리소스 할당량 자동 조절

---

## 🔹 9. Init / Sidecar 컨테이너

- Init Container: 메인 컨테이너 실행 전 준비 작업
- Sidecar: 로그 수집, 프록시 등 역할

---

## 🔹 10. 서비스 종류

| 타입 | 설명 |
|------|------|
| ClusterIP | 클러스터 내부 통신 전용 |
| NodePort | 외부에서 접속 가능한 포트 노출 |
| LoadBalancer | 클라우드 환경 외부 IP 할당 |
| ExternalName | 외부 DNS 서비스 alias |

---

## 🔹 11. Job & CronJob

- Job: 단발성 작업
- CronJob: 주기적 작업 (`*/5 * * * *`)

---

## 🔹 12. Taints & Tolerations

- 특정 Pod만 특정 노드에 배치 가능
- 예: GPU 노드에만 AI 워크로드

---

## 🔹 13. Affinity & AntiAffinity

- Node Affinity: 특정 노드에 배치
- Pod Anti-Affinity: Pod 간 분산 배치 유도

---

## 🔹 14. Admission Controller

API 요청을 수정하거나 거부할 수 있는 미들웨어:

- MutatingAdmissionWebhook
- ValidatingAdmissionWebhook

예: Kyverno, Gatekeeper 등이 여기 기반

---

## 🔹 15. API Aggregation

Metrics Server 등 K8s API 기능 자체를 확장 가능

---

## 🔹 16. Metrics & Logging 흐름

- 메트릭: Pod → kubelet → Metrics Server → Prometheus → Grafana
- 로그: Pod → container runtime → Fluentd → Elasticsearch → Kibana

---

## 🔹 17. DNS & 서비스 디스커버리

- CoreDNS로 `svc.cluster.local` 형태의 내부 DNS 제공
- Pod IP가 바뀌어도 서비스 접근 가능

---

## 🔹 18. QoS Class

| 클래스 | 조건 | 특징 |
|--------|------|------|
| Guaranteed | requests == limits | 가장 보호됨 |
| Burstable | 일부 설정 | 중간 |
| BestEffort | 설정 없음 | 가장 먼저 종료 대상 |

---

## 🔹 19. Kubelet & Node 상태 관리

- Kubelet은 Pod 실행 및 상태 보고
- 응답 없는 노드는 NotReady 처리
- 장애 노드는 자동으로 Pod를 다른 노드에 재배치

---

## 🔹 20. 서비스 메시 (Service Mesh)

예: Istio, Linkerd

- mTLS
- 요청 분할 (Canary, A/B 테스트)
- 트래픽 시각화 및 제어 (Envoy Proxy + Sidecar)

---

## ✅ 핵심 요약

- 선언형 → 자동 상태 동기화
- CRD/Operator로 무한 확장 가능
- Admission Controller로 행위 제어
- DNS/Storage/Security/네트워크까지 완전 자동화된 컨테이너 플랫폼

---

**이론 완성! 이제 실습 혹은 클라우드 연동, CI/CD로 이어가면 돼! 🚀**
