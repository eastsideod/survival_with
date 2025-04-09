
# ASP.NET Core 고급 면접 질문 및 모범 답변

---

### 1. Dependency Injection이란 무엇이며 ASP.NET Core에서 어떻게 사용하는가요?
DI는 객체 간의 의존성을 외부에서 주입받아 결합도를 낮추는 설계 패턴입니다. ASP.NET Core는 DI를 기본적으로 지원하며, `ConfigureServices()`에서 인터페이스와 구현체를 등록하고 생성자 주입 방식으로 사용합니다.
```csharp
services.AddScoped<IProductService, ProductService>();
```

---

### 2. ASP.NET Core에서 Custom Middleware를 작성하려면?
`IMiddleware`를 구현하거나 `RequestDelegate`를 매개로 하는 클래스를 만들어 `app.UseMiddleware<>()` 또는 `app.Use()`로 등록합니다.
```csharp
public class MyMiddleware
{
    private readonly RequestDelegate _next;
    public MyMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        // 작업 전 처리
        await _next(context);
        // 작업 후 처리
    }
}
```

---

### 3. SignalR의 사용 사례와 동작 원리는?
SignalR은 실시간 웹 기능(채팅, 알림 등)을 구현하는 라이브러리입니다. WebSocket을 기반으로 하며, 연결된 클라이언트에 서버에서 메시지를 push 방식으로 전달할 수 있습니다.

---

### 4. ASP.NET Core의 Pipeline 구조를 설명해주세요.
ASP.NET Core는 요청/응답 파이프라인을 Middleware 체인으로 구성합니다. 각 미들웨어는 요청을 처리하고 다음 미들웨어로 전달하거나 응답을 반환합니다. 순서가 매우 중요합니다.

---

### 5. IHostedService란 무엇이고 언제 사용하나요?
`IHostedService`는 백그라운드 작업을 처리하기 위한 인터페이스입니다. 예: 큐 소비, 타이머 기반 작업. `BackgroundService`를 상속해 사용합니다.

---

### 6. Kestrel 서버란 무엇이며 IIS와의 차이는?
Kestrel은 ASP.NET Core에서 사용하는 크로스 플랫폼 웹 서버입니다. IIS는 Windows 전용이며, Kestrel은 기본적으로 IIS 앞에 Reverse Proxy 형태로 함께 사용됩니다.

---

### 7. JWT 인증 흐름과 구현 방법은?
JWT 토큰을 로그인 시 서버에서 발급하고, 이후 요청에서 Authorization 헤더를 통해 전달합니다. 서버는 토큰의 서명과 유효 기간을 확인하여 인증을 수행합니다.
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* 옵션 설정 */ });
```

---

### 8. ASP.NET Core에서 Configuration을 동적으로 바꾸는 방법은?
`IConfiguration`을 통해 앱 설정을 읽으며, `reloadOnChange: true`로 JSON 설정 파일 변경 시 자동 반영 가능합니다. `IOptionsSnapshot`을 이용하면 실시간 변경도 반영됩니다.

---

### 9. gRPC를 ASP.NET에서 사용하는 이유와 장점은?
gRPC는 고성능, 타입 안전, 양방향 스트리밍을 지원하는 RPC 프레임워크입니다. Protobuf 기반이므로 경량화되고, 마이크로서비스 간 통신에 적합합니다.

---

### 10. 성능 개선을 위한 캐싱 전략 (In-Memory Cache, Distributed Cache 등)은?
- In-Memory: 단일 서버에서 빠른 캐싱
- Distributed Cache(Redis): 다중 서버 환경에서 공유 캐시
- Response Cache: 응답 자체를 캐싱
- OutputCache (ASP.NET 8): 결과 전체 캐싱

---

### 11. ASP.NET Core에서 로그를 구성하는 방법은?
`ILogger<T>` 인터페이스를 통해 DI 기반 로그 주입이 가능하며, 콘솔, 파일, Azure 등 다양한 대상에 로그를 출력할 수 있습니다. Serilog, NLog 등의 외부 라이브러리도 통합 가능합니다.

---

### 12. 환경별 설정 관리 방법은?
`appsettings.Development.json`, `appsettings.Production.json`과 같이 환경별 JSON 파일을 구성하며, 환경 변수(`ASPNETCORE_ENVIRONMENT`)로 환경을 구분합니다.

---

### 13. Health Check 기능이란?
애플리케이션의 상태를 모니터링하기 위한 기능입니다. `services.AddHealthChecks()`와 `app.UseHealthChecks()`로 구성하며, Kubernetes liveness/readiness probe에도 활용됩니다.

---

### 14. Minimal API란?
ASP.NET 6+에서 도입된 경량화된 API 스타일입니다. 컨트롤러 없이 라우팅과 핸들러를 간단히 구성할 수 있어 작은 프로젝트에 적합합니다.
```csharp
app.MapGet("/hello", () => "Hello world");
```

---

### 15. ASP.NET Core에서 성능 모니터링 도구는?
- Application Insights
- Prometheus + Grafana
- ELK Stack
- dotnet-counters, dotnet-trace 등 도구 사용

---

