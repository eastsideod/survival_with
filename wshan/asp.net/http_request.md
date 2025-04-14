# HTTP 요청의 구성 요소

HTTP 요청은 웹 애플리케이션과 서버 간의 통신에서 가장 기본적인 단위입니다. 다음과 같은 주요 구성 요소로 이루어져 있습니다:

## 1. URL (Uniform Resource Locator)
- 요청 대상의 주소를 지정
- 예: `https://api.example.com/player/profile`
- 구성 요소:
  - 프로토콜: `https://`
  - 도메인: `api.example.com`
  - 경로: `/player/profile`
  - 쿼리 파라미터: `?id=123&type=weapon` (선택적)

## 2. HTTP 메서드
- 요청의 목적을 나타내는 동사
- 주요 메서드:
  - `GET`: 리소스 조회
  - `POST`: 새 리소스 생성
  - `PUT`: 리소스 전체 수정
  - `PATCH`: 리소스 일부 수정
  - `DELETE`: 리소스 삭제
- ASP.NET Core에서는 어트리뷰트로 지정:
  ```csharp
  [HttpGet("profile")]
  public IActionResult GetProfile() { ... }

  [HttpPost("login")]
  public IActionResult Login() { ... }
  ```

## 3. 헤더 (Headers)
- 요청에 대한 메타데이터를 포함
- 주요 헤더:
  - `Content-Type`: 요청 본문의 형식
    - 서버에게 요청 본문의 데이터 형식을 알려줌
    - 예: `application/json`, `application/x-www-form-urlencoded`, `multipart/form-data`
    - ASP.NET Core에서는 `[Consumes("application/json")]` 어트리뷰트로 지정 가능
    - 필수 헤더는 아니지만, 본문이 있는 요청에서는 권장됨

  - `Authorization`: 인증 정보
    - 클라이언트의 인증 정보를 포함
    - 주로 사용되는 형식:
      - Bearer 토큰: `Authorization: Bearer eyJhbGciOiJIUzI1NiIs...`
      - Basic 인증: `Authorization: Basic base64(username:password)`
    - ASP.NET Core에서는 `[Authorize]` 어트리뷰트로 인증 요구사항 지정

  - `Accept`: 클라이언트가 받아들일 수 있는 응답 형식
    - 서버가 응답할 수 있는 데이터 형식을 지정
    - 여러 형식을 지원하는 경우: `Accept: application/json, application/xml, text/plain`
    - 왼쪽부터 우선순위가 높음
    - 게임 서버에서는 주로 1:1 클라이언트-서버 관계이므로 자주 사용되지 않음
    - 대신 컨트롤러에 `[Produces("application/json")]` 어트리뷰트로 직접 지정하는 것이 일반적

  - `User-Agent`: 클라이언트 정보
    - 클라이언트 애플리케이션의 식별 정보
    - 예: `GameClient/1.0 (Windows NT 10.0)`
    - 서버에서 클라이언트 버전 관리나 호환성 체크에 사용
    - 보안을 위해 위조될 수 있으므로 신뢰성 있는 인증 수단으로 사용하면 안 됨

  - `Cookie`: 세션 정보
    - 서버가 이전에 설정한 쿠키 정보를 포함
    - 주요 쿠키 종류:
      - **세션 관리**
        - `sessionId`: 사용자 세션 식별 ID
        - `authToken`: 인증 토큰 (JWT 등)
        - `userId`: 로그인한 사용자 ID
      - **사용자 환경 설정**
        - `theme`: 다크/라이트 모드 설정
        - `language`: 언어 설정
        - `timezone`: 시간대 설정
      - **게임 관련**
        - `lastServer`: 마지막 접속 서버
        - `characterId`: 현재 선택된 캐릭터 ID
        - `gameSettings`: 게임 내 설정 (음량, 그래픽 품질 등)
      - **보안 관련**
        - `csrfToken`: CSRF 공격 방지 토큰
        - `securityLevel`: 보안 레벨 설정
      - **기타**
        - `consent`: 개인정보 수집 동의 여부
        - `analytics`: 분석 도구 관련 설정
    - 예: `Cookie: sessionId=abc123; theme=dark; lastServer=server1`
    - ASP.NET Core에서는 `[FromCookie]` 어트리뷰트로 받을 수 있음
    - 보안을 위해 중요한 정보(비밀번호, 개인정보 등)는 쿠키에 저장하지 않는 것이 좋음

- ASP.NET Core에서 헤더 접근:
  ```csharp
  var contentType = Request.Headers["Content-Type"];
  var authToken = Request.Headers["Authorization"];
  var userAgent = Request.Headers["User-Agent"];
  var cookies = Request.Cookies;
  ```

## 4. 본문 (Body)
- 요청과 함께 전송되는 데이터
- 주로 POST, PUT, PATCH 요청에서 사용
- 지원되는 형식:
  - JSON: `application/json`
  - Form 데이터: `application/x-www-form-urlencoded`
  - 파일 업로드: `multipart/form-data`
- ASP.NET Core에서 본문 데이터 받기:
  ```csharp
  [HttpPost]
  public IActionResult Create([FromBody] PlayerModel player) { ... }
  ```

## 실제 HTTP 요청 예시
```http
POST /api/player/login HTTP/1.1
Host: api.example.com
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Accept: application/json
User-Agent: GameClient/1.0

{
    "username": "player1",
    "password": "****"
}
```

## ASP.NET Core에서의 요청 처리
1. **라우팅**: URL과 HTTP 메서드를 기반으로 적절한 컨트롤러 액션 선택
2. **모델 바인딩**: 요청 데이터를 C# 객체로 변환 (자세한 내용은 model_binding.md 참조)
3. **유효성 검사**: 입력 데이터 검증
4. **비즈니스 로직 실행**: 요청 처리
5. **응답 생성**: 결과를 HTTP 응답으로 변환

## 보안 고려사항
1. HTTPS 사용 권장
2. 입력 데이터 검증 필수
3. 인증/인가 처리
4. CORS 설정
5. 요청 크기 제한 설정 