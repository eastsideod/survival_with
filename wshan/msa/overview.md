# 마이크로서비스 아키텍처 (MSA)

## 1. MSA 개요

### 1.1 MSA란?
- 독립적으로 배포 가능한 작은 서비스들의 집합
- 각 서비스는 단일 책임을 가짐
- 서비스 간 느슨한 결합
- 독립적인 확장과 배포 가능

### 1.2 MSA의 장단점
1. **장점**
   - 독립적인 확장성
   - 기술 스택의 유연성
   - 장애 격리
   - 팀 간 독립성

2. **단점**
   - 분산 시스템의 복잡성
   - 데이터 일관성 관리
   - 운영 오버헤드
   - 테스트의 어려움

## 2. MSA 설계

### 2.1 서비스 분리 원칙
```csharp
// 게임 시스템 예시
GameSystem/
├── GameService/        # 게임 로직
│   ├── GameController/
│   ├── GameLogic/
│   └── GameState/
├── UserService/        # 사용자 관리
│   ├── UserController/
│   ├── Authentication/
│   └── Profile/
├── MatchmakingService/ # 매치메이킹
│   ├── MatchController/
│   ├── MatchAlgorithm/
│   └── Queue/
└── ChatService/        # 채팅
    ├── ChatController/
    ├── MessageHub/
    └── Room/
```

### 2.2 서비스 간 통신
1. **동기 통신 (HTTP/REST)**
   ```csharp
   public class GameService
   {
       private readonly HttpClient _httpClient;

       public async Task<UserInfo> GetUserInfo(string userId)
       {
           var response = await _httpClient.GetAsync($"http://userservice/api/users/{userId}");
           return await response.Content.ReadAsAsync<UserInfo>();
       }
   }
   ```

2. **비동기 통신 (메시지 큐)**
   ```csharp
   public class MatchmakingService
   {
       private readonly IMessageBus _messageBus;

       public async Task StartMatchmaking(GameRequest request)
       {
           await _messageBus.PublishAsync("matchmaking.started", request);
       }
   }
   ```

3. **gRPC 통신**
   ```csharp
   // 프로토콜 정의
   service GameService {
       rpc GetGameState (GameRequest) returns (GameState);
       rpc UpdateGameState (GameState) returns (Empty);
   }

   // 서비스 구현
   public class GameServiceImpl : GameService.GameServiceBase
   {
       public override async Task<GameState> GetGameState(GameRequest request, ServerCallContext context)
       {
           // 게임 상태 조회 로직
       }
   }
   ```

## 3. 데이터 관리

### 3.1 데이터베이스 분리
```csharp
// 각 서비스별 독립적인 데이터베이스
GameService/
└── Data/
    └── GameDbContext.cs

UserService/
└── Data/
    └── UserDbContext.cs
```

### 3.2 이벤트 소싱
```csharp
public class GameEvent
{
    public Guid Id { get; set; }
    public string EventType { get; set; }
    public string Data { get; set; }
    public DateTime OccurredOn { get; set; }
}

public class GameEventStore
{
    public async Task AppendEvent(GameEvent @event)
    {
        // 이벤트 저장
    }

    public async Task<List<GameEvent>> GetEvents(Guid gameId)
    {
        // 이벤트 조회
    }
}
```

## 4. 모니터링과 로깅

### 4.1 분산 추적
```csharp
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly ITracer _tracer;

    [HttpGet]
    public async Task<IActionResult> GetGameState()
    {
        using var scope = _tracer.BuildSpan("get-game-state").StartActive();
        _logger.LogInformation("Getting game state");
        // ...
    }
}
```

### 4.2 중앙화된 로깅
```csharp
public class LoggingMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"];
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await _next(context);
        }
    }
}
```

## 5. 보안

### 5.1 API 게이트웨이
```csharp
public class ApiGateway
{
    private readonly IAuthenticationService _authService;

    public async Task<IActionResult> HandleRequest(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"];
        if (!await _authService.ValidateToken(token))
        {
            return Unauthorized();
        }
        // 요청 라우팅
    }
}
```

### 5.2 서비스 간 인증
```csharp
public class ServiceAuthentication
{
    private readonly IIdentityServerClient _identityServer;

    public async Task<string> GetServiceToken()
    {
        return await _identityServer.GetClientCredentialsToken();
    }
}
```

## 6. 배포 전략

### 6.1 컨테이너화
```dockerfile
# Dockerfile 예시
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "GameService.dll"]
```

### 6.2 오케스트레이션
```yaml
# Kubernetes Deployment 예시
apiVersion: apps/v1
kind: Deployment
metadata:
  name: game-service
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: game-service
        image: game-service:latest
        ports:
        - containerPort: 80
```

## 7. 모범 사례

### 7.1 설계 원칙
- 단일 책임 원칙
- 느슨한 결합
- 독립적인 배포
- 장애 격리

### 7.2 운영 고려사항
- 모니터링
- 로깅
- 알림
- 자동 복구

### 7.3 성능 최적화
- 캐싱
- 비동기 처리
- 부하 분산
- 확장성 계획

## 8. 참고 자료
- [마이크로서비스 아키텍처 가이드](https://learn.microsoft.com/dotnet/architecture/microservices/)
- [Kubernetes 공식 문서](https://kubernetes.io/docs/home/)
- [Docker 공식 문서](https://docs.docker.com/)
- [Consul 공식 문서](https://www.consul.io/docs) 