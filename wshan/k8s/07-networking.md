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
### 5.1 서비스 DNS
- 서비스 이름: `<service-name>.<namespace>.svc.cluster.local`
- Pod DNS: `<pod-ip>.<namespace>.pod.cluster.local`

### 5.2 DNS 정책 설정
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