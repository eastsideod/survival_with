# Services

## 1. Service란?
- Pod 집합에 대한 네트워크 엔드포인트 제공
- Pod의 IP가 변경되어도 안정적인 접근 제공
- 로드 밸런싱 기능
- 서비스 디스커버리 지원

### 1.1 Service 포트 매핑
- port: 서비스가 노출하는 포트 (클라이언트가 접근하는 포트)
- targetPort: Pod에서 실제로 실행 중인 애플리케이션의 포트
- nodePort: NodePort 타입에서 노드에 노출되는 포트 (30000-32767 범위)

예시:
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
    - name: http
      protocol: TCP
      port: 80        # 서비스 포트
      targetPort: 8080 # Pod의 포트
```

포트 매핑 동작 방식:
1. 클라이언트가 서비스의 port(80)로 요청
2. 서비스가 요청을 받아서 selector와 매칭되는 Pod 중 하나로 전달
3. Pod의 targetPort(8080)로 요청이 전달됨

주의사항:
- port와 targetPort가 같을 수 있음
- targetPort는 Pod의 컨테이너 포트와 일치해야 함
- port는 서비스 타입에 따라 제한이 있을 수 있음 (예: NodePort는 30000-32767)

## 2. Service 타입
### 2.1 ClusterIP
- 기본 서비스 타입
- 클러스터 내부에서만 접근 가능
- Pod 간 통신에 사용

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
- 클러스터 외부에서 접근 가능
- 모든 노드의 특정 포트로 접근
- 개발/테스트 환경에 적합

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

## 3. 서비스 관리 명령어
```bash
# 서비스 생성
kubectl apply -f service.yaml

# 서비스 목록 확인
kubectl get services

# 서비스 상세 정보 확인
kubectl describe service <service-name>

# 서비스 삭제
kubectl delete service <service-name>

# 서비스 엔드포인트 확인
kubectl get endpoints <service-name>
```

### 3.1 엔드포인트 관리 명령어
#### 3.1.1 기본 엔드포인트 명령어
```bash
# 모든 엔드포인트 목록 확인
kubectl get endpoints

# 특정 서비스의 엔드포인트 확인
kubectl get endpoints <service-name>

# 엔드포인트 상세 정보 확인
kubectl describe endpoints <service-name>

# 엔드포인트 YAML 형식으로 확인
kubectl get endpoints <service-name> -o yaml

# 엔드포인트 JSON 형식으로 확인
kubectl get endpoints <service-name> -o json
```

#### 3.1.2 엔드포인트 필터링
```bash
# 특정 네임스페이스의 엔드포인트 확인
kubectl get endpoints -n <namespace>

# 레이블로 엔드포인트 필터링
kubectl get endpoints -l app=my-app

# 특정 필드로 엔드포인트 필터링
kubectl get endpoints --field-selector metadata.namespace=default
```

#### 3.1.3 엔드포인트 모니터링
```bash
# 엔드포인트 변경사항 실시간 모니터링
kubectl get endpoints -w

# 특정 서비스의 엔드포인트 변경사항 모니터링
kubectl get endpoints <service-name> -w

# 엔드포인트 이벤트 확인
kubectl describe endpoints <service-name>
```

#### 3.1.4 엔드포인트 문제 해결
```bash
# 엔드포인트가 없는 서비스 확인
kubectl get services --field-selector spec.selector=null

# 엔드포인트와 서비스 셀렉터 불일치 확인
kubectl get endpoints <service-name> -o yaml | grep -A 5 "subsets:"

# 엔드포인트 상태 확인
kubectl get endpoints <service-name> -o wide
```

#### 3.1.5 엔드포인트 수동 관리
```bash
# 엔드포인트 수동 생성/업데이트
kubectl apply -f endpoint.yaml

# 엔드포인트 수동 삭제
kubectl delete endpoints <service-name>

# 엔드포인트 수동 편집
kubectl edit endpoints <service-name>
```

#### 3.1.6 엔드포인트 예시 YAML
```yaml
apiVersion: v1
kind: Endpoints
metadata:
  name: my-service
subsets:
- addresses:
  - ip: 10.244.1.2
    targetRef:
      kind: Pod
      name: my-pod-1
      namespace: default
  - ip: 10.244.1.3
    targetRef:
      kind: Pod
      name: my-pod-2
      namespace: default
  ports:
  - port: 80
    protocol: TCP
```

## 4. 실습 예제
### 4.1 다중 포트 서비스
```yaml
apiVersion: v1
kind: Service
metadata:
  name: multi-port-service
spec:
  selector:
    app: my-app
  ports:
    - name: http
      protocol: TCP
      port: 80
      targetPort: 9376
    - name: https
      protocol: TCP
      port: 443
      targetPort: 9377
```

### 4.2 세션 어피니티가 있는 서비스
```yaml
apiVersion: v1
kind: Service
metadata:
  name: session-affinity-service
spec:
  selector:
    app: my-app
  ports:
    - port: 80
      targetPort: 9376
  sessionAffinity: ClientIP
  sessionAffinityConfig:
    clientIP:
      timeoutSeconds: 10800
```

## 5. 문제 해결
### 5.1 일반적인 문제
- 서비스에 연결할 수 없는 경우
  - Pod 레이블 확인
  - 서비스 셀렉터 확인
  - 네트워크 정책 확인

- 로드 밸런싱이 작동하지 않는 경우
  - 엔드포인트 확인
  - Pod 상태 확인
  - 네트워크 설정 확인

### 5.2 디버깅 명령어
```bash
# 서비스 엔드포인트 확인
kubectl get endpoints <service-name>

# 서비스 DNS 확인
kubectl exec -it <pod-name> -- nslookup <service-name>

# 네트워크 연결 테스트
kubectl exec -it <pod-name> -- curl <service-name>:<port>
```

## 6. 고급 기능
### 6.1 헤드리스 서비스
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

### 6.2 외부 IP 서비스
```yaml
apiVersion: v1
kind: Service
metadata:
  name: external-ip-service
spec:
  selector:
    app: my-app
  ports:
    - port: 80
      targetPort: 9376
  externalIPs:
    - 80.11.12.10
``` 