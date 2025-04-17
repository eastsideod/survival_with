# RabbitMQ 문제 해결 및 모범 사례

## 1. 일반적인 문제 해결

### 1.1 연결 문제
- **증상**: 연결 실패, 연결 끊김
- **해결 방법**:
  ```csharp
  public class RabbitMQConnectionManager
  {
      private readonly IConnection _connection;
      private readonly ILogger<RabbitMQConnectionManager> _logger;
      private readonly Timer _reconnectTimer;

      public RabbitMQConnectionManager(IConnection connection, ILogger<RabbitMQConnectionManager> logger)
      {
          _connection = connection;
          _logger = logger;
          
          _connection.ConnectionShutdown += (sender, args) =>
          {
              _logger.LogWarning($"연결 끊김: {args.ReplyText}");
              TryReconnect();
          };
      }

      private void TryReconnect()
      {
          _reconnectTimer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
      }
  }
  ```

### 1.2 메시지 손실
- **증상**: 메시지가 사라짐, 처리되지 않음
- **해결 방법**:
  ```csharp
  public class ReliablePublisher
  {
      public void PublishWithConfirmation<T>(string exchange, string routingKey, T message)
      {
          using var channel = _connection.CreateModel();
          channel.ConfirmSelect();
          
          var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
          
          channel.BasicPublish(exchange, routingKey, null, body);
          
          if (!channel.WaitForConfirms(TimeSpan.FromSeconds(5)))
          {
              throw new Exception("메시지 발행 확인 실패");
          }
      }
  }
  ```

## 2. 성능 문제

### 2.1 메시지 처리 지연
- **증상**: 메시지 처리 속도 저하
- **해결 방법**:
  ```csharp
  public class OptimizedConsumer
  {
      private readonly SemaphoreSlim _semaphore;
      
      public OptimizedConsumer(int maxConcurrentMessages)
      {
          _semaphore = new SemaphoreSlim(maxConcurrentMessages);
      }
      
      public async Task ProcessMessage(IModel channel, BasicDeliverEventArgs ea)
      {
          await _semaphore.WaitAsync();
          try
          {
              // 메시지 처리
              await ProcessMessageAsync(ea);
              channel.BasicAck(ea.DeliveryTag, false);
          }
          finally
          {
              _semaphore.Release();
          }
      }
  }
  ```

### 2.2 메모리 사용량
- **증상**: 높은 메모리 사용량
- **해결 방법**:
  ```csharp
  public class MemoryOptimizedPublisher
  {
      private readonly int _batchSize;
      private readonly List<byte[]> _messageBuffer;
      
      public void PublishBatch<T>(string exchange, string routingKey, IEnumerable<T> messages)
      {
          foreach (var message in messages)
          {
              _messageBuffer.Add(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
              
              if (_messageBuffer.Count >= _batchSize)
              {
                  FlushBuffer(exchange, routingKey);
              }
          }
      }
      
      private void FlushBuffer(string exchange, string routingKey)
      {
          // 버퍼 처리 및 비우기
          _messageBuffer.Clear();
      }
  }
  ```

## 3. 모니터링 및 진단

### 3.1 상태 모니터링
```csharp
public class RabbitMQMonitor
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQMonitor> _logger;
    
    public async Task MonitorQueues()
    {
        using var channel = _connection.CreateModel();
        var queues = channel.QueueDeclarePassive("queue-name");
        
        _logger.LogInformation($"큐 상태: {queues.MessageCount} 메시지, {queues.ConsumerCount} 소비자");
    }
}
```

### 3.2 메트릭 수집
```csharp
public class MetricsCollector
{
    private readonly Counter _publishedMessages;
    private readonly Counter _consumedMessages;
    private readonly Histogram _processingTime;
    
    public void RecordMessagePublished()
    {
        _publishedMessages.Inc();
    }
    
    public void RecordMessageProcessed(TimeSpan processingTime)
    {
        _consumedMessages.Inc();
        _processingTime.Observe(processingTime.TotalMilliseconds);
    }
}
```

## 4. 모범 사례

### 4.1 메시지 처리
1. **멱등성 보장**
   ```csharp
   public class IdempotentProcessor
   {
       private readonly IMessageStore _messageStore;
       
       public async Task ProcessMessage<T>(T message, string messageId)
       {
           if (await _messageStore.IsProcessed(messageId))
           {
               return;
           }
           
           await ProcessMessageInternal(message);
           await _messageStore.MarkAsProcessed(messageId);
       }
   }
   ```

2. **에러 처리**
   ```csharp
   public class ErrorHandlingConsumer
   {
       private readonly IDeadLetterHandler _deadLetterHandler;
       
       public async Task ProcessMessage(IModel channel, BasicDeliverEventArgs ea)
       {
           try
           {
               await ProcessMessageInternal(ea);
               channel.BasicAck(ea.DeliveryTag, false);
           }
           catch (Exception ex)
           {
               await _deadLetterHandler.HandleFailedMessage(ea, ex);
               channel.BasicNack(ea.DeliveryTag, false, false);
           }
       }
   }
   ```

### 4.2 운영 모범 사례

1. **리소스 관리**
   - 채널 풀링 사용
   - 연결 재사용
   - 적절한 타임아웃 설정

2. **확장성**
   - 클러스터 구성
   - 부하 분산
   - 모니터링 설정

3. **보안**
   - SSL/TLS 사용
   - 접근 제어
   - 인증 설정

### 4.3 개발 모범 사례

1. **코드 구조**
   - 의존성 주입 사용
   - 인터페이스 기반 설계
   - 단위 테스트 작성

2. **에러 처리**
   - 재시도 메커니즘
   - 데드 레터 큐 사용
   - 로깅 및 모니터링

3. **성능 최적화**
   - 배치 처리
   - 메시지 압축
   - 비동기 처리

## 5. 참고 자료
- [RabbitMQ 문제 해결 가이드](https://www.rabbitmq.com/troubleshooting.html)
- [RabbitMQ 모니터링 가이드](https://www.rabbitmq.com/monitoring.html)
- [RabbitMQ 성능 튜닝 가이드](https://www.rabbitmq.com/performance.html) 