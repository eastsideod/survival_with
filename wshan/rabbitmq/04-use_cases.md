# RabbitMQ 사용 사례

## 5. 게임 서버 특화 사용 사례

### 5.1 게임 매치 상태 동기화
```csharp
public class GameMatchStateSynchronizer
{
    private readonly IConnection _connection;
    private readonly ILogger<GameMatchStateSynchronizer> _logger;

    public async Task SynchronizeMatchState(string matchId, GameState state)
    {
        using var channel = _connection.CreateModel();
        var exchangeName = $"match_{matchId}";
        
        channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
        
        var message = new GameStateMessage
        {
            MatchId = matchId,
            State = state,
            Timestamp = DateTime.UtcNow,
            Version = 1
        };
        
        var body = CompressMessage(message);
        
        channel.BasicPublish(
            exchange: exchangeName,
            routingKey: "",
            basicProperties: null,
            body: body
        );
    }

    private byte[] CompressMessage(GameStateMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        return Compress(Encoding.UTF8.GetBytes(json));
    }
}
```

### 5.2 게임 이벤트 버스
```csharp
public class GameEventBus
{
    private readonly IConnection _connection;
    private readonly ILogger<GameEventBus> _logger;
    private readonly Dictionary<string, List<Action<GameEvent>>> _eventHandlers;

    public void Subscribe(string eventType, Action<GameEvent> handler)
    {
        if (!_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType] = new List<Action<GameEvent>>();
        }
        _eventHandlers[eventType].Add(handler);
    }

    public async Task PublishEvent(GameEvent gameEvent)
    {
        using var channel = _connection.CreateModel();
        var exchangeName = "game_events";
        
        channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);
        
        var message = new EventMessage
        {
            EventId = Guid.NewGuid().ToString(),
            EventType = gameEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(gameEvent),
            Timestamp = DateTime.UtcNow,
            Version = 1
        };
        
        var body = CompressMessage(message);
        
        channel.BasicPublish(
            exchange: exchangeName,
            routingKey: $"game.{gameEvent.GameId}.{message.EventType}",
            basicProperties: null,
            body: body
        );
    }
}
```

## 6. 메시지 처리 최적화

### 6.1 메시지 직렬화/역직렬화 최적화
```csharp
public class OptimizedMessageSerializer
{
    private readonly JsonSerializerOptions _options;

    public OptimizedMessageSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public byte[] Serialize<T>(T message)
    {
        return JsonSerializer.SerializeToUtf8Bytes(message, _options);
    }

    public T Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(data, _options);
    }
}
```

### 6.2 메시지 압축
```csharp
public class MessageCompressor
{
    public byte[] Compress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
        {
            gzip.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    public byte[] Decompress(byte[] compressedData)
    {
        using var input = new MemoryStream(compressedData);
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        {
            gzip.CopyTo(output);
        }
        return output.ToArray();
    }
}
```

### 6.3 메시지 버전 관리
```csharp
public class VersionedMessage
{
    public int Version { get; set; }
    public string MessageType { get; set; }
    public byte[] Payload { get; set; }
}

public class MessageVersionManager
{
    private readonly Dictionary<string, Type> _messageTypes;
    private readonly Dictionary<string, int> _currentVersions;

    public void RegisterMessageType<T>(string messageType, int currentVersion)
    {
        _messageTypes[messageType] = typeof(T);
        _currentVersions[messageType] = currentVersion;
    }

    public object DeserializeVersionedMessage(VersionedMessage message)
    {
        if (!_messageTypes.TryGetValue(message.MessageType, out var type))
        {
            throw new InvalidOperationException($"Unknown message type: {message.MessageType}");
        }

        if (message.Version != _currentVersions[message.MessageType])
        {
            return UpgradeMessage(message, type);
        }

        return JsonSerializer.Deserialize(message.Payload, type);
    }

    private object UpgradeMessage(VersionedMessage message, Type targetType)
    {
        // 메시지 버전 업그레이드 로직
        // 예: 이전 버전의 메시지를 현재 버전으로 변환
        return null;
    }
}
```

## 7. 성능 모니터링 및 최적화

### 7.1 메시지 처리 성능 모니터링
```csharp
public class MessagePerformanceMonitor
{
    private readonly ILogger<MessagePerformanceMonitor> _logger;
    private readonly ConcurrentDictionary<string, MessageMetrics> _metrics;

    public void RecordMessageProcessing(string messageType, TimeSpan processingTime)
    {
        var metrics = _metrics.GetOrAdd(messageType, _ => new MessageMetrics());
        metrics.RecordProcessingTime(processingTime);
        
        if (metrics.AverageProcessingTime > TimeSpan.FromMilliseconds(100))
        {
            _logger.LogWarning($"느린 메시지 처리: {messageType}, 평균 처리 시간: {metrics.AverageProcessingTime}");
        }
    }
}

public class MessageMetrics
{
    private readonly ConcurrentQueue<TimeSpan> _processingTimes;
    private const int MaxSamples = 100;

    public TimeSpan AverageProcessingTime
    {
        get
        {
            if (_processingTimes.IsEmpty) return TimeSpan.Zero;
            return TimeSpan.FromTicks((long)_processingTimes.Average(t => t.Ticks));
        }
    }

    public void RecordProcessingTime(TimeSpan time)
    {
        _processingTimes.Enqueue(time);
        while (_processingTimes.Count > MaxSamples)
        {
            _processingTimes.TryDequeue(out _);
        }
    }
}
```

### 7.2 메시지 처리량 제어
```csharp
public class MessageRateLimiter
{
    private readonly int _maxMessagesPerSecond;
    private readonly SemaphoreSlim _semaphore;
    private readonly Timer _resetTimer;

    public MessageRateLimiter(int maxMessagesPerSecond)
    {
        _maxMessagesPerSecond = maxMessagesPerSecond;
        _semaphore = new SemaphoreSlim(maxMessagesPerSecond);
        _resetTimer = new Timer(ResetSemaphore, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    public async Task<bool> TryAcquire()
    {
        return await _semaphore.WaitAsync(0);
    }

    private void ResetSemaphore(object state)
    {
        var currentCount = _maxMessagesPerSecond - _semaphore.CurrentCount;
        if (currentCount > 0)
        {
            _semaphore.Release(currentCount);
        }
    }
}
```

## 8. 참고 자료
- [RabbitMQ 성능 튜닝 가이드](https://www.rabbitmq.com/performance.html)
- [메시지 압축 최적화](https://www.rabbitmq.com/compression.html)
- [메시지 버전 관리](https://www.rabbitmq.com/versioning.html) 