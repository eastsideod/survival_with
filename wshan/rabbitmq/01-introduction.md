# RabbitMQ 소개

## 1. RabbitMQ란?

### 1.1 기본 개념
- 오픈소스 메시지 브로커
- AMQP(Advanced Message Queuing Protocol) 구현체
- 분산 시스템에서 메시지 기반 통신을 위한 중간자 역할

### 1.2 주요 특징
1. **신뢰성**
   - 메시지 지속성
   - 트랜잭션 지원
   - 메시지 확인(acknowledgment)

2. **유연성**
   - 다양한 메시지 패턴 지원
   - 플러그인 시스템
   - 다중 프로토콜 지원

3. **확장성**
   - 클러스터링 지원
   - 고가용성 구성
   - 부하 분산

## 2. MSA에서의 RabbitMQ 활용

### 2.1 비동기 통신
- 서비스 간 느슨한 결합
- 이벤트 기반 아키텍처
- 서비스 간 의존성 감소

### 2.2 주요 사용 사례
1. **이벤트 처리**
   - 주문 처리
   - 알림 시스템
   - 로그 집계

2. **작업 큐**
   - 백그라운드 작업
   - 배치 처리
   - 비동기 작업

3. **메시지 브로드캐스팅**
   - 실시간 업데이트
   - 상태 변경 알림
   - 시스템 이벤트

## 3. ASP.NET Core와의 통합

### 3.1 기본 설정
```csharp
// 필요한 패키지 설치
dotnet add package RabbitMQ.Client

// 서비스 등록
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IConnection>(sp => 
    {
        var factory = new ConnectionFactory
        {
            HostName = Configuration["RabbitMQ:Host"],
            UserName = Configuration["RabbitMQ:Username"],
            Password = Configuration["RabbitMQ:Password"]
        };
        return factory.CreateConnection();
    });
}
```

### 3.2 메시지 발행
```csharp
public class MessagePublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<MessagePublisher> _logger;

    public MessagePublisher(IConnection connection, ILogger<MessagePublisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public void Publish<T>(string exchange, string routingKey, T message)
    {
        using var channel = _connection.CreateModel();
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        
        channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            basicProperties: null,
            body: body);
            
        _logger.LogInformation($"메시지 발행: {exchange}/{routingKey}");
    }
}
```

### 3.3 메시지 구독
```csharp
public class MessageConsumer
{
    private readonly IConnection _connection;
    private readonly ILogger<MessageConsumer> _logger;

    public MessageConsumer(IConnection connection, ILogger<MessageConsumer> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public void StartConsuming(string queueName)
    {
        var channel = _connection.CreateModel();
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
            
            _logger.LogInformation($"메시지 수신: {queueName}");
            
            // 메시지 처리 로직
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(
            queue: queueName,
            autoAck: false,
            consumer: consumer);
    }
}
```

## 4. Kubernetes에서의 RabbitMQ 운영

### 4.1 배포 구성
```yaml
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: rabbitmq
spec:
  serviceName: rabbitmq
  replicas: 3
  selector:
    matchLabels:
      app: rabbitmq
  template:
    metadata:
      labels:
        app: rabbitmq
    spec:
      containers:
      - name: rabbitmq
        image: rabbitmq:3-management
        ports:
        - containerPort: 5672
          name: amqp
        - containerPort: 15672
          name: management
        env:
        - name: RABBITMQ_ERLANG_COOKIE
          value: "secret-cookie"
        - name: RABBITMQ_DEFAULT_USER
          value: "admin"
        - name: RABBITMQ_DEFAULT_PASS
          value: "password"
```

### 4.2 서비스 구성
```yaml
apiVersion: v1
kind: Service
metadata:
  name: rabbitmq
spec:
  selector:
    app: rabbitmq
  ports:
  - port: 5672
    targetPort: 5672
    name: amqp
  - port: 15672
    targetPort: 15672
    name: management
  type: ClusterIP
```

## 5. 모니터링 및 관리

### 5.1 메트릭 수집
- Prometheus 연동
- Grafana 대시보드
- 알림 설정

### 5.2 로깅
- 구조화된 로깅
- 로그 집계
- 문제 진단

### 5.3 백업 및 복구
- 메시지 백업
- 클러스터 복구
- 재해 복구 계획 