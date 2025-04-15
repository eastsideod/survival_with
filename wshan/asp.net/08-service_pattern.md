# ASP.NET Core Service 패턴

## 1. 서비스 패턴 소개

### 1.1 서비스 패턴이란?
- 비즈니스 로직을 캡슐화하는 계층
- 컨트롤러와 데이터 접근 계층 사이의 중간 계층
- 관심사 분리(Separation of Concerns) 원칙 구현

### 1.2 서비스 패턴의 목적
- 비즈니스 로직의 중앙화
- 코드 재사용성 향상
- 테스트 용이성 제공
- 유지보수성 향상

### 1.3 서비스 계층의 위치
```
Application
├── Controllers/        # HTTP 요청 처리
├── Services/          # 비즈니스 로직
├── Models/           # 데이터 모델
└── Data/             # 데이터 접근
```

## 2. 기본 서비스 구현

### 2.1 서비스 인터페이스
```csharp
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
```

### 2.2 서비스 구현체
```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository repository,
        ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        try
        {
            return await _repository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all products");
            throw;
        }
    }
}
```

### 2.3 서비스 등록
서비스를 사용하기 위해서는 DI 컨테이너에 등록해야 합니다. 일반적으로 `Startup.cs` 또는 `Program.cs`의 `ConfigureServices` 메서드에서 등록합니다.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // 서비스 등록
    services.AddScoped<IProductService, ProductService>();
    services.AddScoped<IProductRepository, ProductRepository>();
    
    // 관련 서비스들도 함께 등록
    services.AddScoped<ICategoryService, CategoryService>();
    services.AddScoped<IInventoryService, InventoryService>();
}
```

서비스 등록 시 고려사항:
1. **수명 주기 선택**
   - `AddScoped`: HTTP 요청당 하나의 인스턴스 (기본값)
   - `AddTransient`: 매번 새로운 인스턴스
   - `AddSingleton`: 애플리케이션 전체에서 단일 인스턴스

2. **의존성 순서**
   - 의존하는 서비스가 먼저 등록되어 있어야 함
   - 예: `ProductService`가 `IProductRepository`에 의존한다면, `IProductRepository`를 먼저 등록

3. **확장 메서드 활용**
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}

// 사용
public void ConfigureServices(IServiceCollection services)
{
    services.AddProductServices();
}
```

## 3. 고급 서비스 패턴

### 3.1 복잡한 비즈니스 로직 처리
```csharp
public class PaymentService : IPaymentService
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IOrderRepository _orderRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<PaymentService> _logger;
    private readonly IEventBus _eventBus;

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        // 1. 결제 정보 검증
        ValidatePaymentRequest(request);
        
        // 2. 결제 처리
        var paymentResult = await _paymentGateway.ProcessPaymentAsync(request);
        
        // 3. 트랜잭션 기록
        await _transactionRepository.AddAsync(new Transaction
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Status = paymentResult.Status,
            TransactionId = paymentResult.TransactionId
        });
        
        // 4. 주문 상태 업데이트
        await _orderRepository.UpdateStatusAsync(
            request.OrderId, 
            paymentResult.Status == PaymentStatus.Success 
                ? OrderStatus.Paid 
                : OrderStatus.PaymentFailed);
        
        // 5. 이벤트 발행
        await _eventBus.PublishAsync(new PaymentProcessedEvent
        {
            OrderId = request.OrderId,
            Status = paymentResult.Status
        });
        
        return paymentResult;
    }
}
```

### 3.2 트랜잭션 관리
```csharp
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Order> CreateOrderAsync(OrderRequest request)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. 재고 확인 및 차감
            foreach (var item in request.Items)
            {
                await _inventoryRepository.ReserveStockAsync(
                    item.ProductId, 
                    item.Quantity);
            }
            
            // 2. 주문 생성
            var order = new Order { ... };
            await _orderRepository.AddAsync(order);
            
            // 3. 트랜잭션 커밋
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return order;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

### 3.3 캐싱 전략
```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;
    private readonly ILogger<ProductService> _logger;

    public async Task<Product> GetProductByIdAsync(int id)
    {
        // 1. 캐시에서 조회
        var cacheKey = $"product_{id}";
        var cachedProduct = await _cache.GetAsync<Product>(cacheKey);
        if (cachedProduct != null)
        {
            _logger.LogInformation("Product {Id} retrieved from cache", id);
            return cachedProduct;
        }
        
        // 2. DB에서 조회
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Product {id} not found");
            
        // 3. 캐시에 저장
        await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
        
        return product;
    }
}
```

## 4. 서비스 테스트

### 4.1 단위 테스트
```csharp
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _service = new ProductService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetProductById_ExistingProduct_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product" };
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
    }
}
```

## 5. 서비스 모니터링

### 5.1 메트릭스 수집
```csharp
public class ServiceMetrics
{
    private readonly Counter _requestCounter;
    private readonly Histogram _responseTime;

    public ServiceMetrics()
    {
        _requestCounter = Metrics.CreateCounter(
            "service_requests_total",
            "Total number of service requests");

        _responseTime = Metrics.CreateHistogram(
            "service_response_time_seconds",
            "Service response time in seconds");
    }

    public void RecordRequest(string serviceName)
    {
        _requestCounter.Inc(new[] { serviceName });
    }

    public void RecordResponseTime(string serviceName, TimeSpan duration)
    {
        _responseTime.Observe(duration.TotalSeconds, new[] { serviceName });
    }
}
```

## 6. 서비스 패턴 설계 가이드라인

### 6.1 기본 원칙
1. **단일 책임 원칙**
   - 각 서비스는 하나의 도메인이나 기능에 집중
   - 너무 많은 책임을 가지지 않도록 주의

2. **의존성 관리**
   - 필요한 의존성만 주입
   - 순환 의존성 피하기
   - 인터페이스 분리 원칙 준수

3. **예외 처리**
   - 비즈니스 예외와 기술적 예외 구분
   - 적절한 예외 계층 구조 설계
   - 명확한 에러 메시지 제공

### 6.2 성능 고려사항
1. **비동기 작업**
   - I/O 작업은 비동기로 처리
   - Task.WhenAll 활용
   - CancellationToken 지원

2. **캐싱 전략**
   - 적절한 캐시 수명 주기 설정
   - 캐시 무효화 전략 수립
   - 분산 캐시 고려

3. **리소스 관리**
   - 연결 풀링 활용
   - 메모리 사용량 모니터링
   - 리소스 해제 보장

### 6.3 보안 고려사항
1. **권한 검증**
   - 역할 기반 접근 제어
   - 리소스 기반 권한 검사
   - 작업 기반 권한 검사

2. **데이터 검증**
   - 입력 데이터 검증
   - 출력 데이터 필터링
   - SQL 인젝션 방지

3. **감사 로깅**
   - 중요 작업 로깅
   - 사용자 활동 추적
   - 보안 이벤트 모니터링

## 7. 참고 자료
- [ASP.NET Core Service Layer](https://learn.microsoft.com/aspnet/core/fundamentals/middleware)
- [Repository Pattern](https://learn.microsoft.com/aspnet/core/data/ef-mvc/intro)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID) 