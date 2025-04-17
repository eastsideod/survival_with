# ConfigMaps와 Secrets

## 1. ConfigMap
### 1.1 ConfigMap이란?
- 설정 데이터를 저장하는 API 오브젝트
- 환경 변수, 명령줄 인수, 설정 파일 등에 사용
- 민감하지 않은 데이터 저장

### 1.2 ConfigMap 생성 방법
#### 1.2.1 명령어로 생성
```bash
# 리터럴 값으로 생성
kubectl create configmap my-config --from-literal=key1=value1 --from-literal=key2=value2

# 파일로부터 생성
kubectl create configmap my-config --from-file=config.properties

# 디렉토리로부터 생성
kubectl create configmap my-config --from-file=config-dir/
```

#### 1.2.2 YAML로 생성
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: game-config
data:
  game.properties: |
    enemy.types=aliens,monsters
    player.maximum-lives=5
  ui.properties: |
    color.good=purple
    color.bad=yellow
```

### 1.3 ConfigMap 사용 방법
#### 1.3.1 환경 변수로 사용
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: configmap-env-pod
spec:
  containers:
  - name: test-container
    image: busybox
    command: ["/bin/sh", "-c", "env"]
    envFrom:
    - configMapRef:
        name: game-config
```

#### 1.3.2 볼륨으로 마운트
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: configmap-volume-pod
spec:
  containers:
  - name: test-container
    image: busybox
    command: ["/bin/sh", "-c", "cat /etc/config/game.properties"]
    volumeMounts:
    - name: config-volume
      mountPath: /etc/config
  volumes:
  - name: config-volume
    configMap:
      name: game-config
```

## 2. Secret
### 2.1 Secret이란?
- 민감한 정보를 저장하는 API 오브젝트
- 비밀번호, OAuth 토큰, SSH 키 등 저장
- base64로 인코딩되어 저장

### 2.2 Secret 생성 방법
#### 2.2.1 명령어로 생성
```bash
# 리터럴 값으로 생성
kubectl create secret generic my-secret --from-literal=username=admin --from-literal=password=secret

# 파일로부터 생성
kubectl create secret generic my-secret --from-file=./username.txt --from-file=./password.txt
```

#### 2.2.2 YAML로 생성
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: my-secret
type: Opaque
data:
  username: YWRtaW4=  # base64로 인코딩된 'admin'
  password: c2VjcmV0  # base64로 인코딩된 'secret'
```

### 2.3 Secret 사용 방법
#### 2.3.1 환경 변수로 사용
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: secret-env-pod
spec:
  containers:
  - name: test-container
    image: busybox
    command: ["/bin/sh", "-c", "env"]
    envFrom:
    - secretRef:
        name: my-secret
```

#### 2.3.2 볼륨으로 마운트
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: secret-volume-pod
spec:
  containers:
  - name: test-container
    image: busybox
    command: ["/bin/sh", "-c", "cat /etc/secret/username"]
    volumeMounts:
    - name: secret-volume
      mountPath: /etc/secret
  volumes:
  - name: secret-volume
    secret:
      secretName: my-secret
```

## 3. 관리 명령어
```bash
# ConfigMap 목록 확인
kubectl get configmaps

# ConfigMap 상세 정보 확인
kubectl describe configmap <configmap-name>

# Secret 목록 확인
kubectl get secrets

# Secret 상세 정보 확인
kubectl describe secret <secret-name>

# ConfigMap/Secret 삭제
kubectl delete configmap <configmap-name>
kubectl delete secret <secret-name>
```

## 4. 실습 예제
### 4.1 ConfigMap을 사용한 애플리케이션 설정
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
data:
  APP_COLOR: blue
  APP_MODE: prod
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: webapp
spec:
  template:
    spec:
      containers:
      - name: webapp
        image: nginx
        envFrom:
        - configMapRef:
            name: app-config
```

### 4.2 Secret을 사용한 데이터베이스 연결
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: db-secret
type: Opaque
data:
  username: ZGI=
  password: cGFzc3dvcmQ=
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: db-app
spec:
  template:
    spec:
      containers:
      - name: db-app
        image: mysql
        envFrom:
        - secretRef:
            name: db-secret
```

## 5. 문제 해결
### 5.1 일반적인 문제
- ConfigMap/Secret이 적용되지 않는 경우
  - 이름 확인
  - 네임스페이스 확인
  - 권한 확인

- 환경 변수가 설정되지 않는 경우
  - ConfigMap/Secret 존재 확인
  - 키 이름 확인
  - 포맷 확인

### 5.2 디버깅 명령어
```bash
# Pod의 환경 변수 확인
kubectl exec -it <pod-name> -- env

# 마운트된 ConfigMap/Secret 확인
kubectl exec -it <pod-name> -- ls /etc/config
kubectl exec -it <pod-name> -- ls /etc/secret
``` 