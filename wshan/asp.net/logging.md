# ASP.NET Core 로깅

## 1. 로깅 개요

### 1.1 로깅의 중요성
- 애플리케이션 동작 모니터링
- 문제 진단 및 해결
- 성능 분석
- 보안 감사

### 1.2 로깅 레벨
1. **Trace**: 가장 상세한 정보
2. **Debug**: 개발 시 디버깅용
3. **Information**: 일반적인 정보
4. **Warning**: 예상치 못한 상황
5. **Error**: 현재 요청 처리 실패
6. **Critical**: 전체 애플리케이션 실패

## 2. 기본 로깅 설정

### 2.1 로깅 구성
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
        builder.AddEventLog();
        builder.SetMinimumLevel(LogLevel.Information);
    });
}
```

### 2.2 로깅 사용
```csharp
public class UserService
{
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
    }

    public async Task<User> GetUserAsync(int id)
    {
        _logger.LogInformation("사용자 조회 시작: {UserId}", id);
        
        try
        {
            var user = await _repository.GetUserAsync(id);
            _logger.LogInformation("사용자 조회 성공: {UserId}", id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "사용자 조회 실패: {UserId}", id);
            throw;
        }
    }
}
```

## 3. 구조화된 로깅

### 3.1 Serilog 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/myapp-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    services.AddLogging(builder => builder.AddSerilog());
}
```

### 3.2 구조화된 로그 예시
```csharp
public class OrderService
{
    private readonly ILogger<OrderService> _logger;

    public async Task ProcessOrderAsync(Order order)
    {
        _logger.LogInformation(
            "주문 처리 시작: {@Order}", 
            new { order.Id, order.TotalAmount, order.CustomerId });

        try
        {
            // 주문 처리 로직
            _logger.LogInformation(
                "주문 처리 완료: {@Order}", 
                new { order.Id, Status = "Completed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "주문 처리 실패: {@Order}", 
                new { order.Id, Error = ex.Message });
            throw;
        }
    }
}
```

## 4. 로깅 모범 사례

### 4.1 로그 메시지 작성
1. **명확한 메시지**
   - 동작 중심의 메시지
   - 구체적인 정보 포함
   - 일관된 형식 사용

2. **컨텍스트 정보**
   - 요청 ID
   - 사용자 ID
   - 세션 ID
   - 타임스탬프

3. **예외 로깅**
   - 전체 스택 트레이스
   - 내부 예외 포함
   - 관련 컨텍스트 정보

### 4.2 성능 고려사항
1. **로그 레벨 관리**
   - 프로덕션에서는 적절한 레벨 설정
   - 불필요한 로그 제거
   - 로그 샘플링 고려

2. **비동기 로깅**
   - 로깅이 성능에 영향을 주지 않도록
   - 백그라운드 처리
   - 버퍼링 사용

3. **로그 로테이션**
   - 파일 크기 제한
   - 보관 기간 설정
   - 압축 정책

### 4.3 보안 고려사항
1. **민감한 정보**
   - 개인정보 제외
   - 비밀번호 마스킹
   - 토큰 값 숨김

2. **로그 접근 제어**
   - 권한 기반 접근
   - 암호화된 저장
   - 감사 로그 유지

## 5. 로깅 모니터링

### 5.1 로그 집계
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(builder =>
    {
        builder.AddApplicationInsights();
        builder.AddAzureWebAppDiagnostics();
    });
}
```

### 5.2 로그 분석
```csharp
public class LogAnalyzer
{
    private readonly ILogger<LogAnalyzer> _logger;

    public void AnalyzeLogs()
    {
        // 에러 패턴 분석
        _logger.LogInformation("에러 발생 빈도 분석 시작");
        
        // 성능 메트릭스 수집
        _logger.LogInformation("응답 시간 분석 시작");
        
        // 사용자 행동 분석
        _logger.LogInformation("사용자 행동 패턴 분석 시작");
    }
}
```

## 6. 로깅 확장

### 6.1 커스텀 로그 프로바이더
```csharp
public class CustomLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new CustomLogger(categoryName);
    }

    public void Dispose()
    {
    }
}

public class CustomLogger : ILogger
{
    private readonly string _categoryName;

    public CustomLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        // 커스텀 로깅 로직
    }
}
```

### 6.2 로그 필터링
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(builder =>
    {
        builder.AddFilter((category, level) =>
        {
            if (category.Contains("Microsoft"))
                return level >= LogLevel.Warning;
            return level >= LogLevel.Information;
        });
    });
}
```

## 7. 참고 자료
- [ASP.NET Core 로깅](https://learn.microsoft.com/aspnet/core/fundamentals/logging)
- [Serilog](https://serilog.net/)
- [Application Insights](https://learn.microsoft.com/azure/azure-monitor/app/app-insights-overview) 