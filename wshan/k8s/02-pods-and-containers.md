# Pods와 Containers

## 1. Pod란?
- Kubernetes에서 가장 작은 배포 단위
- 하나 이상의 컨테이너를 포함하는 논리적 단위
- 동일한 노드에서 실행되며 리소스를 공유
- 고유한 IP 주소를 가짐

## 2. Pod의 특징
- 생명주기: 생성 -> 실행 -> 종료
- 일시적(ephemeral) - 재시작 시 새로운 Pod 생성
- 스케줄링의 기본 단위
- 네트워크와 스토리지 공유

## 3. Pod 정의 예시
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: nginx-pod
  labels:
    app: nginx
spec:
  containers:
  - name: nginx
    image: nginx:1.14.2
    ports:
    - containerPort: 80
    resources:
      limits:
        cpu: "0.5"
        memory: "512Mi"
      requests:
        cpu: "0.25"
        memory: "256Mi"
```

## 4. Multi-Container Pods
### 4.1 Sidecar 패턴
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: web-server
spec:
  containers:
  - name: nginx
    image: nginx:1.14.2
    ports:
    - containerPort: 80
  - name: log-collector
    image: fluentd
    volumeMounts:
    - name: logs
      mountPath: /var/log/nginx
  volumes:
  - name: logs
    emptyDir: {}
```

### 4.2 Ambassador 패턴
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: web-server
spec:
  containers:
  - name: web
    image: my-web-app
  - name: proxy
    image: envoy
    ports:
    - containerPort: 80
```

## 5. Pod 생명주기 관리
### 5.1 프로브(Probes)
- 컨테이너의 상태를 주기적으로 확인하는 메커니즘
- 컨테이너가 정상적으로 동작하는지 모니터링
- 세 가지 타입의 프로브 지원:
  - Liveness Probe: 컨테이너가 살아있는지 확인
  - Readiness Probe: 컨테이너가 요청을 처리할 준비가 되었는지 확인
  - Startup Probe: 컨테이너가 시작되었는지 확인

#### 5.1.1 Liveness Probe
- 컨테이너가 정상적으로 동작하는지 확인
- 실패 시 컨테이너 재시작
- 주요 사용 사례:
  - 데드락 상태 감지
  - 애플리케이션 응답 없음 감지
  - 무한 루프 상태 감지

```yaml
livenessProbe:
  httpGet:
    path: /healthz
    port: 8080
  initialDelaySeconds: 15
  periodSeconds: 20
  timeoutSeconds: 1
  successThreshold: 1
  failureThreshold: 3
```

#### 5.1.2 Readiness Probe
- 컨테이너가 서비스 요청을 처리할 준비가 되었는지 확인
- 실패 시 서비스 엔드포인트에서 제외
- 주요 사용 사례:
  - 애플리케이션 초기화 완료 확인
  - 외부 의존성 준비 상태 확인
  - 트래픽 처리 가능 상태 확인

```yaml
readinessProbe:
  exec:
    command:
    - cat
    - /tmp/healthy
  initialDelaySeconds: 5
  periodSeconds: 10
```

#### 5.1.3 Startup Probe
- 느리게 시작하는 컨테이너를 위한 프로브
- 다른 프로브가 시작되기 전에 완료되어야 함
- 주요 사용 사례:
  - 오래 걸리는 초기화 과정이 있는 경우
  - 레거시 애플리케이션의 시작 시간이 긴 경우

```yaml
startupProbe:
  tcpSocket:
    port: 8080
  failureThreshold: 30
  periodSeconds: 10
```

#### 5.1.4 프로브 설정 옵션
- initialDelaySeconds: 첫 번째 프로브 실행 전 대기 시간
- periodSeconds: 프로브 실행 간격
- timeoutSeconds: 프로브 타임아웃 시간
- successThreshold: 성공으로 판단하기 위한 연속 성공 횟수
- failureThreshold: 실패로 판단하기 위한 연속 실패 횟수

#### 5.1.5 프로브 타입
1. HTTP GET 프로브:
```yaml
httpGet:
  path: /healthz
  port: 8080
  httpHeaders:
  - name: Custom-Header
    value: Awesome
```

2. TCP 소켓 프로브:
```yaml
tcpSocket:
  port: 8080
```

3. Exec 프로브:
```yaml
exec:
  command:
  - cat
  - /tmp/healthy
```

#### 5.1.6 프로브 모니터링
```bash
# Pod 상태 확인
kubectl describe pod <pod-name>

# 컨테이너 로그 확인
kubectl logs <pod-name> -c <container-name>

# 프로브 이벤트 확인
kubectl get events --field-selector involvedObject.name=<pod-name>
```

## 6. Pod 관리 명령어
```bash
# Pod 생성
kubectl apply -f pod.yaml

# Pod 목록 확인
kubectl get pods

# Pod 상세 정보 확인
kubectl describe pod <pod-name>

# Pod 로그 확인
kubectl logs <pod-name>

# Pod 내부 접속
kubectl exec -it <pod-name> -- /bin/bash

# Pod 삭제
kubectl delete pod <pod-name>
```

## 7. 실습 예제
### 7.1 간단한 웹 서버 Pod
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: simple-web
spec:
  containers:
  - name: web
    image: nginx:1.14.2
    ports:
    - containerPort: 80
```

### 7.2 환경 변수가 있는 Pod
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: env-pod
spec:
  containers:
  - name: env-container
    image: busybox
    command: ["/bin/sh", "-c", "env"]
    env:
    - name: ENV_VAR
      value: "test-value"
```

## 8. 문제 해결
- Pod가 Pending 상태로 있는 경우
  - 리소스 부족 확인
  - 노드 상태 확인
  - 스케줄링 제약 확인

- Pod가 CrashLoopBackOff 상태인 경우
  - 로그 확인
  - 리소스 제한 확인
  - 프로브 설정 확인 