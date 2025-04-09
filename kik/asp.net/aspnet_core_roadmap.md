# ASP.NET Core 학습 로드맵 (기초부터 고급까지)

## 📌 목차
1. [기초 이해 (Beginner)](#1-기초-이해-beginner)
2. [중급 응용 (Intermediate)](#2-중급-응용-intermediate)
3. [고급 개발 (Advanced)](#3-고급-개발-advanced)
4. [실전 및 DevOps 통합 (Expert)](#4-실전-및-devops-통합-expert)
5. [학습 자료 및 참고 링크](#5-학습-자료-및-참고-링크)

---

## 1. 기초 이해 (Beginner)

### 🔹 개념
- ASP.NET Core 소개: 크로스 플랫폼, 경량화된 .NET 플랫폼
- 웹 프로젝트 종류
  - MVC (Model-View-Controller)
  - Razor Pages
- 웹 요청 처리 과정: HTTP → Routing → Middleware → Controller
- Razor 문법 이해
- 기본 HTML 폼 처리와 유효성 검사

### 🔹 실습 예제
- 간단한 TODO 리스트
- Razor Pages로 사용자 정보 등록 폼

---

## 2. 중급 응용 (Intermediate)

### 🔹 핵심 기술
- **Entity Framework Core**
  - DbContext, Migration, Code First
  - LINQ를 활용한 데이터 쿼리
- **Dependency Injection (의존성 주입)**
- **서비스 & 리포지토리 패턴**
- **Authentication & Authorization**
  - 쿠키 기반 인증
  - JWT (JSON Web Token) 인증
- **RESTful API 개발**
  - `ApiController`, `[Route]`, `[HttpGet/Post]`
  - Swagger(OpenAPI) 통합
- **Logging & 예외 처리**
  - Serilog, NLog
  - `app.UseExceptionHandler()`

### 🔹 실습 예제
- 회원가입 / 로그인 기능
- 게시판 REST API 제작
- Swagger 문서화

---

## 3. 고급 개발 (Advanced)

### 🔹 고급 기능
- **SignalR**
  - 실시간 채팅, 알림
- **Background Services**
  - HostedService, Worker Service
- **gRPC / WebSockets**
  - gRPC 서비스 정의 및 호출
- **Middleware 확장**
  - Custom Middleware 작성
- **Custom Tag Helpers / View Components**
- **유닛 테스트 / 통합 테스트**
  - xUnit, NUnit, Moq, TestServer

### 🔹 실습 예제
- SignalR 기반 채팅 시스템
- 이메일 발송 백그라운드 서비스
- 서비스 계층 유닛 테스트

---

## 4. 실전 및 DevOps 통합 (Expert)

### 🔹 운영과 보안
- **Docker & Kubernetes**
  - ASP.NET Core 앱 컨테이너화
  - Helm chart를 통한 배포 자동화
- **CI/CD 파이프라인**
  - GitHub Actions / Azure DevOps
- **보안**
  - HTTPS 강제, HSTS
  - CSP(Content Security Policy)
  - OWASP Top 10 대응
- **성능 최적화**
  - Response Caching
  - Output Compression
  - Redis / MemoryCache
- **모니터링**
  - Health Checks
  - Prometheus + Grafana

### 🔹 실습 예제
- 블로그 사이트 클라우드 배포 (Azure or AWS)
- CI/CD + 테스트 자동화 파이프라인 구성
- 고가용성 API 서버 운영

---

## 5. 학습 자료 및 참고 링크

### 📘 공식 문서
- [ASP.NET Core 문서](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Authentication & Identity](https://learn.microsoft.com/aspnet/core/security/authentication/identity)

### 🎥 유튜브 강의
- IAmTimCorey - ASP.NET Core Series
- kudvenkat - ASP.NET Core MVC
- FreeCodeCamp - Full ASP.NET Core Crash Course

### 🛠️ 유용한 도구
- Swagger (Swashbuckle)
- Postman
- Visual Studio / VS Code
- Docker / Kubernetes

---

**👉 TIP:** 학습하면서 작은 프로젝트들을 단계별로 구현해보는 것이 최고의 복습 방법입니다!