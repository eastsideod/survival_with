# Istio 사용 예제

## 1. ASP.NET 기본 서비스 배포

### ASP.NET 예제 애플리케이션 배포

```yaml
# aspnet-app.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: aspnet-app
spec:
  replicas: 3
  selector:
    matchLabels:
      app: aspnet-app
  template:
    metadata:
      labels:
        app: aspnet-app
    spec:
      containers:
      - name: aspnet-app
        image: mcr.microsoft.com/dotnet/aspnet:6.0
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_URLS
          value: "http://+:80"
```

### ASP.NET 서비스 생성

```yaml
# aspnet-service.yaml
apiVersion: v1
kind: Service
metadata:
  name: aspnet-service
spec:
  selector:
    app: aspnet-app
  ports:
  - port: 80
    targetPort: 80
```

## 2. ASP.NET 트래픽 관리 예제

### 가중치 기반 라우팅

```yaml
# weighted-routing.yaml
apiVersion: networking.istio.io/v1alpha3
kind: VirtualService
metadata:
  name: aspnet-weighted-routing
spec:
  hosts:
  - aspnet-service
  http:
  - route:
    - destination:
        host: aspnet-service
        subset: v1
      weight: 80
    - destination:
        host: aspnet-service
        subset: v2
      weight: 20
```

### 헤더 기반 라우팅

```yaml
# header-routing.yaml
apiVersion: networking.istio.io/v1alpha3
kind: VirtualService
metadata:
  name: aspnet-header-routing
spec:
  hosts:
  - aspnet-service
  http:
  - match:
    - headers:
        user-agent:
          exact: mobile
    route:
    - destination:
        host: aspnet-service
        subset: mobile
  - route:
    - destination:
        host: aspnet-service
        subset: web
```

## 3. ASP.NET 보안 예제

### mTLS 활성화

```yaml
# mtls.yaml
apiVersion: security.istio.io/v1beta1
kind: PeerAuthentication
metadata:
  name: aspnet-mtls
  namespace: default
spec:
  mtls:
    mode: STRICT
```

### ASP.NET RBAC 정책

```yaml
# rbac.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-policy
spec:
  selector:
    matchLabels:
      app: aspnet-app
  rules:
  - from:
    - source:
        principals: ["cluster.local/ns/default/sa/aspnet-app"]
    to:
    - operation:
        methods: ["GET", "POST"]
        paths: ["/api/*"]
```

## 4. ASP.NET 관찰성 예제

### 메트릭 수집

```yaml
# metrics.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-metrics
spec:
  metrics:
  - providers:
    - name: prometheus
    overrides:
    - match:
        metric: REQUEST_COUNT
        mode: CLIENT_AND_SERVER
      tagOverrides:
        source_service:
          value: "source.workload.namespace"
        destination_service:
          value: "destination.workload.namespace"
```

### ASP.NET 추적 설정

```yaml
# tracing.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-tracing
spec:
  tracing:
  - randomSamplingPercentage: 100.0
    customTags:
      source_service:
        literal:
          value: "source.workload.namespace"
      destination_service:
        literal:
          value: "destination.workload.namespace"
```

## 5. ASP.NET 정책 예제

### 속도 제한

```yaml
# rate-limit.yaml
apiVersion: policy.istio.io/v1beta1
kind: RateLimit
metadata:
  name: aspnet-rate-limit
spec:
  selector:
    matchLabels:
      app: aspnet-app
  limits:
  - actions:
    - requestHeaders:
        descriptorKey: request-count
        headerName: x-user-id
    rateLimit:
      requestsPerUnit: 100
      unit: MINUTE
```

### ASP.NET 할당량

```yaml
# quota.yaml
apiVersion: policy.istio.io/v1beta1
kind: Quota
metadata:
  name: aspnet-quota
spec:
  selector:
    matchLabels:
      app: aspnet-app
  limits:
  - quota: "1000"
    dimensions:
      source: source.workload.namespace
      destination: destination.workload.namespace
```

## 6. ASP.NET 게이트웨이 예제

### 인그레스 게이트웨이

```yaml
# ingress-gateway.yaml
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: aspnet-gateway
spec:
  selector:
    istio: ingressgateway
  servers:
  - port:
      number: 80
      name: http
      protocol: HTTP
    hosts:
    - "aspnet-app.example.com"
```

### 이그레스 게이트웨이

```yaml
# egress-gateway.yaml
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: aspnet-egress-gateway
spec:
  selector:
    istio: egressgateway
  servers:
  - port:
      number: 443
      name: tls
      protocol: TLS
    hosts:
    - "*.example.com"
    tls:
      mode: PASSTHROUGH
```

## 7. ASP.NET 서비스 메시 확장 예제

### 웹어셈블리 필터

```yaml
# wasm-filter.yaml
apiVersion: extensions.istio.io/v1alpha1
kind: WasmPlugin
metadata:
  name: aspnet-wasm
spec:
  selector:
    matchLabels:
      app: aspnet-app
  url: oci://ghcr.io/istio/wasm/sample-filter:latest
  phase: AUTHN
  pluginConfig:
    key: value
```

### ASP.NET 사용자 정의 어댑터

```yaml
# custom-adapter.yaml
apiVersion: config.istio.io/v1alpha2
kind: adapter
metadata:
  name: aspnet-adapter
spec:
  compiledAdapter: aspnet-adapter
  params:
    param1: value1
    param2: value2
``` 