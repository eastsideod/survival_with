# RabbitMQ 고급 패턴

## 1. 메시지 패턴

### 1.1 Publish/Subscribe
- 메시지를 여러 구독자에게 브로드캐스팅
- Fanout Exchange 사용
- 실시간 업데이트, 알림 시스템에 적합

```csharp
// 발행자
public void PublishToAll(string exchange, T message)
{
    using var channel = _connection.CreateModel();
    channel.ExchangeDeclare(exchange, ExchangeType.Fanout);
    
    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
    channel.BasicPublish(exchange, "", null, body);
}

// 구독자
public void SubscribeToAll(string exchange, string queueName)
{
    var channel = _connection.CreateModel();
    channel.ExchangeDeclare(exchange, ExchangeType.Fanout);
    channel.QueueDeclare(queueName, true, false, false, null);
    channel.QueueBind(queueName, exchange, "");
    
    // 구독 로직...
}
```

### 1.2 Routing
- 메시지를 특정 라우팅 키에 따라 라우팅
- Direct Exchange 사용
- 조건부 메시지 처리에 적합

```csharp
// 발행자
public void PublishWithRouting(string exchange, string routingKey, T message)
{
    using var channel = _connection.CreateModel();
    channel.ExchangeDeclare(exchange, ExchangeType.Direct);
    
    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
    channel.BasicPublish(exchange, routingKey, null, body);
}

// 구독자
public void SubscribeWithRouting(string exchange, string queueName, string routingKey)
{
    var channel = _connection.CreateModel();
    channel.ExchangeDeclare(exchange, ExchangeType.Direct);
    channel.QueueDeclare(queueName, true, false, false, null);
    channel.QueueBind(queueName, exchange, routingKey);
    
    // 구독 로직...
}
```

### 1.3 Topics
- 패턴 기반 메시지 라우팅
- Topic Exchange 사용
- 복잡한 라우팅 규칙에 적합

```csharp
// 발행자
public void PublishWithTopic(string exchange, string topic, T message)
{
    using var channel = _connection.CreateModel();
    channel.ExchangeDeclare(exchange, ExchangeType.Topic);
    
    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
    channel.BasicPublish(exchange, topic, null, body);
}

// 구독자
public void SubscribeWithTopic(string exchange, string queueName, string topicPattern)
{
    var channel = _connection.CreateModel();
    channel.ExchangeDeclare(exchange, ExchangeType.Topic);
    channel.QueueDeclare(queueName, true, false, false, null);
    channel.QueueBind(queueName, exchange, topicPattern);
    
    // 구독 로직...
}
```

## 2. 고급 기능

### 2.1 메시지 TTL
- 메시지 수명 제한
- 큐 레벨 또는 메시지 레벨에서 설정 가능

```csharp
// 큐 레벨 TTL
var args = new Dictionary<string, object>
{
    { "x-message-ttl", 60000 } // 60초
};
channel.QueueDeclare("queue", true, false, false, args);

// 메시지 레벨 TTL
var properties = channel.CreateBasicProperties();
properties.Expiration = "60000";
channel.BasicPublish("exchange", "routingKey", properties, body);
```

### 2.2 Dead Letter Exchange
- 처리되지 않은 메시지를 다른 큐로 라우팅
- 메시지 재시도 및 에러 처리에 유용

```csharp
var args = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", "dlx" },
    { "x-dead-letter-routing-key", "error" }
};
channel.QueueDeclare("queue", true, false, false, args);
```

### 2.3 우선순위 큐
- 메시지 우선순위 설정
- 중요도에 따른 처리 순서 조정

```csharp
var args = new Dictionary<string, object>
{
    { "x-max-priority", 10 }
};
channel.QueueDeclare("queue", true, false, false, args);

var properties = channel.CreateBasicProperties();
properties.Priority = 5;
channel.BasicPublish("exchange", "routingKey", properties, body);
```

## 3. 클러스터링 및 고가용성

### 3.1 클러스터 구성
- 여러 노드로 구성된 클러스터
- 메시지 복제 및 분산
- 장애 복구 지원

```bash
# 노드 조인
rabbitmqctl stop_app
rabbitmqctl join_cluster rabbit@node1
rabbitmqctl start_app
```

### 3.2 미러링 큐
- 큐 복제 설정
- 고가용성 보장

```csharp
var args = new Dictionary<string, object>
{
    { "x-ha-policy", "all" }
};
channel.QueueDeclare("queue", true, false, false, args);
```

## 4. 성능 최적화

### 4.1 배치 처리
- 여러 메시지를 한 번에 처리
- 네트워크 오버헤드 감소

```csharp
public void PublishBatch<T>(string exchange, string routingKey, IEnumerable<T> messages)
{
    using var channel = _connection.CreateModel();
    channel.ConfirmSelect();
    
    foreach (var message in messages)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        channel.BasicPublish(exchange, routingKey, null, body);
    }
    
    channel.WaitForConfirmsOrDie();
}
```

### 4.2 채널 풀링
- 채널 재사용
- 리소스 사용 최적화

```csharp
public class ChannelPool
{
    private readonly ConcurrentQueue<IModel> _pool;
    private readonly IConnection _connection;
    private readonly int _maxSize;

    public ChannelPool(IConnection connection, int maxSize = 10)
    {
        _connection = connection;
        _maxSize = maxSize;
        _pool = new ConcurrentQueue<IModel>();
    }

    public IModel GetChannel()
    {
        if (_pool.TryDequeue(out var channel))
            return channel;

        return _connection.CreateModel();
    }

    public void ReturnChannel(IModel channel)
    {
        if (_pool.Count < _maxSize)
            _pool.Enqueue(channel);
        else
            channel.Dispose();
    }
}
```

## 5. 모니터링 및 관리

### 5.1 메트릭 수집
- Prometheus 연동
- Grafana 대시보드
- 성능 모니터링

```yaml
# Prometheus 설정
scrape_configs:
  - job_name: 'rabbitmq'
    static_configs:
      - targets: ['rabbitmq:15692']
```

### 5.2 알림 설정
- 임계값 기반 알림
- 이벤트 기반 알림
- 문제 조기 감지

```yaml
# Alertmanager 설정
groups:
  - name: rabbitmq
    rules:
      - alert: HighMessageRate
        expr: rate(rabbitmq_queue_messages_published_total[5m]) > 1000
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "높은 메시지 발행률"
          description: "5분 동안 평균 메시지 발행률이 1000을 초과했습니다" 