# Istio 설치 및 설정

## 사전 요구사항

1. **Kubernetes 클러스터**
   - Kubernetes 1.16 이상
   - kubectl 설치
   - 클러스터 관리자 권한
   - ASP.NET Core 애플리케이션 배포 환경

2. **시스템 요구사항**
   - 최소 4GB RAM
   - 2개 이상의 CPU
   - 30GB 이상의 디스크 공간
   - ASP.NET Core 런타임 환경

## 설치 방법

### 1. Istio 다운로드

```bash
# 최신 버전 다운로드
curl -L https://istio.io/downloadIstio | sh -

# 환경 변수 설정
cd istio-*
export PATH=$PWD/bin:$PATH
```

### 2. 설치 프로필 선택

Istio는 ASP.NET 마이크로서비스에 적합한 여러 설치 프로필을 제공합니다:

- **default**: ASP.NET 프로덕션 환경에 적합
- **demo**: ASP.NET 데모/평가용
- **minimal**: ASP.NET 최소 구성
- **remote**: ASP.NET 멀티 클러스터 구성
- **empty**: ASP.NET 사용자 정의 구성

```bash
# 설치 프로필 확인
istioctl profile list

# 설치 프로필 상세 정보 확인
istioctl profile dump demo
```

### 3. 설치 실행

```bash
# 기본 프로필로 설치
istioctl install --set profile=default

# 설치 확인
istioctl verify-install
```

## ASP.NET 설정 구성

### 1. 사이드카 자동 주입 설정

```bash
# ASP.NET 애플리케이션 네임스페이스에 레이블 추가
kubectl label namespace default istio-injection=enabled

# 레이블 확인
kubectl get namespace -L istio-injection
```

### 2. ASP.NET 인그레스 게이트웨이 설정

```yaml
# ingress-gateway.yaml
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: aspnet-ingressgateway
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

### 3. ASP.NET 가상 서비스 설정

```yaml
# virtual-service.yaml
apiVersion: networking.istio.io/v1alpha3
kind: VirtualService
metadata:
  name: aspnet-service
spec:
  hosts:
  - "aspnet-app.example.com"
  gateways:
  - aspnet-ingressgateway
  http:
  - route:
    - destination:
        host: aspnet-service
        port:
          number: 80
```

## ASP.NET 보안 설정

### 1. mTLS 활성화

```bash
# ASP.NET 서비스 네임스페이스에 mTLS 활성화
kubectl apply -f - <<EOF
apiVersion: security.istio.io/v1beta1
kind: PeerAuthentication
metadata:
  name: aspnet-mtls
  namespace: default
spec:
  mtls:
    mode: STRICT
EOF
```

### 2. ASP.NET 권한 부여 정책 설정

```yaml
# authorization-policy.yaml
apiVersion: security.istio.io/v1beta1
kind: AuthorizationPolicy
metadata:
  name: aspnet-auth
  namespace: default
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

## ASP.NET 모니터링 설정

### 1. Prometheus 설정

```bash
# Prometheus 설치
kubectl apply -f https://raw.githubusercontent.com/istio/istio/release-1.16/samples/addons/prometheus.yaml
```

### 2. Grafana 설정

```bash
# Grafana 설치
kubectl apply -f https://raw.githubusercontent.com/istio/istio/release-1.16/samples/addons/grafana.yaml
```

### 3. Kiali 설정

```bash
# Kiali 설치
kubectl apply -f https://raw.githubusercontent.com/istio/istio/release-1.16/samples/addons/kiali.yaml
```

## ASP.NET 업그레이드

### 1. 버전 확인

```bash
# 현재 버전 확인
istioctl version

# 사용 가능한 버전 확인
istioctl x upgrade --versions
```

### 2. 업그레이드 실행

```bash
# 업그레이드 실행
istioctl upgrade
```

## ASP.NET 문제 해결

### 1. 설치 문제 해결

```bash
# 설치 상태 확인
istioctl verify-install

# 구성 확인
istioctl analyze
```

### 2. ASP.NET 사이드카 문제 해결

```bash
# 사이드카 상태 확인
kubectl get pods -n istio-system

# ASP.NET 애플리케이션 로그 확인
kubectl logs -n default -l app=aspnet-app
```

### 3. ASP.NET 네트워크 문제 해결

```bash
# 프록시 상태 확인
istioctl proxy-status

# ASP.NET 서비스 프록시 구성 확인
istioctl proxy-config
``` 