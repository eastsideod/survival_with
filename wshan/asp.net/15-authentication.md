# ASP.NET Core 인증

## 1. 인증 개요

### 1.1 인증이란?
- 사용자의 신원을 확인하는 과정
- 클라이언트가 누구인지 확인하는 메커니즘
- 보안의 첫 번째 단계

### 1.2 주요 인증 방식
1. **JWT (JSON Web Token)**
   - 토큰 기반 인증
   - 상태를 저장하지 않는 방식
   - 모바일/웹 앱에 적합

2. **Cookie 인증**
   - 세션 기반 인증
   - 서버 측 상태 유지
   - 웹 애플리케이션에 적합

3. **OAuth 2.0 / OpenID Connect**
   - 소셜 로그인
   - 외부 인증 제공자 사용
   - SSO(Single Sign-On) 구현

## 2. JWT 인증 구현

### 2.1 JWT 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            };
        });
}
```

### 2.2 토큰 생성
```csharp
public class AuthService
{
    private readonly IConfiguration _configuration;

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 2.3 컨트롤러에서 사용
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // 사용자 인증 로직
        var token = _authService.GenerateToken(user);
        return Ok(new { Token = token });
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // 프로필 정보 반환
    }
}
```

## 3. Cookie 인증 구현

### 3.1 Cookie 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "AuthCookie";
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(1);
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });
}
```

### 3.2 로그인 처리
```csharp
public class AccountController : Controller
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // 사용자 인증 로직
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, 
            CookieAuthenticationDefaults.AuthenticationScheme);
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }
}
```

## 4. OAuth 2.0 구현

### 4.1 Google 로그인 설정
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = Configuration["Google:ClientId"];
            options.ClientSecret = Configuration["Google:ClientSecret"];
        });
}
```

### 4.2 소셜 로그인 처리
```csharp
[HttpPost("external-login")]
public IActionResult ExternalLogin(string provider)
{
    var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
    var properties = _signInManager.ConfigureExternalAuthenticationProperties(
        provider, redirectUrl);
    return Challenge(properties, provider);
}

[HttpGet("external-login-callback")]
public async Task<IActionResult> ExternalLoginCallback()
{
    var info = await _signInManager.GetExternalLoginInfoAsync();
    if (info == null)
        return RedirectToAction("Login");

    // 사용자 정보 처리
    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
    // 추가 로직
}
```

## 5. 인증 모범 사례

### 5.1 보안 고려사항
1. **토큰 관리**
   - 짧은 만료 시간 설정
   - 리프레시 토큰 사용
   - 토큰 저장소 보안

2. **비밀번호 처리**
   - 강력한 해싱 알고리즘 사용
   - 솔트(Salt) 사용
   - 비밀번호 정책 적용

3. **세션 관리**
   - 적절한 세션 타임아웃
   - 동시 로그인 제한
   - 로그아웃 처리

### 5.2 성능 고려사항
1. **토큰 검증**
   - 서명 검증 최적화
   - 캐싱 전략 수립
   - 불필요한 검증 제거

2. **세션 저장**
   - 분산 캐시 사용
   - 메모리 사용 최적화
   - 세션 데이터 최소화

### 5.3 확장성 고려사항
1. **분산 환경**
   - 상태 저장 방식 선택
   - 세션 공유 전략
   - 부하 분산 고려

2. **마이크로서비스**
   - 인증 서비스 분리
   - 토큰 기반 통신
   - 서비스 간 인증 