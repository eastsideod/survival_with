# ASP.NET Core 모델 바인딩

모델 바인딩은 HTTP 요청의 데이터를 C# 객체로 자동 변환하는 기능입니다. ASP.NET Core는 다양한 소스에서 데이터를 바인딩할 수 있습니다.

## 기본 바인딩

### 1. 쿼리 스트링 바인딩
```csharp
// 단일 값
public IActionResult GetPlayer([FromQuery] int id)
{
    // URL: /api/player?id=123
}

// 여러 값
public IActionResult Search([FromQuery] string name, [FromQuery] int? level)
{
    // URL: /api/player/search?name=홍길동&level=10
}
```

### 2. 라우트 파라미터 바인딩
```csharp
[HttpGet("{id}")]
public IActionResult GetPlayer([FromRoute] int id)
{
    // URL: /api/player/123
}

[HttpGet("{id}/items/{itemId}")]
public IActionResult GetItem([FromRoute] int id, [FromRoute] int itemId)
{
    // URL: /api/player/123/items/456
}
```

### 3. 요청 본문 바인딩
```csharp
[HttpPost]
public IActionResult Create([FromBody] PlayerModel player)
{
    // Body: { "name": "홍길동", "level": 1 }
}
```

### 4. 폼 데이터 바인딩
```csharp
[HttpPost]
public IActionResult Update([FromForm] PlayerModel player)
{
    // Form: name=홍길동&level=1
}
```

## 모델 바인딩 어트리뷰트

### 1. 데이터 소스 지정 어트리뷰트
1. **FromRoute**
   - URL 경로에서 데이터를 바인딩
   - 예: `/api/player/{id}`에서 `id` 값 추출
   - 기본적으로 숫자, 문자열 등의 단순 타입에 사용
   ```csharp
   [HttpGet("{id}")]
   public IActionResult GetPlayer([FromRoute] int id)
   {
       // URL: /api/player/123
   }
   ```

2. **FromQuery**
   - URL 쿼리 스트링에서 데이터를 바인딩
   - 예: `/api/player?name=홍길동&level=10`
   - 필터링, 정렬, 페이지네이션 등에 사용
   ```csharp
   public IActionResult Search([FromQuery] string name, [FromQuery] int? level)
   {
       // URL: /api/player/search?name=홍길동&level=10
   }
   ```

3. **FromBody**
   - HTTP 요청 본문에서 데이터를 바인딩
   - 주로 JSON, XML 형식의 데이터 처리
   - 복잡한 객체나 컬렉션에 사용
   ```csharp
   [HttpPost]
   public IActionResult Create([FromBody] PlayerModel player)
   {
       // Body: { "name": "홍길동", "level": 1 }
   }
   ```

4. **FromForm**
   - HTML 폼 데이터에서 바인딩
   - `multipart/form-data` 또는 `application/x-www-form-urlencoded` 형식
   - 파일 업로드나 폼 제출에 사용
   ```csharp
   [HttpPost]
   public IActionResult Update([FromForm] PlayerModel player)
   {
       // Form: name=홍길동&level=1
   }
   ```

5. **FromHeader**
   - HTTP 헤더에서 데이터를 바인딩
   - 인증 토큰, 클라이언트 정보 등에 사용
   ```csharp
   public IActionResult GetProfile([FromHeader] string authorization)
   {
       // Header: Authorization: Bearer token123
   }
   ```

6. **FromServices**
   - 의존성 주입 컨테이너에서 서비스를 주입
   - 컨트롤러 생성자 대신 액션 메서드에서 직접 주입
   ```csharp
   public IActionResult GetData([FromServices] IDataService dataService)
   {
       // dataService 사용
   }
   ```

### 2. 바인딩 동작 제어 어트리뷰트
1. **BindRequired**
   - 필수 바인딩 속성 지정
   - 바인딩 실패 시 ModelState.IsValid가 false
   ```csharp
   public class PlayerModel
   {
       [BindRequired]
       public string Name { get; set; }
   }
   ```

2. **BindNever**
   - 바인딩에서 제외할 속성 지정
   - 보안상 민감한 데이터에 사용
   - 클라이언트가 보낸 요청에서 해당 속성의 값을 무시
   - 서버 측 로직에서는 여전히 사용 가능
   ```csharp
   public class PlayerModel
   {
       [BindNever]
       public string SecretKey { get; set; }
   }
   ```

3. **Bind**
   - 특정 속성만 바인딩 허용
   - 화이트리스트 방식의 바인딩 제어
   ```csharp
   [Bind("Name,Level")]
   public class UpdateModel { ... }
   ```

### BindNever 어트리뷰트의 목적과 사용 이유

1. **데이터 모델의 완전성 유지**
   - 모델 클래스는 데이터베이스의 테이블 구조나 도메인 모델을 반영
   - `BindNever`를 사용하면 모델의 구조를 유지하면서도 클라이언트 바인딩을 제어 가능
   - 예시:
   ```csharp
   public class PlayerUpdateModel
   {
       public string Username { get; set; }
       public string DisplayName { get; set; }
       
       [BindNever]
       public string PasswordHash { get; set; }  // 데이터베이스에 저장되어야 하는 필드
   }
   ```

2. **서버 측 로직에서의 사용**
   - `BindNever` 속성은 클라이언트 바인딩만 제한
   - 서버 코드에서는 여전히 해당 속성을 사용 가능
   ```csharp
   [HttpPut("profile")]
   public IActionResult UpdateProfile([FromBody] PlayerUpdateModel model)
   {
       // 클라이언트가 보낸 PasswordHash는 무시됨
       // 서버에서 새로운 비밀번호 해시를 생성
       var hashedPassword = _passwordService.HashPassword(newPassword);
       model.PasswordHash = hashedPassword;  // 서버에서 직접 설정
       
       _repository.UpdatePlayer(model);
   }
   ```

3. **보안과 유지보수성의 균형**
   - 필드를 제거하는 대신 `BindNever`를 사용하면:
     - 보안은 유지 (클라이언트가 수정할 수 없음)
     - 코드의 가독성과 유지보수성도 유지
     - 데이터 구조의 명확성도 유지
   - 예시:
   ```csharp
   public class UserModel
   {
       public string Username { get; set; }
       
       [BindNever]
       public bool IsAdmin { get; set; }  // 관리자 권한
       
       [BindNever]
       public List<string> Permissions { get; set; }  // 권한 목록
   }
   ```

4. **데이터 전송 계층과 도메인 계층의 분리**
   - 모델은 도메인 계층의 데이터 구조를 나타냄
   - `BindNever`는 "이 필드는 클라이언트에서 설정할 수 없다"는 의미
   - 서버 내부 로직에서는 여전히 이 필드들을 사용 가능
   ```csharp
   public class CharacterModel
   {
       public string Name { get; set; }
       public int Level { get; set; }
       
       [BindNever]
       public string AccountId { get; set; }  // 서버에서만 관리
       
       [BindNever]
       public int Gold { get; set; }  // 서버에서만 관리
   }
   ```

### 3. 바인딩 동작 방식
1. **우선순위**
   - 명시적 어트리뷰트 > 기본 바인딩 규칙
   - FromRoute > FromQuery > FromBody > FromForm > FromHeader

2. **바인딩 프로세스**
   - 요청 데이터 소스 식별
   - 모델 생성자 호출
   - 속성 값 설정
   - 유효성 검사 실행

3. **에러 처리**
   - 바인딩 실패 시 ModelState에 에러 추가
   - 기본값 설정 (nullable 타입의 경우 null)
   - 형식 변환 실패 시 예외 발생

## 복합 객체 바인딩

### 1. 중첩된 객체
```csharp
public class PlayerModel
{
    public string Name { get; set; }
    public int Level { get; set; }
    public CharacterStats Stats { get; set; }
}

public class CharacterStats
{
    public int Strength { get; set; }
    public int Dexterity { get; set; }
}

// 요청 본문 예시:
{
    "name": "홍길동",
    "level": 1,
    "stats": {
        "strength": 10,
        "dexterity": 8
    }
}
```

### 2. 컬렉션
```csharp
public class PlayerModel
{
    public string Name { get; set; }
    public List<ItemModel> Inventory { get; set; }
}

public class ItemModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// 요청 본문 예시:
{
    "name": "홍길동",
    "inventory": [
        { "id": 1, "name": "검" },
        { "id": 2, "name": "방패" }
    ]
}
```

## 사용자 정의 바인딩

### 1. 커스텀 모델 바인더
```csharp
public class CustomModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue("custom");
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        var value = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        // 사용자 정의 바인딩 로직
        var model = new CustomModel
        {
            Value = value
        };

        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}

[ModelBinder(BinderType = typeof(CustomModelBinder))]
public class CustomModel
{
    public string Value { get; set; }
}
```

### 2. 바인딩 동작 제어
```csharp
public class PlayerModel
{
    // 필수 속성
    [Required(ErrorMessage = "이름은 필수입니다")]
    public string Name { get; set; }

    // 기본값 설정
    public int Level { get; set; } = 1;

    // 바인딩 제외
    [BindNever]
    public string SecretKey { get; set; }

    // 특정 속성만 바인딩
    [Bind("Name,Level")]
    public class UpdateModel { ... }
}
```

## 바인딩 검증

### 1. 모델 상태 검증
```csharp
[HttpPost]
public IActionResult Create([FromBody] PlayerModel player)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    // ...
}
```

### 2. 사용자 정의 검증
```csharp
public class PlayerModel : IValidatableObject
{
    public string Name { get; set; }
    public int Level { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Level < 1)
        {
            yield return new ValidationResult(
                "레벨은 1 이상이어야 합니다",
                new[] { nameof(Level) });
        }
    }
}
```

## 주의사항

1. **성능**
   - 복잡한 객체 구조는 바인딩 성능에 영향을 줄 수 있음
   - 필요한 속성만 포함하는 DTO 사용 권장

2. **보안**
   - 민감한 정보는 바인딩에서 제외
   - `[BindNever]` 어트리뷰트 사용

3. **유효성 검사**
   - 모든 입력 데이터 검증 필수
   - 서버 측 검증은 필수, 클라이언트 측 검증은 선택

4. **버전 관리**
   - API 버전 변경시 하위 호환성 유지
   - 새로운 필드 추가는 가능, 기존 필드 제거는 주의

## 보안 관련 모델 바인딩 예시

### 1. 민감한 데이터 보호
```csharp
public class PlayerUpdateModel
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    
    [BindNever]
    public string PasswordHash { get; set; }  // 해시된 비밀번호
    
    [BindNever]
    public string Email { get; set; }  // 이메일 주소
    
    [BindNever]
    public string SecurityQuestion { get; set; }  // 보안 질문
}

// 사용 예시
[HttpPut("profile")]
public IActionResult UpdateProfile([FromBody] PlayerUpdateModel model)
{
    // model.PasswordHash, model.Email, model.SecurityQuestion은
    // 요청에서 바인딩되지 않음
    // 서버에서만 설정 가능
}
```

### 2. 권한 관련 데이터 보호
```csharp
public class UserModel
{
    public string Username { get; set; }
    
    [BindNever]
    public bool IsAdmin { get; set; }  // 관리자 권한
    
    [BindNever]
    public List<string> Permissions { get; set; }  // 권한 목록
    
    [BindNever]
    public DateTime LastLoginTime { get; set; }  // 마지막 로그인 시간
}

// 사용 예시
[HttpPost("users")]
public IActionResult CreateUser([FromBody] UserModel model)
{
    // model.IsAdmin, model.Permissions, model.LastLoginTime은
    // 클라이언트가 설정할 수 없음
    // 서버에서만 설정 가능
}
```

### 3. 게임 관련 보안 데이터
```csharp
public class CharacterModel
{
    public string Name { get; set; }
    public int Level { get; set; }
    
    [BindNever]
    public string AccountId { get; set; }  // 계정 ID
    
    [BindNever]
    public int Gold { get; set; }  // 게임 내 화폐
    
    [BindNever]
    public List<Item> Inventory { get; set; }  // 인벤토리 아이템
}

// 사용 예시
[HttpPost("characters")]
public IActionResult CreateCharacter([FromBody] CharacterModel model)
{
    // model.AccountId, model.Gold, model.Inventory는
    // 클라이언트가 직접 설정할 수 없음
    // 서버에서만 초기화 가능
}
```

### 4. 보안 이점
1. **데이터 무결성 보호**
   - 클라이언트가 민감한 데이터를 수정할 수 없음
   - 서버에서만 중요한 데이터를 관리 가능

2. **권한 상승 방지**
   - 클라이언트가 관리자 권한이나 특수 권한을 설정할 수 없음
   - 서버에서만 권한을 부여할 수 있음

3. **데이터 조작 방지**
   - 게임 내 화폐나 아이템을 직접 수정할 수 없음
   - 서버에서만 게임 경제 시스템을 관리 가능

4. **개인정보 보호**
   - 이메일, 전화번호 등 개인정보를 클라이언트가 수정할 수 없음
   - 서버에서만 개인정보를 관리 가능

## 복합 객체 바인딩

### 1. 중첩된 객체
```csharp
public class PlayerModel
{
    public string Name { get; set; }
    public int Level { get; set; }
    public CharacterStats Stats { get; set; }
}

public class CharacterStats
{
    public int Strength { get; set; }
    public int Dexterity { get; set; }
}

// 요청 본문 예시:
{
    "name": "홍길동",
    "level": 1,
    "stats": {
        "strength": 10,
        "dexterity": 8
    }
}
```

### 2. 컬렉션
```csharp
public class PlayerModel
{
    public string Name { get; set; }
    public List<ItemModel> Inventory { get; set; }
}

public class ItemModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// 요청 본문 예시:
{
    "name": "홍길동",
    "inventory": [
        { "id": 1, "name": "검" },
        { "id": 2, "name": "방패" }
    ]
}
``` 