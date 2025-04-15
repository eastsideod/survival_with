# ASP.NET Core 데이터베이스

## 1. Entity Framework Core 개요

### 1.1 EF Core란?
- .NET용 ORM (Object-Relational Mapper)
- 데이터베이스 작업을 객체 지향적으로 처리
- 다양한 데이터베이스 지원

### 1.2 주요 기능
- 코드 우선 접근 방식
- 마이그레이션 지원
- LINQ 쿼리 지원
- 변경 추적
- 성능 최적화

## 2. 기본 설정

### 2.1 NuGet 패키지 설치
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### 2.2 DbContext 설정
```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 모델 구성
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
```

### 2.3 서비스 등록
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection")));
}
```

## 3. 모델 정의

### 3.1 기본 모델
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Post> Posts { get; set; }
}

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<Comment> Comments { get; set; }
}
```

### 3.2 모델 구성
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>(entity =>
    {
        entity.Property(e => e.Username)
            .IsRequired()  // NULL 값 허용하지 않음
            .HasMaxLength(50);  // 최대 길이 50자로 제한

        entity.Property(e => e.Email)
            .IsRequired()  // NULL 값 허용하지 않음
            .HasMaxLength(100);  // 최대 길이 100자로 제한

        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETDATE()");  // 기본값으로 현재 시간 설정
    });
}
```

모델 구성은 데이터베이스 테이블의 구조와 제약 조건을 정의하는 중요한 단계입니다. 이 구성은 다음과 같은 목적으로 사용됩니다:

1. **데이터 무결성 보장**
   - `IsRequired()`: 필수 필드 지정으로 NULL 값 방지
   - `HasMaxLength()`: 문자열 길이 제한으로 데이터 일관성 유지
   - 기본값 설정으로 누락된 데이터 방지

2. **데이터베이스 최적화**
   - 적절한 데이터 타입과 크기 지정으로 저장 공간 최적화
   - 인덱스 설정으로 쿼리 성능 향상
   - 제약 조건으로 데이터 검증

3. **비즈니스 규칙 적용**
   - 도메인 규칙을 데이터베이스 레벨에서 강제
   - 애플리케이션 코드와 데이터베이스 간의 일관성 유지
   - 데이터 유효성 검사 규칙 정의

4. **기본값 및 자동화 설정**
   - `HasDefaultValueSql()`: 자동 생성되는 값 설정
   - 타임스탬프 자동 업데이트
   - 순차적 ID 생성

5. **관계 정의**
   - 테이블 간의 관계 설정
   - 외래 키 제약 조건
   - 캐스케이드 삭제 규칙

이러한 모델 구성을 통해:
- 데이터의 정확성과 일관성을 보장
- 데이터베이스 성능을 최적화
- 애플리케이션의 비즈니스 규칙을 데이터베이스 레벨에서 강제
- 개발자의 실수를 방지하고 코드 품질을 향상

## 4. 데이터 작업

### 4.1 기본 CRUD 작업
```csharp
public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 생성
    public async Task<User> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    // 조회
    public async Task<User> GetUser(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    // 수정
    public async Task UpdateUser(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    // 삭제
    public async Task DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
```

### 4.2 쿼리 작성
```csharp
public class PostService
{
    private readonly ApplicationDbContext _context;

    // 기본 쿼리
    public async Task<List<Post>> GetRecentPosts(int count)
    {
        return await _context.Posts
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    // 조인 쿼리
    public async Task<List<Post>> GetPostsWithUsers()
    {
        return await _context.Posts
            .Include(p => p.User)
            .ToListAsync();
    }

    // 조건부 쿼리
    public async Task<List<Post>> SearchPosts(string keyword)
    {
        return await _context.Posts
            .Where(p => p.Title.Contains(keyword) || 
                       p.Content.Contains(keyword))
            .ToListAsync();
    }
}
```

## 5. 마이그레이션

### 5.1 마이그레이션 생성
```bash
dotnet ef migrations add InitialCreate
```

### 5.2 데이터베이스 업데이트
```bash
dotnet ef database update
```

### 5.3 마이그레이션 제거
```bash
dotnet ef migrations remove
```

## 6. 성능 최적화

### 6.1 인덱스 설정
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Post>()
        .HasIndex(p => p.Title);
        
    modelBuilder.Entity<Comment>()
        .HasIndex(c => new { c.PostId, c.CreatedAt });
}
```

### 6.2 쿼리 최적화
```csharp
// 필요한 데이터만 선택
public async Task<List<PostSummary>> GetPostSummaries()
{
    return await _context.Posts
        .Select(p => new PostSummary
        {
            Id = p.Id,
            Title = p.Title,
            AuthorName = p.User.Username,
            CommentCount = p.Comments.Count
        })
        .ToListAsync();
}

// 페이지네이션
public async Task<PaginatedList<Post>> GetPosts(int pageNumber, int pageSize)
{
    var totalItems = await _context.Posts.CountAsync();
    var items = await _context.Posts
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PaginatedList<Post>(items, totalItems, pageNumber, pageSize);
}
```

## 7. 트랜잭션 관리

### 7.1 기본 트랜잭션
```csharp
public async Task TransferMoney(int fromAccountId, int toAccountId, decimal amount)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        var fromAccount = await _context.Accounts.FindAsync(fromAccountId);
        var toAccount = await _context.Accounts.FindAsync(toAccountId);

        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### 7.2 분산 트랜잭션
```csharp
public async Task ProcessOrder(Order order)
{
    // TransactionScope를 사용하여 분산 트랜잭션 시작
    // TransactionScopeAsyncFlowOption.Enabled는 비동기 작업을 지원
    using var scope = new TransactionScope(
        TransactionScopeAsyncFlowOption.Enabled);
    try
    {
        // 주문 서비스에서 주문 생성
        await _orderService.CreateOrder(order);
        
        // 재고 서비스에서 재고 업데이트
        await _inventoryService.UpdateStock(order.Items);
        
        // 결제 서비스에서 결제 처리
        await _paymentService.ProcessPayment(order);

        // 모든 작업이 성공하면 트랜잭션 커밋
        scope.Complete();
    }
    catch
    {
        // 오류 발생 시 트랜잭션 롤백
        scope.Dispose();
        throw;
    }
}
```

분산 트랜잭션은 여러 서비스나 데이터베이스에 걸쳐 있는 작업을 하나의 논리적 단위로 처리하는 메커니즘입니다. 이는 다음과 같은 상황에서 중요합니다:

1. **분산 트랜잭션의 필요성**
   - 여러 마이크로서비스 간의 데이터 일관성 유지
   - 여러 데이터베이스에 걸친 작업의 원자성 보장
   - 부분 실패 시 전체 롤백 보장

2. **TransactionScope의 동작 방식**
   - `TransactionScope`는 암시적 트랜잭션 관리 제공
   - 내부적으로 DTC(Distributed Transaction Coordinator) 사용
   - 모든 참여 서비스가 트랜잭션에 참여

3. **주요 특징**
   - **원자성(Atomicity)**: 모든 작업이 성공하거나 모두 실패
   - **일관성(Consistency)**: 트랜잭션 전후 데이터 일관성 유지
   - **격리성(Isolation)**: 동시 트랜잭션 간 간섭 방지
   - **지속성(Durability)**: 커밋된 트랜잭션은 영구 저장

4. **사용 시 고려사항**
   - **성능 영향**: 분산 트랜잭션은 성능 오버헤드가 큼
   - **복잡성**: 구현과 디버깅이 복잡할 수 있음
   - **가용성**: 일부 서비스 장애 시 전체 트랜잭션 실패 가능

5. **대안적 접근 방식**
   - **사가 패턴**: 장기 실행 트랜잭션을 여러 단계로 분해
   - **이벤트 소싱**: 상태 변경을 이벤트로 기록
   - **보상 트랜잭션**: 실패 시 이전 작업을 취소하는 보상 작업 실행

6. **게임 서버에서의 활용 예시**
   ```csharp
   public async Task ProcessGameTransaction(GameTransaction transaction)
   {
       using var scope = new TransactionScope(
           TransactionScopeAsyncFlowOption.Enabled);
       try
       {
           // 인벤토리 서비스에서 아이템 추가
           await _inventoryService.AddItem(transaction.PlayerId, transaction.ItemId);
           
           // 통화 서비스에서 통화 차감
           await _currencyService.DeductCurrency(transaction.PlayerId, transaction.Amount);
           
           // 로그 서비스에 거래 기록
           await _loggingService.LogTransaction(transaction);

           scope.Complete();
       }
       catch
       {
           scope.Dispose();
           throw;
       }
   }
   ```

7. **모범 사례**
   - 트랜잭션 범위를 최소화
   - 타임아웃 설정으로 교착 상태 방지
   - 적절한 격리 수준 선택
   - 오류 처리 및 복구 전략 수립

## 8. 모범 사례

### 8.1 성능 고려사항
1. **인덱스 전략**
   - 자주 조회되는 컬럼에 인덱스
   - 복합 인덱스 활용
   - 인덱스 유지보수

2. **쿼리 최적화**
   - 필요한 데이터만 조회
   - N+1 문제 방지
   - 적절한 조인 사용

3. **연결 관리**
   - 연결 풀링 활용
   - 비동기 작업 사용
   - 리소스 해제

### 8.2 보안 고려사항
1. **SQL 인젝션 방지**
   - 파라미터화된 쿼리 사용
   - 저장 프로시저 활용
   - 입력 검증

2. **데이터 접근**
   - 최소 권한 원칙
   - 암호화된 연결
   - 감사 로깅

### 8.3 유지보수 고려사항
1. **마이그레이션 관리**
   - 체계적인 버전 관리
   - 롤백 계획
   - 데이터 보존

2. **모델 설계**
   - 명확한 관계 정의
   - 일관된 네이밍
   - 문서화 