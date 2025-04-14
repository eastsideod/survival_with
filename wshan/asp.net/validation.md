# ASP.NET Core 유효성 검사

ASP.NET Core는 모델 바인딩 후 자동으로 유효성 검사를 수행합니다. 이는 개발자가 직접 작성한 검증 로직과 함께 동작합니다.

## 1. 기본 유효성 검사

### 1.1 데이터 어노테이션을 이용한 검증
```csharp
public class PlayerModel
{
    [Required(ErrorMessage = "이름은 필수입니다")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "이름은 3~20자여야 합니다")]
    public string Name { get; set; }

    [Range(1, 100, ErrorMessage = "레벨은 1~100 사이여야 합니다")]
    public int Level { get; set; }

    [EmailAddress(ErrorMessage = "올바른 이메일 형식이 아닙니다")]
    public string Email { get; set; }

    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "영문과 숫자만 사용 가능합니다")]
    public string Nickname { get; set; }
}
```

### 1.2 자동 검증 결과 확인
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

## 2. 사용자 정의 검증

### 2.1 IValidatableObject 인터페이스 구현
```csharp
public class PlayerModel : IValidatableObject
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int MaxHp { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxHp < Level * 10)
        {
            yield return new ValidationResult(
                "최대 HP는 레벨의 10배 이상이어야 합니다",
                new[] { nameof(MaxHp) });
        }
    }
}
```

### 2.2 커스텀 검증 어트리뷰트
```csharp
public class ValidLevelAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var player = (PlayerModel)validationContext.ObjectInstance;
        
        if (player.Level < 1)
        {
            return new ValidationResult("레벨은 1 이상이어야 합니다");
        }

        return ValidationResult.Success;
    }
}

public class PlayerModel
{
    [ValidLevel]
    public int Level { get; set; }
}
```

## 3. 클라이언트 측 검증

### 3.1 jQuery Validation
```html
<form id="playerForm">
    <input type="text" name="Name" data-val="true" 
           data-val-required="이름은 필수입니다" />
    <span class="field-validation-valid" data-valmsg-for="Name"></span>
</form>

<script>
    $("#playerForm").validate();
</script>
```

### 3.2 ASP.NET Core Tag Helpers
```html
<form asp-action="Create">
    <div class="form-group">
        <label asp-for="Name"></label>
        <input asp-for="Name" class="form-control" />
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>
</form>
```

## 4. 유효성 검사 메시지

### 4.1 기본 메시지 커스터마이징
```csharp
services.AddMvc()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });
```

### 4.2 리소스 파일 사용
```csharp
public class PlayerModel
{
    [Required(ErrorMessageResourceName = "Required", 
              ErrorMessageResourceType = typeof(Resources.ValidationMessages))]
    public string Name { get; set; }
}
```

## 5. 유효성 검사 동작 제어

### 5.1 검증 비활성화
```csharp
[ApiController]
public class PlayerController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] PlayerModel player)
    {
        // ModelState.IsValid 검사 생략
        return Ok();
    }
}
```

### 5.2 특정 속성만 검증
```csharp
[HttpPost]
public IActionResult Update([FromBody] PlayerModel player)
{
    // Name 속성만 검증
    if (!TryValidateModel(player, nameof(PlayerModel.Name)))
    {
        return BadRequest(ModelState);
    }
    // ...
}
```

## 6. 유효성 검사 미들웨어

### 6.1 전역 예외 처리
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseExceptionHandler("/error");
    // ...
}
```

### 6.2 API 응답 포맷
```csharp
services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Message = e.Value.Errors.First().ErrorMessage
                });

            return new BadRequestObjectResult(new
            {
                Code = "VALIDATION_ERROR",
                Errors = errors
            });
        };
    });
```

## 7. 유효성 검사 모범 사례

1. **서버 측 검증은 필수**
   - 클라이언트 측 검증은 사용자 경험을 위한 것
   - 실제 데이터 검증은 서버에서 수행

2. **명확한 에러 메시지**
   - 사용자가 이해하기 쉬운 메시지 제공
   - 문제 해결 방법을 제시

3. **적절한 검증 수준**
   - 너무 엄격한 검증은 사용자 경험을 해칠 수 있음
   - 보안과 사용성의 균형 필요

4. **성능 고려**
   - 복잡한 검증 로직은 성능에 영향을 줄 수 있음
   - 필요한 검증만 수행

5. **일관된 검증 규칙**
   - 전체 애플리케이션에서 일관된 검증 규칙 적용
   - 재사용 가능한 검증 로직 구현 

## 8. 유효성 검사 실패 응답

### 8.1 기본 응답 형식
```json
{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "One or more validation errors occurred.",
    "status": 400,
    "traceId": "00-1234567890abcdef-9876543210fedcba-00",
    "errors": {
        "Name": [
            "이름은 필수입니다"
        ],
        "Level": [
            "레벨은 1~100 사이여야 합니다"
        ]
    }
}
```

### 8.2 커스텀 응답 형식 설정
```csharp
services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Message = e.Value.Errors.First().ErrorMessage
                });

            return new BadRequestObjectResult(new
            {
                Code = "VALIDATION_ERROR",
                Message = "입력 데이터가 유효하지 않습니다",
                Errors = errors
            });
        };
    });
```

이렇게 설정하면 다음과 같은 응답이 전송됩니다:
```json
{
    "Code": "VALIDATION_ERROR",
    "Message": "입력 데이터가 유효하지 않습니다",
    "Errors": [
        {
            "Field": "Name",
            "Message": "이름은 필수입니다"
        },
        {
            "Field": "Level",
            "Message": "레벨은 1~100 사이여야 합니다"
        }
    ]
}
```

### 8.3 개별 액션에서의 응답 처리
```csharp
[HttpPost]
public IActionResult Create([FromBody] PlayerModel player)
{
    if (!ModelState.IsValid)
    {
        var errors = ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .ToDictionary(
                e => e.Key,
                e => e.Value.Errors.Select(x => x.ErrorMessage).ToList()
            );

        return BadRequest(new
        {
            Success = false,
            Errors = errors
        });
    }
    // ...
}
```

### 8.4 클라이언트 측 처리 예시
```javascript
// JavaScript 예시
async function createPlayer(playerData) {
    try {
        const response = await fetch('/api/player', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(playerData)
        });

        if (!response.ok) {
            const errorData = await response.json();
            
            // 유효성 검사 오류 처리
            if (errorData.Code === 'VALIDATION_ERROR') {
                errorData.Errors.forEach(error => {
                    console.error(`${error.Field}: ${error.Message}`);
                    // UI에 오류 메시지 표시
                    showError(error.Field, error.Message);
                });
                return;
            }
            
            // 기타 오류 처리
            throw new Error(errorData.Message);
        }

        // 성공 처리
        const result = await response.json();
        console.log('플레이어 생성 성공:', result);
    } catch (error) {
        console.error('오류 발생:', error);
    }
}
```

### 8.5 응답 형식 선택 기준

1. **기본 응답 형식 사용**
   - 간단한 API
   - 표준화된 응답이 필요한 경우
   - 개발 초기 단계

2. **커스텀 응답 형식 사용**
   - 클라이언트 요구사항이 있는 경우
   - 더 자세한 오류 정보가 필요한 경우
   - 다국어 지원이 필요한 경우

3. **개별 액션에서 처리**
   - 특정 엔드포인트만 다른 형식이 필요한 경우
   - 비즈니스 로직에 따른 특수한 오류 처리가 필요한 경우 