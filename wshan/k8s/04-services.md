# Services

## 1. Service란?
- Pod 집합에 대한 네트워크 엔드포인트 제공
- Pod의 IP가 변경되어도 안정적인 접근 제공
- 로드 밸런싱 기능
- 서비스 디스커버리 지원

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

```yaml
apiVersion: v1
kind: Service
metadata:
  name: my-service
spec:
  type: ExternalName
  externalName: my.database.example.com
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