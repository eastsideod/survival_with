# DI Container (의존성 주입 컨테이너)

## 1. DI Container 소개

### 1.1 DI Container란?
- 객체의 생성과 의존성 관리를 담당하는 프레임워크
- 객체의 생명주기 관리
- 의존성 해결 및 주입 자동화
- 서비스 등록 및 해결을 위한 중앙 집중식 관리

### 1.2 ASP.NET Core의 DI Container 구조
1. **IServiceCollection**
   - 서비스 등록을 위한 컬렉션 인터페이스
   - 서비스와 구현체의 매핑 정보 저장
   - `ConfigureServices` 메서드에서 사용

2. **IServiceProvider**
   - 실제 DI Container 인터페이스
   - 등록된 서비스의 인스턴스 생성 및 관리
   - 의존성 해결 담당

3. **ServiceDescriptor**
   - 서비스 등록 정보를 담는 클래스
   - 서비스 타입, 구현체 타입, 수명 주기 정보 포함

## 2. DI Container의 동작 방식

### 2.1 서비스 등록 과정
```csharp
// 1. 서비스 등록 (IServiceCollection 사용)
public void ConfigureServices(IServiceCollection services)
{
    // 기본 등록
    services.AddScoped<IService, Service>();
    
    // 수명 주기 지정
    services.AddSingleton<ICacheService, CacheService>();
    services.AddTransient<IValidator, Validator>();
    
    // 팩토리 등록
    services.AddScoped<IService>(sp => new Service(sp.GetRequiredService<ILogger>()));
}
```

### 2.2 의존성 해결 과정
```csharp
// 1. 직접 해결 (IServiceProvider 사용)
public class SomeClass
{
    private readonly IServiceProvider _serviceProvider;
    
    public SomeClass(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void DoSomething()
    {
        var service = _serviceProvider.GetRequiredService<IService>();
        // 사용
    }
}

// 2. 생성자 주입 (자동으로 IServiceProvider가 처리)
public class Client
{
    private readonly IService _service;
    
    public Client(IService service)  // DI Container가 자동으로 주입
    {
        _service = service;
    }
}
```

### 2.3 생명주기 관리
```csharp
// 1. Singleton
var singleton1 = serviceProvider.GetService<IService>();
var singleton2 = serviceProvider.GetService<IService>();
// singleton1 == singleton2

// 2. Scoped
using (var scope = serviceProvider.CreateScope())
{
    var scoped1 = scope.ServiceProvider.GetService<IService>();
    var scoped2 = scope.ServiceProvider.GetService<IService>();
    // scoped1 == scoped2
}

// 3. Transient
var transient1 = serviceProvider.GetService<IService>();
var transient2 = serviceProvider.GetService<IService>();
// transient1 != transient2
```

## 3. DI Container의 장점

### 3.1 코드 품질 향상
- 결합도 감소
- 테스트 용이성 증가
- 코드 재사용성 향상
- 유지보수성 개선

### 3.2 개발 생산성
- 의존성 관리 자동화
- 설정 중앙화
- 일관된 객체 생성
- 리소스 관리 용이

### 3.3 애플리케이션 구조
- 모듈화 촉진
- 관심사 분리
- 확장성 향상
- 유연한 구성

## 4. 주요 DI Container 구현체

### 4.1 .NET Core 내장 DI Container
- Microsoft.Extensions.DependencyInjection
- 경량화된 기본 구현
- ASP.NET Core와 통합
- `IServiceCollection`과 `IServiceProvider` 인터페이스 제공

### 4.2 인기 있는 DI Container
- Autofac
- Ninject
- Unity
- Simple Injector
- Castle Windsor

## 5. DI Container 선택 기준

### 5.1 성능
- 의존성 해결 속도
- 메모리 사용량
- 스레드 안전성

### 5.2 기능
- 수명 주기 관리
- 인터셉터 지원
- 속성 주입
- 자식 컨테이너

### 5.3 사용성
- 설정 용이성
- 문서화 품질
- 커뮤니티 지원
- 학습 곡선

## 6. DI Container 사용 시 고려사항

### 6.1 서비스 등록
- 적절한 수명 주기 선택
- 순환 의존성 방지
- 명확한 인터페이스 정의

### 6.2 성능
- 불필요한 서비스 해결 방지
- 적절한 캐싱 전략
- 리소스 관리

### 6.3 테스트
- Mock 객체 주입
- 테스트 환경 구성
- 격리된 테스트

## 7. 참고 자료
- [Dependency Injection in .NET](https://docs.microsoft.com/dotnet/core/extensions/dependency-injection)
- [Inversion of Control Containers and the Dependency Injection pattern](https://martinfowler.com/articles/injection.html)
- [Dependency Injection Principles, Practices, and Patterns](https://www.manning.com/books/dependency-injection-principles-practices-patterns) 