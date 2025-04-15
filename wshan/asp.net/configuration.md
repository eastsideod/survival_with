# ASP.NET Core 설정 관리

## 1. 설정 개요

### 1.1 설정이란?
- 애플리케이션의 동작을 제어하는 값들의 모음
- 환경별로 다른 설정값 사용 가능
- 런타임에 설정값 변경 가능

### 1.2 설정 소스
- JSON 파일 (appsettings.json)
- 환경 변수
- 명령줄 인수
- 사용자 비밀
- Azure Key Vault
- 사용자 지정 설정 공급자

## 2. 기본 설정

### 2.1 appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GameDB;Trusted_Connection=True;"
  },
  "GameSettings": {
    "MaxPlayers": 100,
    "MaxLevel": 100,
    "ServerName": "GameServer1"
  }
}
```

### 2.2 환경별 설정
```json
// appsettings.Development.json
{
  "GameSettings": {
    "MaxPlayers": 10,
    "ServerName": "DevServer"
  }
}

// appsettings.Production.json
{
  "GameSettings": {
    "MaxPlayers": 1000,
    "ServerName": "ProductionServer"
  }
}
```

## 3. 설정 접근

### 3.1 IConfiguration 사용
```csharp
public class GameService
{
    private readonly IConfiguration _configuration;

    public GameService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public int GetMaxPlayers()
    {
        return _configuration.GetValue<int>("GameSettings:MaxPlayers");
    }
}
```

### 3.2 옵션 패턴
```csharp
public class GameSettings
{
    public int MaxPlayers { get; set; }
    public int MaxLevel { get; set; }
    public string ServerName { get; set; }
}

public void ConfigureServices(IServiceCollection services)
{
    services.Configure<GameSettings>(Configuration.GetSection("GameSettings"));
}

public class GameService
{
    private readonly GameSettings _settings;

    public GameService(IOptions<GameSettings> settings)
    {
        _settings = settings.Value;
    }

    public int GetMaxPlayers()
    {
        return _settings.MaxPlayers;
    }
}
```

## 4. 환경 변수

### 4.1 환경 변수 설정
```powershell
$env:GameSettings__MaxPlayers = "100"
$env:GameSettings__ServerName = "ProductionServer"
```

### 4.2 환경 변수 접근
```csharp
public class GameService
{
    private readonly IConfiguration _configuration;

    public GameService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetServerName()
    {
        return _configuration["GameSettings:ServerName"];
    }
}
```

## 5. 사용자 비밀

### 5.1 비밀 관리자 도구
```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=GameDB;User Id=sa;Password=secret;"
```

### 5.2 비밀 사용
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<GameDbContext>(options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection")));
}
```

## 6. Azure Key Vault

### 6.1 Key Vault 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAzureKeyVault(
        new Uri($"https://{Configuration["KeyVault:Vault"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}
```

### 6.2 비밀 접근
```csharp
public class GameService
{
    private readonly IConfiguration _configuration;

    public GameService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetApiKey()
    {
        return _configuration["GameApiKey"];
    }
}
```

## 7. 설정 변경 감지

### 7.1 IOptionsSnapshot 사용
```csharp
public class GameService
{
    private readonly GameSettings _settings;

    public GameService(IOptionsSnapshot<GameSettings> settings)
    {
        _settings = settings.Value;
    }

    public int GetMaxPlayers()
    {
        return _settings.MaxPlayers;
    }
}
```

### 7.2 IOptionsMonitor 사용
```csharp
public class GameService : IDisposable
{
    private readonly GameSettings _settings;
    private readonly IDisposable _changeToken;

    public GameService(IOptionsMonitor<GameSettings> settings)
    {
        _settings = settings.CurrentValue;
        _changeToken = settings.OnChange(newSettings =>
        {
            // 설정 변경 시 처리
        });
    }

    public void Dispose()
    {
        _changeToken?.Dispose();
    }
}
```

## 8. 사용자 지정 설정 공급자

### 8.1 설정 공급자 구현
```csharp
public class CustomConfigurationProvider : ConfigurationProvider
{
    public override void Load()
    {
        // 설정 로드 로직
        Data["CustomSetting"] = "CustomValue";
    }
}

public class CustomConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new CustomConfigurationProvider();
    }
}
```

### 8.2 공급자 등록
```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.Add(new CustomConfigurationSource());
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

## 9. 설정 검증

### 9.1 데이터 어노테이션 사용
```csharp
public class GameSettings
{
    [Range(1, 1000)]
    public int MaxPlayers { get; set; }

    [Required]
    public string ServerName { get; set; }
}

public void ConfigureServices(IServiceCollection services)
{
    services.AddOptions<GameSettings>()
        .Bind(Configuration.GetSection("GameSettings"))
        .ValidateDataAnnotations();
}
```

### 9.2 사용자 지정 검증
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddOptions<GameSettings>()
        .Bind(Configuration.GetSection("GameSettings"))
        .Validate(settings =>
        {
            if (settings.MaxPlayers < 1)
            {
                return false;
            }
            return true;
        }, "MaxPlayers must be greater than 0");
}
```

## 10. 설정 모범 사례

### 10.1 보안
- 민감한 정보는 환경 변수나 Key Vault에 저장
- 개발 환경과 프로덕션 환경 분리
- 설정값 암호화 고려

### 10.2 성능
- 자주 변경되는 설정은 IOptionsSnapshot 사용
- 설정 변경 감지 시 적절한 캐싱 전략 사용
- 불필요한 설정 로드 방지

### 10.3 유지보수
- 설정값 문서화
- 환경별 설정 파일 분리
- 설정값 변경 이력 관리

## 11. 참고 자료
- [ASP.NET Core 설정](https://learn.microsoft.com/aspnet/core/fundamentals/configuration)
- [옵션 패턴](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/options)
- [Azure Key Vault](https://learn.microsoft.com/azure/key-vault/general/overview)
- [사용자 비밀](https://learn.microsoft.com/aspnet/core/security/app-secrets) 