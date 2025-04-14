# ASP.NET Core 미들웨어

## 1. 미들웨어 개요

### 1.1 미들웨어란?
- ASP.NET Core에서 모든 HTTP 요청에 공통적으로 적용되는 처리 로직을 구현하는 컴포넌트
- 모든 요청은 반드시 미들웨어 파이프라인을 거치게 됨
- 각 미들웨어는 요청을 검증하거나 변환하는 역할 수행

### 1.2 미들웨어의 핵심 특징
1. **전역적 처리**
   - 모든 HTTP 요청에 자동 적용
   - 컨트롤러에 개별 구현 불필요
   - 일관된 처리 보장

2. **파이프라인 기반 처리**
   - 요청이 순차적으로 여러 미들웨어를 거침
   - 각 단계에서 요청 검증/변환 가능
   - 필요시 파이프라인 조기 종료 가능

3. **재사용성과 유지보수성**
   - 공통 로직의 중앙화
   - 버그 수정이 한 곳에서만 필요
   - 기능 추가/제거 용이

## 2. 미들웨어 구현

### 2.1 기본 구조
```csharp
public class CustomMiddleware
{
    private readonly RequestDelegate _next;

    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 요청 처리 전 로직
        await _next(context); // 다음 미들웨어 호출
        // 응답 처리 후 로직
    }
}
```

### 2.2 등록 방법

#### 2.2.1 직접 등록
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseMiddleware<CustomMiddleware>();
}
```

#### 2.2.2 확장 메서드 사용 (권장)
```csharp
public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CustomMiddleware>();
    }
}

// 사용
public void Configure(IApplicationBuilder app)
{
    app.UseCustomMiddleware();
}
```

## 3. 미들웨어 등록 순서

### 3.1 기본 순서
1. **예외 처리 미들웨어**
   ```csharp
   app.UseExceptionHandler("/error");
   ```

2. **로깅 미들웨어**
   ```csharp
   app.UseRequestLogging();
   ```

3. **인증/인가 미들웨어**
   ```csharp
   app.UseAuthentication();
   app.UseAuthorization();
   ```

4. **라우팅 미들웨어**
   ```csharp
   app.UseRouting();
   ```

5. **엔드포인트 실행**
   ```csharp
   app.UseEndpoints(endpoints => {
       endpoints.MapControllers();
   });
   ```

### 3.2 올바른 등록 예시
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseExceptionHandler("/error");
    app.UseRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRouting();
    app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
    });
}
```

## 4. 게임 서버에서 자주 사용하는 미들웨어

### 4.1 인증/인가 미들웨어
```csharp
public class GameAuthenticationMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!IsValidGameToken(context))
        {
            context.Response.StatusCode = 401;
            return;
        }
        await _next(context);
    }
}
```

### 4.2 로깅 미들웨어
```csharp
public class GameLoggingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        await _next(context);
        var duration = DateTime.UtcNow - startTime;
        _logger.LogInformation($"게임 요청 처리: {context.Request.Path} - {duration.TotalMilliseconds}ms");
    }
}
```

### 4.3 예외 처리 미들웨어
```csharp
public class GameExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (GameException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
    }
}
```

## 5. 미들웨어 모범 사례

1. **성능 고려**
   - 불필요한 미들웨어 제거
   - 비동기 작업 사용
   - 메모리 사용 최적화

2. **에러 처리**
   - 모든 예외 적절히 처리
   - 의미 있는 에러 메시지 제공
   - 로깅 포함

3. **보안**
   - 입력 데이터 검증
   - 적절한 헤더 설정
   - CORS 정책 설정

4. **테스트 용이성**
   - 의존성 주입 사용
   - 단위 테스트 가능한 구조
   - 모듈화된 설계