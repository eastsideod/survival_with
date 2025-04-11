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