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
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: nginx
spec:
  containers:
  - name: nginx
    image: nginx:1.14.2
    livenessProbe:
      httpGet:
        path: /
        port: 80
      initialDelaySeconds: 15
      periodSeconds: 20
    readinessProbe:
      httpGet:
        path: /
        port: 80
      initialDelaySeconds: 5
      periodSeconds: 10
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