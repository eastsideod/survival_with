# Istio 아키텍처

## 전체 아키텍처

Istio는 ASP.NET 마이크로서비스 아키텍처를 위한 두 가지 주요 컴포넌트로 구성되어 있습니다:

1. **데이터 플레인 (Data Plane)**
   - ASP.NET 서비스 간 HTTP/HTTPS 트래픽을 처리하는 계층
   - 서비스 간 통신을 담당
   - 트래픽 제어, 보안, 모니터링 기능 수행
   - Envoy 프록시로 구현
   - 각 ASP.NET 서비스 파드에 사이드카로 배포

2. **컨트롤 플레인 (Control Plane)**
   - ASP.NET 서비스 메시의 중앙 관리 계층
   - 구성 관리 및 정책 시행
   - 서비스 검색 및 라우팅 규칙 관리
   - 보안 정책 관리
   - Istiod, Citadel, Galley 등으로 구성

## 데이터 플레인

### Envoy 프록시

Envoy는 ASP.NET 서비스의 HTTP/HTTPS 트래픽을 처리하는 핵심 프록시 컴포넌트로, 다음과 같은 기능을 제공합니다:

- ASP.NET 서비스 동적 검색
  - Kubernetes API 서버를 통해 ASP.NET 서비스의 실시간 상태 모니터링
  - 서비스 인스턴스의 자동 등록 및 해제
  - 서비스 엔드포인트의 동적 업데이트
  - 서비스 메타데이터 관리 (버전, 레이블 등)
  - 서비스 간 의존성 추적
  - 서비스 상태 변경 시 자동 라우팅 업데이트
  - 서비스 디스커버리 패턴 구현 (예: ASP.NET Core의 HttpClientFactory와 통합)
- HTTP/HTTPS 로드 밸런싱
- TLS 종료
  - 암호화된 트래픽을 받아서 복호화
  - 내부 ASP.NET 서비스로 평문 전달
  - 다시 암호화하여 외부로 전송
  - 인증서 자동 관리
  - 보안 통신 채널 설정
- HTTP/2 및 gRPC 프록시
  - HTTP/2 프로토콜 지원
    - 멀티플렉싱 (단일 연결에서 여러 요청/응답 처리)
    - 헤더 압축
    - 서버 푸시
  - gRPC 지원
    - 양방향 스트리밍
    - 자동 로드 밸런싱
    - 서비스 간 통신 최적화
- 서킷 브레이커
  - 장애 전파 방지
    - 실패한 ASP.NET 서비스에 대한 요청 차단
    - 장애 복구 시간 제공
  - 부하 제어
    - 동시 연결 수 제한
    - 요청 큐 크기 제한
  - 자동 복구
    - 일정 시간 후 자동 재시도
    - 점진적 복구
- 헬스 체크
- 스테이지 롤아웃
  - 카나리아 배포
    - 새 ASP.NET 버전의 점진적 배포
    - 트래픽 분산 제어
  - 블루/그린 배포
    - 새 ASP.NET 버전과 이전 버전의 동시 운영
    - 즉시 전환 또는 점진적 전환
  - A/B 테스트
    - 사용자 그룹별 다른 ASP.NET 버전 제공
    - 성능 및 기능 비교
- 메트릭 수집

### 사이드카 패턴

- 각 ASP.NET 서비스 파드에 Envoy 프록시가 사이드카로 배포됨
  - ASP.NET 애플리케이션 컨테이너와 함께 동일한 파드에서 실행
  - 애플리케이션과 독립적으로 운영
  - 네트워크 통신을 프록시하여 제어
- ASP.NET 서비스 간의 모든 통신을 프록시
  - 인바운드/아웃바운드 트래픽 처리
  - 트래픽 라우팅 및 제어
  - 보안 및 모니터링 기능 제공
- ASP.NET 서비스 코드 변경 없이 기능 추가 가능
  - 인프라 수준의 기능 분리
  - 애플리케이션 로직과 인프라 로직의 분리
  - 유연한 기능 확장

## 컨트롤 플레인

### Istiod

Istiod는 Istio의 중앙 관리 컴포넌트로, 다음과 같은 기능을 제공합니다:

- 서비스 검색
- 구성 관리
- 인증서 관리
- Envoy 프록시 구성

### Citadel

보안 관련 기능을 담당하는 컴포넌트:

- 인증서 발급 및 관리
- 서비스 간 인증
- 사용자 인증
- 키 및 인증서 관리

### Galley

구성 검증 및 관리:

- 구성 검증
- 구성 분배
- 구성 변환

## 통신 흐름

1. **ASP.NET 서비스 간 통신**
   - ASP.NET 서비스 A → Envoy 프록시 A → Envoy 프록시 B → ASP.NET 서비스 B
   - ASP.NET 애플리케이션 예시:
     ```csharp
     // ASP.NET 서비스 A에서 서비스 B 호출
     public class ServiceAController : ControllerBase
     {
         private readonly HttpClient _httpClient;

         public ServiceAController(IHttpClientFactory httpClientFactory)
         {
             _httpClient = httpClientFactory.CreateClient();
         }

         [HttpGet]
         public async Task<IActionResult> GetData()
         {
             // 서비스 B의 URL로 요청
             // Istio가 자동으로 트래픽을 관리
             var response = await _httpClient.GetAsync("http://service-b/api/data");
             return Ok(await response.Content.ReadAsStringAsync());
         }
     }
     ```

2. **컨트롤 플레인 통신**
   - Istiod → Envoy 프록시 (구성 업데이트)
   - Envoy 프록시 → Istiod (상태 보고)

3. **트래픽 처리 과정**
   - 모든 네트워크 통신은 Envoy 사이드카를 통해 처리됨
   - 인바운드 트래픽: 외부 → Envoy 사이드카 → ASP.NET 애플리케이션
   - 아웃바운드 트래픽: ASP.NET 애플리케이션 → Envoy 사이드카 → 외부
   - 서비스 간 통신: ASP.NET 서비스 A → Envoy 사이드카 A → Envoy 사이드카 B → ASP.NET 서비스 B

## 확장성

Istio는 다음과 같은 방식으로 확장 가능합니다:

1. **수평적 확장**
   - Envoy 프록시의 자동 스케일링
   - 컨트롤 플레인 컴포넌트의 복제

2. **기능적 확장**
   - Mixer 어댑터
   - 사용자 정의 정책
   - 확장된 메트릭

## 고가용성

Istio는 다음과 같은 고가용성 기능을 제공합니다:

1. **컨트롤 플레인**
   - 다중 인스턴스 배포
   - 상태 동기화
   - 장애 복구

2. **데이터 플레인**
   - 자동 장애 감지
   - 트래픽 재라우팅
   - 서킷 브레이커 