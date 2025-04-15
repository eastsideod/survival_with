# ASP.NET Core 실시간 기능

## 1. SignalR 개요

### 1.1 SignalR이란?
- 실시간 양방향 통신을 위한 라이브러리
- WebSocket, Server-Sent Events, Long Polling 등 다양한 전송 방식 지원
- 자동 전송 방식 선택 및 폴백 처리

### 1.2 기본 설정
```csharp
// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddSignalR();
}

public void Configure(IApplicationBuilder app)
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHub<GameHub>("/gameHub");
    });
}
```

## 2. Hub 구현

### 2.1 Hub란?
Hub는 SignalR의 핵심 구성 요소로, 클라이언트와 서버 간의 실시간 양방향 통신을 처리하는 중앙 허브 역할을 합니다. 주요 특징은 다음과 같습니다:

1. **통신 중앙화**
   - 모든 클라이언트-서버 통신의 중앙 집중식 엔드포인트
   - 연결 관리 및 메시지 라우팅 담당
   - 그룹 관리 및 브로드캐스팅 기능 제공

2. **주요 기능**
   - **연결 관리**: 클라이언트 연결/해제 처리
   - **메시지 라우팅**: 클라이언트 간 메시지 전달
   - **그룹 관리**: 클라이언트를 그룹으로 구성
   - **상태 관리**: 연결 상태 및 컨텍스트 정보 관리

3. **생명주기**
   ```csharp
   public class GameHub : Hub
   {
       // 연결 시 호출
       public override async Task OnConnectedAsync()
       {
           await base.OnConnectedAsync();
       }

       // 연결 해제 시 호출
       public override async Task OnDisconnectedAsync(Exception exception)
       {
           await base.OnDisconnectedAsync(exception);
       }

       // 재연결 시 호출
       public override async Task OnReconnectedAsync()
       {
           await base.OnReconnectedAsync();
       }
   }
   ```

### 2.2 기본 Hub 구현
```csharp
public class GameHub : Hub
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameHub> _logger;

    public GameHub(IGameService gameService, ILogger<GameHub> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    // 게임 참가 메서드
    public async Task JoinGame(string gameId)
    {
        // 클라이언트를 특정 게임 그룹에 추가
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        
        // 그룹 내 모든 클라이언트에게 참가 알림
        await Clients.Group(gameId).SendAsync("PlayerJoined", Context.ConnectionId);
        
        _logger.LogInformation($"Player {Context.ConnectionId} joined game {gameId}");
    }

    // 메시지 전송 메서드
    public async Task SendMessage(string gameId, string message)
    {
        // 특정 게임 그룹에 메시지 브로드캐스트
        await Clients.Group(gameId).SendAsync("ReceiveMessage", 
            Context.ConnectionId, message);
            
        _logger.LogInformation($"Message sent to game {gameId} by {Context.ConnectionId}");
    }
}
```

### 2.3 Hub의 주요 구성 요소

1. **Context 속성**
   ```csharp
   public class GameHub : Hub
   {
       public void GetConnectionInfo()
       {
           // 현재 연결 ID
           var connectionId = Context.ConnectionId;
           
           // 사용자 정보
           var user = Context.User;
           
           // HTTP 요청 정보
           var httpContext = Context.GetHttpContext();
           
           // 그룹 정보
           var groups = Context.Groups;
       }
   }
   ```

2. **Clients 속성**
   ```csharp
   public class GameHub : Hub
   {
       public async Task SendMessages()
       {
           // 모든 클라이언트에게 메시지 전송
           await Clients.All.SendAsync("Broadcast", "Hello everyone!");
           
           // 특정 클라이언트에게 메시지 전송
           await Clients.Client("connectionId").SendAsync("Private", "Hello!");
           
           // 특정 그룹에게 메시지 전송
           await Clients.Group("gameId").SendAsync("GroupMessage", "Hello group!");
           
           // 여러 클라이언트에게 메시지 전송
           await Clients.Clients(new[] { "id1", "id2" }).SendAsync("Multiple", "Hello!");
       }
   }
   ```

3. **Groups 속성**
   ```csharp
   public class GameHub : Hub
   {
       public async Task ManageGroups()
       {
           // 그룹에 추가
           await Groups.AddToGroupAsync(Context.ConnectionId, "gameId");
           
           // 그룹에서 제거
           await Groups.RemoveFromGroupAsync(Context.ConnectionId, "gameId");
           
           // 그룹 목록 조회
           var groups = await Groups.GetGroupsAsync();
       }
   }
   ```

### 2.4 Hub 사용 시 고려사항

1. **상태 관리**
   - Hub는 상태 비저장(stateless)으로 설계
   - 영구적인 상태는 외부 저장소에 보관
   - 연결 상태는 Context를 통해 관리

2. **성능 최적화**
   - 메시지 크기 최적화
   - 연결 수 제한
   - 적절한 타임아웃 설정

3. **오류 처리**
   - 예외 처리 및 로깅
   - 재연결 메커니즘 구현
   - 클라이언트 오류 처리

4. **보안**
   - 인증 및 권한 부여
   - 메시지 유효성 검사
   - CORS 설정

## 3. 클라이언트 구현

### 3.1 JavaScript 클라이언트
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .withAutomaticReconnect()
    .build();

connection.on("PlayerJoined", (connectionId) => {
    console.log(`Player joined: ${connectionId}`);
});

connection.on("ReceiveMessage", (senderId, message) => {
    console.log(`${senderId}: ${message}`);
});

connection.start()
    .then(() => {
        console.log("Connected to game hub");
        connection.invoke("JoinGame", "game123");
    })
    .catch(err => console.error(err));
```

### 3.2 .NET 클라이언트
```csharp
var connection = new HubConnectionBuilder()
    .WithUrl("https://localhost:5001/gameHub")
    .WithAutomaticReconnect()
    .Build();

connection.On<string>("PlayerJoined", (connectionId) =>
{
    Console.WriteLine($"Player joined: {connectionId}");
});

await connection.StartAsync();
await connection.InvokeAsync("JoinGame", "game123");
```

## 4. 그룹 관리

### 4.1 그룹 가입/탈퇴
```csharp
public class GameHub : Hub
{
    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).SendAsync("PlayerJoined", Context.ConnectionId);
    }

    public async Task LeaveGame(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        await Clients.Group(gameId).SendAsync("PlayerLeft", Context.ConnectionId);
    }
}
```

### 4.2 그룹 메시지 전송
```csharp
public class GameHub : Hub
{
    public async Task BroadcastToGame(string gameId, string message)
    {
        await Clients.Group(gameId).SendAsync("GameMessage", message);
    }

    public async Task SendToPlayer(string gameId, string playerId, string message)
    {
        await Clients.Client(playerId).SendAsync("PrivateMessage", message);
    }
}
```

## 5. 연결 관리

### 5.1 연결 상태 관리
```csharp
public class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _gameService.PlayerConnected(userId, Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _gameService.PlayerDisconnected(userId, Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
```

### 5.2 재연결 처리
```csharp
public class GameHub : Hub
{
    public override async Task OnReconnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _gameService.PlayerReconnected(userId, Context.ConnectionId);
        await base.OnReconnectedAsync();
    }
}
```

## 6. 스케일 아웃

### 6.1 Redis 백플레인
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSignalR()
        .AddStackExchangeRedis(options =>
        {
            options.Configuration.EndPoints.Add("localhost", 6379);
        });
}
```

### 6.2 Azure SignalR Service
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSignalR()
        .AddAzureSignalR("AzureSignalRConnectionString");
}
```

## 7. 성능 최적화

### 7.1 메시지 압축
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSignalR()
        .AddMessagePackProtocol();
}
```

### 7.2 연결 제한
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSignalR(options =>
    {
        options.MaximumReceiveMessageSize = 65536;
        options.EnableDetailedErrors = false;
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    });
}
```

## 8. 보안

### 8.1 인증
```csharp
[Authorize]
public class GameHub : Hub
{
    public async Task SendMessage(string gameId, string message)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Clients.Group(gameId).SendAsync("ReceiveMessage", userId, message);
    }
}
```

### 8.2 CORS 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddCors(options =>
    {
        options.AddPolicy("SignalRCors", builder =>
        {
            builder.WithOrigins("https://example.com")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
    });
}

public void Configure(IApplicationBuilder app)
{
    app.UseCors("SignalRCors");
}
```

## 9. 모니터링

### 9.1 로깅
```csharp
public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }

    public async Task SendMessage(string gameId, string message)
    {
        _logger.LogInformation(
            "Message sent to game {GameId} by {ConnectionId}", 
            gameId, Context.ConnectionId);
        await Clients.Group(gameId).SendAsync("ReceiveMessage", message);
    }
}
```

### 9.2 메트릭스
```csharp
public class GameHub : Hub
{
    private readonly IMetricsCollector _metrics;

    public GameHub(IMetricsCollector metrics)
    {
        _metrics = metrics;
    }

    public async Task SendMessage(string gameId, string message)
    {
        _metrics.RecordMessageSent(gameId);
        await Clients.Group(gameId).SendAsync("ReceiveMessage", message);
    }
}
```

## 10. 참고 자료
- [ASP.NET Core SignalR](https://learn.microsoft.com/aspnet/core/signalr/introduction)
- [SignalR JavaScript Client](https://learn.microsoft.com/aspnet/core/signalr/javascript-client)
- [SignalR .NET Client](https://learn.microsoft.com/aspnet/core/signalr/dotnet-client)
- [SignalR Scaleout](https://learn.microsoft.com/aspnet/core/signalr/scale) 