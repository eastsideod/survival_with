# Networking

## 1. Kubernetes 네트워크 모델
### 1.1 기본 원칙
- 모든 Pod는 IP 주소를 가짐
- Pod 간 직접 통신 가능
- 노드 간 Pod 통신 가능
- 서비스 디스커버리 지원

### 1.2 네트워크 플러그인
- CNI(Container Network Interface) 기반
- 다양한 구현체 존재
  - Calico
  - Flannel
  - Weave Net
  - Cilium

#### 1.2.1 CNI란?
- 컨테이너 네트워크 인터페이스 표준
- 컨테이너 런타임과 네트워크 플러그인 간의 통신 규약
- 주요 기능:
  - 컨테이너 네트워크 네임스페이스 생성
  - 네트워크 인터페이스 추가/제거
  - IP 주소 할당
  - 라우팅 설정

#### 1.2.2 CNI 동작 방식
1. 컨테이너 생성 시:
   - 컨테이너 런타임이 CNI 플러그인 호출
   - 네트워크 네임스페이스 생성
   - 네트워크 인터페이스 추가
   - IP 주소 할당

2. 컨테이너 삭제 시:
   - 컨테이너 런타임이 CNI 플러그인 호출
   - 네트워크 인터페이스 제거
   - IP 주소 반환
   - 네트워크 네임스페이스 정리

#### 1.2.3 주요 CNI 플러그인
1. Calico:
   - BGP 기반 네트워킹
   - 네트워크 정책 지원
   - 고성능 데이터 플레인
   - 클라우드 네이티브 환경에 적합

2. Flannel:
   - 단순한 오버레이 네트워크
   - VXLAN, host-gw 등 다양한 백엔드 지원
   - 설정이 간단하고 가벼움
   - 소규모 클러스터에 적합

3. Weave Net:
   - 멀티클라우드 지원
   - 자동 IP 주소 관리
   - 내장 DNS 서비스
   - 암호화된 네트워크 통신

4. Cilium:
   - eBPF 기반 네트워킹
   - 고급 보안 기능
   - 서비스 메시 통합
   - 고성능 관측성

#### 1.2.4 CNI 설정 예시
```json
{
  "cniVersion": "0.4.0",
  "name": "mynet",
  "type": "bridge",
  "bridge": "cni0",
  "isGateway": true,
  "ipMasq": true,
  "ipam": {
    "type": "host-local",
    "subnet": "10.22.0.0/16",
    "routes": [
      { "dst": "0.0.0.0/0" }
    ]
  }
}
```

#### 1.2.5 CNI 관리 명령어
```bash
# CNI 플러그인 설치 확인
ls /opt/cni/bin/

# CNI 설정 확인
ls /etc/cni/net.d/

# 네트워크 인터페이스 확인
ip link show

# 라우팅 테이블 확인
ip route show

# CNI 플러그인 로그 확인
journalctl -u kubelet | grep cni
```

#### 1.2.6 CNI 문제 해결
1. 일반적인 문제:
   - Pod 네트워크 연결 실패
   - IP 주소 할당 실패
   - 라우팅 문제
   - 네트워크 정책 적용 실패

2. 디버깅 명령어:
```bash
# Pod 네트워크 네임스페이스 확인
kubectl exec -it <pod-name> -- ip addr

# CNI 플러그인 로그 확인
kubectl logs -n kube-system -l k8s-app=kube-dns

# 네트워크 정책 확인
kubectl get networkpolicies --all-namespaces

# CNI 플러그인 상태 확인
kubectl get pods -n kube-system -l k8s-app=kube-dns
```

### 1.3 VPC (Virtual Private Cloud)
- 클라우드 환경에서의 가상 네트워크 격리
- Kubernetes 클러스터의 네트워크 격리와 보안 제공
- 주요 기능:
  - 서브넷 관리
  - 라우팅 테이블 설정
  - 보안 그룹 설정
  - NAT 게이트웨이 구성
- 클라우드 제공자별 VPC 구현:
  - AWS VPC
  - GCP VPC
  - Azure VNet

### 1.4 kube-proxy
- Kubernetes 클러스터의 네트워크 프록시 컴포넌트
- 각 노드에서 실행되며 서비스의 네트워크 통신 관리
- 주요 기능:
  - 서비스 IP와 Pod IP 간의 매핑
  - 로드 밸런싱
  - 네트워크 정책 적용
  - 서비스 디스커버리 지원

#### 1.4.1 kube-proxy 동작 모드
1. userspace 모드:
   - 초기 구현 방식
   - 사용자 공간에서 프록시 처리
   - 성능이 낮지만 안정적
   - 현재는 거의 사용되지 않음

2. iptables 모드:
   - 기본 동작 모드
   - 리눅스 커널의 iptables 사용
   - 높은 성능과 낮은 지연시간
   - 규칙이 많아질 경우 성능 저하 가능

3. IPVS 모드:
   - 고성능 로드 밸런싱 지원
   - 다양한 로드 밸런싱 알고리즘 제공
   - 대규모 클러스터에 적합
   - 리눅스 커널의 IPVS 사용

#### 1.4.2 kube-proxy 설정
```yaml
# kube-proxy 설정 예시
apiVersion: kubeproxy.config.k8s.io/v1alpha1
kind: KubeProxyConfiguration
mode: "ipvs"
ipvs:
  scheduler: "rr"  # round-robin
  minSyncPeriod: 5s
  syncPeriod: 30s
  excludeCIDRs: []
```

#### 1.4.3 kube-proxy 관리 명령어
```bash
# kube-proxy 상태 확인
kubectl get pods -n kube-system -l k8s-app=kube-proxy

# kube-proxy 로그 확인
kubectl logs -n kube-system -l k8s-app=kube-proxy

# iptables 규칙 확인 (iptables 모드)
iptables -L -t nat

# IPVS 규칙 확인 (IPVS 모드)
ipvsadm -L
```

#### 1.4.4 kube-proxy 문제 해결
1. 일반적인 문제:
   - 서비스 연결 실패
   - 로드 밸런싱 불균형
   - 네트워크 정책 적용 실패
   - 성능 저하

2. 디버깅 명령어:
```bash
# kube-proxy 상태 확인
kubectl describe pod -n kube-system -l k8s-app=kube-proxy

# 서비스 엔드포인트 확인
kubectl get endpoints <service-name>

# 네트워크 연결 테스트
kubectl exec -it <pod-name> -- curl <service-name>:<port>

# iptables 규칙 디버깅
iptables -L -t nat -v
```

#### 1.4.5 kube-proxy 최적화
1. 모드 선택:
   - 소규모 클러스터: iptables 모드
   - 대규모 클러스터: IPVS 모드
   - 특수한 요구사항: userspace 모드

2. 성능 튜닝:
   - 동기화 주기 조정
   - 규칙 정리 주기 설정
   - 메모리 사용량 제한
   - CPU 리소스 할당

3. 모니터링:
   - 메트릭 수집
   - 로그 레벨 설정
   - 성능 메트릭 추적
   - 리소스 사용량 모니터링

## 2. 서비스 네트워킹
### 2.1 ClusterIP
```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  type: ClusterIP
  selector:
    app: my-app
  ports:
    - protocol: TCP
      port: 80
      targetPort: 9376
```

### 2.2 NodePort
```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  type: NodePort
  selector:
    app: my-app
  ports:
    - port: 80
      targetPort: 9376
      nodePort: 30007
```

### 2.3 LoadBalancer
- 클라우드 제공자의 로드 밸런서와 연동
- 외부 트래픽을 서비스로 전달
- 프로덕션 환경에 적합

```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  type: LoadBalancer
  selector:
    app: my-app
  ports:
    - port: 80
      targetPort: 9376
```

### 2.4 ExternalName
- 외부 서비스를 클러스터 내부에서 참조
- DNS CNAME 레코드 생성
- 클러스터 내부에서 외부 서비스에 접근할 때 사용
- 주요 사용 사례:
  - 외부 데이터베이스 서비스 연결
  - 다른 클러스터의 서비스 연결
  - 클라우드 제공자의 관리형 서비스 연결

```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  type: ExternalName
  externalName: my.database.example.com
```

### 2.5 ExternalName 서비스의 동작 방식
1. DNS 해결 과정:
   - 클러스터 내부에서 `my-service.default.svc.cluster.local`로 요청
   - CoreDNS가 CNAME 레코드를 확인
   - `my.database.example.com`으로 리다이렉트
   - 외부 DNS 서버에서 실제 IP 주소 확인

2. 특징:
   - 실제 IP 주소를 가지지 않음
   - DNS 레벨에서의 리다이렉션만 수행
   - 서비스 셀렉터(selector)가 없음
   - 포트 정의가 필요 없음

3. 사용 예시:
```yaml
# 외부 데이터베이스 연결
apiVersion: v1
kind: Service
metadata:
  name: external-db
spec:
  type: ExternalName
  externalName: database.prod.example.com

# 다른 클러스터의 서비스 연결
apiVersion: v1
kind: Service
metadata:
  name: other-cluster-service
spec:
  type: ExternalName
  externalName: service.other-cluster.svc.cluster.local

# 클라우드 제공자 서비스 연결
apiVersion: v1
kind: Service
metadata:
  name: cloud-storage
spec:
  type: ExternalName
  externalName: storage.googleapis.com
```

4. 제한사항:
   - TLS/SSL 인증서 검증 문제 발생 가능
   - DNS 캐싱으로 인한 지연 발생 가능
   - 외부 서비스의 IP 변경 시 DNS TTL 고려 필요
   - 네트워크 정책으로 인한 접근 제한 가능

5. 모니터링과 문제 해결:
```bash
# ExternalName 서비스 확인
kubectl get service external-db -o wide

# DNS 해결 테스트
kubectl exec -it <pod-name> -- nslookup external-db

# DNS 캐시 확인
kubectl exec -it <pod-name> -- cat /etc/resolv.conf

# 연결 테스트
kubectl exec -it <pod-name> -- curl -v external-db
```

## 3. Ingress
### 3.1 기본 Ingress
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: minimal-ingress
spec:
  rules:
  - host: foo.bar.com
    http:
      paths:
      - path: /testpath
        pathType: Prefix
        backend:
          service:
            name: test
            port:
              number: 80
```

### 3.2 TLS 설정
```yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: tls-example-ingress
spec:
  tls:
  - hosts:
    - https-example.foo.com
    secretName: testsecret-tls
  rules:
  - host: https-example.foo.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: service1
            port:
              number: 80
```

## 4. 네트워크 정책
### 4.1 기본 네트워크 정책
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: test-network-policy
spec:
  podSelector:
    matchLabels:
      role: db
  policyTypes:
  - Ingress
  - Egress
  ingress:
  - from:
    - ipBlock:
        cidr: 172.17.0.0/16
        except:
        - 172.17.1.0/24
    - namespaceSelector:
        matchLabels:
          project: myproject
    - podSelector:
        matchLabels:
          role: frontend
    ports:
    - protocol: TCP
      port: 6379
  egress:
  - to:
    - ipBlock:
        cidr: 10.0.0.0/24
    ports:
    - protocol: TCP
      port: 5978
```

## 5. DNS
### 5.1 서비스 디스커버리와 DNS
- Kubernetes에서 서비스 디스커버리의 기본 방식
- Pod나 Service에 접근하는 방법:
  - Service 이름으로 접근: `my-service.default.svc.cluster.local`
  - Pod 이름으로 접근: `pod-ip.default.pod.cluster.local`
  - 간단한 형식: `my-service` (같은 네임스페이스 내에서)
- DNS 레코드 타입:
  - A 레코드: 서비스 IP 주소
  - SRV 레코드: 서비스 포트 정보
  - PTR 레코드: 역방향 DNS 조회

### 5.2 DNS 기반 서비스 접근 예시
```bash
# 같은 네임스페이스 내에서 서비스 접근
curl http://my-service

# 다른 네임스페이스의 서비스 접근
curl http://my-service.other-namespace

# 전체 도메인으로 접근
curl http://my-service.default.svc.cluster.local

# Pod에 직접 접근 (StatefulSet의 경우)
curl http://web-0.nginx.default.svc.cluster.local
```

### 5.3 DNS 정책 설정
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: dns-example
spec:
  containers:
    - name: test
      image: nginx
  dnsPolicy: "ClusterFirst"
  dnsConfig:
    nameservers:
      - 1.2.3.4
    searches:
      - ns1.svc.cluster-domain.example
      - my.dns.search.suffix
    options:
      - name: ndots
        value: "2"
      - name: edns0
```

### 5.4 DNS 정책 타입
- ClusterFirst: 클러스터 DNS를 우선 사용
  - 클러스터 내부 도메인 쿼리는 CoreDNS로 전달
  - 외부 도메인 쿼리는 업스트림 DNS로 전달
- ClusterFirstWithHostNet: 호스트 네트워크 사용 시에도 클러스터 DNS 우선
  - 호스트 네트워크를 사용하는 Pod에 적합
  - 호스트의 DNS 설정을 유지하면서 클러스터 DNS 사용
- Default: Pod의 DNS 설정을 상속
  - Pod가 실행되는 노드의 DNS 설정 사용
  - kubelet의 --resolv-conf 파라미터로 설정
- None: DNS 설정을 수동으로 구성
  - dnsConfig 필드를 통해 커스텀 DNS 설정
  - 고급 DNS 설정이 필요한 경우 사용

### 5.5 CoreDNS
- Kubernetes의 기본 DNS 서버
- 주요 기능:
  - 서비스 디스커버리
  - DNS 캐싱
  - 외부 DNS 연동
  - DNS 정책 적용
- 설정 예시:
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: coredns
  namespace: kube-system
data:
  Corefile: |
    .:53 {
        errors
        health
        ready
        kubernetes cluster.local in-addr.arpa ip6.arpa {
           pods insecure
           fallthrough in-addr.arpa ip6.arpa
        }
        prometheus :9153
        forward . /etc/resolv.conf
        cache 30
        loop
        reload
        loadbalance
    }
```

### 5.6 DNS 해결 프로세스
1. Pod 내부 DNS 쿼리 발생
2. CoreDNS 서버로 쿼리 전달
3. CoreDNS의 처리 순서:
   - 캐시 확인
   - Kubernetes API 확인 (서비스/Pod 정보)
   - 업스트림 DNS 서버로 전달
4. 결과 반환 및 캐싱

### 5.7 DNS 최적화
- DNS 캐싱 설정
  - TTL 값 조정
  - 캐시 크기 설정
- 업스트림 DNS 서버 설정
  - 여러 DNS 서버 설정
  - 타임아웃 설정
- DNS 쿼리 최적화
  - ndots 설정 조정
  - search 도메인 최적화

### 5.8 DNS 문제 해결
- DNS 쿼리 테스트
```bash
# Pod 내부에서 DNS 쿼리 테스트
kubectl exec -it <pod-name> -- nslookup <service-name>

# CoreDNS 로그 확인
kubectl logs -n kube-system -l k8s-app=kube-dns

# DNS 설정 확인
kubectl exec -it <pod-name> -- cat /etc/resolv.conf
```

## 6. 관리 명령어
```bash
# 서비스 목록 확인
kubectl get services

# Ingress 목록 확인
kubectl get ingress

# 네트워크 정책 목록 확인
kubectl get networkpolicies

# DNS 확인
kubectl exec -it <pod-name> -- nslookup <service-name>

# 네트워크 연결 테스트
kubectl exec -it <pod-name> -- curl <service-name>:<port>
```

## 7. 문제 해결
### 7.1 일반적인 문제
- 서비스에 연결할 수 없는 경우
  - 서비스 상태 확인
  - 엔드포인트 확인
  - 네트워크 정책 확인

- DNS 해결 실패
  - CoreDNS 상태 확인
  - 네트워크 정책 확인
  - DNS 설정 확인

### 7.2 디버깅 명령어
```bash
# 서비스 엔드포인트 확인
kubectl get endpoints <service-name>

# Pod 네트워크 정보 확인
kubectl exec -it <pod-name> -- ip addr

# 네트워크 정책 로그 확인
kubectl logs -n kube-system -l k8s-app=cilium
```

## 8. 고급 기능
### 8.1 헤드리스 서비스
```yaml
apiVersion: v1
kind: Service
metadata:
  name: headless-service
spec:
  clusterIP: None
  selector:
    app: my-app
  ports:
    - port: 80
      targetPort: 9376
```

### 8.2 ExternalName 서비스
```yaml
apiVersion: v1
kind: Service
metadata:
  name: external-service
spec:
  type: ExternalName
  externalName: my.database.example.com
``` 