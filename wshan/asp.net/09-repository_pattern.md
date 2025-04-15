# ASP.NET Core 리포지토리 패턴

## 1. 리포지토리 패턴 개요

### 1.1 리포지토리 패턴이란?
- 데이터 접근 계층을 캡슐화하는 디자인 패턴
- 비즈니스 로직과 데이터 접근 로직 분리
- 데이터 소스 변경에 대한 유연성 제공
- ASP.NET Core의 기능을 활용하여 구현하는 디자인 패턴

### 1.2 ASP.NET Core와의 관계
- ASP.NET Core의 `DbContext`를 활용
- 의존성 주입(DI) 시스템을 활용하여 리포지토리 등록
- Entity Framework Core의 기능을 기반으로 구현
- 프레임워크 자체의 기능이 아닌, 개발자가 구현하는 패턴

### 1.3 장점
- 코드 재사용성 향상
- 테스트 용이성
- 유지보수성 개선
- 데이터 접근 로직 중앙화
- 데이터 소스 변경에 대한 유연성
- 비즈니스 로직과 데이터 접근 로직의 명확한 분리

### 1.4 주요 구성 요소
1. **리포지토리 인터페이스**
   - 데이터 접근 계약 정의
   - CRUD 작업 명세
   - 도메인별 특화 메서드 정의

2. **리포지토리 구현체**
   - Entity Framework Core 활용
   - 실제 데이터베이스 작업 구현
   - 비동기 작업 지원

3. **Unit of Work**
   - 트랜잭션 관리
   - 여러 리포지토리 조정
   - 변경사항 일괄 저장

## 2. 기본 구현

### 2.1 인터페이스 정의
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);
}
```

### 2.2 기본 구현체
```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.FindAsync(id) != null;
    }
}
```

## 3. 특화된 리포지토리

### 3.1 도메인별 리포지토리
```csharp
public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<bool> IsEmailUniqueAsync(string email);
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DbContext context) : base(context)
    {
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _dbSet.AnyAsync(u => u.Email == email);
    }
}
```

### 3.2 복합 쿼리 리포지토리
```csharp
public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
    Task<OrderSummary> GetOrderSummaryAsync(int orderId);
    Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
}

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(DbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<OrderSummary> GetOrderSummaryAsync(int orderId)
    {
        return await _dbSet
            .Where(o => o.Id == orderId)
            .Select(o => new OrderSummary
            {
                OrderId = o.Id,
                TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                ItemCount = o.OrderItems.Count,
                OrderDate = o.OrderDate
            })
            .FirstOrDefaultAsync();
    }
}
```

## 4. Unit of Work 패턴

### 4.1 Unit of Work 인터페이스
```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IOrderRepository Orders { get; }
    IProductRepository Products { get; }
    Task<int> SaveChangesAsync();
}
```

### 4.2 Unit of Work 구현
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private IUserRepository _users;
    private IOrderRepository _orders;
    private IProductRepository _products;

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => 
        _users ??= new UserRepository(_context);
    
    public IOrderRepository Orders => 
        _orders ??= new OrderRepository(_context);
    
    public IProductRepository Products => 
        _products ??= new ProductRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

## 5. 서비스 계층에서 사용

### 5.1 서비스 구현
```csharp
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Order> CreateOrderAsync(OrderRequest request)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            OrderItems = request.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();

        return order;
    }

    public async Task<OrderSummary> GetOrderSummaryAsync(int orderId)
    {
        return await _unitOfWork.Orders.GetOrderSummaryAsync(orderId);
    }
}
```

## 6. 의존성 주입 설정

### 6.1 서비스 등록
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IOrderRepository, OrderRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
    
    services.AddScoped<OrderService>();
    services.AddScoped<UserService>();
    services.AddScoped<ProductService>();
}
```

## 7. 모범 사례

### 7.1 설계 고려사항
1. **인터페이스 분리**
   - 명확한 책임 정의
   - 필요한 메서드만 노출
   - 확장성 고려

2. **성능 최적화**
   - 필요한 데이터만 조회
   - 적절한 인덱스 사용
   - 쿼리 최적화

3. **트랜잭션 관리**
   - Unit of Work 패턴 활용
   - 일관성 유지
   - 롤백 처리

### 7.2 구현 고려사항
1. **비동기 작업**
   - 모든 데이터베이스 작업은 비동기로
   - 적절한 예외 처리
   - 리소스 관리

2. **테스트 용이성**
   - 인터페이스 기반 설계
   - 의존성 주입 활용
   - 모의 객체 사용

3. **코드 재사용**
   - 공통 기능 추상화
   - 확장 메서드 활용
   - 상속 구조 설계

### 7.3 유지보수 고려사항
1. **문서화**
   - 메서드 목적 명시
   - 매개변수 설명
   - 반환 값 설명

2. **버전 관리**
   - 인터페이스 변경 시 주의
   - 하위 호환성 유지
   - 마이그레이션 계획

3. **모니터링**
   - 성능 메트릭스 수집
   - 에러 로깅
   - 사용 패턴 분석

## 8. 구현 시 고려사항

### 8.1 ASP.NET Core 특화 고려사항
1. **의존성 주입 활용**
   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       // 리포지토리 등록
       services.AddScoped<IRepository<User>, UserRepository>();
       
       // Unit of Work 등록
       services.AddScoped<IUnitOfWork, UnitOfWork>();
       
       // 서비스 등록
       services.AddScoped<UserService>();
   }
   ```

2. **비동기 작업 처리**
   ```csharp
   public async Task<User> GetUserAsync(int id)
   {
       return await _dbSet.FindAsync(id);
   }
   ```

3. **예외 처리**
   ```csharp
   public async Task AddAsync(T entity)
   {
       try
       {
           await _dbSet.AddAsync(entity);
           await _context.SaveChangesAsync();
       }
       catch (DbUpdateException ex)
       {
           // 데이터베이스 예외 처리
       }
   }
   ```

### 8.2 성능 최적화
1. **쿼리 최적화**
   ```csharp
   public async Task<IEnumerable<Order>> GetOrdersWithDetailsAsync()
   {
       return await _dbSet
           .Include(o => o.OrderItems)
           .Include(o => o.Customer)
           .AsNoTracking()  // 읽기 전용 쿼리 최적화
           .ToListAsync();
   }
   ```

2. **메모리 관리**
   ```csharp
   public void Dispose()
   {
       _context?.Dispose();
   }
   ```

### 8.3 테스트 전략
1. **단위 테스트**
   ```csharp
   [Fact]
   public async Task GetUserById_ReturnsUser()
   {
       // Arrange
       var mockContext = new Mock<DbContext>();
       var repository = new UserRepository(mockContext.Object);
       
       // Act
       var result = await repository.GetByIdAsync(1);
       
       // Assert
       Assert.NotNull(result);
   }
   ```

2. **통합 테스트**
   ```csharp
   [Fact]
   public async Task CreateOrder_WithValidData_SavesToDatabase()
   {
       // Arrange
       var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "TestDb")
           .Options;
           
       // Act & Assert
       using (var context = new ApplicationDbContext(options))
       {
           var repository = new OrderRepository(context);
           // 테스트 수행
       }
   }
   ```

## 9. 결론

### 9.1 리포지토리 패턴의 가치
- 코드 구조화 및 유지보수성 향상
- 테스트 용이성 제공
- 데이터 접근 로직의 중앙화
- 비즈니스 로직과 데이터 접근 로직의 명확한 분리

### 9.2 구현 시 주의사항
- 과도한 추상화 지양
- 실제 필요에 따른 구현
- 성능 고려
- 테스트 용이성 확보

### 9.3 참고 자료
- [Entity Framework Core Documentation](https://learn.microsoft.com/ef/core/)
- [Repository Pattern in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/repository-pattern)
- [Unit of Work Pattern](https://martinfowler.com/eaaCatalog/unitOfWork.html) 