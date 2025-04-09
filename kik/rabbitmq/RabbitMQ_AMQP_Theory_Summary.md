# RabbitMQ & AMQP 프로토콜 이론 요약 (초급 ~ 고급)

## 🚀 소개
RabbitMQ는 Erlang으로 작성된 오픈소스 메시지 브로커이며, AMQP (Advanced Message Queuing Protocol)를 구현합니다. 마이크로서비스, 분산 시스템 간의 비동기 메시지 통신을 가능하게 하여 유연하고 안정적인 아키텍처를 제공합니다. 이 문서는 *RabbitMQ in Depth*, *Mastering RabbitMQ*, *RabbitMQ Cookbook* 등의 자료를 바탕으로 RabbitMQ 및 AMQP에 대한 이론을 초급부터 고급까지 정리한 내용입니다.

---

## 🧑‍🎓 1. 핵심 개념 (초급)
### RabbitMQ란?
- 메시지를 비동기적으로 전달하는 **메시지 브로커**입니다.
- AMQP 0-9-1을 기본 프로토콜로 사용합니다.
- Erlang 기반으로 매우 높은 안정성과 클러스터링 기능을 제공합니다.

### 핵심 구성 요소
| 구성 요소 | 설명 |
|------------|------|
| Producer | 메시지를 생성하고 보냅니다. |
| Exchange | 메시지를 받아서 규칙에 따라 큐로 라우팅합니다. |
| Queue | 메시지를 저장하고 소비자에게 전달합니다. FIFO 방식입니다. |
| Consumer | 메시지를 가져와 처리하는 역할을 합니다. |

### Queue의 종류
| 종류 | 설명 |
|------|------|
| Durable Queue | 브로커 재시작 후에도 유지되는 큐 (디스크에 저장됨) |
| Transient Queue | 메모리 기반 큐로, 재시작 시 삭제됨 |
| Auto-delete Queue | 모든 consumer가 연결을 끊으면 자동 삭제됨 |
| Exclusive Queue | 단일 연결에만 바인딩되는 큐 (연결 종료 시 삭제됨) |

### 메시지 흐름
1. Producer가 메시지를 Exchange에 보냄
2. Exchange가 Binding 규칙에 따라 Queue로 라우팅
3. Consumer가 Queue에서 메시지를 소비함

### Exchange 타입
| 타입 | 설명 |
|------|------|
| Direct | routing key와 정확히 일치하는 큐로 전달 |
| Fanout | 모든 바운딩된 큐로 브로드캐스트 |
| Topic | 와일드카드(`*`, `#`) 기반 패턴 매칭 |
| Headers | 헤더 속성 기반 라우팅 |

---

## 📚 2. AMQP 프로토콜 이론 (중급)
### AMQP란?
- **프레임 기반 이진 프로토콜**로, 클라이언트-브로커 간 RPC(Remote Procedure Call) 방식으로 통신합니다.

### AMQP 구조와 개념
- **Connection**: TCP를 통해 하나의 연결을 생성
- **Channel**: Connection 내부에서 다중 논리 연결 (가볍고 고속)
- **Exchange**: 메시지를 큐로 라우팅하는 역할
- **Queue**: 메시지를 저장하는 공간
- **Binding**: Exchange와 Queue 간의 연결 규칙

### 프레임 종류 및 구조 (Binary Frame Structure)
| 프레임 종류 | 설명 |
|-------------|------|
| Method Frame | AMQP 명령 전송 (`Basic.Publish`, `Queue.Declare` 등) |
| Header Frame | 메시지의 속성과 바디 크기 포함 (Basic.Properties 포함) |
| Body Frame | 실제 메시지의 본문 데이터 (여러 개로 분할 가능) |
| Heartbeat Frame | 연결 상태 확인용 주기적 신호 (프레임 크기 고정: 8바이트) |

#### Frame 구조 (공통)
- **1 byte**: Frame type (1=method, 2=header, 3=body, 8=heartbeat)
- **2 bytes**: Channel ID
- **4 bytes**: Payload size (n)
- **n bytes**: Payload (frame-specific content)
- **1 byte**: Frame end marker (0xCE)

#### QoS (Quality of Service)
- `prefetch_count`를 설정하여 한 번에 처리할 수 있는 메시지 수 제한 가능
- Consumer가 ack하지 않은 메시지를 너무 많이 받지 않도록 제한

#### TTL (Time To Live)
- 메시지 또는 큐 수준에서 TTL 설정 가능
- TTL 초과 시 메시지는 삭제되거나 DLX로 이동됨

---

## 🔧 3. 고급 AMQP 메시징 기능 (고급)
### 메시지 식별 관련 필드
| 필드 | 설명 |
|------|------|
| `message-id` | 메시지를 고유하게 식별 (UUID 형식 권장) |
| `correlation-id` | 요청-응답 메시지 상관관계 추적 (RPC 패턴에서 유용) |
| `delivery-tag` | Consumer 측에서 메시지를 구분하는 ID (채널 단위, 브로커가 할당) |

### 라우팅 전략 비교
| 전략 | 특징 | 사용 예시 |
|-------|------|-----------|
| Direct | exact match | 알림 타입: email, sms |
| Topic | 패턴 기반 | `logs.*`, `sensor.#` 등 |
| Headers | 헤더값 기반 | 복잡한 조건 또는 키-값 기반 |
| Fanout | 모두에게 전달 | 채널 전체 알림, 이벤트 브로드캐스트 |

### 고급 라우팅 기술 및 Failover 방식
(생략 - 위 내용 포함되어 있음)

---

## 📌 참고 문헌
- *RabbitMQ in Depth* (Gavin M. Roy)
- *Mastering RabbitMQ* (Emrah Ayanoglu)
- *RabbitMQ Cookbook* (Sigismondo Boschi, Gabriele Santomaggio)

---

※ 이 문서는 실무에서 RabbitMQ의 이론적 기반을 체계적으로 정리하고 학습하거나 설계 시 참고할 수 있도록 작성된 문서입니다.
