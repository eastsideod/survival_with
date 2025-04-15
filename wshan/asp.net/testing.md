# ASP.NET Core 테스트

## 1. 테스트 개요

### 1.1 테스트의 중요성
- 코드 품질 보장
- 버그 조기 발견
- 리팩토링 안전성
- 문서화 역할

### 1.2 테스트 종류
1. **단위 테스트**
   - 개별 컴포넌트 테스트
   - 빠른 실행
   - 격리된 환경

2. **통합 테스트**
   - 컴포넌트 간 상호작용
   - 실제 환경 시뮬레이션
   - 종단 간 테스트

3. **성능 테스트**
   - 응답 시간 측정
   - 부하 테스트
   - 스트레스 테스트

## 2. 단위 테스트

### 2.1 기본 설정
```csharp
// 필요한 패키지 설치
dotnet add package xunit
dotnet add package Moq
dotnet add package FluentAssertions
```

### 2.2 서비스 테스트
```csharp
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetUserById_ExistingUser_ReturnsUser()
    {
        // Arrange
        var expectedUser = new User { Id = 1, Name = "Test User" };
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _service.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetUserById_NonExistingUser_ThrowsException()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.GetUserByIdAsync(1));
    }
}
```

### 2.3 컨트롤러 테스트
```csharp
public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _controller = new UsersController(_mockService.Object);
    }

    [Fact]
    public async Task GetUser_ExistingUser_ReturnsOk()
    {
        // Arrange
        var user = new User { Id = 1, Name = "Test User" };
        _mockService.Setup(s => s.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(1, returnedUser.Id);
        Assert.Equal("Test User", returnedUser.Name);
    }
}
```

## 3. 통합 테스트

### 3.1 테스트 서버 설정
```csharp
public class IntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public IntegrationTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetUser_ReturnsUser()
    {
        // Arrange
        var userId = 1;

        // Act
        var response = await _client.GetAsync($"/api/users/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadAsAsync<User>();
        Assert.Equal(userId, user.Id);
    }
}
```

### 3.2 데이터베이스 테스트
```csharp
public class DatabaseTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public DatabaseTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddUser_SavesToDatabase()
    {
        // Arrange
        var user = new User { Name = "Test User" };

        // Act
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users.FirstOrDefaultAsync();
        Assert.NotNull(savedUser);
        Assert.Equal("Test User", savedUser.Name);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

## 4. 성능 테스트

### 4.1 벤치마크 테스트
```csharp
[MemoryDiagnoser]
public class UserServiceBenchmark
{
    private readonly UserService _service;
    private readonly List<int> _userIds;

    public UserServiceBenchmark()
    {
        var repository = new Mock<IUserRepository>();
        var logger = new Mock<ILogger<UserService>>();
        _service = new UserService(repository.Object, logger.Object);
        _userIds = Enumerable.Range(1, 1000).ToList();
    }

    [Benchmark]
    public async Task GetUsers_Performance()
    {
        foreach (var id in _userIds)
        {
            await _service.GetUserByIdAsync(id);
        }
    }
}
```

### 4.2 부하 테스트
```csharp
public class LoadTests
{
    private readonly HttpClient _client;

    public LoadTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("https://api.example.com") };
    }

    [Fact]
    public async Task ConcurrentRequests_HandlesLoad()
    {
        // Arrange
        var tasks = new List<Task>();
        var userIds = Enumerable.Range(1, 100).ToList();

        // Act
        foreach (var id in userIds)
        {
            tasks.Add(_client.GetAsync($"/api/users/{id}"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert
        Assert.All(responses, r => r.EnsureSuccessStatusCode());
    }
}
```

## 5. 테스트 모범 사례

### 5.1 테스트 작성 원칙
1. **AAA 패턴**
   - Arrange: 테스트 준비
   - Act: 테스트 실행
   - Assert: 결과 검증

2. **명확한 테스트 이름**
   - 테스트 대상
   - 예상 동작
   - 예상 결과

3. **독립적인 테스트**
   - 테스트 간 의존성 없음
   - 실행 순서 무관
   - 격리된 환경

### 5.2 테스트 커버리지
1. **코드 커버리지**
   - 최소 80% 이상 권장
   - 중요한 비즈니스 로직 100%
   - 테스트되지 않은 코드 식별

2. **테스트 범위**
   - 핵심 기능 우선
   - 엣지 케이스 포함
   - 실패 케이스 포함

### 5.3 테스트 유지보수
1. **리팩토링**
   - 테스트 코드도 리팩토링
   - 중복 제거
   - 가독성 향상

2. **문서화**
   - 테스트 목적 설명
   - 테스트 데이터 설명
   - 특별한 고려사항 기록

3. **지속적인 통합**
   - 자동화된 테스트 실행
   - 빠른 피드백
   - 품질 게이트 