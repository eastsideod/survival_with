# ASP.NET Core 의존성 주입

## 1. 소개
ASP.NET Core는 의존성 주입을 핵심 설계 원칙으로 채택했습니다. 내장된 DI 컨테이너를 통해 서비스의 생명주기와 의존성을 관리하며, 컨트롤러, 미들웨어, 서비스 등 모든 컴포넌트에서 DI를 활용할 수 있습니다.

## 2. ASP.NET Core에서의 DI 설정

### 2.1 서비스 등록
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 기본 서비스 등록
        services.AddControllers();
        services.AddDbContext<ApplicationDbContext>();
        
        // 사용자 정의 서비스 등록
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.AddTransient<IEmailService, SmtpEmailService>();
    }
}
```

### 2.2 서비스 수명 주기
ASP.NET Core는 세 가지 서비스 수명 주기를 제공합니다:

1. **Singleton**
   - 애플리케이션 전체에서 단일 인스턴스
   - 첫 번째 요청 시 생성
   - `services.AddSingleton<TService, TImplementation>()`

2. **Scoped**
   - HTTP 요청당 하나의 인스턴스
   - 요청 시작 시 생성, 종료 시 해제
   - `services.AddScoped<TService, TImplementation>()`

3. **Transient**
   - 서비스 요청마다 새로운 인스턴스
   - 사용 후 즉시 해제
   - `services.AddTransient<TService, TImplementation>()`

## 3. ASP.NET Core 컴포넌트에서의 DI 사용

### 3.1 컨트롤러에서의 DI
```csharp
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
    
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        _logger.LogInformation("Getting user {Id}", id);
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
            
        return user;
    }
}
```

### 3.2 미들웨어에서의 DI
```csharp
public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomMiddleware> _logger;
    
    public CustomMiddleware(
        RequestDelegate next,
        ILogger<CustomMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Custom middleware processing");
        await _next(context);
    }
}
```

### 3.3 서비스에서의 DI
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;
    
    public UserService(
        IUserRepository repository,
        IEmailService emailService,
        ILogger<UserService> logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }
}
```

## 4. ASP.NET Core DI 기능

### 4.1 옵션 패턴
```csharp
// 설정 클래스
public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
}

// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
}

// 서비스에서 사용
public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }
}
```

### 4.2 팩토리 패턴
```csharp
public interface IUserServiceFactory
{
    IUserService Create(string type);
}

public class UserServiceFactory : IUserServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public UserServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IUserService Create(string type)
    {
        return type switch
        {
            "admin" => _serviceProvider.GetRequiredService<AdminUserService>(),
            "regular" => _serviceProvider.GetRequiredService<RegularUserService>(),
            _ => throw new ArgumentException("Invalid user type")
        };
    }
}
```

## 5. ASP.NET Core DI 모범 사례

### 5.1 서비스 등록
- 명확한 수명 주기 선택
- 인터페이스 기반 등록
- 확장 메서드 활용
- 설정 관련 서비스는 옵션 패턴 사용

### 5.2 의존성 관리
- 생성자 주입 사용
- 최소한의 의존성 유지
- 순환 의존성 방지
- 명확한 책임 분리

### 5.3 성능 고려사항
- 적절한 수명 주기 선택
- 불필요한 서비스 해결 방지
- 리소스 관리 주의
- 스코프 관리

## 6. 참고 자료
- [ASP.NET Core 의존성 주입](https://learn.microsoft.com/aspnet/core/fundamentals/dependency-injection)
- [ASP.NET Core 옵션 패턴](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/options)
- [ASP.NET Core 미들웨어](https://learn.microsoft.com/aspnet/core/fundamentals/middleware) 