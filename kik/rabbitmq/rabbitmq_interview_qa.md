# 💼 RabbitMQ 면접 질문 & 답변 정리

---

## 🧩 1. 기본 개념

### ❓ Q1. RabbitMQ는 무엇이며, 어떤 상황에서 사용하나요?
✅ **A:**  
RabbitMQ는 AMQP 기반의 메시지 브로커로, 비동기 메시지 전달, 시스템 간 decoupling, 이벤트 기반 아키텍처 구현 시 사용됩니다.

---

### ❓ Q2. AMQP란?
✅ **A:**  
Advanced Message Queuing Protocol로, 메시지 브로커와 클라이언트 간 통신 프로토콜입니다. RabbitMQ는 AMQP 0-9-1을 따릅니다.

---

### ❓ Q3. RabbitMQ에서 Exchange는 무엇인가요?
✅ **A:**  
Exchange는 메시지를 적절한 Queue로 라우팅하는 역할을 하며, 타입에는 `direct`, `topic`, `fanout`, `headers`가 있습니다.

---

## 📦 2. 메시징 구조 & 라우팅

### ❓ Q4. Direct vs Topic Exchange 차이는?
✅ **A:**
| 항목 | Direct | Topic |
|------|--------|-------|
| 라우팅 방식 | exact match | 패턴 기반 |
| 예시 | `"order.created"` | `"order.*"`, `"order.#"` |
| 용도 | 단일 큐 지정 | 다중 큐 분기 처리 |

---

### ❓ Q5. 메시지를 Queue에 보내는 기준은 무엇인가요?
✅ **A:**  
Producer → Exchange → Routing Key → Queue로 전달됩니다. 직접 큐에 보내는 것이 아닌 Exchange를 통해 간접 전달됩니다.

---

## 🔐 3. 신뢰성 & 고가용성

### ❓ Q6. 메시지 유실 방지 방법은?
✅ **A:**
- durable queue 설정
- persistent 메시지 전송
- 수동 ack 처리
- DLX 설정

---

### ❓ Q7. Quorum Queue란?
✅ **A:**  
RabbitMQ의 고가용성 큐로 Raft 알고리즘 기반이며, 리더-팔로워 구조로 구성되고 과반수 동의로 메시지를 커밋합니다.

---

## 🧪 4. 실무 시나리오

### ❓ Q8. Retry 구조는 어떻게 설계하나요?
✅ **A:**
- `BasicNack(requeue=false)` → DLX로 이동
- DLX 큐는 TTL 이후 원래 큐로 메시지를 재전송
- `x-death` 헤더로 retry 횟수 추적 가능

---

### ❓ Q9. RPC 패턴은 어떻게 구현하나요?
✅ **A:**
- 요청 시: `reply-to`, `correlation-id` 설정
- 응답 시: 해당 정보 복사하여 응답 전송
- 클라이언트는 `correlation-id`로 응답 식별

---

### ❓ Q10. Federation과 Shovel 차이?
✅ **A:**
| 항목 | Federation | Shovel |
|------|------------|--------|
| 구조 | 브로커 간 구독 연결 | 큐/익스체인지 간 복사 |
| 용도 | 원격 메시지 수신 | 메시지 복제, 백업 |
| 구성 | 정책 기반 | 설정 기반 (스크립트) |

---

## 🛠️ 5. 운영 & 관리

### ❓ Q11. 메시지 중복 처리는 어떻게 하나요?
✅ **A:**
- `message-id` 설정
- idempotent 처리 로직 구현
- Redis/DB에 메시지 ID 저장 후 처리 여부 판단

---

### ❓ Q12. Virtual Host는 왜 쓰나요?
✅ **A:**
서비스, 환경(dev/prod 등) 간 격리된 메시징 공간을 제공하며, 권한 분리와 보안 측면에서 유리합니다.

---

### ❓ Q13. Monitoring은 어떻게 하나요?
✅ **A:**
- Web UI (management plugin)
- CLI (`rabbitmqctl list_queues` 등)
- Prometheus + Grafana
- DLX 큐 backlog, consumer lag 추적

---

## 🔍 6. RabbitMQ Streams 관련

### ❓ Q14. RabbitMQ Streams란? Kafka와의 차이는?
✅ **A:**  
로그 기반 고속 메시지 처리 구조로 offset 기반 소비가 가능합니다. Kafka는 완전한 분산 로그 처리 시스템이고, Streams는 MQ + 로그 스트림 중간 형태입니다.

---

## ✅ 마무리 팁

- 질문에 RabbitMQ뿐 아니라 AMQP, 메시징 패턴을 함께 설명하면 좋음
- 실무 경험 예시를 덧붙이면 효과적

필요시 PDF, 암기카드, 실전 예제 형식으로 확장 가능!
