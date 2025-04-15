# ASP.NET Core 쿠버네티스 배포

## 1. 쿠버네티스 배포 개요

### 1.1 쿠버네티스의 장점
- 컨테이너 오케스트레이션
- 자동 스케일링
- 서비스 디스커버리
- 로드 밸런싱
- 롤백 및 롤링 업데이트 지원

### 1.2 기본 구성 요소
- **Pod**: 컨테이너 실행 단위
- **Deployment**: Pod의 배포 및 관리
- **Service**: Pod 그룹에 대한 네트워크 엔드포인트
- **ConfigMap**: 설정 데이터 관리
- **Secret**: 민감한 정보 관리

## 2. Docker 이미지 준비

### 2.1 Dockerfile
```dockerfile
# 빌드 스테이지
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GameServer.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

# 게시 스테이지
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# 최종 스테이지
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameServer.dll"]
```

### 2.2 이미지 빌드 및 푸시
```bash
# 이미지 빌드
docker build -t gameserver:latest .

# 이미지 태그 지정
docker tag gameserver:latest your-registry/gameserver:latest

# 레지스트리에 푸시
docker push your-registry/gameserver:latest
```

## 3. 쿠버네티스 매니페스트

### 3.1 Deployment
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gameserver
  labels:
    app: gameserver
spec:
  replicas: 3
  selector:
    matchLabels:
      app: gameserver
  template:
    metadata:
      labels:
        app: gameserver
    spec:
      containers:
      - name: gameserver
        image: your-registry/gameserver:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: db-secret
              key: connection-string
        resources:
          requests:
            cpu: "500m"
            memory: "512Mi"
          limits:
            cpu: "1000m"
            memory: "1Gi"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
```

### 3.2 Service
```yaml
apiVersion: v1
kind: Service
metadata:
  name: gameserver
spec:
  selector:
    app: gameserver
  ports:
  - port: 80
    targetPort: 80
  type: LoadBalancer
```

### 3.3 ConfigMap
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: gameserver-config
data:
  appsettings.json: |
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning"
        }
      },
      "GameSettings": {
        "MaxPlayers": "100",
        "MatchTimeout": "300"
      }
    }
```

### 3.4 Secret
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: db-secret
type: Opaque
data:
  connection-string: <base64-encoded-connection-string>
```

## 4. 배포 전략

### 4.1 롤링 업데이트
```yaml
spec:
  strategy:
    type: RollingUpdate
    rollingUpdate:
      maxSurge: 1
      maxUnavailable: 0
```

### 4.2 블루-그린 배포
```yaml
# v1 배포
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gameserver-v1
  labels:
    version: v1
    app: gameserver

# v2 배포
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gameserver-v2
  labels:
    version: v2
    app: gameserver

# 서비스 전환
apiVersion: v1
kind: Service
metadata:
  name: gameserver
spec:
  selector:
    version: v2  # v1에서 v2로 전환
```

## 5. 모니터링 및 로깅

### 5.1 Prometheus 메트릭스
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMetrics();
}

public void Configure(IApplicationBuilder app)
{
    app.UseMetrics();
}
```

### 5.2 로깅 설정
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: logging-config
data:
  log-config.json: |
    {
      "Serilog": {
        "MinimumLevel": "Information",
        "WriteTo": [
          {
            "Name": "Console"
          },
          {
            "Name": "Elasticsearch",
            "Args": {
              "nodeUris": "http://elasticsearch:9200"
            }
          }
        ]
      }
    }
```

## 6. 확장성 설정

### 6.1 Horizontal Pod Autoscaler
```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: gameserver-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: gameserver
  minReplicas: 3
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

### 6.2 리소스 제한
```yaml
resources:
  requests:
    cpu: "500m"
    memory: "512Mi"
  limits:
    cpu: "1000m"
    memory: "1Gi"
```

## 7. 보안 설정

### 7.1 네트워크 정책
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: gameserver-policy
spec:
  podSelector:
    matchLabels:
      app: gameserver
  ingress:
  - from:
    - podSelector:
        matchLabels:
          app: client
    ports:
    - protocol: TCP
      port: 80
```

### 7.2 RBAC (Role-Based Access Control) 설정
```yaml
# Role 정의 - 특정 리소스에 대한 권한을 정의
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  name: gameserver-role
  namespace: gameserver-ns
rules:
- apiGroups: [""]  # core API group
  resources: ["pods", "services"]  # 접근 가능한 리소스
  verbs: ["get", "list", "watch"]  # 허용된 작업

# RoleBinding - Role을 특정 사용자/서비스 계정에 연결
apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: gameserver-role-binding
  namespace: gameserver-ns
subjects:
- kind: ServiceAccount
  name: gameserver-sa
  namespace: gameserver-ns
roleRef:
  kind: Role
  name: gameserver-role
  apiGroup: rbac.authorization.k8s.io
```

RBAC는 쿠버네티스에서 리소스에 대한 접근 권한을 관리하는 시스템입니다. 주요 구성 요소와 특징은 다음과 같습니다:

1. **RBAC의 주요 구성 요소**
   - **Role**: 특정 네임스페이스 내에서 리소스에 대한 권한을 정의
   - **ClusterRole**: 클러스터 전체에서 리소스에 대한 권한을 정의
   - **RoleBinding**: Role을 특정 사용자/서비스 계정에 연결
   - **ClusterRoleBinding**: ClusterRole을 클러스터 전체의 사용자/서비스 계정에 연결

2. **권한 관리 방식**
   - **리소스 기반**: pods, services, deployments 등 특정 리소스에 대한 접근 제어
   - **동작 기반**: get, list, create, update, delete 등의 작업 권한 설정
   - **네임스페이스 기반**: 특정 네임스페이스 내에서만 권한 부여 가능

3. **게임 서버에서의 RBAC 활용**
   ```yaml
   # 게임 서버 서비스 계정
   apiVersion: v1
   kind: ServiceAccount
   metadata:
     name: gameserver-sa
     namespace: gameserver-ns

   # 게임 서버 Role
   apiVersion: rbac.authorization.k8s.io/v1
   kind: Role
   metadata:
     name: gameserver-role
     namespace: gameserver-ns
   rules:
   - apiGroups: [""]
     resources: ["pods", "services"]
     verbs: ["get", "list", "watch"]
   - apiGroups: ["apps"]
     resources: ["deployments"]
     verbs: ["get", "list", "watch"]

   # 게임 서버 RoleBinding
   apiVersion: rbac.authorization.k8s.io/v1
   kind: RoleBinding
   metadata:
     name: gameserver-role-binding
     namespace: gameserver-ns
   subjects:
   - kind: ServiceAccount
     name: gameserver-sa
     namespace: gameserver-ns
   roleRef:
     kind: Role
     name: gameserver-role
     apiGroup: rbac.authorization.k8s.io
   ```

4. **RBAC의 장점**
   - **세분화된 접근 제어**: 리소스별, 동작별로 권한을 세밀하게 설정 가능
   - **보안 강화**: 최소 권한 원칙 적용으로 보안 강화
   - **관리 용이성**: 역할 기반으로 권한을 그룹화하여 관리 용이
   - **감사 추적**: 누가 어떤 작업을 했는지 추적 가능

5. **모범 사례**
   - 최소 권한 원칙 준수
   - 네임스페이스별 권한 분리
   - 정기적인 권한 검토
   - 서비스 계정 사용 권장
   - Role과 RoleBinding의 명확한 네이밍

6. **주의사항**
   - 과도한 권한 부여 방지
   - 민감한 리소스에 대한 접근 제한
   - 정기적인 권한 감사
   - 불필요한 ClusterRole 사용 자제

## 8. 배포 모범 사례

### 8.1 환경 설정
- 환경 변수와 ConfigMap 활용
- 민감한 정보는 Secret으로 관리
- 환경별 설정 분리

### 8.2 성능 최적화
- 적절한 리소스 요청 및 제한 설정
- Pod 배치 전략 최적화
- 네트워크 정책 구성

### 8.3 가용성
- 다중 리전 배포 고려
- Pod 안티어피니티 설정
- 장애 복구 전략 수립

### 8.4 모니터링
- Prometheus 메트릭스 수집
- 로그 집계 시스템 구성
- 알림 설정 