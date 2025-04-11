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
```csharp
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    // 모든 액션 메서드는 "api/player"로 시작하는 URL에 매핑됩니다
}
```

2. **액션 메서드 레벨 라우팅**
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    // POST api/player/login
}

[HttpGet("{id}")]
public async Task<IActionResult> GetPlayer(int id)
{
    // GET api/player/123
}
```

### 컨트롤러 레벨 vs 액션 메서드 레벨 라우팅 차이점

1. **컨트롤러 레벨 라우팅**
   - 컨트롤러 전체에 적용되는 기본 URL 패턴을 정의
   - 해당 컨트롤러의 모든 액션 메서드에 공통적으로 적용
   - 예시:
   ```csharp
   [Route("api/[controller]")]  // api/player
   public class PlayerController : ControllerBase
   {
       public IActionResult Get() { ... }  // api/player
       public IActionResult Post() { ... } // api/player
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
   }
   ```

3. **주요 차이점**
   - **범위**: 컨트롤러 레벨은 전체 컨트롤러에 적용, 액션 레벨은 특정 메서드에만 적용
   - **URL 구조**: 컨트롤러 레벨은 기본 경로, 액션 레벨은 세부 경로를 정의
   - **재사용성**: 컨트롤러 레벨은 공통 경로를 한 번만 정의, 액션 레벨은 각 메서드별로 정의

4. **게임 서버에서의 사용**
   - 컨트롤러 레벨에서 `api/[controller]`로 기본 경로 설정
   - 액션 메서드 레벨에서 각 기능별로 세부 경로 정의
     - 예: `api/player/login`, `api/player/profile`, `api/player/items` 등

### 여러 루트 경로 동시 사용
- 한 ASP.NET Core 프로젝트에서 여러 개의 루트 경로를 동시에 사용할 수 있습니다
- 예시:
```csharp
// api 루트 경로
[Route("api")]
public class PlayerController : ControllerBase
{
    [HttpPost("player/login")]
    public IActionResult Login() { ... }
}

// qwewe 루트 경로
[Route("qwewe")]
public class PlayerController2 : ControllerBase
{
    [HttpPost("player/login")]
    public IActionResult Login() { ... }
}
```
- 이렇게 하면:
  - `POST api/player/login`
  - `POST qwewe/player/login`
- 두 개의 다른 루트 경로로 동일한 기능에 접근할 수 있습니다

### 루트 경로와 하위 경로 설정
- 루트 경로는 URL에서 가장 기본이 되는 경로를 말합니다
- 실제 프로젝트에서는 루트 경로와 하위 경로가 함께 사용되는 것이 일반적입니다
- 예시:
  - `api/player/login`에서 `api`는 루트 경로, `player`와 `login`은 하위 경로
  - `api/item/list`에서 `api`는 루트 경로, `item`과 `list`는 하위 경로

- 루트 경로와 하위 경로 설정 예시:
```csharp
[Route("api")]  // 루트 경로를 "api"로 설정
public class UserController : ControllerBase
{
    [HttpPost("user")]  // 하위 경로 설정
    public IActionResult Login() { ... }
}
```
- 이 경우:
  - `POST api/user`로 요청이 들어오면 Login 메서드가 실행됩니다
  - `api`는 루트 경로, `user`는 하위 경로입니다

### [controller] 토큰의 동작 방식
- `[controller]`는 컨트롤러 클래스 이름에서 "Controller"를 제외한 부분으로 대체됩니다
- 예시:
```csharp
[Route("user/[controller]")]
public class PlayerController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login() { ... }
}
```
- 위 예시에서:
  1. `[controller]`는 `PlayerController`에서 "Controller"를 제외한 `Player`로 대체
  2. 전체 URL은 `user/player/login`이 됨
  3. 클라이언트는 `POST user/player/login`으로 요청해야 함

- 만약 `user/login`으로 요청하고 싶다면:
  1. 컨트롤러 이름을 `UserController`로 만들거나
  2. 라우트를 `[Route("user")]`로 변경해야 함 