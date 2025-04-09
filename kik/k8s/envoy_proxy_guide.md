# Envoy Proxy: 입문부터 고급까지 완벽 가이드

> 참고 문서: [Envoy v1.33.2 공식 문서](https://www.envoyproxy.io/docs/envoy/v1.33.2/)

---

## 🧑 초급자 가이드: Envoy란 무엇인가?

### Envoy 개요
- **Envoy**는 L7(애플리케이션 계층) 프록시 및 서비스 메시 구성 요소입니다.
- **Cloud Native** 애플리케이션에서 **트래픽 관리, 로깅, 보안** 등의 역할을 수행합니다.

### 주요 기능
- **서비스 디스커버리**: 서비스가 어디에 있는지 자동으로 탐색
- **로드 밸런싱**: 여러 인스턴스 간 트래픽 분산
- **헬스 체크**: 서비스 상태 확인
- **트래픽 라우팅**: 요청을 다양한 규칙에 따라 목적지로 전달
- **TLS 종료 및 재암호화**
- **강력한 모니터링 및 트레이싱 지원**

### 초간단 아키텍처 예시
```
[ Client ] ---> [ Envoy Proxy ] ---> [ Backend Service ]
```

---

## 🧑‍💻 중급자 가이드: Envoy 구성 및 사용

### 핵심 구성 요소
- **Listener**: Envoy가 클라이언트 요청을 수신하는 지점
- **Cluster**: Envoy가 트래픽을 전달할 백엔드 서비스 그룹
- **Route**: 요청 조건에 따라 라우팅 동작 지정
- **Filter**: 요청 및 응답을 수정/검사하는 기능 단위

### 간단한 Static Config 예제
```yaml
static_resources:
  listeners:
    - name: listener_0
      address:
        socket_address: { address: 0.0.0.0, port_value: 10000 }
      filter_chains:
        - filters:
            - name: envoy.filters.network.http_connection_manager
              typed_config:
                "@type": type.googleapis.com/envoy.extensions.filters.network.http_connection_manager.v3.HttpConnectionManager
                stat_prefix: ingress_http
                route_config:
                  name: local_route
                  virtual_hosts:
                    - name: backend
                      domains: ["*"]
                      routes:
                        - match: { prefix: "/" }
                          route: { cluster: service1 }
                http_filters:
                  - name: envoy.filters.http.router
  clusters:
    - name: service1
      connect_timeout: 0.25s
      type: LOGICAL_DNS
      load_assignment:
        cluster_name: service1
        endpoints:
          - lb_endpoints:
              - endpoint:
                  address:
                    socket_address: { address: service1, port_value: 80 }
```

### 운영 중 고려사항
- **Configuration Reloading**: xDS(dynamic config) 또는 hot-reload 가능
- **Observability**: Statsd, Prometheus, Zipkin, Jaeger 통합 가능
- **Security**: TLS, mTLS, RBAC 정책 가능

---

## 🧠 고급자 가이드: Envoy 심화 및 실전 적용

### xDS API
- Envoy는 xDS API를 통해 동적으로 구성을 관리함
- 주요 xDS 컴포넌트:
  - **LDS** (Listener Discovery Service)
  - **CDS** (Cluster Discovery Service)
  - **RDS** (Route Discovery Service)
  - **EDS** (Endpoint Discovery Service)

### 필터 체인 고급 구성
- 필터를 체인으로 연결해 요청/응답을 처리
- 예: 인증 → 로깅 → 라우팅

### 확장성
- Envoy는 **WebAssembly (Wasm)** 기반 필터 확장을 지원
- 다양한 언어로 커스텀 필터 개발 가능

### 실전 시나리오
#### 1. 서비스 메시
- Istio, Consul, AWS App Mesh 등이 Envoy를 데이터 플레인으로 사용

#### 2. API Gateway
- Envoy 자체를 API Gateway로 구성 가능

#### 3. Canary / A/B 테스트
- 트래픽의 일정 비율을 새로운 버전에 전달 가능

### 고급 예제: Rate Limiting Filter
```yaml
http_filters:
  - name: envoy.filters.http.local_ratelimit
    typed_config:
      "@type": type.googleapis.com/envoy.extensions.filters.http.local_ratelimit.v3.LocalRateLimit
      stat_prefix: http_local_rate_limiter
      token_bucket:
        max_tokens: 100
        tokens_per_fill: 10
        fill_interval: 1s
```

---

## 📊 관찰 가능성과 통합

### 통합 가능한 툴
- **Zipkin / Jaeger**: 트레이싱
- **Prometheus / Grafana**: 메트릭 수집/시각화
- **Fluentd / EFK**: 로그 수집

### Envoy 통계 예시
- `http.<stat_prefix>.downstream_rq_total`
- `cluster.<name>.upstream_rq_time`

---

## ✅ 마무리

Envoy는 단순한 프록시를 넘어서, **현대적 마이크로서비스 아키텍처**의 핵심 구성 요소입니다. 학습 곡선은 있지만, 충분히 학습할 가치가 있는 강력한 도구입니다. 

이 문서에서 소개한 수준별 개념과 실습 예제들을 통해 입문자부터 전문가까지 모두 Envoy를 효과적으로 활용할 수 있습니다.

---

> 작성자: ChatGPT (2025년 기준)
> 문서 버전: v1.0 based on Envoy v1.33.2
