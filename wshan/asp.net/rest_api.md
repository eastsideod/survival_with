# ASP.NET Core REST API 설계

## 1. RESTful API 기본 원칙

### 1.1 REST 원칙
1. **자원 중심 설계**
   - 명사형 URI 사용: URL을 동사가 아닌 명사로 구성하여 자원을 중심으로 설계
     * 의도: URL만 보고도 어떤 자원을 다루는지 직관적으로 파악 가능
     * 패턴화: HTTP 메서드(GET, POST, PUT, DELETE)와 명사형 URL의 조합으로 동작을 유추 가능
       - GET /users → 모든 사용자 조회
       - GET /users/1 → ID가 1인 사용자 조회
       - POST /users → 새 사용자 생성
       - PUT /users/1 → ID가 1인 사용자 수정
       - DELETE /users/1 → ID가 1인 사용자 삭제
     * 예시: `/users` (동사형: `/getUsers`), `/orders` (동사형: `/getOrders`)
   - 계층적 구조: 자원 간의 관계를 URL 구조로 표현
     * 예시: `/users/1/orders` (사용자 1의 주문 목록)
   - 일관된 네이밍: 모든 엔드포인트가 동일한 패턴을 따르도록 설계

2. **HTTP 메서드 활용**
   - GET: 조회
   - POST: 생성
   - PUT: 전체 수정
   - PATCH: 부분 수정
   - DELETE: 삭제

3. **상태 코드 사용**
   - 2xx: 성공
   - 4xx: 클라이언트 오류
   - 5xx: 서버 오류

### 1.2 API 버전 관리
1. **버전 관리 설정**
   ```csharp
   services.AddApiVersioning(options =>
   {
       options.DefaultApiVersion = new ApiVersion(1, 0);
       options.AssumeDefaultVersionWhenUnspecified = true;
       options.ApiVersionReader = ApiVersionReader.Combine(
           new UrlSegmentApiVersionReader(),
           new HeaderApiVersionReader("X-API-Version")
       );
       options.ReportApiVersions = true;
   });
   ```

2. **버전 관리 방법**
   ```csharp
   // URL 기반 버전 관리
   [ApiVersion("1.0")]
   [Route("api/v{version:apiVersion}/[controller]")]
   public class UsersController : ControllerBase
   {
       // v1.0 API
   }

   [ApiVersion("2.0")]
   [Route("api/v{version:apiVersion}/[controller]")]
   public class UsersV2Controller : ControllerBase
   {
       // v2.0 API (새로운 기능 추가)
   }

   // 헤더 기반 버전 관리
   [ApiVersion("1.0")]
   [Route("api/[controller]")]
   public class UsersController : ControllerBase
   {
       // v1.0 API
   }
   ```

3. **클라이언트 요청 예시**
   ```
   // URL 기반
   GET /api/v1/users
   GET /api/v2/users

   // 헤더 기반
   GET /api/users
   Accept: application/json; version=1.0
   또는
   X-API-Version: 1.0
   ```

4. **버전 관리의 장점**
   - **하위 호환성**: 기존 클라이언트는 계속 v1.0을 사용 가능
   - **점진적 전환**: 클라이언트가 새로운 버전으로 천천히 마이그레이션 가능
   - **명확한 변경사항**: 버전별로 어떤 기능이 추가/변경되었는지 명확히 구분

5. **실제 사용 예시**
   ```
   v1.0 API: /api/v1/users
   - 기본적인 사용자 정보 제공
   - 이름, 이메일, 가입일

   v2.0 API: /api/v2/users
   - v1.0의 모든 기능 유지
   - 추가: 프로필 이미지, 소셜 미디어 링크
   - 변경: 이메일 형식 검증 강화
   ```

## 2. REST API 설계 패턴

### 2.1 컨트롤러 구조
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
            
        return Ok(user);
    }
}
```

### 2.2 DTO 패턴
DTO(Data Transfer Object) 패턴은 데이터베이스의 스키마와 클라이언트에게 보여줄 데이터 구조를 분리하기 위한 패턴입니다.

1. **DTO의 핵심 개념**
   - 데이터베이스에는 모든 정보를 저장하되
   - 클라이언트에게는 필요한 정보만 선택적으로 전송
   - 각 상황(목록 조회, 상세 조회, 생성 등)에 맞는 최적의 데이터 구조 제공
   - 양방향 데이터 전송에 사용 (클라이언트 ↔ 서버)

2. **DTO의 양방향 사용**
   ```csharp
   // 1. 클라이언트 → 서버 (입력용 DTO)
   public class CreateUserDto
   {
       [Required]
       public string Name { get; set; }
       
       [Required]
       [EmailAddress]
       public string Email { get; set; }
       
       [Required]
       [MinLength(6)]
       public string Password { get; set; }
   }

   // 2. 서버 → 클라이언트 (응답용 DTO)
   public class UserDetailDto
   {
       public int Id { get; set; }
       public string Name { get; set; }
       public string Email { get; set; }
   }
   ```

3. **DTO 사용 예시 (양방향)**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class UsersController : ControllerBase
   {
       // 1. 클라이언트 → 서버 (입력)
       [HttpPost]
       public async Task<ActionResult<UserDetailDto>> CreateUser(CreateUserDto userDto)
       {
           // 입력 데이터 검증
           if (!ModelState.IsValid)
               return BadRequest(ModelState);

           // DTO를 엔티티로 변환하여 저장
           var user = new User
           {
               Name = userDto.Name,
               Email = userDto.Email,
               PasswordHash = HashPassword(userDto.Password)
           };
           await _dbContext.Users.AddAsync(user);
           await _dbContext.SaveChangesAsync();

           // 저장된 데이터를 응답용 DTO로 변환하여 반환
           return Ok(new UserDetailDto
           {
               Id = user.Id,
               Name = user.Name,
               Email = user.Email
           });
       }

       // 2. 서버 → 클라이언트 (응답)
       [HttpGet]
       public async Task<ActionResult<IEnumerable<UserListDto>>> GetUsers()
       {
           var users = await _dbContext.Users.ToListAsync();
           return Ok(users.Select(u => new UserListDto
           {
               Id = u.Id,
               Name = u.Name
           }));
       }
   }
   ```

## 3. REST API 문서화

### 3.1 Swagger 설정
Swagger는 API 문서화와 테스트를 위한 도구입니다.

1. **Swagger의 주요 기능**
   - API 문서 자동 생성
     * API 엔드포인트 목록
     * 각 엔드포인트의 설명
     * 요청/응답 형식
     * 파라미터 설명
   - 대화형 API 테스트
     * API를 직접 호출해볼 수 있는 UI 제공
     * 파라미터 입력 및 결과 확인 가능

2. **Swagger 문서 생성**
   - 프로그램 내의 모든 API 컨트롤러를 자동으로 스캔
   - 각 API의 경로, 메서드, 파라미터, 응답 형식 등을 JSON 형식으로 생성
   - `/swagger/v1/swagger.json` 경로에서 문서에 접근 가능

3. **Swagger 설정 예시**
   ```csharp
   // 1. API 컨트롤러 예시
   [ApiController]
   [Route("api/[controller]")]
   public class UsersController : ControllerBase
   {
       [HttpGet]
       public IActionResult GetUsers() { ... }

       [HttpGet("{id}")]
       public IActionResult GetUser(int id) { ... }
   }

   [ApiController]
   [Route("api/[controller]")]
   public class ProductsController : ControllerBase
   {
       [HttpGet]
       public IActionResult GetProducts() { ... }
   }

   // 2. Swagger 설정
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddSwaggerGen(c =>
       {
           c.SwaggerDoc("v1", new OpenApiInfo 
           { 
               Title = "My API", 
               Version = "v1",
               Description = "사용자 관리 API",
               Contact = new OpenApiContact
               {
                   Name = "개발팀",
                   Email = "dev@example.com"
               }
           });
       });
   }

   public void Configure(IApplicationBuilder app)
   {
       app.UseSwagger();
       app.UseSwaggerUI(c =>
       {
           c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
       });
   }
   ```

4. **Swagger UI 접근**
   - 개발 환경에서 `/swagger` 경로로 접근
   - 모든 API의 목록과 상세 정보를 보기 좋게 표시
   - 각 API를 직접 테스트할 수 있는 인터페이스 제공
   - Users와 Products 컨트롤러의 모든 API를 확인하고 테스트 가능

### 3.2 API 문서화 예시
```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    /// <summary>
    /// 모든 사용자를 조회합니다.
    /// </summary>
    /// <returns>사용자 목록</returns>
    /// <response code="200">성공적으로 조회됨</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        // ...
    }

    /// <summary>
    /// 특정 사용자를 조회합니다.
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <returns>사용자 정보</returns>
    /// <response code="200">성공적으로 조회됨</response>
    /// <response code="404">사용자를 찾을 수 없음</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        // ...
    }
}
```

## 4. REST API 보안

### 4.1 인증/인가
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        // ...
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto userDto)
    {
        // ...
    }
}
```

### 4.2 CORS 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder => builder
                .WithOrigins("https://example.com")
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
}

public void Configure(IApplicationBuilder app)
{
    app.UseCors("AllowSpecificOrigin");
}
```

## 5. REST API 모범 사례

### 5.1 응답 형식
1. **일관된 응답 구조**
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

[HttpGet]
public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
{
    try
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(new ApiResponse<IEnumerable<UserDto>>
        {
            Success = true,
            Data = users
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new ApiResponse<IEnumerable<UserDto>>
        {
            Success = false,
            Message = ex.Message
        });
    }
}
```

2. **에러 처리**
```csharp
public class ApiException : Exception
{
    public int StatusCode { get; set; }
    public string ErrorCode { get; set; }

    public ApiException(string message, int statusCode, string errorCode)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}

public void Configure(IApplicationBuilder app)
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var exception = context.Features.Get<IExceptionHandlerFeature>();
            if (exception != null)
            {
                var response = new ApiResponse<object>
                {
                    Success = false,
                    Message = exception.Error.Message
                };

                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(response);
            }
        });
    });
}
```

### 5.2 성능 최적화
1. **페이징 처리**
```csharp
public class PagedResponse<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public T Data { get; set; }
}

[HttpGet]
public async Task<ActionResult<PagedResponse<IEnumerable<UserDto>>>> GetUsers(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
{
    var (users, totalRecords) = await _userService.GetUsersAsync(pageNumber, pageSize);
    
    return Ok(new PagedResponse<IEnumerable<UserDto>>
    {
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
        TotalRecords = totalRecords,
        Data = users
    });
}
```

2. **캐싱 전략**
   - 클라이언트 측 캐싱
     ```csharp
     [HttpGet("{id}")]
     [ResponseCache(Duration = 60)]
     public async Task<ActionResult<UserDto>> GetUser(int id)
     {
         // ...
     }
     ```
     **HTTP 응답 헤더**
     ```
     Cache-Control: public, max-age=60
     Expires: Wed, 21 Oct 2023 07:28:00 GMT
     Vary: Accept-Encoding
     ```

   - 서버 측 캐싱
     ```csharp
     // 메모리 캐시
     if (_memoryCache.TryGetValue(cacheKey, out UserDto cachedUser))
     {
         return Ok(cachedUser);
     }

     // 분산 캐시 (Redis)
     var cachedUser = await _distributedCache.GetStringAsync(cacheKey);
     ```

   - 캐시 무효화
     ```csharp
     _memoryCache.Remove(cacheKey);
     // 또는
     await _distributedCache.RemoveAsync(cacheKey);