# 🧠 Kubernetes 기술 리더 면접 질문 & 예시 답변 모음

---

## 📦 Kubernetes & Container

### Q1. Deployment와 StatefulSet의 차이는?
**A:** Deployment는 stateless 애플리케이션을 위해 설계되며, Pod는 서로 교체 가능하다. 반면 StatefulSet은 각 Pod에 고유한 identity(이름, 네트워크, 볼륨)를 부여해 stateful한 워크로드에 적합하다. 예: MySQL, Kafka 등.

### Q2. readinessProbe와 livenessProbe의 차이는?
**A:** readinessProbe는 서비스가 요청을 받을 준비가 되었는지 확인하고, livenessProbe는 애플리케이션이 살아있는지를 확인한다. readiness는 트래픽 수신 결정, liveness는 재시작 여부를 결정한다.

---

## 🔐 보안 및 정책

### Q3. PodSecurityAdmission(PSA)과 Kyverno의 차이는?
**A:** PSA는 K8s 내장 기능으로 네임스페이스 수준의 정책 집합을 강제하지만 유연성이 낮다. Kyverno는 사용자 정의 정책(CR)을 통해 보다 정교하고 동적인 정책 적용이 가능하다.

### Q4. Secret을 안전하게 관리하는 방법은?
**A:** Secret은 etcd에 base64 인코딩으로 저장되며 암호화 설정을 권장한다. 또한 External Secrets Operator를 사용하여 AWS Secrets Manager 등 외부 vault 연동을 통해 보안성을 강화할 수 있다.

---

## ⚙️ GitOps & Platform

### Q5. Helm chart의 구조는 어떻게 되고 lifecycle hook은 언제 쓰나요?
**A:** Helm chart는 Chart.yaml, values.yaml, templates/로 구성된다. lifecycle hook은 pre-install, post-upgrade 등 특정 이벤트 직전/직후에 작업을 실행할 수 있다. 예: 마이그레이션 스크립트 실행.

### Q6. ArgoCD App-of-Apps 패턴의 장점은?
**A:** 수백 개의 앱을 계층적으로 관리할 수 있으며, Git 디렉토리 구조와 일치하게 클러스터 상태를 구성할 수 있다. 멀티클러스터 GitOps에 적합하다.

---

## 🌐 아키텍처 설계

### Q7. Crossplane과 Terraform을 병행하거나 구분하는 기준은?
**A:** Terraform은 선언적 IaC이며 외부 실행 기반이고, Crossplane은 K8s 내부에서 컨트롤러가 상태를 유지한다. GitOps 환경에서는 Crossplane이 더 적합하다.

### Q8. 멀티클러스터 환경에서 DNS 라우팅은 어떻게 구성할 수 있는가?
**A:** ExternalDNS + 클라우드 기반 LoadBalancer(GCP LB, AWS ALB) + GeoDNS 또는 클러스터간 Gateway/Istio 활용.

---

## 📊 Observability

### Q9. SLI, SLO, SLA의 차이는?
**A:** SLI는 측정지표 (예: 응답시간), SLO는 목표 (예: 99.9% 성공률), SLA는 계약된 보장 수준이다. 운영 기준 수립과 경보 설정에 활용된다.

### Q10. Prometheus Alertmanager의 흐름은?
**A:** Prometheus가 alert rule을 통해 알람을 발생 → Alertmanager가 수신 → 라우팅/그룹핑 후 Slack, Email, Webhook 등으로 전송.

---

## 💬 커뮤니케이션 / 리더십

### Q11. 기술 도입 시 팀원 설득을 어떻게 하셨나요?
**A:** 직접 PoC를 통해 검증하고, 장단점을 명확히 문서화한 뒤 팀 내 기술 공유회를 열어 의견을 수렴했다. 구성원의 우려사항을 함께 반영한 설계안을 제시했다.

### Q12. 사내 교육이나 기술 온보딩은 어떻게 진행했나요?
**A:** 문서 기반 튜토리얼, 실습 리포지토리, 가상 워크숍을 활용해 신규 인원의 기술 적응 속도를 높였다. Backstage를 통해 셀프서비스 템플릿도 제공했다.

---

