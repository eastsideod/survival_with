# ASP.NET Core 캐싱

## 1. 캐싱 개요

### 1.1 캐싱이란?
- 자주 사용되는 데이터를 임시 저장소에 보관
- 데이터베이스나 외부 서비스 호출을 줄여 성능 향상
- 응답 시간 단축 및 시스템 부하 감소

### 1.2 캐싱의 장점
- 성능 향상
- 시스템 부하 감소
- 비용 절감
- 사용자 경험 개선

## 2. 캐싱 전략

### 2.1 메모리 캐싱 (IMemoryCache)
- 단일 서버의 메모리에 데이터를 저장
- 매우 빠른 접근 속도 (메모리 직접 접근)
- 서버 재시작 시 데이터 손실
- 주로 단일 서버 환경이나 임시 데이터 저장에 사용

```csharp
// 메모리 캐시 설정
public void ConfigureServices(IServiceCollection services)
{
    services.AddMemoryCache();
}

// 게임 세션 캐시 예시
public class GameSessionCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<GameSessionCache> _logger;

    public GameSessionCache(IMemoryCache cache, ILogger<GameSessionCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public void AddGameSession(string sessionId, GameSession session)
    {
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1))
            .SetPriority(CacheItemPriority.High)
            .RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                _logger.LogInformation($"캐시 항목 제거: {key}, 이유: {reason}");
            });

        _cache.Set(sessionId, session, cacheOptions);
    }

    public GameSession GetGameSession(string sessionId)
    {
        if (_cache.TryGetValue(sessionId, out GameSession session))
        {
            return session;
        }
        return null;
    }
}
```

### 2.2 분산 캐싱 (IDistributedCache)
- 여러 서버 간에 공유되는 캐시 저장소
- Redis, SQL Server, NCache 등 다양한 백엔드 지원
- 서버 간 데이터 공유 가능
- 영구적인 데이터 저장 가능

```csharp
// Redis 캐시 설정
public void ConfigureServices(IServiceCollection services)
{
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = Configuration.GetConnectionString("Redis");
        options.InstanceName = "GameServer_";
    });
}

// 게임 리더보드 캐시 예시
public class GameLeaderboardCache
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<GameLeaderboardCache> _logger;
    private readonly DistributedCacheEntryOptions _defaultOptions;

    public GameLeaderboardCache(IDistributedCache cache, ILogger<GameLeaderboardCache> logger)
    {
        _cache = cache;
        _logger = logger;
        
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };
    }

    public async Task AddLeaderboardAsync(string gameId, LeaderboardData data)
    {
        try
        {
            var serializedData = JsonSerializer.Serialize(data);
            await _cache.SetStringAsync(
                $"leaderboard_{gameId}",
                serializedData,
                _defaultOptions
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"리더보드 캐시 저장 실패: {gameId}");
            throw;
        }
    }

    public async Task<LeaderboardData> GetLeaderboardAsync(string gameId)
    {
        try
        {
            var cachedData = await _cache.GetStringAsync($"leaderboard_{gameId}");
            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<LeaderboardData>(cachedData);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"리더보드 캐시 조회 실패: {gameId}");
            throw;
        }
    }
}
```

### 2.3 캐시 구현체 비교

| 특성 | IMemoryCache | IDistributedCache |
|------|-------------|------------------|
| 저장 위치 | 서버 메모리 | 외부 저장소 (Redis 등) |
| 접근 속도 | 매우 빠름 | 상대적으로 느림 |
| 데이터 공유 | 불가능 | 가능 |
| 영구성 | 서버 재시작 시 손실 | 영구 저장 가능 |
| 사용 사례 | 임시 데이터, 세션 | 공유 데이터, 리더보드 |
| 확장성 | 제한적 | 높음 |
| 복잡성 | 낮음 | 높음 |

### 2.4 게임 서버에서의 캐시 사용 사례

1. **게임 세션 데이터**
   - `IMemoryCache`: 현재 활성화된 게임 세션
   - `IDistributedCache`: 게임 결과, 리더보드

2. **플레이어 데이터**
   - `IMemoryCache`: 현재 온라인 플레이어 정보
   - `IDistributedCache`: 플레이어 통계, 업적

3. **게임 상태**
   - `IMemoryCache`: 현재 게임 룸 상태
   - `IDistributedCache`: 게임 매치 히스토리

4. **시스템 설정**
   - `IMemoryCache`: 자주 변경되지 않는 설정
   - `IDistributedCache`: 모든 서버에서 공유해야 하는 설정

## 3. 응답 캐싱

### 3.1 컨트롤러 레벨 캐싱
```csharp
[ApiController]
[Route("api/[controller]")]
[ResponseCache(Duration = 60)]
public class GameController : ControllerBase
{
    [HttpGet("leaderboard")]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "gameType", "season" })]
    public async Task<IActionResult> GetLeaderboard(string gameType, string season)
    {
        // ...
    }
}
```

### 3.2 미들웨어 설정
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseResponseCaching();
    
    app.Use(async (context, next) =>
    {
        context.Response.GetTypedHeaders().CacheControl = 
            new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(60)
            };
            
        await next();
    });
}
```

## 4. 캐싱 모범 사례

### 4.1 캐싱 전략
1. **적절한 캐시 수명**
   - 데이터 변경 빈도 고려
   - 비즈니스 요구사항 반영
   - 리소스 제약 고려

2. **캐시 무효화**
   - 데이터 변경 시 캐시 제거
   - 조건부 캐싱
   - 버전 관리

3. **캐시 키 설계**
   - 명확하고 일관된 키 구조
   - 충돌 방지
   - 효율적인 키 생성

### 4.2 성능 최적화
1. **메모리 관리**
   - 캐시 크기 제한
   - 메모리 사용량 모니터링
   - 오래된 데이터 정리

2. **네트워크 최적화**
   - 분산 캐시 전략
   - 지역성 고려
   - 대역폭 사용 최적화

3. **병렬 처리**
   - 동시성 제어
   - 락 최소화
   - 데드락 방지

### 4.3 보안 고려사항
1. **데이터 보호**
   - 민감한 데이터 캐싱 제한
   - 암호화 고려
   - 접근 제어

2. **캐시 공격 방지**
   - 캐시 포이즈닝 방지
   - 캐시 사이드 채널 공격 방지
   - 입력 검증

3. **감사 및 모니터링**
   - 캐시 접근 로깅
   - 성능 메트릭스 수집
   - 이상 징후 감지

## 5. 참고 자료
- [ASP.NET Core 캐싱 공식 문서](https://learn.microsoft.com/dotnet/core/extensions/caching)
- [Redis 캐싱 가이드](https://redis.io/topics/caching)
- [캐싱 패턴과 전략](https://docs.microsoft.com/dotnet/architecture/microservices/architect-microservice-container-applications/implement-resilient-applications/caching) 