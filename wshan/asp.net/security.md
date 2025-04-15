# ASP.NET Core 보안

## 1. 보안 개요

### 1.1 보안의 중요성
- 데이터 보호
- 사용자 정보 보안
- 시스템 무결성 유지
- 법적/규제 준수

### 1.2 주요 보안 위협
1. **인젝션 공격**
   - SQL 인젝션
   - XSS (Cross-Site Scripting)
   - CSRF (Cross-Site Request Forgery)

2. **인증/인가 관련**
   - 세션 하이재킹
   - 비밀번호 크래킹
   - 권한 상승

3. **데이터 노출**
   - 민감 정보 노출
   - 불충분한 암호화
   - 잘못된 설정

## 2. 기본 보안 설정

### 2.1 HTTPS 강제
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseHttpsRedirection();
    // ...
}
```

### 2.2 보안 헤더 설정
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseHsts();
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        await next();
    });
}
```

## 3. 데이터 보호

### 3.1 데이터 암호화
```csharp
public class DataProtectionService
{
    private readonly IDataProtector _protector;

    public DataProtectionService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("Purpose.String");
    }

    public string Encrypt(string input)
    {
        return _protector.Protect(input);
    }

    public string Decrypt(string protectedData)
    {
        return _protector.Unprotect(protectedData);
    }
}
```

### 3.2 민감 정보 관리
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"\\server\share\directory\"))
        .SetApplicationName("MyApplication");
}
```

## 4. 입력 검증

### 4.1 XSS 방지
```csharp
[HttpPost]
public IActionResult CreateComment([FromBody] CommentModel comment)
{
    // HTML 인코딩
    comment.Content = WebUtility.HtmlEncode(comment.Content);
    
    // 추가 검증
    if (string.IsNullOrEmpty(comment.Content))
    {
        return BadRequest("댓글 내용이 비어있습니다.");
    }
    
    // 저장
    _commentRepository.Add(comment);
    return Ok();
}
```

### 4.2 SQL 인젝션 방지
```csharp
public class UserRepository
{
    private readonly DbContext _context;

    public async Task<User> GetUserByUsername(string username)
    {
        // 파라미터화된 쿼리 사용
        return await _context.Users
            .FromSqlRaw("SELECT * FROM Users WHERE Username = {0}", username)
            .FirstOrDefaultAsync();
    }
}
```

## 5. CSRF 방지

### 5.1 토큰 기반 방어
```csharp
[ValidateAntiForgeryToken]
public class AccountController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        // 비밀번호 변경 로직
    }
}
```

### 5.2 SameSite 쿠키 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<CookiePolicyOptions>(options =>
    {
        options.MinimumSameSitePolicy = SameSiteMode.Strict;
        options.HttpOnly = HttpOnlyPolicy.Always;
        options.Secure = CookieSecurePolicy.Always;
    });
}
```

## 6. 보안 모범 사례

### 6.1 코드 보안
1. **입력 검증**
   - 모든 입력 데이터 검증
   - 화이트리스트 기반 검증
   - 적절한 에러 메시지

2. **출력 인코딩**
   - HTML 인코딩
   - JavaScript 인코딩
   - URL 인코딩

3. **에러 처리**
   - 상세한 에러 메시지 제한
   - 로깅 전략 수립
   - 적절한 예외 처리

### 6.2 인프라 보안
1. **네트워크 보안**
   - 방화벽 설정
   - SSL/TLS 구성
   - 네트워크 분리

2. **서버 보안**
   - 최신 보안 패치 적용
   - 불필요한 서비스 제거
   - 접근 제어 설정

3. **데이터 보안**
   - 암호화 저장
   - 백업 전략
   - 데이터 마스킹

### 6.3 운영 보안
1. **모니터링**
   - 보안 이벤트 모니터링
   - 이상 징후 감지
   - 로그 분석

2. **응답 계획**
   - 보안 사고 대응 절차
   - 복구 계획
   - 통신 계획

3. **정기 점검**
   - 보안 취약점 스캔
   - 침투 테스트
   - 코드 리뷰 