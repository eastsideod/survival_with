좋아요. ASP.NET Core에 대한 입문부터 고급까지의 전체 학습 플랜을 구성하고, GitHub 및 MSDN을 포함한 주요 학습 자료들을 추가로 정리해드릴게요. 학습 기간은 6개월에서 1년 사이, 하루 2시간 학습을 기준으로 설계하겠습니다.

완성된 플랜은 한국어로, 길이 제한 없이 마크다운 형식으로 제공하겠습니다. 준비되면 알려드릴게요!

# ASP.NET Core 단계별 학습 플랜 (6개월~1년)

## 1주차: ASP.NET Core 소개 및 개발 환경 설정
- **학습 목표:**
  - .NET 플랫폼과 ASP.NET Core의 개념을 이해합니다 (크로스 플랫폼, 오픈 소스, 고성능 웹 프레임워크 등).
  - 개발에 필요한 도구를 설치하고 환경을 구성합니다 (Visual Studio 또는 VS Code, .NET SDK 등).
  - 간단한 “Hello World” ASP.NET Core 애플리케이션을 실행하여 환경이 제대로 동작하는지 확인합니다.
- **추천 자료:**
  - [ASP.NET Core 개요 – 공식 문서] ([ASP.NET Core 개요 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-9.0#:~:text=ASP,%EA%B0%84%20%EA%B3%A0%EC%84%B1%EB%8A%A5%20%EC%98%A4%ED%94%88%20%EC%86%8C%EC%8A%A4%20%ED%94%84%EB%A0%88%EC%9E%84%EC%9B%8C%ED%81%AC%EC%9E%85%EB%8B%88%EB%8B%A4)) ([ASP.NET Core 개요 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-9.0#:~:text=ASP)) – ASP.NET Core의 특징과 장점을 한국어로 설명한 문서입니다.
  - [Visual Studio 2022 다운로드] ([GitHub - PacktPublishing/ASP.NET-8-Best-Practices: ASP.NET-8-Best-Practices, Published by Packt](https://github.com/PacktPublishing/ASP.NET-8-Best-Practices#:~:text=,SQL%20Server%20Management%20Studio)) 또는 [Visual Studio Code] ([Introduction to Razor Pages in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-9.0#:~:text=,Visual%20Studio%20for%20Mac)) – Windows 사용자는 Visual Studio(또는 VS Code) 설치, Mac/Linux 사용자는 VS Code 사용을 권장.
  - [ASP.NET Core 샘플 프로젝트 만들기 – MS Learn 자습서] ([ASP.NET Core MVC 시작 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0#:~:text=%EC%9E%91%EC%84%B1%EC%9E%90%3A%20Rick%20Anderson)) ([ASP.NET Core MVC 시작 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0#:~:text=,%EA%B2%80%EC%83%89%20%EB%B0%8F%20%EC%9C%A0%ED%9A%A8%EC%84%B1%20%EA%B2%80%EC%82%AC%20%EC%B6%94%EA%B0%80)) – MVC 템플릿으로 새 프로젝트를 생성하고 실행하는 방법을 단계별로 안내합니다.
- **실습:**
  - .NET SDK를 설치하고 `dotnet --version`으로 버전을 확인합니다.
  - (IDE 설정) Visual Studio에서 “ASP.NET Core 웹 애플리케이션” 템플릿으로 프로젝트 생성하거나, VS Code 터미널에서 `dotnet new web -o MyFirstApp` 명령으로 기본 웹앱을 생성합니다.
  - 애플리케이션을 실행하여(`dotnet run` 또는 F5) 브라우저에서 “Hello World” 페이지가 뜨는지 확인합니다.
  - 실습 후 Git 저장소를 초기화하여(예: GitHub에 프로젝트 생성) 코드 형상관리를 시작합니다.

## 2주차: C# 기초 및 .NET Core 기본 익히기
- **학습 목표:**
  - ASP.NET Core 개발에 필요한 C# 언어의 기초 문법을 복습합니다 (변수, 클래스, 컬렉션, LINQ 등).
  - 비동기 프로그래밍 패턴(Async/Await)과 `Task`의 개념을 이해하여 I/O 작업을 비동기로 처리하는 방법을 익힙니다.
  - .NET Core의 프로젝트 구조(.csproj)와 명령줄 도구(`dotnet` CLI)의 기본 사용법을 익힙니다.
- **추천 자료:**
  - [C# 언어 둘러보기 – 공식 가이드] ([개요 - A tour of C# | Microsoft Learn](https://learn.microsoft.com/ko-kr/dotnet/csharp/tour-of-csharp/overview#:~:text=%EC%9D%B4%20%EB%AC%B8%EC%84%9C%EC%9D%98%20%EB%82%B4%EC%9A%A9)) ([개요 - A tour of C# | Microsoft Learn](https://learn.microsoft.com/ko-kr/dotnet/csharp/tour-of-csharp/overview#:~:text=Hello%20World)) – C#이 어떤 언어인지 개괄적으로 설명하는 문서입니다.
  - [C# 기초 문법 – Microsoft Learn] ([C# 가이드 - .NET 관리 언어 | Microsoft Learn](https://learn.microsoft.com/ko-kr/dotnet/csharp/#:~:text=,16)) ([C# 가이드 - .NET 관리 언어 | Microsoft Learn](https://learn.microsoft.com/ko-kr/dotnet/csharp/#:~:text=,%EB%B9%84%EB%8F%99%EA%B8%B0%20%ED%94%84%EB%A1%9C%EA%B7%B8%EB%9E%98%EB%B0%8D)) – 형식 시스템, 객체 지향, 예외 처리 등 C# 기본 개념 정리.
  - [비동기 프로그래밍 (async/await) 가이드] ([비동기 프로그래밍 - C# | Microsoft Learn](https://learn.microsoft.com/ko-kr/dotnet/csharp/asynchronous-programming/#:~:text=%EC%9D%B4%20%EB%AC%B8%EC%84%9C%EC%9D%98%20%EB%82%B4%EC%9A%A9)) ([비동기 프로그래밍 - C# | Microsoft Learn](https://learn.microsoft.com/ko-kr/dotnet/csharp/asynchronous-programming/#:~:text=%EC%98%88%EC%A0%9C%EB%A5%BC%20%EC%82%AC%EC%9A%A9%ED%95%A9%EB%8B%88%EB%8B%A4,%EC%A7%80%EC%B9%A8%EC%9D%80%20%EB%AA%A9%EB%A1%9D%EC%9C%BC%EB%A1%9C%20%EC%A0%9C%EA%B3%B5%EB%90%A0%20%EC%88%98%20%EC%9E%88%EC%8A%B5%EB%8B%88%EB%8B%A4)) – C#에서 async/await 키워드를 사용하는 방법과 개념을 설명합니다.
- **실습:**
  - 간단한 C# 콘솔 프로그램을 작성해 봅니다. 예를 들어, `List<int>`에 1~100까지 숫자를 넣고 LINQ로 짝수만 필터링하여 출력하는 코드를 작성합니다.
  - `async/await` 연습: `HttpClient`로 웹 요청을 비동기 처리하는 콘솔 앱을 만들어봅니다 (예: GitHub API 호출). 응답을 받아 콘솔에 일부 데이터를 출력하세요.
  - .NET CLI 사용 연습: 기존 ASP.NET Core 프로젝트 폴더에서 `dotnet build`로 빌드하고 `dotnet run`으로 실행하는 과정을 터미널에서 시도해 봅니다.
  - 이 주까지의 내용을 바탕으로, 이후 실습에 사용할 개인 GitHub 저장소에 간단한 README를 작성하고 커밋합니다.

## 3주차: ASP.NET Core 아키텍처와 요청 파이프라인 이해
- **학습 목표:**
  - ASP.NET Core의 **미들웨어(Middleware)** 파이프라인 개념을 이해합니다. HTTP 요청이 서버에 도달해서 응답이 돌아갈 때까지 거치는 일련의 미들웨어 구성 요소들을 학습합니다 ([ASP.NET Core 미들웨어 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#:~:text=%EB%AF%B8%EB%93%A4%EC%9B%A8%EC%96%B4%EB%8A%94%20%EC%9A%94%EC%B2%AD%20%EB%B0%8F%20%EC%9D%91%EB%8B%B5%EC%9D%84%20%EC%B2%98%EB%A6%AC%ED%95%98%EB%8A%94,%EA%B0%81%20%EA%B5%AC%EC%84%B1%20%EC%9A%94%EC%86%8C)) ([ASP.NET Core 미들웨어 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#:~:text=%EC%9A%94%EC%B2%AD%20%EB%8C%80%EB%A6%AC%EC%9E%90%EB%8A%94%20%EC%9A%94%EC%B2%AD%20%ED%8C%8C%EC%9D%B4%ED%94%84%EB%9D%BC%EC%9D%B8%EC%9D%84%20%EB%B9%8C%EB%93%9C%ED%95%98%EB%8A%94,%EB%8C%80%EB%A6%AC%EC%9E%90%EB%8A%94%20%EA%B0%81%20HTTP%20%EC%9A%94%EC%B2%AD%EC%9D%84%20%EC%B2%98%EB%A6%AC%ED%95%A9%EB%8B%88%EB%8B%A4)).
  - 프로그램 시작 시 `Program.cs`/`Startup.cs`에서 어떻게 미들웨어를 구성하는지 (예: `app.UseRouting()`, `app.UseEndpoints()` 등) 살펴봅니다.
  - 내장 미들웨어들의 역할 (정적 파일 제공, 라우팅, 인증 등)을 알아보고 커스텀 미들웨어를 만들어봅니다.
- **추천 자료:**
  - [ASP.NET Core 미들웨어 – 공식 문서] ([ASP.NET Core 미들웨어 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#:~:text=%EB%AF%B8%EB%93%A4%EC%9B%A8%EC%96%B4%EB%8A%94%20%EC%9A%94%EC%B2%AD%20%EB%B0%8F%20%EC%9D%91%EB%8B%B5%EC%9D%84%20%EC%B2%98%EB%A6%AC%ED%95%98%EB%8A%94,%EA%B0%81%20%EA%B5%AC%EC%84%B1%20%EC%9A%94%EC%86%8C)) ([ASP.NET Core 미들웨어 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#:~:text=%EB%AC%B4%EB%AA%85%20%EB%A9%94%EC%84%9C%EB%93%9C%EB%A5%BC%20%EB%AF%B8%EB%93%A4%EC%9B%A8%EC%96%B4%20%EB%9D%BC%EA%B3%A0%20%ED%95%98%EB%A9%B0%2C,%EC%9D%B4%EB%A5%BC%20%ED%84%B0%EB%AF%B8%EB%84%90%20%EB%AF%B8%EB%93%A4%EC%9B%A8%EC%96%B4%20%EB%9D%BC%EA%B3%A0%20%ED%95%A9%EB%8B%88%EB%8B%A4)) – 미들웨어의 개념과 요청 처리 파이프라인 구성을 설명한 문서입니다.
  - [ASP.NET Core 요청 처리 과정] – 요청이 **Kestrel 웹서버** → 미들웨어 파이프라인 → MVC/엔드포인트로 전달되는 흐름을 다룬 블로그나 동영상 자료.
  - [ASP.NET 8 Best Practices (Ch.3) 예제 코드] – Packt의 **EmojiMiddleware** 샘플 코드 (GitHub 저장소의 *Chapter03 - Middleware* 폴더)를 참고하여 커스텀 미들웨어 구현 방식을 살펴봅니다.
- **실습:**
  - 기존 프로젝트의 `Program.cs`에서 `app.UseDeveloperExceptionPage()` (개발용 예외 미들웨어), `app.UseStaticFiles()`, `app.UseRouting()`, `app.UseEndpoints()` 등 기본 미들웨어 설정을 확인합니다. 각 미들웨어를 켜고 끄면서 동작 변화를 실험해 보세요.
  - **커스텀 미들웨어 작성:** 간단한 로깅 미들웨어를 직접 만들어봅니다. 예를 들어 모든 요청의 경로와 시간을 콘솔에 기록하고 다음 미들웨어로 넘기는 클래스 형태의 미들웨어를 구현하여 `app.Use(...)`로 등록합니다 ([ASP.NET Core 미들웨어 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#:~:text=%EB%AF%B8%EB%93%A4%EC%9B%A8%EC%96%B4%EB%8A%94%20%EC%9A%94%EC%B2%AD%20%EB%B0%8F%20%EC%9D%91%EB%8B%B5%EC%9D%84%20%EC%B2%98%EB%A6%AC%ED%95%98%EB%8A%94,%EA%B0%81%20%EA%B5%AC%EC%84%B1%20%EC%9A%94%EC%86%8C)).
  - 미들웨어 파이프라인 순서에 따라 동작이 달라짐을 체험합니다. (예: `app.UseStaticFiles()`를 `UseRouting()`보다 뒤에 배치하면 정적 파일이 제공되지 않는 상황 등을 테스트)
  - 이러한 실습 결과와 개념을 정리해서 자신의 말로 간략히 주석이나 노트에 작성해 봅니다.

## 4주차: ASP.NET Core 첫 웹 애플리케이션 구축 (MVC 구조 이해)
- **학습 목표:**
  - ASP.NET Core **MVC** (Model-View-Controller) 패턴의 구조를 이해합니다. Controller는 요청을 받고, Model을 통해 데이터 처리를 한 뒤, View에 데이터를 전달해 응답을 생성합니다.
  - MVC 웹앱을 실제로 만들어 보면서 프로젝트 구조(Controllers, Views, wwwroot 등)를 파악합니다. Razor 기반 뷰 파일(.cshtml)의 구성과 기본 문법(@ 지시어 등)을 익힙니다.
  - 간단한 페이지를 추가하거나 수정하면서 MVC의 기본 동작 (라우팅과 액션 메서드 매핑)을 체험합니다.
- **추천 자료:**
  - [ASP.NET Core MVC 시작하기 – 자습서] ([ASP.NET Core MVC 시작 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0#:~:text=%EC%9D%B4%20%EC%9E%90%EC%8A%B5%EC%84%9C%EC%97%90%EC%84%9C%EB%8A%94%20%EC%BB%A8%ED%8A%B8%EB%A1%A4%EB%9F%AC%EC%99%80%20%EB%B3%B4%EA%B8%B0%EB%A5%BC%20%EC%82%AC%EC%9A%A9%ED%95%98%EC%97%AC,%EB%B0%8F%20Blazor%EC%9D%98%20UI%20%EA%B0%9C%EB%B0%9C%EC%9D%84%20%EB%B9%84%EA%B5%90%ED%95%A9%EB%8B%88%EB%8B%A4)) ([ASP.NET Core MVC 시작 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0#:~:text=,%EA%B2%80%EC%83%89%20%EB%B0%8F%20%EC%9C%A0%ED%9A%A8%EC%84%B1%20%EA%B2%80%EC%82%AC%20%EC%B6%94%EA%B0%80)) – 영화 목록 예제 애플리케이션을 만드는 공식 튜토리얼 시리즈입니다. (프로젝트 생성부터 모델 추가, 데이터베이스 연동, 검증까지 포함)
  - **ASP.NET Core in Action** 도서 Chapter 2 “Your First Application” – MVC 기본 프로젝트를 생성하고 각 파일의 역할을 설명하는 내용을 참고합니다 (도서 소스 코드 및 설명 참고).
  - [ASP.NET Core 라우팅 기본] – 기본적인 attribute/컨벤션 라우팅 방식과 URL 패턴 매핑을 설명한 문서나 블로그.
- **실습:**
  - **공식 자습서 따라하기:** 새 MVC 웹앱을 만들어 “영화 관리” 예제를 구현합니다 ([ASP.NET Core MVC 시작 | Microsoft Learn](https://learn.microsoft.com/ko-kr/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-8.0#:~:text=%EC%9D%B4%20%EC%9E%90%EC%8A%B5%EC%84%9C%EC%97%90%EC%84%9C%EB%8A%94%20%EC%BB%A8%ED%8A%B8%EB%A1%A4%EB%9F%AC%EC%99%80%20%EB%B3%B4%EA%B8%B0%EB%A5%BC%20%EC%82%AC%EC%9A%A9%ED%95%98%EC%97%AC,%EB%B0%8F%20Blazor%EC%9D%98%20UI%20%EA%B0%9C%EB%B0%9C%EC%9D%84%20%EB%B9%84%EA%B5%90%ED%95%A9%EB%8B%88%EB%8B%A4)). Visual Studio의 **MVC 템플릿**을 사용하거나 자습서 지침에 따라 진행하세요.
    - 이 과정에서 `HomeController`, `Home/Index` 뷰 등이 어떻게 동작하는지 파악합니다. `Controllers` 폴더와 `Views` 폴더 구조를 살펴보고, `_Layout.cshtml` 레이아웃 페이지를 찾아 공통 레이아웃 구성을 이해합니다.
  - **컨트롤러와 뷰 추가:** Students라는 새 컨트롤러를 추가하고 `Index`, `Details` 액션 메서드 및 대응하는 뷰(.cshtml)를 작성해봅니다. 임의의 간단한 데이터(예: 학생 목록)를 모델 없이 컨트롤러에서 ViewBag/ViewData로 뷰에 전달하여 출력해 보세요.
  - **Razor 문법 연습:** 뷰 파일에서 C# 코드를 사용하는 방법(@ 지시자)을 연습합니다. 조건문이나 반복문을 Razor 구문으로 작성하여 동적으로 콘텐츠를 표시해 봅니다.
  - 결과물을 브라우저로 확인하고, MVC 패턴에서 각 부분이 어떤 역할을 하는지 주석으로 정리해 봅니다. 이후 주차 실습에 이 코드를 계속 확장해 사용할 것이므로, **이 프로젝트를 깃(Git)**에 커밋합니다.

## 5주차: MVC 심화 – 모델 및 뷰 기능 익히기 (폼 입력, Tag Helper 등)
- **학습 목표:**
  - **모델(Model)** 클래스의 역할을 이해하고, 간단한 도메인 모델을 정의하여 컨트롤러에서 사용합니다. 예를 들어, 학생 관리의 Student 클래스 등을 만들어봅니다.
  - **모델 바인딩(Model Binding)**과 **폼 처리** 과정을 학습합니다. HTML 폼에 입력된 값이 어떻게 컨트롤러 액션 매개변수나 모델 객체로 바인딩되는지 이해합니다.
  - Razor **Tag Helper**와 **HTML Helper**를 사용하여 폼 요소를 생성하고, 검증 메시지를 출력하는 방법을 익힙니다 ([ASP.NET Core in Action.pdf](file://file-KzdCSWvxk8QPMaKSFf5sZa#:~:text=In%20this%20chapter%2C%20you%E2%80%99ll%20primarily,add%20the%20ability%20to%20submit)) ([ASP.NET Core in Action.pdf](file://file-KzdCSWvxk8QPMaKSFf5sZa#:~:text=As%20you%20develop%20the%20application%2C,when%20they%20refresh%20their%20browser)).
- **추천 자료:**
  - [ASP.NET Core MVC 모델 및 모델 바인딩] – 모델 클래스 정의와 폼 POST 요청 시 모델 바인딩 과정을 설명한 공식 문서.
  - [Tag Helper를 사용한 폼 구축] – Razor 페이지에서 `<form asp-action>`이나 `<input asp-for>` 등의 태그 헬퍼를 활용하여 폼을 만드는 방법을 다룬 자료 ([ASP.NET Core in Action.pdf](file://file-KzdCSWvxk8QPMaKSFf5sZa#:~:text=In%20this%20chapter%2C%20you%E2%80%99ll%20primarily,add%20the%20ability%20to%20submit)) ([ASP.NET Core in Action.pdf](file://file-KzdCSWvxk8QPMaKSFf5sZa#:~:text=As%20you%20develop%20the%20application%2C,when%20they%20refresh%20their%20browser)). (예: **ASP.NET Core in Action** 도서 8장 “Building forms with Tag Helpers” 참고)
  - [데이터 주석(DataAnnotations)과 검증] – `Required`, `StringLength` 등의 특성을 모델 속성에 적용해 입력값 검증을 수행하는 방법 (MSDN 문서 참고).
- **실습:**
  - **모델 클래스 작성:** 실습용 프로젝트에 간단한 도메인 모델을 도입합니다. 예를 들어 `Student` 클래스에 학번, 이름, 입학년도 등을 프로퍼티로 정의하고, `StudentsController`에서 이 모델 리스트를 생성하여 뷰로 전달합니다.
  - **학생 등록 폼 만들기:** StudentsController에 `Create()` GET 액션 (빈 폼 뷰 반환)과 POST 액션(폼 입력 처리)을 추가합니다. 뷰에서는 `<form asp-action="Create">`와 `<input asp-for="Name">` 등의 Tag Helper를 사용해 학생 정보를 입력받는 폼을 구성하세요 ([ASP.NET Core in Action.pdf](file://file-KzdCSWvxk8QPMaKSFf5sZa#:~:text=platform%2C%20the%20desire%20to%20play,editors%20reared%20its%20head%20again)) ([ASP.NET Core in Action.pdf](file://file-KzdCSWvxk8QPMaKSFf5sZa#:~:text=Figure%208,all%20generated%20using%20Tag%20Helpers)). 제출 시 유효성 검사를 수행하고, 성공하면 임시로 메모리에 학생 리스트를 추가한 뒤 목록 페이지로 리디렉션합니다.
  - **검증 및 오류 표시:** `Student` 모델에 `[Required]` 등 데이터 주석을 달아 일부 필드를 필수 입력으로 지정하고, 뷰에 `<span asp-validation-for="Name"></span>` 등을 배치하여 검증 오류 메시지를 표시해봅니다. 클라이언트 쪽 검증이 동작하는지도 확인하세요 (기본적으로 jQuery Validation이 포함됨).
  - **부분 뷰/레이아웃 활용:** 헤더나 네비게이션 바와 같이 여러 뷰에서 공통되는 부분은 *_Layout.cshtml*이나 Partial View로 분리되어 있다는 것을 확인하고, 직접 간단한 Partial View를 하나 만들어보세요 (예: 학생 목록 항목을 표시하는 부분을 `_StudentListPartial.cshtml`로 만들고 `@Html.Partial` 또는 `Partial Tag Helper`로 포함).
  - 폼 처리와 모델 바인딩, 검증 흐름을 이해한 내용을 정리합니다.

## 6주차: .NET 데이터 액세스 – Entity Framework Core 시작하기 (1)
- **학습 목표:**
  - **Entity Framework Core (EF Core)**를 사용하여 데이터베이스를 연동하는 방법을 학습합니다. EF Core의 DbContext와 DbSet 개념, Code-First 마이그레이션을 이해합니다.
  - 데이터 액세스 계층을 프로젝트에 추가하고, 간단한 CRUD(Create, Read, Update, Delete) 기능을 구현해봅니다.
  - EF Core에서 **LINQ**를 사용하여 데이터를 조회하고 필터링하는 방법을 익힙니다.
- **추천 자료:**
  - [EF Core 시작하기 – (콘솔 앱 예제)] ([Getting Started - EF Core - Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app#:~:text=Getting%20Started%20,database%20using%20Entity%20Framework%20Core)) – SQLite를 사용하는 간단한 .NET Core 콘솔 앱 튜토리얼로 EF Core의 기본 사용법(SQL 연결, 기본 CRUD)을 익힐 수 있습니다.
  - [ASP.NET Core MVC와 EF Core 자습서 (Contoso University 1부)] ([Tutorial: Get started with EF Core in an ASP.NET MVC web app | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-9.0#:~:text=By%20Tom%20Dykstra%20and%20Rick,Anderson)) ([Tutorial: Get started with EF Core in an ASP.NET MVC web app | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-9.0#:~:text=The%20Contoso%20University%20sample%20web,Core%20and%20Visual%20Studio)) – Contoso University 예제로 ASP.NET Core에서 EF Core를 설정하고 첫 마이그레이션을 적용하는 과정을 다룬 튜토리얼입니다.
  - [EF Core DbContext 및 마이그레이션 – MS Docs] – DbContext를 구성하고 `Add-Migration`/`Update-Database` 명령으로 데이터베이스를 생성/갱신하는 방법 안내.
  - **ASP.NET 8 Best Practices (Ch.5)** – EF Core를 효과적으로 사용하는 패턴과 팁이 정리된 자료 (예: Repository 패턴 여부, NoTracking 쿼리 등). 필요시 코드 참고.
- **실습:**
  - **EF Core NuGet 패키지 추가:** 현재 ASP.NET Core 프로젝트에 `Microsoft.EntityFrameworkCore.SqlServer` (또는 원하는 DB 공급자)와 `Microsoft.EntityFrameworkCore.Tools` 패키지를 추가합니다. (Visual Studio NuGet 관리자 또는 `dotnet add package` 명령 사용)
  - **DbContext 생성:** 예제용으로 `SchoolContext` 클래스를 작성합니다. `DbContext`를 상속하고, `DbSet<Student> Students` 등을 정의하세요. 그리고 `Startup`/`Program`에서 `services.AddDbContext<SchoolContext>()`으로 의존성 주입을 설정합니다.
  - **연결 문자열 설정:** *appsettings.json*에 로컬 데이터베이스 연결 문자열을 추가합니다 (예: LocalDB나 SQLite 사용). SchoolContext의 `OnConfiguring` 또는 DI 옵션에서 해당 연결 문자열을 사용하도록 구성합니다.
  - **마이그레이션 및 DB 생성:** Package Manager Console에서 `Add-Migration Init`을 실행하여 초기 마이그레이션을 생성하고, `Update-Database`로 데이터베이스를 만듭니다. (이때 생성된 마이그레이션 코드를 확인하며 EF가 Student 모델에 따라 테이블을 만드는지 확인합니다.)
  - **기본 데이터 시드(Optional):** Student 예제의 초기 데이터를 위해 `Student` 엔티티 몇 건을 생성하여 DbContext에 추가하고 `SaveChanges()`를 호출하는 코드를 프로그램 시작 시 한번 실행하거나, 마이그레이션 Seed 메커니즘을 이용해봅니다.
  - **학생 목록 조회:** StudentsController의 Index 액션에서 이전에 메모리에 저장하던 학생 리스트 대신 DbContext를 주입받아 `context.Students.ToListAsync()`로 DB에서 학생 목록을 가져오도록 수정합니다 ([Tutorial: Get started with EF Core in an ASP.NET MVC web app | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-9.0#:~:text=,ongoing%20work%20with%20a%20result)) ([Tutorial: Get started with EF Core in an ASP.NET MVC web app | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-9.0#:~:text=,ongoing%20work%20with%20a%20result)). 페이지를 실행해 DB의 데이터가 표시되는지 확인합니다.
  - 이 과정에서 발생한 오류나 어려움을 노트에 기록하고, 해결 방법(예: 의존성 주입 설정 누락 등의 문제)을 정리해 둡니다.

## 7주차: 데이터 액세스 – Entity Framework Core 활용 (2)
- **학습 목표:**
  - **EF Core 고급 기능**을 학습합니다. 여러 엔티티 간의 **관계(Relationship)** 맵핑을 설정하고 탐색 속성을 통해 관련 데이터를 로드하는 방법을 익힙니다 (예: 일대다 관계, Lazy/Eager 로딩 등).
  - 복수 테이블을 다루는 **LINQ 쿼리** 작성법을 익히고, EF의 **추적/비추적 쿼리** 개념을 이해합니다. (변경 추적, AsNoTracking 사용 시점)
  - EF Core의 마이그레이션을 사용하여 모델 변경을 데이터베이스에 반영하는 방법 (추가 마이그레이션 생성)을 연습합니다.
- **추천 자료:**
  - [EF Core 관계 맵핑 소개] ([Introduction to relationships - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/modeling/relationships#:~:text=This%20document%20provides%20a%20simple,Core%20maps%20between%20the%20two)) ([Introduction to relationships - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/modeling/relationships#:~:text=In%20the%20classes%20above%2C%20there,on%20which%20it%20is%20published)) – 엔티티 사이의 일대다, 다대다 관계를 코드로 구성하고 EF가 DB에서 외래 키로 표현하는 방식을 설명한 문서입니다.
  - [Contoso University 자습서 2~3부] – 관련된 데이터 읽어오기(Eager loading 등)와 데이터 업데이트/삭제를 다루는 연속 튜토리얼 (학생과 수강과목 관계 등 실습) ([Tutorial: Get started with EF Core in an ASP.NET MVC web app | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-9.0#:~:text=This%20tutorial%20teaches%20ASP,some%20material%20the%20other%20doesn%27t)) ([Tutorial: Get started with EF Core in an ASP.NET MVC web app | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-9.0#:~:text=The%20Contoso%20University%20sample%20web,Core%20and%20Visual%20Studio)).
  - [EF Core에서의 Lazy vs Eager Loading] – 필요한 경우에만 데이터 로드하기 위한 기법과, Include 메서드 사용법 등을 정리한 블로그.
  - **ASP.NET 8 Best Practices**의 EF 관련 장(Ch.5) – 성능을 높이는 EF 패턴 (예: DDD 접근법에서 Context 수명 관리, Repository/Unit of Work 적용 시 고려사항 등) 참고.
- **실습:**
  - **엔티티 관계 구성:** 새로운 엔티티 `Course`(과목)와 `Enrollment`(수강) 등을 정의해봅니다. 예를 들어 Student : Enrollment = 1:N, Course : Enrollment = 1:N 관계를 갖도록 모델 클래스를 작성합니다. 각 클래스에 탐색 속성을 추가하고, `SchoolContext`의 `OnModelCreating`에서 Fluent API로 관계를 명시적으로 설정하거나 Data Annotations([ForeignKey] 등)을 사용하세요.
  - **마이그레이션 업데이트:** 새 엔티티를 추가했으므로 `Add-Migration AddCourseAndEnrollment` -> `Update-Database`를 실행하여 DB 스키마를 갱신합니다. 데이터베이스에 Course 테이블 등이 생성되었는지 확인합니다.
  - **관련 데이터 다루기:** 예를 들어 Students 상세 페이지에서 그 학생의 수강 과목 리스트를 함께 보여주려 한다고 가정합니다. StudentsController의 Details 액션에서 `context.Students.Include(s => s.Enrollments).ThenInclude(e => e.Course)`를 사용하여 해당 학생과 연관된 수강 및 과목 정보를 eager loading 합니다 ([Introduction to relationships - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/modeling/relationships#:~:text=A%20relationship%20defines%20how%20two,posts%20published%20on%20that%20blog)) ([Introduction to relationships - EF Core | Microsoft Learn](https://learn.microsoft.com/en-us/ef/core/modeling/relationships#:~:text=,get%3B%20set%3B)). 뷰에서 학생과 수강 과목명을 출력해보세요.
  - **데이터 수정 및 삭제:** 간단한 UI를 통해 Course나 Student 데이터를 수정/삭제하는 기능을 추가해봅니다. Edit, Delete 액션 메서드를 컨트롤러에 구현하고 EF의 `Update`, `Remove` 메서드를 사용해보세요. (이 과정에서 **동시성**이나 **추적 문제**를 겪을 수 있는데, 동일한 컨텍스트 인스턴스 사용이나 AsNoTracking 활용 등을 고려합니다.)
  - **성능 테스트(간단):** 학생 수나 과목 수를 임의로 늘려본 뒤 페이지 로드 시간이 크게 느려지지 않는지 확인합니다. 필요한 경우 `.AsNoTracking()`을 쿼리에 붙여 읽기 전용 시 성능을 높이는 것을 실험해보세요.
  - 이번 주까지 구현한 내용으로, **학생/과목 관리 웹앱**의 기본 골격이 완성되었습니다. 전체 코드를 리팩터링하여 Service 계층을 도입하거나 Repository 패턴을 적용해볼 수도 있지만, 우선은 EF Core 자체에 익숙해지는 데 집중합니다. 작동하는 코드를 Git에 커밋하고, 느낀 점을 간단히 기록합니다.

## 8주차: 인증과 권한 부여 – ASP.NET Core Identity (1)
- **학습 목표:**
  - **ASP.NET Core Identity**를 이용한 **사용자 인증(Authentication)** 기능을 프로젝트에 도입합니다. Identity가 제공하는 회원 가입, 로그인, 로그아웃 등 기본 UI와 데이터 모델을 이해합니다.
  - Identity의 기본 구성요소 (UserManager, SignInManager 등)와 Identity DB 스키마(사용자 테이블 등)를 살펴봅니다.
  - 쿠키 기반 인증의 동작 방식을 높은 수준에서 이해하고, [Authorize] 특성을 사용하여 **인가(Authorization)**를 적용하는 기본 방법을 배웁니다.
- **추천 자료:**
  - [ASP.NET Core Identity 소개 – 공식 문서] ([Introduction to Identity on ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0#:~:text=ASP)) ([Introduction to Identity on ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0#:~:text=The%20Identity%20source%20code%20is,the%20template%20interaction%20with%20Identity)) – Identity 시스템의 개요와 주요 기능 (사용자 관리, 외부 로그인 등)을 설명하는 문서입니다.
  - [ASP.NET Core Identity 설정 및 스캐폴딩] ([Introduction to Identity on ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0#:~:text=Identity)) – Identity UI 라이브러리를 프로젝트에 추가하고, 스캐폴드로 기본 페이지들을 생성/커스터마이징하는 방법을 다룬 자료.
  - [Identity 예제 – Packt ASP.NET 8 Best Practices] – 해당 책의 저장소에서 Identity 설정 부분이나 OAuth 연동 등이 다루어졌다면 참고.
  - (선택) [ASP.NET Core 인증 및 쿠키] – 쿠키 인증 미들웨어가 Identity와 어떻게 연계되는지, 보안 스탬프나 만료 등의 개념 (심화 내용).
- **실습:**
  - **Identity 추가:** 기존 프로젝트에 Identity를 활성화합니다. Visual Studio의 스캐폴딩 기능을 사용하면 **Identity** 관련 페이지들과 `ApplicationDbContext`를 자동 추가할 수 있습니다 ([Introduction to Identity on ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-9.0#:~:text=The%20Identity%20source%20code%20is,the%20template%20interaction%20with%20Identity)). 또는 Startup/Program에서 `services.AddDefaultIdentity<IdentityUser>()` 및 `app.UseAuthentication()`, `app.UseAuthorization()`을 설정하고, Identity용 DbContext를 추가하는 방법으로 구성할 수도 있습니다.
  - `dotnet ef migrations add IdentityInit` -> `Update-Database`를 통해 Identity용 테이블들이 생성되도록 합니다 (AspNetUsers, AspNetRoles 등).
  - **회원가입/로그인 UI 확인:** 스캐폴딩한 Identity 페이지 (Areas\Identity\Pages\...)를 실행하여 회원 가입, 이메일 확인(생략 가능), 로그인, 로그아웃 흐름을 테스트합니다. 새 사용자를 등록하면 AspNetUsers 테이블에 데이터가 생성되는지 확인하세요.
  - **[Authorize] 속성 적용:** StudentsController 등 민감한 기능의 컨트롤러/액션에 `[Authorize]` 특성을 붙여 인증된 사용자만 접근 가능하도록 설정합니다. 로그인하지 않은 상태로 접근 시 자동으로 로그인 페이지로 리디렉션되는지 확인합니다.
  - **역할(Role) 추가 – (Optional):** 기본 사용자 외에 역할 개념을 도입해보고 싶다면, `RoleManager<IdentityRole>`를 사용하여 애플리케이션 초기화 시 Admin 역할을 만들고 특정 사용자를 Admin으로 지정해보세요. 그리고 `[Authorize(Roles="Admin")]`로 접근 제한을 거는 실험을 합니다 ([Role-based authorization in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-9.0#:~:text=Learn%20learn,roles%20to%20the%20Authorize%20attribute)).
  - Identity 시스템이 생성한 쿠키를 브라우저 개발자 도구에서 확인하고, 로그인 상태가 유지되는 방식을 이해합니다. 또한 잘못된 비밀번호로 로그인 시도를 해보면서 Identity가 제공하는 오류 메시지 및 잠금 기능(기본 5회 실패 잠금 등)이 동작하는지도 확인합니다.
  - 이번 주차까지 **기본적인 인증 기능**이 애플리케이션에 추가되었습니다. 아직 UI 스타일이나 과정이 미흡할 수 있으나, 우선 기본 흐름을 익히는 데 집중합니다. 구현 내용을 커밋합니다.

## 9주차: 보안 강화 – 권한 부여(Authorization)과 보안 모범 사례
- **학습 목표:**
  - Role 및 **Policy 기반 Authorization**을 활용하여 세분화된 **권한 부여**를 구현합니다. (예: 관리자는 모든 기능 접근 가능하나 일반 사용자는 읽기만 가능 등) ([Policy-based authorization in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-9.0#:~:text=Policy,NET%20Core%20app))
  - **ASP.NET Core 보안**의 핵심 모범 사례를 학습합니다. XSS, CSRF, SQL 인젝션 등 웹 취약점에 대비하는 방법과 ASP.NET Core에서의 기본 방어 기제를 이해합니다.
  - HTTPS 사용, 데이터 보호(Data Protection), Secret Manager 등 앱 보안을 강화하는 요소들을 파악합니다.
- **추천 자료:**
  - [역할 기반 권한 부여] ([Role-based authorization in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-9.0#:~:text=Role,roles%20to%20the%20Authorize%20attribute)) / [정책 기반 권한 부여] ([Policy-based authorization in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-9.0#:~:text=Policy,NET%20Core%20app)) – `Authorize` 특성에 정책이나 역할을 적용하는 방법과 `AuthorizationHandler`를 통한 커스텀 정책 구현 방법을 설명한 공식 문서.
  - [ASP.NET Core 보안 주제 모음] ([ASP.NET Core security topics | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-9.0#:~:text=Common%20Vulnerabilities%20in%20software)) ([ASP.NET Core security topics | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-9.0#:~:text=There%20are%20more%20vulnerabilities%20that,of%20the%20table%20of%20contents)) – XSS, CSRF, Open Redirect 등 **OWASP Top 10** 관련 취약점을 ASP.NET Core에서 방지하는 방법에 대한 문서 목록입니다.
  - [XSS 공격 방지] ([Prevent Cross-Site Scripting (XSS) in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/cross-site-scripting?view=aspnetcore-9.0#:~:text=Cross,validating%2C%20encoding%20or%20escaping%20it)) ([Prevent Cross-Site Scripting (XSS) in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/cross-site-scripting?view=aspnetcore-9.0#:~:text=Protecting%20your%20application%20against%20XSS)) / [CSRF 공격 방지] ([Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-9.0#:~:text=Cross,known%20as%20XSRF%20or%20CSRF)) ([Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-9.0#:~:text=1,with%20a%20valid%20authentication%20cookie)) – 각각 출력 인코딩, Anti-forgery 토큰 등 구체적 대책을 다룹니다.
  - **ASP.NET 8 Best Practices** 보안 장 – 실제 현업에서 자주 언급되는 보안 체크리스트와 권장 사항 (예: HTTPS 리다이렉트, 안전한 비밀번호 정책 등)을 정리한 부분이 있다면 참조합니다.
- **실습:**
  - **세분화된 권한 부여 적용:** 앞서 Identity에서 역할을 도입했다면, **관리자 전용 기능**을 하나 만들어봅니다. 예를 들어 Course 생성/편집 페이지는 Admin만 접근 가능하도록 `[Authorize(Roles="Admin")]`으로 제한합니다. Admin 사용자로 로그인해서는 접근되고, 일반 사용자로는 접근 거부(403) 되는지 확인합니다.
    - 정책 기반으로 해보려면 `AuthorizationPolicyBuilder`를 사용해 “RequireAdministrator” 같은 정책을 추가하고 `[Authorize(Policy="RequireAdministrator")]`로 적용해도 됩니다.
  - **보안 설정 점검:** 앱 전체에 **HTTPS**가 적용되도록 `app.UseHttpsRedirection()`을 활성화하고, 개발 환경에서도 SSL로 실행합니다. 또 `CookiePolicyOptions` 설정에서 Secure, SameSite 옵션 등을 조정해봅니다 (기본값 확인).
  - **XSS 방지 실습:** 의도적으로 XSS 취약점이 있을 법한 시나리오를 만들어봅니다. 예를 들어, 학생 이름에 `"<script>alert('xss')</script>"` 같은 값을 넣어보고 페이지에 출력할 때 `<text>@student.Name</text>` 대신 `@Html.Raw`로 출력하여 XSS가 발생하는지 시험합니다. 그런 다음 올바르게 `@student.Name` (자동 인코딩)으로 출력하여 공격이 무력화됨을 확인합니다 ([Prevent Cross-Site Scripting (XSS) in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/cross-site-scripting?view=aspnetcore-9.0#:~:text=Cross,validating%2C%20encoding%20or%20escaping%20it)) ([Prevent Cross-Site Scripting (XSS) in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/cross-site-scripting?view=aspnetcore-9.0#:~:text=element,introducing%20XSS%20into%20their%20applications)).
  - **CSRF 방지 확인:** 학생 등록 폼이 CSRF 방어를 하고 있는지 확인합니다. 기본적으로 Razor Form에는 `antiforgery` 토큰이 포함되지만, 개발자 도구로 토큰 값을 삭제한 채 요청을 보내거나, 다른 사이트에서 우리 사이트의 폼 액션으로 POST를 보내는 시나리오를 가정해봅니다. ASP.NET Core의 `[ValidateAntiForgeryToken]`이 기본 적용되어 있을 경우 요청이 거부되어야 합니다 ([Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-9.0#:~:text=Cross,known%20as%20XSRF%20or%20CSRF)) ([Prevent Cross-Site Request Forgery (XSRF/CSRF) attacks in ASP.NET Core | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-9.0#:~:text=The%20malicious%20site%2C%20%60www.bad,similar%20to%20the%20following%20example)).
  - **SQL 인젝션 방지:** EF Core 사용 시 SQL 인젝션은 LINQ로부터 자동 방어되지만, Raw SQL을 쓰는 경우를 시험해봅니다. `context.Students.FromSqlRaw("SELECT * ...")` 등을 쓸 때 사용자 입력을 직접 문자열 연결하면 위험함을 인지하고, 항상 파라미터 바인딩을 사용해야 함을 확인합니다.
  - **Secret 관리:** DB 연결 문자열 등 민감정보는 *appsettings.json*에 평문 저장되지 않도록, 개발 단계에서는 **Secret Manager** 도구를 이용해 로컬 시크릿으로 관리해봅니다 ([ASP.NET Core security topics | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-9.0#:~:text=Configuration%20data%20guidelines%3A)). (예: `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<value>"`).
  - 이번 주 실습으로 애플리케이션의 보안 수준을 한층 높였습니다. 발견된 취약점 사례가 있다면 기록하고, 어떻게 개선했는지 함께 정리합니다. 코드 및 설정 변경사항을 커밋합니다.

## 10주차: **실전 프로젝트** – 요구사항 분석 및 아키텍처 설계
- **학습 목표:**
  - 앞서 학습한 기술을 모두 종합하여 **실전 프로젝트**를 계획합니다. 작은 규모의 웹 애플리케이션 요구사항을 정의하고, 이를 구현하기 위한 아키텍처를 설계합니다.
  - 솔루션 구조를 개선합니다. 프로젝트를 계층별로 나누거나 (예: Web, Core, Infrastructure 프로젝트 분리) 클린 아키텍처를 일부 도입하여 코드 구조를 정리합니다.
  - Git 브랜치 전략 등 형상관리 방식도 간략히 수립합니다 (예: `main` 브랜치 + 기능별 브랜치 워크플로).
- **추천 자료:**
  - **프로젝트 아이디어 선정:** 본인이 관심 있는 도메인으로 간단한 웹앱 주제를 정합니다 (예: “학생 성적 관리”, “도서 대여 시스템”, “간단한 블로그” 등). 참고로 Microsoft의 [eShopOnWeb 샘플] ([Common web application architectures - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures))처럼 **계층형 아키텍처**를 따르는 예제가 있으니 구조를 참고하세요.

     ([Common web application architectures - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)) *주석: eShopOnWeb 참조 애플리케이션의 솔루션 구조 예시로, 핵심 비즈니스 로직(Core), 인프라스트럭처(Infrastructure), 웹 계층(Web) 및 테스트 프로젝트로 구성되어 있습니다. 이러한 계층화된 구조는 앱의 확장성과 유지보수성을 향상시킵니다.*

  - [Common web application architectures – Microsoft] ([Common web application architectures - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#:~:text=Image%3A%20Typical%20application%20layers)) ([Common web application architectures - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#:~:text=Figure%205,or%20layer)) – 전통적인 N-레이어 아키텍처와 프로젝트 분리 예시를 보여줍니다. (그림 5-3 참조)
  - [Clean Architecture 예제 (Steve Smith의 eShopOnWeb)] – GitHub의 dotnet-architecture/eShopOnWeb ([eShop vs eShopOnWeb? : r/dotnet - Reddit](https://www.reddit.com/r/dotnet/comments/17zymsw/eshop_vs_eshoponweb/#:~:text=eShop%20vs%20eShopOnWeb%3F%20%3A%20r%2Fdotnet,architecture%2FeShopOnWeb%3A%20Sample%20ASP)) 저장소를 살펴보고, Domain 모델과 Infrastructure, UI가 분리된 구조, 그리고 디자인 패턴(예: Specification 패턴 등)을 참고합니다.
  - **ASP.NET 8 Best Practices** 1~2장 – 소스 제어 전략, 솔루션 구조 및 미들웨어/서비스 구성에 대한 모범 사례를 다루고 있으므로, 프로젝트 시작 전에 일독합니다.
- **실습 (설계):**
  - **요구사항 정의:** 구현할 프로젝트의 간단한 요구사항을 명문화합니다. 예를 들어 “학생 성적 관리 시스템”이라면, *학생 정보*와 *성적*을 CRUD 할 수 있고, 교직원/학생 두 역할이 있으며, 학생은 자신의 성적만 열람 가능 등 요건을 정리합니다.
  - **기능 목록 및 우선순위:** 구현할 주요 기능을 목록화하고 주차별로 배분합니다. (예: 11~12주차에 핵심 CRUD 구현, 13주차에 인증/권한, 14주차에 UI 향상 등)
  - **솔루션 구조 설계:** 현재까지 단일 프로젝트로 진행했다면, 솔루션을 **다중 프로젝트**로 리팩터링합니다. 새로운 클래스 라이브러리 프로젝트를 만들어 **Core(또는 Domain)** 영역에 엔티티와 인터페이스, **Infrastructure** 영역에 EF Core 구현 등을 담습니다. ASP.NET Core 프로젝트는 **Web**으로서 Presentation 계층을 맡습니다 ([Common web application architectures - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#:~:text=Figure%205,or%20layer)). 프로젝트 간 참조를 설정하고, DI 컨테이너에 Infrastructure의 구현을 주입합니다.
  - **계층 분리 적용:** 예를 들어 Student 및 Course 엔티티를 Core 프로젝트로 이동시키고, SchoolContext와 Repository 구현을 Infrastructure로 옮깁니다. Web 프로젝트의 컨트롤러는 Infrastructure에 직접 의존하지 않고 Core에 정의된 인터페이스(IStudentRepository 등)에 의존하도록 코드를 약간 수정합니다. (이때 의존성 주입 설정에서 `AddScoped<IStudentRepository, StudentRepository>`를 Infrastructure에 작성)
  - **공통 유틸리티 구성:** 프로젝트 전반에서 사용할 공통 기능(예: 로깅, 에러 처리)을 설계합니다. 이미 .NET 내장 Logging을 쓰고 있다면, 추후 Serilog 같은 라이브러리를 활용할 여지도 생각합니다. 또한 글로벌 예외 처리 미들웨어, 필터 등을 적용할지 여부도 결정합니다.
  - **형상관리 및 배포 전략 논의:** 코드 형상관리를 위해 Git 브랜치를 어떻게 운영할지 팀원이 있다 가정하고 정합니다. (Solo인 경우 main에 커밋하되 주요 기능 개발 전 태그를 찍어둔다든지). 또한 추후 **배포**는 Azure에 할 예정이므로, Azure App Service를 사용할지, Docker로 컨테이너를 만들어 배포할지 미리 선택해 둡니다.
  - **문서화:** 위 결정 사항들을 README나 노트에 기록해 둡니다. 폴더 구조도 README에 업데이트하고, 이 주차의 설계 산출물을 깃 저장소에 커밋합니다.

  *🔔 실전 프로젝트 단계 돌입:* 이제부터 몇 주에 걸쳐 설계한 프로젝트를 구현합니다. 실제 업무에서와 같이 단계적으로 개발하고 우선 기본 기능을 완성한 뒤 점진적으로 고급 기능과 개선을 추가할 것입니다.

## 11주차: 실전 프로젝트 – 도메인 모델 및 데이터베이스 구축
- **학습 목표:**
  - 프로젝트의 **핵심 도메인 모델**을 정의하고 엔티티 간 관계를 구현합니다. 이전 주에 설계한 Core 프로젝트의 엔티티들을 구체화합니다.
  - **마이그레이션 적용** 등 EF Core 데이터베이스 초기화를 수행하고, 개발 편의를 위한 **시드 데이터**를 추가합니다.
  - Repository 패턴이나 Unit of Work를 도입하기로 했다면, 해당 인터페이스와 구현 클래스를 작성하여 데이터 액세스 계층을 캡슐화합니다.
- **추천 자료:**
  - [엔터프라이즈 애플리케이션의 계층 설계] – DDD(도메인 주도 설계) 관점에서 엔티티와 밸류 오브젝트를 구분하고 도메인 서비스를 설계하는 내용 (심화). 필요시 핵심 개념만 참조.
  - [EF Core Migrations 고급] – 개발/테스트/프로덕션 환경별로 DB 스키마를 관리하는 팁, 마이그레이션 ID 충돌 해결 등 (문제 발생 시 참고).
  - **ASP.NET 8 Best Practices** 5장(Entity Framework) – 대규모 애플리케이션에서 EF Core 사용 시 권장되는 패턴들을 다시 확인합니다 (예: Context 인스턴스 단위 테스트용으로 InMemory 사용 등).
- **실습:**
  - **도메인 모델 구현:** 예를 들어 학생 성적 관리 시스템이라면 Student, Course, Enrollment 등의 클래스(엔터티)를 Core 프로젝트에 모두 정의합니다. 필요한 경우 Grade 같은 Enum이나 값 객체도 정의하세요. 각 클래스에 데이터 주석 또는 Fluent API 설정은 Core에 최대한 담고, 복잡한 설정은 Infrastructure의 OnModelCreating에서 수행합니다.
  - **Repository 패턴 적용 (선택):** Core 프로젝트에 IRepository<T> 인터페이스와 도메인별 리포지토리 인터페이스(IStudentRepository 등)를 정의합니다. Infrastructure 프로젝트에서 EfCore 기반 리포지토리 구현(StudentRepository : IStudentRepository)을 작성합니다. 이때 IQueryable을 그대로 노출할지, 메서드 단위로 캡슐화할지 결정합니다. (단순한 경우 서비스/컨트롤러에서 바로 DbContext 쓰는 것도 괜찮습니다 – Best Practices 도서에서도 상황에 따라 다르게 권장됨을 참고하세요.)
  - **DbContext 세팅:** Infrastructure의 SchoolContext를 업데이트하여 신규 엔티티들을 DbSet으로 포함시킵니다. 그리고 필요한 관계 설정이나 시드 데이터를 OnModelCreating에 추가합니다. 예를 들어 Grade Enum을 문자열로 저장하거나, 초기 몇 명의 Student와 Course 추가 등을 Seed로 구현합니다.
  - **마이그레이션 생성 & 적용:** 새로운 도메인 모델 기반으로 `Add-Migration FullSchemaInit`을 실행하고 DB를 업데이트합니다. 테이블이 모두 생성되고 FK 관계도 정확히 맺어졌는지 검사합니다. (개발 단계에서는 DB를 삭제 후 다시 만들 수도 있습니다.)
  - **DI 설정:** Startup/Program에 Repository 패턴을 도입했다면, 해당 인터페이스-구현 바인딩을 추가합니다. 또한 다른 서비스가 있다면 (예: 성적 계산 도메인 서비스 등) 이를 `AddTransient` 등으로 등록합니다.
  - **기본 페이지 연결:** Web 프로젝트의 컨트롤러들을 업데이트하여 이제 Repository나 DbContext를 통해 실제 DB 데이터를 다루게 합니다. 예를 들어 StudentController의 Index 액션에서 `studentRepo.ListAll()` (또는 `context.Students.Include(e=>e.Enrollments)`)로 학생 목록을 가져와 뷰에 전달합니다.
  - **동작 확인:** 애플리케이션을 실행하여 주요 화면(학생 목록, 상세 등)이 오류 없이 나오고, 시드된 초기 데이터가 보이는지 확인합니다. 또한 CRUD 기능 중 읽기 이외에 아직 구현 안 된 부분(create/edit/delete)은 향후 주차에 구현할 계획이라 TODO 주석을 남겨둡니다.
  - 이번 주차에는 데이터베이스 중심으로 작업했으므로, 프런트엔드 변화는 크지 않을 수 있습니다. 하지만 **탄탄한 도메인 계층**이 구성되었다는 것이 핵심 성과입니다. 구조 변경 내용을 README 또는 개발 노트에 기록하고 커밋합니다.

## 12주차: 실전 프로젝트 – 주요 기능 구현 (핵심 CRUD 완성)
- **학습 목표:**
  - 도메인 핵심 기능들을 실제로 동작하도록 구현합니다. 각 CRUD 기능과 비즈니스 로직(예: 성적 계산, 재고 처리 등)을 작성합니다.
  - **유저 인터페이스(UI)**를 보강합니다. Bootstrap 등의 CSS 프레임워크를 활용해 기본적인 디자인을 적용하고, 사용자 경험을 개선합니다 (필요 시 클라이언트 스크립트 추가).
  - **검증 및 예외 처리**를 강화하여 품질을 높입니다. 잘못된 입력에 대한 서버-클라이언트 양측의 검증, 예외 상황 시 오류 메시지 표시 등을 구현합니다.
- **추천 자료:**
  - [Bootstrap 5 사용 가이드] – ASP.NET Core 프로젝트에 Bootstrap을 적용하여 반응형 디자인을 만드는 방법. 기본 레이아웃에 부트스트랩 클래스 적용 사례 등을 참고합니다.
  - [검증 요약 및 사용자 피드백 표시] – ModelState 검증 실패 시 `ModelState.IsValid` 활용, ValidationSummary 태그헬퍼로 종합 오류 표시 등 문서.
  - [글로벌 예외 처리 및 로깅] – `UseExceptionHandler` 미들웨어 설정으로 사용자에게 공용 오류 페이지를 제공하고, 로그에는 자세한 스택을 남기는 방법 (필요 시).
- **실습:**
  - **CRUD 구현 완성:** 프로젝트의 모든 주요 엔티티에 대해 Create/Read/Update/Delete 기능을 만듭니다. 예를 들어 *과목 관리* 페이지를 만들어 과목 목록 조회, 추가/편집/삭제를 할 수 있도록 합니다. 이미 StudentsController 등을 만들었다면 해당 컨트롤러에 Create/Edit/Delete 액션과 뷰를 추가하세요. (스캐폴딩 기능을 사용하면 자동 생성된 CRUD 코드를 얻을 수 있습니다. 이를 참고하여 수동으로 리팩터링해도 좋습니다.)
  - **비즈니스 로직 구현:** 만약 도메인상 특별한 로직이 있다면 구현합니다. 예를 들어 성적 평점 계산 함수, 수강신청 시 제약 조건 체크(이미 들은 과목은 제외) 등 규칙을 서비스 계층이나 도메인 메서드로 작성하고 컨트롤러에서 호출합니다.
  - **UI 향상:** _Layout.cshtml에 부트스트랩 CSS와 JS 링크를 추가합니다 (혹은 LibMan으로 bootstrap 설치). Navbar를 꾸미고, 테이블에 부트스트랩 클래스를 적용하여 가독성을 높입니다. 폼 요소들도 적절히 클래스(.form-control 등)를 부여합니다. 필요하면 간단한 JS를 작성하여 UX를 개선합니다 (예: 삭제 버튼 클릭 시 확인 대화창 띄우기 등).
  - **클라이언트 검증 확인:** 이전에 설정한 DataAnnotations가 클라이언트 쪽에서도 작동하는지 확인합니다. (Scaffolding한 Identity UI 참고하여 jQuery Validation이 스크립트로 포함됐는지 확인). 폼에 잘못된 값 입력 시 즉시 오류 메시지가 표시되는지 테스트하고, ModelState 검증이 서버에서도 이중 체크되는지 살핍니다.
  - **예외 처리 시나리오:** 의도적으로 오류를 일으켜 봅니다 (예: 존재하지 않는 ID로 상세 페이지 접근, DB 연결 문자열 오류 등). `Startup`에 `app.UseExceptionHandler("/Home/Error")` 등 설정을 추가하여 사용자에게는 친절한 오류 페이지를 보여주고 개발 모드에서는 상세 오류를 볼 수 있도록 합니다. Error 페이지를 커스터마이징해 “문제가 발생했습니다” 정도의 안내와 추적 ID (혹은 그냥 날짜) 등을 표시해봅니다.
  - 구현한 기능들을 실제 사용자 시나리오에 따라 하나씩 테스트해보고, 발견된 **버그를 수정**합니다. (예: 누락된 null 체크, 잘못된 라우팅 등)
  - 이번 주차가 지나면 애플리케이션의 **주요 기능이 거의 완성**된 상태가 됩니다. 아직 리팩토링 여지나 최적화, 고급 기능이 남아있지만, 동작하는 제품을 얻는 것이 우선이었습니다. 애플리케이션을 로컬에서 종합적으로 테스트한 후, 현재까지 버전을 커밋합니다.

## 13주차: 실전 프로젝트 – 테스트 작성 (단위 테스트 및 통합 테스트)
- **학습 목표:**
  - 프로젝트의 핵심 로직에 대한 **단위 테스트(Unit Test)**를 작성합니다. 비즈니스 로직 함수나 컨트롤러의 메서드가 예상대로 동작하는지 검증합니다.
  - **통합 테스트(Integration Test)** 환경을 구축합니다. TestServer 또는 WebApplicationFactory를 사용하여 실제 웹 요청-응답 사이클을 테스트하고, EF Core InMemory 또는 SQLite로 테스트용 DB를 활용합니다.
  - 테스트를 통해 발견된 버그를 수정하고, 코드 품질을 향상시킵니다 (테스트하기 어려운 부분은 리팩터링하는 것도 포함).
- **추천 자료:**
  - [ASP.NET Core 단위 테스트 공식 문서] – xUnit 등을 사용하여 컨트롤러를 단위 테스트하는 방법, Fake DbContext 주입 등 기본 예제가 나와 있습니다.
  - [통합 테스트 공식 문서] – `WebApplicationFactory<TEntry>`를 활용하여 전체 ASP.NET Core 앱을 메모리 상에서 호스팅하고 `HttpClient`로 요청을 보내는 방식의 테스트를 설명합니다.
  - [EF Core InMemory 제공자 사용법] – 단위 테스트 시 가벼운 InMemory DB로 대체하여 빠르게 테스트하는 방법 (InMemory는 관계 제약 등이 실제와 달라 통합테스트보다는 단위테스트용으로 적합).
  - **ASP.NET Core in Action** 23장 (Testing) – 다양한 레벨의 테스트 (서비스 테스트, 미들웨어 테스트, 컨트롤러 테스트 등) 예제 코 ([GitHub - andrewlock/asp-dot-net-core-in-action-2e: Source code examples for ASP.NET Core in Action, Second Edition](https://github.com/andrewlock/asp-dot-net-core-in-action-2e#:~:text=%2A%20ExchangeRates%20,Also%20shows%20a)) ([GitHub - andrewlock/asp-dot-net-core-in-action-2e: Source code examples for ASP.NET Core in Action, Second Edition](https://github.com/andrewlock/asp-dot-net-core-in-action-2e#:~:text=Chapter%2023))】를 참고해도 좋습니다.
- **실습:**
  - **테스트 프로젝트 생성:** 솔루션에 xUnit 기반의 새 테스트 프로젝트(예: *MyApp.Tests*)를 추가합니다. `Microsoft.AspNetCore.Mvc.Testing` 패키지도 설치합니다 (통합테스트 용). 테스트 프로젝트에서 Web 프로젝트를 참조하고, 필요하면 Infrastructure도 참조합니다.
  - **단위 테스트 – 도메인/서비스:** 비즈니스 로직 함수를 테스트합니다. 예를 들어 성적 평균 계산 메서드가 있다면 여러 케이스의 입력을 주어 기대한 결과가 나오는지 `Assert.Equal`로 확인합니다. 또한 잘못된 입력 시 예외 던지도록 한 경우 해당 예외가 발생하는지 `Assert.Throws`로 검증합니다.
  - **단위 테스트 – 컨트롤러 (with fake):** 컨트롤러의 액션을 DB 없이 테스트하려면, Repository나 DbContext를 **모의 객체(Mock)**로 대체해야 합니다. MOQ 라이브러리 등을 사용하여 IStudentRepository의 가짜 구현을 만들고 원하는 데이터를 반환하게 한 뒤, 컨트롤러 액션을 직접 호출해 `ViewResult`나 `RedirectToAction` 결과를 검사합니다. 예: `var result = studentController.Details(5); Assert.IsType<ViewResult>(result);` 등의 검증.
  - **통합 테스트 – 웹 전체:** `WebApplicationFactory<Program>`을 이용해 애플리케이션을 테스트 서버로 구동하고 `HttpClient`로 엔드포인트를 호출해 봅니다. 예를 들어 `/Students` GET 요청을 보내 200 OK와 페이지 내용에 “Students List” 문자열이 포함되는지 확인합니다. 데이터베이스가 필요하면, 별도 설정으로 Context를 InMemory로 바꾸거나 테스트용 SQLite DB를 사용하게 구성합니다 (테스트 시 appsettings.Test.json 등을 읽게 할 수도 있고, WebApplicationFactory override를 활용합니다).
  - **테스트 실행 및 개선:** 작성한 테스트들을 `dotnet test`로 실행합니다. 실패한 테스트가 있다면 원인을 파악하여 코드 수정 혹은 테스트 수정으로 통과시킵니다. (예: 컨트롤러에서 null 체크가 빠졌다면 추가하는 등) 모든 핵심 시나리오에 대한 테스트가 통과하면, 이후 리팩터링 시 테스트가 안전망이 되어줄 것입니다.
  - **코드 커버리지 측정 (선택):** 커버리지 도구를 활용해 테스트가 부족한 부분이 없는지 점검합니다. 중요한 도메인 로직이 커버되지 않았다면 테스트를 추가합니다.
  - 이번 주차 작업으로 프로젝트에 대한 **신뢰성**이 높아졌습니다. 테스트 코드도 저장소에 커밋하고, CI/CD에서 자동으로 돌릴 계획을 세워둡니다 (다음 주 CI/CD 구성 예정).

## 14주차: 성능 개선 및 모니터링 준비
- **학습 목표:**
  - 애플리케이션의 **성능 병목**이 될 수 있는 부분을 식별하고 최적화합니다. (예: 불필요한 DB 쿼리 줄이기, 필요한 경우 캐싱 도입 등)
  - **캐싱 전략**을 수립합니다. 메모리 캐시를 적용하여 빈번히 조회되는 데이터(예: 과목 목록 등)를 캐싱하고 적절한 만료 정책을 설정합니다.
  - 애플리케이션 **모니터링**을 위한 준비를 합니다. 로그 수준을 조정하고, Azure Application Insights 등의 APM 도구를 통합할 계획을 세웁니다.
- **추천 자료:**
  - [ASP.NET Core 성능 최적화 모범 사례 ([ASP.NET Core Best Practices | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-9.0#:~:text=This%20article%20provides%20guidelines%20for,NET%20Core%20apps)) ([ASP.NET Core Best Practices | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-9.0#:~:text=Avoid%20blocking%20calls))】 – 공식 문서의 Best Practices에서 캐싱, 비동기 호출, 스레드풀 등 성능 관련 권장 사항을 확인합니다.
  - [응용 프로그램 캐싱 개요] – 메모리 캐시(IMemoryCache)와 분산 캐시(IDistributedCache)의 사용 방법과 사 ([ASP.NET Core Best Practices | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-9.0#:~:text=Cache%20aggressively))】.
  - [프로파일링 도구] – Visual Studio 진단 도구 또는 MiniProfiler 사용법 (SQL 쿼리 실행시간 등을 추적) – 성능 병목 발견에 활용.
  - [애플리케이션 인사이트 소개] – Azure Application Insights를 통해 요청/의존성/예외 추적 및 메트릭 수집하는 방법 (SDK 설치 및 설정).
- **실습:**
  - **SQL 쿼리 최적화:** EF Core로 발생하는 쿼리를 점검합니다. 예를 들어 학생 목록을 조회할 때 수강내역을 필요 이상으로 Include하여 N+1 문제가 없는지 확인합니다. 필요한 경우 쿼리를 조정하거나, Projection을 사용해 DTO로 필요한 데이터만 SELECT합니다. LINQ로 복잡한 연산을 할 때 서버 필터링이 잘 적용되는지도 살핍니다.
  - **캐싱 구현:** 빈번히 변경되지 않는 데이터에 메모리 캐시를 적용해봅니다. 예를 들어 과목 목록은 자주 바뀌지 않는다면, CourseController의 목록 액션에서 처음 호출 시 `IMemoryCache`에 데이터를 저장하고 이후 요청부터는 캐시를 반환하도록 구현합니 ([ASP.NET Core Best Practices | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices?view=aspnetcore-9.0#:~:text=This%20article%20provides%20guidelines%20for,NET%20Core%20apps))】. 캐시 만료 정책을 5분 등으로 설정하여 갱신되도록 합니다.
    - 또 다른 예로, 통계 정보(전체 학생 수 등)를 표시한다면 이 값도 캐시하여 1시간 간격으로만 DB 질의하도록 할 수 있습니다.
  - **이미지/정적파일 최적화 (Optional):** 만약 프로젝트에서 이미지 업로드/표시를 한다면, 이미지 크기를 조정하거나 정적 파일 응답에 적절한 캐시 헤더를 붙여 브라우저 캐시를 활용합니다. (UseStaticFiles 기본 StaticFileOptions에서 `OnPrepareResponse`로 Cache-Control 헤더 설정)
  - **로깅 설정 조정:** `appsettings.Development.json`에서 로그 레벨을 Debug로 유지하되, `appsettings.Production.json` (미리 만들어둔다면)에선 Warning 이상만 로그하도록 설정합니다. DataAccess 같이 채팅ty한 로그는 Category 별로 Level을 높이거나 필요시에만 출력하도록 조정합니다.
  - **Application Insights 통합 (선택):** Azure에 배포할 것을 대비하여, Application Insights SDK (`Microsoft.ApplicationInsights.AspNetCore`)를 프로젝트에 설치하고 Instrumentation Key를 설정합니다. 로컬에서 Application Insights에 텔레메트리 보내기가 가능하다면 시도해보고, Azure 배포 후 제대로 작동할 수 있도록 준비만 합니다.
  - **부하 테스트 (간이):** Visual Studio의 부하 테스트 기능 또는 Postman 스크립트를 활용하여 다수의 동시 요청을 시뮬레이션해 봅니다. 10명 동시 접속 시 응답 시간이 크게 느려지거나 오류 발생이 없는지 확인합니다. 성능 이슈가 드러나면 원인을 분석합니다 (예: DB Lock 등).
  - 최적화한 부분이 실제로 효과가 있는지 로그나 프로파일러로 확인합니다. (캐시 적중 시 로그 출력 등으로 확인). 성능 관련 수정사항과 앞으로 Azure 환경에서 고려할 점(예: 분산 캐시로 전환 필요 등)을 문서화하고 커밋합니다.

## 15주차: Azure 클라우드 배포 준비 (환경 설정 및 Docker 컨테이너)
- **학습 목표:**
  - 애플리케이션을 **클라우드에 배포**하기 위한 준비를 합니다. Azure App Service 사용 방법 또는 Docker 컨테이너화 전략을 결정합니다.
  - **환경별 설정**을 관리합니다. 개발(dev), 스테이징(staging), 프로덕션(prod) 환경에 따라 다른 설정(예: DB 연결 문자열, 로그 레벨 등)이 적용되도록 `ASPNETCORE_ENVIRONMENT` 및 설정 파일을 구성합니다.
  - Docker를 사용해 애플리케이션을 컨테이너 이미지로 만들고, 로컬에서 컨테이너 실행 테스트를 합니다 (필요시).
- **추천 자료:**
  - [Azure App Service로 앱 배포하기 ([Deploy ASP.NET Core apps to Azure App Service | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/?view=aspnetcore-9.0#:~:text=App%20Service%20Documentation%20is%20the,NET%20Core%20apps%20are)) ([Deploy ASP.NET Core apps to Azure App Service | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/?view=aspnetcore-9.0#:~:text=Publish%20an%20ASP,App%20Service%20using%20Visual%20Studio))】 – Visual Studio를 통해 Azure에 게시하는 방법과 Azure Portal 설정에 대한 튜토리얼.
  - [GitHub Actions로 Azure 배포 파이프라인 구성] – GitHub Actions에서 `azure/webapps-deploy` 사용해 App Service에 CI/CD 하는 예제 (다음 주 CI/CD에서 활용).
  - [Dockerizing ASP.NET Core 앱] – Dockerfile 작성법, multi-stage build로 최적화 빌드, docker-compose로 DB와 함께 실행 등 실습 자료.
  - [환경별 설정 관리] – `appsettings.Staging.json` 파일 사용과 Azure의 App Settings(환경변수) 재정의 방안 설명.
- **실습:**
  - **Azure 자원 준비:** Azure 포털에서 App Service(리눅스/윈도우 계획 선택)와 Azure SQL Database 등을 미리 생성해 둡니다. (만약 Docker로 할 경우 Azure Container Registry와 Web App for Containers 준비)
  - **환경 설정 파일:** 프로젝트 루트에 *appsettings.Staging.json*, *appsettings.Production.json*을 추가합니다. 예를 들어 Production에서는 실제 Azure SQL 연결문자열 사용, 로깅은 Warning 수준, Application Insights 키 포함 등의 설정을 넣습니다. 이러한 설정은 배포 시 `ASPNETCORE_ENVIRONMENT`에 따라 자동 로드됩니다.
  - **Docker 컨테이너화 (옵션):** Docker가 설치되어 있다면 프로젝트에 Docker 지원을 추가합니다. `Dockerfile`을 작성하여 애플리케이션을 빌드하고 실행하는 이미지를 만듭니다. 예를 들어 `FROM mcr.microsoft.com/dotnet/aspnet:8.0` 기반으로 `COPY` 후 `ENTRYPOINT` 설정. 그런 다음 `docker build -t myapp:latest .`로 이미지를 빌드하고 `docker run -p 5000:80 myapp:latest`로 로컬 실행해봅니다. 로컬에서 잘 작동하면 이 이미지를 Azure 컨테이너 레지스트리에 푸시하거나, 또는 나중에 GitHub Actions에서 빌드하도록 계획합니다.
  - **테스트/프로덕션 DB 준비:** Azure SQL을 사용한다면, 로컬 개발용과 별도로 Azure에 초기 마이그레이션을 적용할 필요가 있습니다. 방법1: DbContext 연결 문자열을 Azure DB로 바꾸고 `dotnet ef database update`를 원격 실행 (보안 주의). 방법2: 애플리케이션 첫 구동 시 자동 마이그레이션 적용 코드를 넣어서, Azure에서 앱이 처음 시작될 때 `context.Database.Migrate()`를 호출하게 합니다. (작은 앱에서는 편리하지만, 운영환경에서 임의 마이그레이션은 신중해야 함)
  - **배포 리허설 – 수동:** Visual Studio의 **게시(Publish)** 기능을 이용해 App Service에 배포를 시도해봅니 ([Deploy ASP.NET Core apps to Azure App Service | Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/azure-apps/?view=aspnetcore-9.0#:~:text=Learn%20learn,Azure%20App%20Service%20on%20Windows))】. 또는 CLI를 이용해 `dotnet publish`로 배포 폴더를 만들고, Azure FTP/CLI로 업로드합니다. 배포된 앱이 Azure에서 정상 작동하는지 (페이지 열기, DB 연결 확인 등) 테스트합니다.
  - **DNS/SSL 등 설정:** 사용자 접근을 위해 커스텀 도메인을 사용하거나 Azure에서 생성된 *.azurewebsites.net 도메인을 그대로 쓸지 결정합니다. SSL은 기본 제공되므로 HTTP -> HTTPS 리다이렉트를 App Service 설정에서 “온”으로 맞춥니다 (또는 이미 UseHttpsRedirection이 있으므로).
  - 이 주차 실습으로 실제 **클라우드에서 앱을 호스팅**할 준비가 완료되었습니다. 아직 CI/CD 자동화는 안 했더라도, 수동으로 배포된 앱을 보면서 확인합니다. 배포 관련 설정과 Docker파일 등을 커밋하고, Azure에 저장하기 민감한 정보(Instrumentation Key 등)는 환경변수로 관리했는지 재점검합니다.

## 16주차: CI/CD 파이프라인 구성 및 배포 자동화
- **학습 목표:**
  - **지속적 통합/배포(CI/CD)** 파이프라인을 구축합니다. 코드 푸시 시 자동으로 빌드와 테스트가 실행되고, 배포까지 이어지도록 설정합니다.
  - GitHub Actions 또는 Azure DevOps 등을 사용하여 워크플로우를 작성하고, 애플리케이션을 **무중단 배포(blue-green 또는 slot)**하는 전략을 고려합니다 (Azure Deployment Slots 활용).
  - 배포 후 모니터링과 피드백 루프를 설정합니다. (예: 배포 성공/실패 알림, Application Insights 경고 등을 설정)
- **추천 자료:**
  - [GitHub Actions로 .NET 빌드/테스트 ([How To Build a CI/CD Pipeline With GitHub Actions And .NET](https://www.milanjovanovic.tech/blog/how-to-build-ci-cd-pipeline-with-github-actions-and-dotnet#:~:text=Continuous%20Integration%20With%20GitHub%20Actions)) ([How To Build a CI/CD Pipeline With GitHub Actions And .NET](https://www.milanjovanovic.tech/blog/how-to-build-ci-cd-pipeline-with-github-actions-and-dotnet#:~:text=name%3A%20Build%20%26%20Test))】 – GitHub Actions YAML 예시 (dotnet restore/build/test 수행 ([How To Build a CI/CD Pipeline With GitHub Actions And .NET](https://www.milanjovanovic.tech/blog/how-to-build-ci-cd-pipeline-with-github-actions-and-dotnet#:~:text=name%3A%20Build%20%26%20Test))】.
  - [GitHub Actions로 Azure App Service 배포] – azure/login 및 azure/webapps-deploy 액션 사용 예제. (MS Learn 튜토리얼이나 블로그 참고)
  - [Azure DevOps 파이프라인 .yml] – Azure DevOps 사용 시 .NET 빌드/테스트/배포 파이프라인 예제.
  - [슬롯 배포 가이드] – Azure App Service의 Staging 슬롯에 먼저 배포하고 `Swap`으로 프로덕션으로 전환하는 방법 (무중단 배포)에 대한 문서.
- **실습:**
  - **CI 구성:** GitHub Actions를 선택한 경우, 프로젝트 저장소에 `.github/workflows/ci.yaml` 파일을 만듭니다. 워크플로우에서 `ubuntu-latest` 환경으로 checkout -> `dotnet restore` -> `dotnet build --configuration Release` -> `dotnet test --no-build` 순으로 작업을 정의합니 ([How To Build a CI/CD Pipeline With GitHub Actions And .NET](https://www.milanjovanovic.tech/blog/how-to-build-ci-cd-pipeline-with-github-actions-and-dotnet#:~:text=Here%27s%20a%20GitHub%20Actions%20workflow,NET%20project)) ([How To Build a CI/CD Pipeline With GitHub Actions And .NET](https://www.milanjovanovic.tech/blog/how-to-build-ci-cd-pipeline-with-github-actions-and-dotnet#:~:text=jobs%3A%20build%3A%20runs))】. 테스트 실패 시 중단되도록 하고, 성공 시 artifact로 필요한 파일을 저장할지 결정합니다.
  - **CD 구성:** 같은 워크플로우나 별도 workflow에서 배포 단계를 추가합니다. GitHub Secrets에 Azure Publish Profile이나 서비스 연결 정보를 저장하고, 액션에서 이를 사용해 Azure에 로그인합니다. 그 후 `azure/webapps-deploy@v2` 액션으로 빌드된 아티팩트를 배포합니다 (혹은 Docker 이미지를 push 후 Azure WebApp for Containers 설정).
    - YAML 예시:
      ```yaml
      - uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      - uses: azure/webapps-deploy@v2
        with:
          app-name: <App Service 이름>
          slot-name: staging
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: <패키지 경로>
      ```
      배포를 Staging 슬롯으로 하고, Swap을 트리거하는 액션이나 Azure CLI 커맨드를 이어서 실행하도록 할 수 있습니다.
  - **파이프라인 테스트:** GitHub 저장소에 코드를 푸시하여 Actions가 실행되는지 확인합니다. 빌드나 테스트 단계에서 실패한다면 워크플로우 YAML이나 코드를 수정합니다. 배포까지 성공하면 Azure Portal에서 업데이트된 내용이 반영되었는지 확인합니다.
  - **알림 설정:** GitHub Actions의 상태를 Slack이나 Teams로 알리는 연동을 추가하거나 (옵션), Azure App Service의 Alerts(Application Insights 스마트 감지 등)를 설정하여 앱 오류가 발생하면 메일을 받도록 구성합니다.
  - **최종 점검:** 배포 자동화 후, 새로운 기능을 하나 추가하거나 문구를 수정한 뒤 커밋하여 전체 CI/CD 파이프라인이 문제없이 동작하는지 본격적으로 테스트합니다. 프로덕션 슬롯에 배포될 때 중단 없이 이전 버전에서 새 버전으로 전환되는지 (슬롯 사용 시) 직접 웹앱에 접속하여 확인합니다.
  - 이제 코드 변경 -> 자동 테스트 -> 자동 배포의 **DevOps 사이클**이 완성되었습니다. 이는 중급 개발자로서 실무 적용에 큰 도움을 줄 것입니다. 파이프라인 설정 파일과 관련 문서를 최종 커밋하고, 필요시 README에 배포 방법/URL 등을 추가합니다.

## 17주차 이후: 고급 주제 학습 및 지속적인 개선
- **학습 목표:** (지속적)
  - 위에서 완성한 프로젝트를 토대로 새로운 **고급 주제**들을 실험하고 학습합니다. 예를 들어 **실시간 기능**이 필요하다면 SignalR을 도입하고, **클라이언트 사이드 Blazor**로 일부 페이지를 작성해본다든지, **마이크로서비스**로의 확장 가능성을 검토합니다.
  - 기술 트렌드에 맞추어 .NET 최신 버전(.NET 9 이후)에서 추가되는 ASP.NET Core 기능을 추적하고, 프로젝트에 적용해봅니다 (예: Minimal API의 활용, 성능 향상 기능, Native AOT 등).
  - 오픈 소스 프로젝트에 기여하거나, 본인의 프로젝트를 GitHub에 공개하여 피드백을 받아봅니다.
- **추천 자료 (지속 학습):**
  - [실시간 기능 – SignalR ([Tutorial: Get started with ASP.NET Core SignalR - Learn Microsoft](https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-9.0#:~:text=Tutorial%3A%20Get%20started%20with%20ASP,have%20a%20working%20chat%20app))】 – SignalR을 이용한 채팅 앱 튜토리얼, 프로젝트에 알림 기능으로 적용해보기.
  - [Blazor WebAssembly] – Blazor로 SPA 구성해보기 (기존 MVC 뷰 일부 대체하거나, 새 Blazor 프로젝트 실습).
  - [마이크로서비스 패턴] – 도커 컴포즈로 서비스 2~3개(API Gateway + 인증 서비스 등) 만들어 통신해보기, gRPC 통신 적용 등.
  - [성능 튜닝 심화] – BenchmarkDotNet으로 특정 메서드 벤치마크, Span<T> 등의 고성능 API 활용 연습.
  - [.NET 커뮤니티 참여] – 최신 소식을 접할 수 있는 [ASP.NET Community Standup], [GitHub Discussions], [블로그 (Andrew Lock 등)] 팔로우.
- **실습 (지속):**
  - **SignalR 적용:** 프로젝트에 공지사항 실시간 알림 Hub를 추가하고, 클라이언트에서 Javascript로 SignalR Hub에 접속하여 새로운 공지가 생기면 즉시 표시되도록 구현해봅니 ([Tutorial: Get started with ASP.NET Core SignalR - Learn Microsoft](https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-9.0#:~:text=Tutorial%3A%20Get%20started%20with%20ASP,have%20a%20working%20chat%20app))】. 이를 통해 실시간 WebSocket 통신 흐름을 이해합니다.
  - **Blazor 컴포넌트 시도:** 예를 들어 성적 입력 페이지를 Blazor Server로 만들어보기. Blazor 프로젝트를 추가하고, 기존 ASP.NET Core 앱과 통합(hosting)해서 일부 경로를 Blazor로 라우팅합니다. C#만으로 리치 UI를 구현해보고, 필요 시 JS 상호운용(JSInterop)도 체험합니다.
  - **마이크로서비스 분리 실험:** 학생 관리와 성적 관리를 별도 **Minimal API** 서비스로 분리하고, 기존 앱에서 해당 API를 호출하여 데이터를 가져오도록 변경해봅니다. 이를 통해 서비스 경계를 이해하고, 분산 트랜잭션이나 API 게이트웨이 필요성을 고민해봅니다. (이러한 리팩터링은 큰 변경이므로 실험용 브랜치에서 시도)
  - **코드 리뷰 & 리팩터링:** 동료 개발자(또는 온라인 커뮤니티)에 코드 리뷰를 요청해봅니다. 피드백을 바탕으로 더 나은 코드 구조(예: 더 깔끔한 LINQ, 디자인 패턴 적용 등)로 개선해봅니다.
  - **문서화:** 프로젝트 사용 매뉴얼이나 기술 결정에 대한 문서를 작성해봅니다. 또한 XML 주석을 이용해 공개 API에 주석을 달고 Swagger를 통해 API 문서화를 시도해도 좋습니다.
  - 이러한 지속적인 학습을 통해 ASP.NET Core를 **심화 수준**까지 다루게 되며, 실무 적용 역량을 기르게 됩니다. 마지막으로, 프로젝트를 포트폴리오로 정리하거나, GitHub에 오픈소스로 공개해두면 구직 및 커리어에도 도움이 될 것입니다.

---

**📌 정리:** 6개월 간의 집중 학습을 통해 ASP.NET Core의 기본부터 실전 적용, 그리고 클라우드 배포와 DevOps까지 경험했습니다. 이후 남은 기간(~1년까지)은 새로운 기술을 프로젝트에 접목하고 코드를 지속적으로 개선하는 데 활용하세요. Microsoft Docs, GitHub 오픈 소스, 커뮤니티를 활발히 참고하며 최신 정보를 따라가는 것도 잊지 마세요. **중급 개발자**로서 이 과정을 마치면 ASP.NET Core를 이용한 웹 개발 전반을 스스로 수행할 수 있을 정도의 역량을 갖추게 될 것입니다. 🚀

