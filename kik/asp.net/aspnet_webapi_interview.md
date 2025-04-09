
# ASP.NET Web API 면접 질문 및 모범 답변

## 🧱 기본 수준

### 1. ASP.NET Web API란 무엇인가요?
ASP.NET Web API는 HTTP 기반의 RESTful 서비스를 만들 수 있도록 해주는 프레임워크입니다. 클라이언트(웹, 모바일 등)와 서버 간의 통신을 위해 JSON, XML 등 다양한 포맷으로 데이터를 주고받을 수 있습니다.

---

### 2. Web API와 MVC의 차이점은?
MVC는 HTML 기반 뷰를 반환하여 UI 중심의 웹앱을 만들 때 사용합니다. 반면 Web API는 주로 JSON/XML과 같은 데이터를 반환하며, 프론트엔드 애플리케이션 또는 모바일 앱의 백엔드 역할을 합니다.

---

### 3. HTTP Verb(GET, POST, PUT, DELETE)의 역할은?
- `GET`: 데이터 조회  
- `POST`: 데이터 생성  
- `PUT`: 전체 데이터 수정  
- `PATCH`: 일부 데이터 수정  
- `DELETE`: 데이터 삭제

---

### 4. Web API에서 Routing이란 무엇인가요?
Routing은 들어오는 HTTP 요청을 적절한 컨트롤러의 액션 메서드로 매핑하는 역할을 합니다. ASP.NET Core에서는 `MapControllerRoute` 또는 attribute routing(`[HttpGet("api/products/{id}")]`)을 자주 사용합니다.

---

### 5. Model Binding이란?
Model Binding은 클라이언트에서 전송된 데이터를 자동으로 C# 객체로 변환해주는 기능입니다.

---

## ⚙️ 중급 수준

### 6. 데이터 검증(Validation)은 어떻게 하나요?
Data Annotation 속성(예: `[Required]`, `[MaxLength]`, `[Range]`)을 모델에 지정하고, `ModelState.IsValid`로 유효성을 검사합니다. `[ApiController]`를 사용하면 자동 검사도 지원됩니다.

---

### 7. 의존성 주입(DI)을 Web API에서 어떻게 활용하나요?
Startup.cs의 `ConfigureServices` 메서드에서 서비스를 등록하고, 생성자 주입을 통해 컨트롤러나 서비스에서 사용합니다.
```csharp
services.AddScoped<IProductService, ProductService>();
```

---

### 8. 필터(Filter)의 역할은?
공통 처리 로직(인증, 로깅 등)을 삽입할 수 있으며, 대표적으로 `ActionFilter`, `ExceptionFilter`, `AuthorizationFilter`가 있습니다.

---

### 9. CORS란?
다른 도메인에서 API를 호출할 수 있도록 허용하는 보안 정책입니다. `services.AddCors()`와 `app.UseCors()`로 설정합니다.

---

### 10. Swagger란 무엇이며 어떻게 쓰나요?
Swagger는 API 문서를 자동 생성하고 테스트할 수 있는 도구입니다. `Swashbuckle.AspNetCore` 패키지를 사용하여 설정합니다.

---

## 🚀 고급 수준

### 11. JWT를 이용한 인증 방식은?
로그인 시 JWT 토큰을 발급하고, 클라이언트는 Authorization 헤더에 포함시켜 요청합니다. 서버는 토큰을 검증하여 인증합니다.

---

### 12. 버전 관리는 어떻게 하나요?
URL, 쿼리스트링, 헤더 등을 통해 버전 정보를 전달합니다. `Microsoft.AspNetCore.Mvc.Versioning` 패키지를 사용하면 설정이 간편합니다.

---

### 13. Rate Limiting은 어떻게 구현하나요?
IP별 요청 횟수를 추적하여 제한하는 방식입니다. `AspNetCoreRateLimit` 라이브러리나 Redis 등을 활용합니다.

---

### 14. 비동기 처리(async/await)을 사용하는 이유는?
I/O 작업을 효율적으로 처리하여 스레드 블로킹을 방지하고 서버 성능을 높입니다.

---

### 15. 성능 향상 방법은?
- 캐싱
- Gzip 압축
- 페이징 처리
- 비동기 처리
- 미들웨어 최소화
- 데이터베이스 최적화
