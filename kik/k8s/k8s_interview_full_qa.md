# 📘 Kubernetes 면접 질문 & 예시 답변 모음

---

## 🚀 기본 개념

### Q1. Kubernetes란 무엇인가요?
**A:** Kubernetes는 컨테이너화된 애플리케이션의 배포, 확장, 관리를 자동화하는 오픈소스 플랫폼입니다. 클러스터 환경에서 다양한 노드에서 컨테이너를 효율적으로 운영할 수 있도록 도와줍니다.

### Q2. Pod, ReplicaSet, Deployment의 관계는?
**A:** Pod는 컨테이너의 최소 단위입니다. ReplicaSet은 동일한 Pod의 복제본을 관리하며, Deployment는 ReplicaSet을 관리하고 롤링 업데이트, 롤백 등을 제공합니다.

---

## 📦 스케줄링 & 리소스

### Q3. Kubernetes 스케줄링은 어떻게 작동하나요?
**A:** kube-scheduler는 새로운 Pod가 생성될 때, 리소스 요청, nodeSelector, affinity, taints/tolerations 등을 고려해 적절한 노드에 배치합니다.

### Q4. Resource request와 limit의 차이는?
**A:** request는 스케줄링 시 기준이 되는 최소 자원이고, limit은 컨테이너가 사용할 수 있는 최대 자원입니다. limit 초과 시 throttling 또는 종료됩니다.

---

## 🔐 보안

### Q5. RBAC이란 무엇인가요?
**A:** Role-Based Access Control로, 사용자/서비스계정이 어떤 리소스에 어떤 작업(읽기, 쓰기 등)을 할 수 있는지를 제어합니다.

### Q6. NetworkPolicy란?
**A:** Pod 간 또는 외부 트래픽에 대한 통신 허용/차단을 정의하는 리소스입니다. namespace, label 기반으로 제어합니다.

---

## ⚙️ 실전 운영

### Q7. livenessProbe와 readinessProbe의 차이는?
**A:** livenessProbe는 컨테이너가 살아있는지를, readinessProbe는 요청을 받을 준비가 되었는지를 확인합니다. readiness 실패 시 서비스에서 제외되고, liveness 실패 시 재시작됩니다.

### Q8. ConfigMap과 Secret 차이점은?
**A:** ConfigMap은 일반 설정 데이터를 담고, Secret은 민감한 데이터를 base64로 인코딩해서 담습니다. Secret은 암호화 및 RBAC 적용이 필요합니다.

---

## 📊 관측 (Observability)

### Q9. Kubernetes 클러스터 모니터링 도구는?
**A:** Prometheus (메트릭 수집), Grafana (시각화), Alertmanager (경보), Loki (로그 수집), OpenTelemetry (추적)를 주로 사용합니다.

---

## ☸️ 고급 기능

### Q10. StatefulSet은 어떤 상황에서 사용하나요?
**A:** 각 Pod가 고유한 스토리지와 네트워크 ID를 유지해야 하는 경우 사용합니다. 예: 데이터베이스, Kafka 등.

### Q11. Helm이란 무엇인가요?
**A:** Kubernetes 리소스를 패키징하여 템플릿으로 관리할 수 있는 도구입니다. Helm Chart로 앱을 배포하고 업그레이드, 롤백 등을 자동화할 수 있습니다.

---

## 🔁 GitOps & CI/CD

### Q12. GitOps란?
**A:** Git을 단일 소스로 사용하여 애플리케이션 및 인프라를 정의하고, Git 변경에 따라 자동으로 클러스터에 적용하는 운영 방식입니다. ArgoCD, Flux 등이 있습니다.

---

