# ASP.NET Core 데이터베이스 마이그레이션

## 1. 마이그레이션 개요

### 1.1 마이그레이션이란?
- 데이터베이스 스키마 변경을 관리하는 시스템
- 코드 우선 접근 방식에서 데이터베이스 스키마를 자동으로 생성/업데이트
- 버전 관리와 롤백 지원

### 1.2 주요 기능
- 스키마 변경 추적
- 자동 SQL 생성
- 데이터 마이그레이션
- 버전 관리
- 롤백 지원

## 2. 기본 설정

### 2.1 필요한 패키지 설치
```bash
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
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
}
```

## 3. 마이그레이션 명령어

### 3.1 초기 마이그레이션 생성
```bash
dotnet ef migrations add InitialCreate
```

### 3.2 데이터베이스 업데이트
```bash
dotnet ef database update
```

### 3.3 특정 마이그레이션으로 업데이트
```bash
dotnet ef database update MigrationName
```

### 3.4 마이그레이션 제거
```bash
dotnet ef migrations remove
```

### 3.5 마이그레이션 스크립트 생성
```bash
dotnet ef migrations script
```

## 4. 마이그레이션 파일 구조

### 4.1 기본 구조
```csharp
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Username = table.Column<string>(nullable: false),
                Email = table.Column<string>(nullable: false),
                CreatedAt = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Users");
    }
}
```

### 4.2 데이터 마이그레이션
```csharp
public partial class AddDefaultUsers : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Users",
            columns: new[] { "Username", "Email", "CreatedAt" },
            values: new object[] { "admin", "admin@example.com", DateTime.UtcNow });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Users",
            keyColumn: "Username",
            keyValue: "admin");
    }
}
```

## 5. 고급 마이그레이션

### 5.1 복잡한 스키마 변경
```csharp
public partial class AddUserProfile : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 기존 테이블에 컬럼 추가
        migrationBuilder.AddColumn<string>(
            name: "ProfileImage",
            table: "Users",
            nullable: true);

        // 새 테이블 생성
        migrationBuilder.CreateTable(
            name: "UserProfiles",
            columns: table => new
            {
                UserId = table.Column<int>(nullable: false),
                Bio = table.Column<string>(nullable: true),
                Location = table.Column<string>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserProfiles", x => x.UserId);
                table.ForeignKey(
                    name: "FK_UserProfiles_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // 인덱스 생성
        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "UserProfiles");
        migrationBuilder.DropIndex(name: "IX_Users_Email");
        migrationBuilder.DropColumn(name: "ProfileImage", table: "Users");
    }
}
```

### 5.2 조건부 마이그레이션
```csharp
public partial class UpdateUserTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // 컬럼이 존재하지 않을 때만 추가
        if (!migrationBuilder.HasColumn("Users", "LastLogin"))
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                nullable: true);
        }

        // 기본값 설정
        migrationBuilder.AlterColumn<DateTime>(
            name: "CreatedAt",
            table: "Users",
            nullable: false,
            defaultValueSql: "GETDATE()");
    }
}
```

## 6. 마이그레이션 모범 사례

### 6.1 설계 고려사항
1. **작은 단위의 변경**
   - 한 번에 너무 많은 변경을 하지 않기
   - 논리적으로 관련된 변경을 함께 묶기
   - 명확한 마이그레이션 이름 사용

2. **데이터 보존**
   - 기존 데이터 마이그레이션 계획
   - 데이터 무결성 보장
   - 롤백 계획 수립

3. **성능 고려**
   - 대량의 데이터 변경 시 배치 처리
   - 인덱스 생성/삭제 시점 고려
   - 트랜잭션 사용

### 6.2 구현 고려사항
1. **마이그레이션 순서**
   - 의존성 고려
   - 외래 키 제약 조건
   - 데이터 무결성

2. **테스트**
   - 개발 환경에서 테스트
   - 스테이징 환경에서 검증
   - 롤백 테스트

3. **문서화**
   - 마이그레이션 목적 기록
   - 변경 사항 설명
   - 특별한 고려사항 기록

### 6.3 운영 고려사항
1. **배포 전략**
   - 다운타임 최소화
   - 롤백 계획
   - 모니터링

2. **버전 관리**
   - 마이그레이션 파일 관리
   - 데이터베이스 버전 추적
   - 변경 이력 유지

3. **보안**
   - 민감한 데이터 처리
   - 접근 권한 관리
   - 감사 로깅 