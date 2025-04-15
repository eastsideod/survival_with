# ASP.NET Core 권한 부여

## 1. 권한 부여 개요

### 1.1 권한 부여란?
- 인증된 사용자가 특정 리소스에 접근할 수 있는 권한을 확인하는 과정
- 사용자가 무엇을 할 수 있는지 결정하는 메커니즘
- 보안의 두 번째 단계

### 1.2 주요 권한 부여 방식
1. **역할 기반 권한 부여 (RBAC)**
   - 사용자 역할에 따른 권한 부여
   - 간단하고 직관적인 구현
   - 정적인 권한 관리

2. **정책 기반 권한 부여 (PBAC)**
   - 복잡한 규칙에 따른 권한 부여
   - 유연한 권한 관리
   - 동적인 권한 결정

3. **클레임 기반 권한 부여**
   - 사용자 특성에 따른 권한 부여
   - 세분화된 권한 관리
   - 확장 가능한 구조

## 2. 역할 기반 권한 부여 구현

### 2.1 역할 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdminRole", 
            policy => policy.RequireRole("Admin"));
        options.AddPolicy("RequireUserRole", 
            policy => policy.RequireRole("User"));
    });
}
```

### 2.2 컨트롤러에서 사용
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult ManageUsers()
    {
        // 관리자 또는 매니저만 접근 가능
    }
}
```

## 3. 정책 기반 권한 부여 구현

### 3.1 정책 정의
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        options.AddPolicy("MinimumAge", policy =>
            policy.Requirements.Add(new MinimumAgeRequirement(18)));
        
        options.AddPolicy("CanEditPost", policy =>
            policy.Requirements.Add(new PostEditRequirement()));
    });
}
```

### 3.2 요구사항 클래스
```csharp
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}
```

### 3.3 핸들러 구현
```csharp
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirement requirement)
    {
        var dateOfBirthClaim = context.User.FindFirst(
            c => c.Type == ClaimTypes.DateOfBirth);
            
        if (dateOfBirthClaim != null)
        {
            var dateOfBirth = Convert.ToDateTime(dateOfBirthClaim.Value);
            var age = DateTime.Today.Year - dateOfBirth.Year;
            
            if (age >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
```

## 4. 클레임 기반 권한 부여 구현

### 4.1 클레임이란?
클레임은 사용자에 대한 정보나 특성을 나타내는 단위입니다. 주요 특징은 다음과 같습니다:

1. **클레임의 정의**
   - 사용자의 특성이나 권한을 나타내는 정보 단위
   - "이름:값" 쌍으로 구성된 단순한 데이터 구조
   - 인증된 사용자의 신원과 권한을 표현하는 기본 단위

2. **클레임의 구조**
   ```csharp
   // 클레임의 기본 구조
   Claim claim = new Claim(
       type: "Permission",      // 클레임 타입
       value: "EditProfile",    // 클레임 값
       valueType: "string",     // 값의 타입
       issuer: "MyApp"          // 발급자
   );
   ```

3. **주요 클레임 타입**
   - **기본 클레임**: 이름, 이메일, 역할 등 기본적인 사용자 정보
   - **권한 클레임**: 특정 기능에 대한 접근 권한
   - **커스텀 클레임**: 애플리케이션 특화된 정보

4. **클레임의 장점**
   - **유연성**: 다양한 종류의 정보를 표현 가능
   - **확장성**: 새로운 클레임 타입 추가 용이
   - **세분화**: 세밀한 권한 제어 가능
   - **이식성**: 다른 시스템과의 연동 용이

### 4.2 클레임 기반 정책
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        // 특정 클레임 값이 있는지 확인
        options.AddPolicy("CanEditProfile", policy =>
            policy.RequireClaim("Permission", "EditProfile"));
            
        // 여러 클레임 값 중 하나라도 있는지 확인
        options.AddPolicy("CanManageUsers", policy =>
            policy.RequireClaim("Permission", "EditUser", "DeleteUser"));
            
        // 특정 클레임 타입이 있는지 확인
        options.AddPolicy("HasEmail", policy =>
            policy.RequireClaim(ClaimTypes.Email));
    });
}
```

### 4.3 클레임 사용 예시
```csharp
public class UserController : Controller
{
    // 클레임 생성
    public async Task<IActionResult> AddClaim(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        await _userManager.AddClaimAsync(user, 
            new Claim("Permission", "EditProfile"));
        return Ok();
    }

    // 클레임 확인
    [Authorize(Policy = "CanEditProfile")]
    public IActionResult EditProfile()
    {
        // 현재 사용자의 클레임 확인
        var hasPermission = User.HasClaim("Permission", "EditProfile");
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        
        return View();
    }

    // 클레임 기반 권한 검사
    public IActionResult CheckPermission()
    {
        var authResult = await _authorizationService
            .AuthorizeAsync(User, null, "CanEditProfile");
            
        if (authResult.Succeeded)
        {
            // 권한이 있는 경우
        }
        else
        {
            // 권한이 없는 경우
        }
    }
}
```

### 4.4 클레임 관리 모범 사례

1. **클레임 설계**
   - 명확하고 일관된 클레임 타입 사용
   - 필요한 최소한의 클레임만 포함
   - 중복 클레임 방지

2. **보안 고려사항**
   - 민감한 정보는 클레임에 포함하지 않기
   - 클레임 값의 유효성 검증
   - 클레임 발급자 확인

3. **성능 최적화**
   - 자주 사용되는 클레임 캐싱
   - 불필요한 클레임 제거
   - 클레임 검사 최적화

## 5. 리소스 기반 권한 부여

### 5.1 리소스 핸들러
```csharp
public class PostAuthorizationHandler : 
    AuthorizationHandler<PostEditRequirement, Post>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PostEditRequirement requirement,
        Post resource)
    {
        if (context.User.Identity?.Name == resource.Author)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

### 5.2 컨트롤러에서 사용
```csharp
public class PostController : Controller
{
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        
        var authResult = await _authorizationService
            .AuthorizeAsync(User, post, "CanEditPost");
            
        if (!authResult.Succeeded)
        {
            return Forbid();
        }

        return View(post);
    }
}
```

## 6. 권한 부여 모범 사례

### 6.1 보안 고려사항
1. **최소 권한 원칙**
   - 필요한 최소한의 권한만 부여
   - 권한 상승 방지
   - 정기적인 권한 검토

2. **권한 관리**
   - 중앙화된 권한 관리
   - 권한 변경 추적
   - 권한 위임 제어

3. **감사 로깅**
   - 권한 관련 이벤트 기록
   - 접근 시도 모니터링
   - 보안 위반 감지

### 6.2 성능 고려사항
1. **권한 캐싱**
   - 자주 사용되는 권한 정보 캐시
   - 캐시 무효화 전략
   - 분산 캐시 사용

2. **권한 검사 최적화**
   - 불필요한 권한 검사 제거
   - 일괄 권한 검사
   - 비동기 권한 검사

### 6.3 확장성 고려사항
1. **권한 구조 설계**
   - 모듈화된 권한 시스템
   - 확장 가능한 정책 정의
   - 동적 권한 관리

2. **분산 환경**
   - 권한 정보 동기화
   - 분산 캐시 사용
   - 권한 서비스 분리 