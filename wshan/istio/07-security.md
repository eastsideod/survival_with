# Istio 보안

## 1. ASP.NET 인증

### mTLS 설정

```yaml
# mtls-config.yaml
apiVersion: security.istio.io/v1beta1
kind: PeerAuthentication
metadata:
  name: aspnet-mtls
  namespace: default
spec:
  mtls:
    mode: STRICT
```

### ASP.NET JWT 인증

```yaml
# jwt-auth.yaml
apiVersion: security.istio.io/v1beta1
kind: RequestAuthentication
metadata:
  name: aspnet-jwt-auth
spec:
  selector:
    matchLabels:
      app: aspnet-app
  jwtRules:
  - issuer: "https://auth.example.com"
    jwksUri: "https://auth.example.com/.well-known/jwks.json"
```

## 2. ASP.NET 권한 부여

### RBAC 설정

```yaml
# rbac-config.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-rbac-config
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

### ASP.NET 조건부 접근 제어

```yaml
# conditional-access.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-conditional-access
spec:
  selector:
    matchLabels:
      app: aspnet-app
  rules:
  - from:
    - source:
        requestPrincipals: ["*"]
    when:
    - key: request.headers[x-user-role]
      values: ["admin"]
    to:
    - operation:
        methods: ["*"]
        paths: ["/admin/*"]
```

## 3. ASP.NET 암호화

### TLS 설정

```yaml
# tls-config.yaml
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: aspnet-tls-gateway
spec:
  selector:
    istio: ingressgateway
  servers:
  - port:
      number: 443
      name: https
      protocol: HTTPS
    tls:
      mode: SIMPLE
      credentialName: aspnet-tls-cert
    hosts:
    - "aspnet-app.example.com"
```

### ASP.NET 인증서 관리

```yaml
# cert-manager.yaml
apiVersion: cert-manager.io/v1
kind: Certificate
metadata:
  name: aspnet-cert
spec:
  secretName: aspnet-tls-cert
  dnsNames:
  - "aspnet-app.example.com"
  issuerRef:
    name: letsencrypt-prod
    kind: ClusterIssuer
```

## 4. ASP.NET 네트워크 보안

### 네트워크 정책

```yaml
# network-policy.yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: aspnet-network-policy
spec:
  podSelector:
    matchLabels:
      app: aspnet-app
  policyTypes:
  - Ingress
  - Egress
  ingress:
  - from:
    - namespaceSelector:
        matchLabels:
          istio-injection: enabled
  egress:
  - to:
    - namespaceSelector:
        matchLabels:
          istio-injection: enabled
```

### ASP.NET 서비스 격리

```yaml
# service-isolation.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-service-isolation
spec:
  selector:
    matchLabels:
      app: aspnet-app
  rules:
  - from:
    - source:
        namespaces: ["default"]
    to:
    - operation:
        methods: ["GET", "POST"]
```

## 5. ASP.NET 보안 감사

### 감사 로깅

```yaml
# audit-logging.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-audit-logging
spec:
  logging:
  - providers:
    - name: stackdriver
    filter:
      expression: "response.code >= 400"
    overrides:
    - match:
        mode: CLIENT_AND_SERVER
      tagOverrides:
        audit_event:
          value: "true"
```

### ASP.NET 보안 이벤트 모니터링

```yaml
# security-monitoring.yaml
apiVersion: monitoring.coreos.com/v1
kind: PrometheusRule
metadata:
  name: aspnet-security-alerts
spec:
  groups:
  - name: aspnet.security.rules
    rules:
    - alert: ASPNetSecurityPolicyViolation
      expr: rate(istio_security_policy_violations_total[5m]) > 0
      for: 5m
      labels:
        severity: critical
      annotations:
        summary: ASP.NET Security policy violation detected
```

## 6. ASP.NET 보안 정책

### 정책 시행

```yaml
# policy-enforcement.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-policy-enforcement
spec:
  selector:
    matchLabels:
      app: aspnet-app
  rules:
  - from:
    - source:
        requestPrincipals: ["*"]
    when:
    - key: request.headers[x-security-level]
      values: ["high"]
    to:
    - operation:
        methods: ["*"]
```

### ASP.NET 정책 검증

```yaml
# policy-validation.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-policy-validation
spec:
  selector:
    matchLabels:
      app: aspnet-app
  rules:
  - from:
    - source:
        requestPrincipals: ["*"]
    when:
    - key: request.headers[x-validation]
      values: ["true"]
    to:
    - operation:
        methods: ["*"]
```

## 7. ASP.NET 보안 모범 사례

### 보안 구성 검사

```yaml
# security-config-check.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-security-config-check
spec:
  selector:
    matchLabels:
      app: aspnet-app
  rules:
  - from:
    - source:
        requestPrincipals: ["*"]
    when:
    - key: request.headers[x-security-check]
      values: ["true"]
    to:
    - operation:
        methods: ["*"]
```

### ASP.NET 보안 업데이트

```yaml
# security-updates.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-security-updates
spec:
  selector:
    matchLabels:
      app: aspnet-app
  rules:
  - from:
    - source:
        requestPrincipals: ["*"]
    when:
    - key: request.headers[x-security-version]
      values: ["latest"]
    to:
    - operation:
        methods: ["*"]
``` 