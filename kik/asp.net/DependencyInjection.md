
# Dependency Injection

[toc]

## link

https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-9.0

## Singleton 

- 최초 요청 발생 시 생성되며 이후에는 동일한 객체 재사용.

**Singleton 서비스 정의하기**

```csharp

// Interface 정의는 해도, 하지 않아도 상관없음.
public interface ISampleSingletonService
{
}

public class SampleSingletonService : ISampleSingletonService
{
    private readonly ILogger<SampleSingletonService> _logger;

    public SampleSingletonService(ILogger<SampleSingletonService> logger)
    {
        _logger = logger;
        _logger.LogInformation("SampleSingletonService created...");
    }
}
```

**Singleton 서비스 등록하기**

```csharp
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    // 생략...

    // Singleton Service 추가.
    builder.Services.AddSingleton<ISampleSingletonService, SampleSingletonService>();

    var app = builder.Build();
```

**Controller 에 의존성 주입하기(생성자)**

```csharp
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ISampleSingletonService _sampleSingletonService;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        ISampleSingletonService singletonService)
    {
        _logger = logger;
        _sampleSingletonService = singletonService;
    }

```

---

## Transient

- 모든 접근 대상마다 새롭게 생성(요청 내에서도 다른 컨트롤러에 새로운 객체 전달)


**Transient 서비스 정의하기**

```csharp

// Interface 정의는 해도, 하지 않아도 상관없음.
public interface ISampleTransientService
{
}


public class SampleTransientService : ISampleTransientService
{
    private readonly ILogger<SampleTransientService> _logger;

    public SampleTransientService(ILogger<SampleTransientService> logger)
    {
        _logger = logger;

        _logger.LogInformation("SampleTransientService created...");
    }
}
```

**Transient 서비스 등록하기**

```csharp
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    // 생략...

    // Transient Service 추가.
    builder.Services.AddTransient<ISampleTransientService, SampleTransientService>();

    var app = builder.Build();
```

**Transient 에 의존성 주입하기(생성자)**

```csharp
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ISampleTransientService _sampleTransientService;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        ISampleTransientService sampleTransientService)
    {
        _logger = logger;
        _sampleTransientService = sampleTransientService;
    }

```

---

## Scoped

- 개별 요청마다 새로 생성.


---

## 주의 사항

- Singleton 서비스에 Scoped 또는 Transient 서비스를 Inject하지 말것(싱글톤 처럼 동작하게 된다)
- Scoped 서비스에 Transient 서비스를 Inject 하지 말 것(요청마다 만들어지는 것 처럼 동작하게 한다)