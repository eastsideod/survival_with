# ASP.NET Core 라우팅

라우팅은 들어온 HTTP 요청의 URL을 분석하여 어떤 엔드포인트로 보낼지 결정하는 과정입니다. 게임 서버에서는 주로 attribute routing을 사용합니다.

## HTTP 요청의 구성 요소
HTTP 요청은 다음과 같은 주요 구성 요소로 이루어져 있습니다:

1. **URL (Uniform Resource Locator)**
   - 요청 대상의 주소를 지정
   - 예: `https://api.example.com/player/profile`
   - 프로토콜(`https://`), 도메인(`api.example.com`), 경로(`/player/profile`)로 구성

2. **HTTP 메서드**
   - 요청의 목적을 나타내는 동사
   - 주요 메서드:
     - `GET`: 리소스 조회
     - `POST`: 새 리소스 생성
     - `PUT`: 리소스 전체 수정
     - `PATCH`: 리소스 일부 수정
     - `DELETE`: 리소스 삭제
   - ASP.NET Core에서는 `[HttpGet]`, `[HttpPost]` 등의 어트리뷰트로 지정

3. **헤더 (Headers)**
   - 요청에 대한 메타데이터를 포함
   - 주요 헤더:
     - `Content-Type`: 요청 본문의 형식 (예: `application/json`)
     - `Authorization`: 인증 정보
     - `Accept`: 클라이언트가 받아들일 수 있는 응답 형식
     - `User-Agent`: 클라이언트 정보

4. **본문 (Body)**
   - 요청과 함께 전송되는 데이터
   - 주로 POST, PUT, PATCH 요청에서 사용
   - JSON, XML, Form 데이터 등 다양한 형식 가능
   - ASP.NET Core에서는 `[FromBody]` 어트리뷰트로 받을 수 있음

예시:
```http
POST /api/player/login HTTP/1.1
Host: api.example.com
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...

{
    "username": "player1",
    "password": "****"
}
```

## URL 대소문자 구분
- ASP.NET Core의 라우팅은 기본적으로 대소문자를 구분하지 않습니다
- 예시:
```csharp
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    [HttpGet("profile")]
    public IActionResult GetProfile() { ... }
}
```
- 다음 URL들은 모두 같은 엔드포인트로 라우팅됩니다:
  - `api/player/profile`
  - `api/Player/profile`
  - `api/PLAYER/profile`
  - `api/player/PROFILE`
- 하지만 RESTful API 설계에서는 일관성을 위해 소문자를 사용하는 것이 일반적입니다

## Attribute Routing

Attribute Routing은 컨트롤러나 액션 메서드에 직접 라우트를 지정하는 방식입니다.

1. **컨트롤러 레벨 라우팅**
   - 컨트롤러 전체에 적용되는 기본 URL 패턴을 정의
   - 해당 컨트롤러의 모든 액션 메서드에 공통적으로 적용
   - HTTP 메서드 매핑
     - 메서드 이름이 HTTP 메서드와 일치하면 자동으로 매핑 (예: Get() → GET, Post() → POST)
     - 더 명시적인 라우팅을 위해 [HttpGet], [HttpPost] 등의 어트리뷰트 사용 권장
   - `[controller]` 토큰
     - 컨트롤러 클래스 이름에서 "Controller"를 제외한 부분으로 자동 대체
     - 예: `PlayerController` → `player`
   - 예시:
   ```csharp
   [Route("api/[controller]")]  // api/player
   public class PlayerController : ControllerBase
   {
       // GET /api/player
       public IActionResult Get() { ... }

       // POST /api/player
       public IActionResult Post() { ... }

       // 명시적 어트리뷰트 사용 예시
       [HttpGet("all")]  // GET /api/player/all
       public IActionResult GetAll() { ... }

       [HttpPost("create")]  // POST /api/player/create
       public IActionResult Create() { ... }
   }
   ```

2. **액션 메서드 레벨 라우팅**
   - 특정 액션 메서드에만 적용되는 URL 패턴을 정의
   - 컨트롤러 레벨 라우팅에 추가되는 경로
   - 예시:
   ```csharp
   [Route("api/[controller]")]  // api/player
   public class PlayerController : ControllerBase
   {
       [HttpGet("profile")]     // api/player/profile
       public IActionResult GetProfile() { ... }

       [HttpPost("login")]      // api/player/login
       public IActionResult Login() { ... }

       [HttpGet("{id}")]        // api/player/123
       public IActionResult GetPlayer(int id) { ... }
   }
   ```

   - 직접 URL 경로 지정하기:
   ```csharp
   // [controller] 토큰 대신 직접 경로 지정
   [Route("user")]  // user/login
   public class UserController : ControllerBase
   {
       [HttpPost("login")]
       public IActionResult Login() { ... }
   }
   ``` 