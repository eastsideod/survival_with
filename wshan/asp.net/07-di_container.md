# DI Container (의존성 주입 컨테이너)

## 1. DI Container 소개

### 1.1 DI Container란?
- 객체의 생성과 의존성 관리를 담당하는 프레임워크
- 객체의 생명주기 관리
- 의존성 해결 및 주입 자동화
- 서비스 등록 및 해결을 위한 중앙 집중식 관리

### 1.2 ASP.NET Core의 DI Container 구조
1. **IServiceCollection**
   - 서비스 등록을 위한 컬렉션 인터페이스
   - 서비스와 구현체의 매핑 정보 저장
   - `ConfigureServices` 메서드에서 사용

2. **IServiceProvider**
   - 실제 DI Container 인터페이스
   - 등록된 서비스의 인스턴스 생성 및 관리
   - 의존성 해결 담당

3. **ServiceDescriptor**
   - 서비스 등록 정보를 담는 클래스
   - 서비스 타입, 구현체 타입, 수명 주기 정보 포함

## 2. DI Container의 동작 방식

### 2.1 서비스 등록 과정
```csharp
// 1. 서비스 등록 (IServiceCollection 사용)
public void ConfigureServices(IServiceCollection services)
{
    // 기본 등록
    services.AddScoped<IService, Service>();
    
    // 수명 주기 지정
    services.AddSingleton<ICacheService, CacheService>();
    services.AddTransient<IValidator, Validator>();
    
    // 팩토리 등록
    services.AddScoped<IService>(sp => new Service(sp.GetRequiredService<ILogger>()));
}
```

### 2.2 동일 인터페이스에 대한 다중 구현체 등록
```csharp
public interface IGameService
{
    void ProcessGame();
}

public class GameServiceA : IGameService
{
    public void ProcessGame() => Console.WriteLine("GameServiceA 처리");
}

public class GameServiceB : IGameService
{
    public void ProcessGame() => Console.WriteLine("GameServiceB 처리");
}

// 서비스 등록
public void ConfigureServices(IServiceCollection services)
{
    // 1. 명시적 이름으로 등록
    services.AddScoped<IGameService, GameServiceA>("ServiceA");
    services.AddScoped<IGameService, GameServiceB>("ServiceB");

    // 2. 팩토리 패턴으로 등록
    services.AddScoped<IGameService>(sp => 
        sp.GetRequiredService<GameServiceFactory>().Create("ServiceA"));
}

// 사용 예시
public class GameController
{
    private readonly IEnumerable<IGameService> _gameServices;
    private readonly IGameService _specificService;

    // 1. 모든 구현체 주입
    public GameController(IEnumerable<IGameService> gameServices)
    {
        _gameServices = gameServices;
    }

    // 2. 특정 구현체 주입
    public GameController([FromKeyedServices("ServiceA")] IGameService gameService)
    {
        _specificService = gameService;
    }

    public void ProcessGames()
    {
        // 모든 구현체 실행
        foreach (var service in _gameServices)
        {
            service.ProcessGame();
        }

        // 특정 구현체 실행
        _specificService.ProcessGame();
    }
}
```

### 2.3 DI 팩토리 패턴
DI 팩토리 패턴은 서비스의 생성과 의존성 주입을 더 유연하게 관리할 수 있게 해주는 패턴입니다.

1. **기본 팩토리 패턴**
```csharp
// 1. 팩토리 인터페이스 정의
public interface IGameServiceFactory
{
    IGameService Create(string type);
}

// 2. 팩토리 구현
public class GameServiceFactory : IGameServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public GameServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IGameService Create(string type)
    {
        return type switch
        {
            "PvP" => _serviceProvider.GetRequiredService<PvPGameService>(),
            "PvE" => _serviceProvider.GetRequiredService<PvEGameService>(),
            "Coop" => _serviceProvider.GetRequiredService<CoopGameService>(),
            _ => throw new ArgumentException($"Unknown game type: {type}")
        };
    }
}

// 3. 서비스 등록
public void ConfigureServices(IServiceCollection services)
{
    // 각 게임 서비스 구현체 등록
    services.AddScoped<PvPGameService>();
    services.AddScoped<PvEGameService>();
    services.AddScoped<CoopGameService>();
    
    // 팩토리 등록
    services.AddScoped<IGameServiceFactory, GameServiceFactory>();
}

// 4. 컨트롤러에서 사용
public class GameController : ControllerBase
{
    private readonly IGameServiceFactory _gameServiceFactory;
    
    public GameController(IGameServiceFactory gameServiceFactory)
    {
        _gameServiceFactory = gameServiceFactory;
    }
    
    [HttpPost("start")]
    public IActionResult StartGame([FromBody] StartGameRequest request)
    {
        var gameService = _gameServiceFactory.Create(request.GameType);
        var result = gameService.StartGame(request);
        return Ok(result);
    }
}
```

2. **고급 팩토리 패턴**
```csharp
// 1. 게임 타입별 설정 클래스
public class GameSettings
{
    public string GameType { get; set; }
    public int MaxPlayers { get; set; }
    public TimeSpan TimeLimit { get; set; }
}

// 2. 설정 기반 팩토리
public class GameServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    
    public GameServiceFactory(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }
    
    public IGameService Create(GameSettings settings)
    {
        var service = settings.GameType switch
        {
            "PvP" => _serviceProvider.GetRequiredService<PvPGameService>(),
            "PvE" => _serviceProvider.GetRequiredService<PvEGameService>(),
            "Coop" => _serviceProvider.GetRequiredService<CoopGameService>(),
            _ => throw new ArgumentException($"Unknown game type: {settings.GameType}")
        };
        
        // 설정 적용
        service.Configure(settings);
        return service;
    }
}

// 3. 비동기 팩토리
public class AsyncGameServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public AsyncGameServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task<IGameService> CreateAsync(string type)
    {
        var service = type switch
        {
            "PvP" => _serviceProvider.GetRequiredService<PvPGameService>(),
            "PvE" => _serviceProvider.GetRequiredService<PvEGameService>(),
            "Coop" => _serviceProvider.GetRequiredService<CoopGameService>(),
            _ => throw new ArgumentException($"Unknown game type: {type}")
        };
        
        // 비동기 초기화
        await service.InitializeAsync();
        return service;
    }
}
```

3. **팩토리 패턴의 장점**
   - **유연성**: 런타임에 구현체 선택 가능
   - **확장성**: 새로운 구현체 추가 용이
   - **테스트 용이성**: 모의 객체 주입이 용이
   - **설정 관리**: 구현체별 설정 관리 가능
   - **생명주기 관리**: 각 구현체의 생명주기 독립적 관리

4. **팩토리 패턴 사용 시 고려사항**
   - **성능**: 팩토리 호출 오버헤드 고려
   - **스레드 안전성**: 동시성 제어 필요
   - **의존성 순환**: 팩토리와 서비스 간 순환 참조 방지
   - **설정 관리**: 설정 변경 시 캐시 무효화 필요
   - **예외 처리**: 구현체 생성 실패 시 적절한 예외 처리

5. **게임 서버에서의 활용 예시**
```csharp
// 게임 모드별 서비스 생성
public class GameModeFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public GameModeFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IGameMode CreateMode(GameModeType type, GameSettings settings)
    {
        var mode = type switch
        {
            GameModeType.DeathMatch => _serviceProvider.GetRequiredService<DeathMatchMode>(),
            GameModeType.TeamBattle => _serviceProvider.GetRequiredService<TeamBattleMode>(),
            GameModeType.Survival => _serviceProvider.GetRequiredService<SurvivalMode>(),
            _ => throw new ArgumentException($"Unknown game mode: {type}")
        };
        
        mode.Initialize(settings);
        return mode;
    }
}

// 게임 세션 생성
public class GameSessionFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IGameModeFactory _modeFactory;
    
    public GameSessionFactory(
        IServiceProvider serviceProvider,
        IGameModeFactory modeFactory)
    {
        _serviceProvider = serviceProvider;
        _modeFactory = modeFactory;
    }
    
    public async Task<GameSession> CreateSessionAsync(CreateSessionRequest request)
    {
        var mode = _modeFactory.CreateMode(request.ModeType, request.Settings);
        var session = _serviceProvider.GetRequiredService<GameSession>();
        
        await session.InitializeAsync(mode, request.Players);
        return session;
    }
}
```

### 2.4 의존성 해결 과정
```csharp
// 1. 직접 해결 (IServiceProvider 사용)
public class SomeClass
{
    private readonly IServiceProvider _serviceProvider;
    
    public SomeClass(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void DoSomething()
    {
        var service = _serviceProvider.GetRequiredService<IService>();
        // 사용
    }
}

// 2. 생성자 주입 (자동으로 IServiceProvider가 처리)
public class Client
{
    private readonly IService _service;
    
    public Client(IService service)  // DI Container가 자동으로 주입
    {
        _service = service;
    }
}
```

### 2.5 생명주기 관리
```csharp
// 1. Singleton
var singleton1 = serviceProvider.GetService<IService>();
var singleton2 = serviceProvider.GetService<IService>();
// singleton1 == singleton2

// 2. Scoped
using (var scope = serviceProvider.CreateScope())
{
    var scoped1 = scope.ServiceProvider.GetService<IService>();
    var scoped2 = scope.ServiceProvider.GetService<IService>();
    // scoped1 == scoped2
}

// 3. Transient
var transient1 = serviceProvider.GetService<IService>();
var transient2 = serviceProvider.GetService<IService>();
// transient1 != transient2
```

### 2.6 실수로 인한 다중 구현체 등록 문제
같은 인터페이스에 대해 다른 클래스를 실수로 추가했을 때 발생할 수 있는 문제들:

1. **의존성 해결 실패**
   ```csharp
   // 실수로 같은 인터페이스에 대해 두 개의 구현체를 등록
   services.AddScoped<IGameService, GameServiceA>();
   services.AddScoped<IGameService, GameServiceB>();  // 실수로 추가

   // 컨트롤러에서 사용 시
   public class GameController
   {
       private readonly IGameService _gameService;  // 어떤 구현체가 주입될지 불명확

       public GameController(IGameService gameService)
       {
           _gameService = gameService;  // 런타임 에러 발생
       }
   }
   ```

2. **발생 가능한 문제**
   - **런타임 예외**: DI 컨테이너가 어떤 구현체를 주입해야 할지 결정할 수 없어 예외 발생
   - **예상치 못한 동작**: 마지막에 등록된 구현체가 사용되어 의도하지 않은 동작 발생
   - **디버깅 어려움**: 의존성 주입 문제는 컴파일 타임에 발견되지 않아 디버깅이 어려움
   - **코드 유지보수성 저하**: 의도하지 않은 구현체가 사용되어 버그 발생 가능성 증가

3. **해결 방법**
   - **명시적 이름 사용**: 각 구현체에 고유한 이름을 지정하여 구분
     ```csharp
     services.AddKeyedScoped<IGameService, GameServiceA>("ServiceA");
     services.AddKeyedScoped<IGameService, GameServiceB>("ServiceB");
     ```
   - **팩토리 패턴 사용**: 구현체 선택 로직을 팩토리로 분리
     ```csharp
     services.AddScoped<IGameService>(sp => 
         sp.GetRequiredService<GameServiceFactory>().Create("ServiceA"));
     ```
   - **코드 리뷰 강화**: 의존성 주입 관련 코드를 리뷰할 때 주의 깊게 검토
   - **정적 분석 도구 활용**: 같은 인터페이스에 대한 다중 등록을 감지하는 도구 사용

4. **예방 방법**
   - **명확한 네이밍**: 각 구현체의 목적을 명확히 하는 이름 사용
   - **문서화**: 각 구현체의 용도와 사용 시점을 문서화
   - **테스트**: 의존성 주입 관련 테스트 코드 작성
   - **아키텍처 검토**: 구현체 추가 전 아키텍처 검토 수행

## 3. DI Container의 장점

### 3.1 코드 품질 향상
- 결합도 감소
- 테스트 용이성 증가
- 코드 재사용성 향상
- 유지보수성 개선

### 3.2 개발 생산성
- 의존성 관리 자동화
- 설정 중앙화
- 일관된 객체 생성
- 리소스 관리 용이

### 3.3 애플리케이션 구조
- 모듈화 촉진
- 관심사 분리
- 확장성 향상
- 유연한 구성

## 4. 주요 DI Container 구현체

### 4.1 .NET Core 내장 DI Container
- Microsoft.Extensions.DependencyInjection
- 경량화된 기본 구현
- ASP.NET Core와 통합
- `IServiceCollection`과 `IServiceProvider` 인터페이스 제공

### 4.2 인기 있는 DI Container
- Autofac
- Ninject
- Unity
- Simple Injector
- Castle Windsor

## 5. DI Container 선택 기준

### 5.1 성능
- 의존성 해결 속도
- 메모리 사용량
- 스레드 안전성

### 5.2 기능
- 수명 주기 관리
- 인터셉터 지원
- 속성 주입
- 자식 컨테이너

### 5.3 사용성
- 설정 용이성
- 문서화 품질
- 커뮤니티 지원
- 학습 곡선

## 6. DI Container 사용 시 고려사항

### 6.1 서비스 등록
- 적절한 수명 주기 선택
- 순환 의존성 방지
- 명확한 인터페이스 정의

### 6.2 성능
- 불필요한 서비스 해결 방지
- 적절한 캐싱 전략
- 리소스 관리

### 6.3 테스트
- Mock 객체 주입
- 테스트 환경 구성
- 격리된 테스트

## 7. 참고 자료
- [Dependency Injection in .NET](https://docs.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Inversion of Control Containers and the Dependency Injection pattern](https://martinfowler.com/articles/injection.html)
- [Dependency Injection Principles, Practices, and Patterns](https://www.manning.com/books/dependency-injection-principles-practices-patterns) 