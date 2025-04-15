# 서비스 디스커버리 (Service Discovery)

## 1. 서비스 디스커버리 개요

### 1.1 정의와 필요성
- 마이크로서비스 아키텍처에서 서비스들이 서로를 찾고 통신할 수 있게 해주는 메커니즘
- 동적으로 추가되거나 제거되는 서비스를 자동으로 인식하고 통신 가능
- 서비스 간의 위치 투명성 제공

### 1.2 주요 기능
1. **동적 서비스 검색**
   - 서비스 위치 변경 시 자동 업데이트
   - 새로운 서비스 자동 인식
   - 서비스 제거 시 자동 제외

2. **부하 분산**
   - 여러 인스턴스 중 적절한 서비스 선택
   - 라운드 로빈, 최소 연결 등 다양한 알고리즘
   - 서비스별 가중치 설정

3. **장애 감지**
   - 주기적인 헬스 체크
   - 문제 서비스 자동 제외
   - 서비스 복구 시 자동 재등록

4. **확장성**
   - 새로운 서비스 인스턴스 쉽게 추가
   - 수평적 확장 용이
   - 동적 스케일링 지원

## 2. Kubernetes 환경에서의 서비스 디스커버리

### 2.1 Kubernetes 서비스 디스커버리
1. **기본 서비스 디스커버리**
   ```yaml
   # game-service.yaml
   apiVersion: v1
   kind: Service
   metadata:
     name: game-service
   spec:
     selector:
       app: game-server
     ports:
       - protocol: TCP
         port: 80
         targetPort: 5000
     type: ClusterIP
   ```

2. **서비스 타입**
   - ClusterIP: 클러스터 내부 통신
   - NodePort: 외부 접근 가능
   - LoadBalancer: 클라우드 로드밸런서 통합
   - ExternalName: 외부 서비스 매핑

### 2.2 Kubernetes DNS
```csharp
// ASP.NET Core에서 Kubernetes 서비스 접근
public class GameService
{
    private readonly HttpClient _httpClient;

    public async Task<UserInfo> GetUserInfo(string userId)
    {
        // Kubernetes DNS를 통한 서비스 접근
        var response = await _httpClient.GetAsync($"http://user-service/api/users/{userId}");
        return await response.Content.ReadAsAsync<UserInfo>();
    }
}
```

### 2.3 Kubernetes 서비스 메시 (Istio)
```yaml
# istio-virtual-service.yaml
apiVersion: networking.istio.io/v1alpha3
kind: VirtualService
metadata:
  name: game-service
spec:
  hosts:
    - game-service
  http:
    - route:
        - destination:
            host: game-service
            subset: v1
          weight: 90
        - destination:
            host: game-service
            subset: v2
          weight: 10
```

## 3. ASP.NET 게임 서버 구현

### 3.1 서비스 등록
```csharp
public class GameServiceRegistration : IHostedService
{
    private readonly IConsulClient _consulClient;
    private readonly IConfiguration _configuration;
    private string _serviceId;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _serviceId = $"game-service-{Guid.NewGuid()}";
        
        var registration = new AgentServiceRegistration
        {
            ID = _serviceId,
            Name = "game-service",
            Address = _configuration["Service:Address"],
            Port = int.Parse(_configuration["Service:Port"]),
            Tags = new[] { "game", "matchmaking" },
            Check = new AgentServiceCheck
            {
                HTTP = $"http://{_configuration["Service:Address"]}:{_configuration["Service:Port"]}/health",
                Interval = TimeSpan.FromSeconds(5),
                Timeout = TimeSpan.FromSeconds(2),
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
            }
        };

        await _consulClient.Agent.ServiceRegister(registration);
    }
}
```

### 3.2 헬스 체크
```csharp
public class GameHealthCheckController : ControllerBase
{
    private readonly IGameServer _gameServer;
    private readonly ILogger<GameHealthCheckController> _logger;

    [HttpGet("health")]
    public async Task<IActionResult> CheckHealth()
    {
        try
        {
            var serverStatus = await _gameServer.GetServerStatus();
            var playerCount = await _gameServer.GetPlayerCount();
            var roomStatus = await _gameServer.GetRoomStatus();
            
            if (serverStatus.IsHealthy && 
                playerCount < _gameServer.MaxPlayers && 
                roomStatus.IsStable)
            {
                return Ok(new { 
                    status = "healthy",
                    players = playerCount,
                    rooms = roomStatus.ActiveRooms
                });
            }
            
            return StatusCode(503, new { 
                status = "unhealthy",
                reason = "Server overload or unstable"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new { status = "unhealthy", error = ex.Message });
        }
    }
}
```

### 3.3 서비스 검색
```csharp
public class GameServiceDiscovery
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger<GameServiceDiscovery> _logger;
    private readonly ConcurrentDictionary<string, string> _serviceCache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(30);

    public async Task<string> FindGameServer(string gameType)
    {
        try
        {
            if (_serviceCache.TryGetValue(gameType, out var cachedAddress))
            {
                return cachedAddress;
            }

            var services = await _consulClient.Health.Service(
                "game-service",
                tag: gameType,
                passingOnly: true
            );

            var healthyServer = services.Response
                .OrderBy(s => s.Service.Tags.Contains("preferred") ? 0 : 1)
                .FirstOrDefault();

            if (healthyServer != null)
            {
                var address = $"http://{healthyServer.Service.Address}:{healthyServer.Service.Port}";
                _serviceCache.TryAdd(gameType, address);
                
                _ = Task.Delay(_cacheDuration).ContinueWith(_ => 
                    _serviceCache.TryRemove(gameType, out _)
                );

                return address;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding game server");
            return null;
        }
    }
}
```

### 3.4 Kubernetes 설정
```yaml
# game-service-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: game-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: game-server
  template:
    metadata:
      labels:
        app: game-server
    spec:
      containers:
      - name: game-service
        image: game-service:latest
        ports:
        - containerPort: 5000
        livenessProbe:
          httpGet:
            path: /health
            port: 5000
          initialDelaySeconds: 5
          periodSeconds: 5
        readinessProbe:
          httpGet:
            path: /health
            port: 5000
          initialDelaySeconds: 5
          periodSeconds: 5
```

## 4. 운영 고려사항

### 4.1 성능 최적화
- 짧은 헬스 체크 간격 (5초)
- 빠른 타임아웃 설정 (2초)
- 서비스 정보 캐싱
- Kubernetes 서비스 메시 활용

### 4.2 장애 처리
- Kubernetes 자동 복구
- Pod 재시작 정책
- 서비스 메시를 통한 장애 격리
- Circuit Breaker 패턴 적용

### 4.3 부하 분산
- Kubernetes 서비스 로드밸런싱
- 서비스 메시 트래픽 관리
- 게임 타입별 서버 선택
- 동적 스케일링

### 4.4 모니터링
- Kubernetes 대시보드
- Prometheus 메트릭 수집
- Grafana 대시보드
- 분산 추적 (Jaeger)

## 5. 참고 자료
- [Kubernetes 서비스 디스커버리](https://kubernetes.io/docs/concepts/services-networking/service/)
- [Istio 서비스 메시](https://istio.io/latest/docs/concepts/what-is-istio/)
- [Consul 공식 문서](https://www.consul.io/docs)
- [ASP.NET Core Kubernetes 가이드](https://learn.microsoft.com/dotnet/architecture/cloud-native/kubernetes)