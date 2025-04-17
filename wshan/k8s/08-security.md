# Security

## 1. 인증(Authentication)
### 1.1 사용자 인증
- X.509 클라이언트 인증서
- 정적 토큰 파일
- OpenID Connect
- 웹훅 토큰 인증

### 1.2 서비스 어카운트
```yaml
apiVersion: v1
kind: ServiceAccount
metadata:
  name: my-service-account
```

## 2. 권한 부여(Authorization)
### 2.1 RBAC (Role-Based Access Control)
#### 2.1.1 Role
```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: Role
metadata:
  namespace: default
  name: pod-reader
rules:
- apiGroups: [""]
  resources: ["pods"]
  verbs: ["get", "watch", "list"]
```

#### 2.1.2 RoleBinding
```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: RoleBinding
metadata:
  name: read-pods
  namespace: default
subjects:
- kind: User
  name: jane
  apiGroup: rbac.authorization.k8s.io
roleRef:
  kind: Role
  name: pod-reader
  apiGroup: rbac.authorization.k8s.io
```

#### 2.1.3 ClusterRole
```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRole
metadata:
  name: secret-reader
rules:
- apiGroups: [""]
  resources: ["secrets"]
  verbs: ["get", "watch", "list"]
```

#### 2.1.4 ClusterRoleBinding
```yaml
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: read-secrets-global
subjects:
- kind: Group
  name: manager
  apiGroup: rbac.authorization.k8s.io
roleRef:
  kind: ClusterRole
  name: secret-reader
  apiGroup: rbac.authorization.k8s.io
```

## 3. 시크릿 관리
### 3.1 Secret 생성
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: mysecret
type: Opaque
data:
  username: YWRtaW4=
  password: MWYyZDFlMmU2N2Rm
```

### 3.2 Secret 사용
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: secret-test-pod
spec:
  containers:
  - name: test-container
    image: nginx
    volumeMounts:
    - name: secret-volume
      mountPath: /etc/secret
  volumes:
  - name: secret-volume
    secret:
      secretName: mysecret
```

## 4. Pod 보안
### 4.1 SecurityContext
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: security-context-demo
spec:
  securityContext:
    runAsUser: 1000
    runAsGroup: 3000
    fsGroup: 2000
  containers:
  - name: sec-ctx-demo
    image: busybox
    command: ["sh", "-c", "sleep 1h"]
    securityContext:
      allowPrivilegeEscalation: false
      capabilities:
        add: ["NET_ADMIN", "SYS_TIME"]
```

### 4.2 PodSecurityPolicy (Deprecated)
```yaml
apiVersion: policy/v1beta1
kind: PodSecurityPolicy
metadata:
  name: restricted
spec:
  privileged: false
  allowPrivilegeEscalation: false
  requiredDropCapabilities:
    - ALL
  volumes:
    - 'configMap'
    - 'emptyDir'
    - 'projected'
    - 'secret'
    - 'downwardAPI'
    - 'persistentVolumeClaim'
  hostNetwork: false
  hostIPC: false
  hostPID: false
  runAsUser:
    rule: 'MustRunAsNonRoot'
  seLinux:
    rule: 'RunAsAny'
  supplementalGroups:
    rule: 'MustRunAs'
    ranges:
      - min: 1
        max: 65535
  fsGroup:
    rule: 'MustRunAs'
    ranges:
      - min: 1
        max: 65535
```

## 5. 네트워크 정책
### 5.1 기본 네트워크 정책
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: default-deny
spec:
  podSelector: {}
  policyTypes:
  - Ingress
  - Egress
```

### 5.2 특정 Pod에 대한 네트워크 정책
```yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: db-policy
spec:
  podSelector:
    matchLabels:
      role: db
  policyTypes:
  - Ingress
  ingress:
  - from:
    - podSelector:
        matchLabels:
          role: frontend
    ports:
    - protocol: TCP
      port: 6379
```

## 6. 관리 명령어
```bash
# RBAC 리소스 확인
kubectl get roles
kubectl get rolebindings
kubectl get clusterroles
kubectl get clusterrolebindings

# 서비스 어카운트 확인
kubectl get serviceaccounts

# 시크릿 확인
kubectl get secrets

# 네트워크 정책 확인
kubectl get networkpolicies
```

## 7. 문제 해결
### 7.1 일반적인 문제
- 권한 부여 실패
  - RBAC 설정 확인
  - 사용자/그룹 확인
  - 네임스페이스 확인

- 시크릿 접근 실패
  - 시크릿 존재 확인
  - 권한 확인
  - 마운트 경로 확인

### 7.2 디버깅 명령어
```bash
# RBAC 권한 확인
kubectl auth can-i create pods
kubectl auth can-i delete pods --as system:serviceaccount:default:my-sa

# 시크릿 내용 확인
kubectl get secret mysecret -o jsonpath='{.data}'

# 네트워크 정책 로그 확인
kubectl logs -n kube-system -l k8s-app=cilium
```

## 8. 보안 모범 사례
### 8.1 일반적인 보안 지침
- 최소 권한 원칙 적용
- 정기적인 보안 업데이트
- 시크릿 관리 강화
- 네트워크 격리 구현

### 8.2 컨테이너 보안
- 비특권 사용자로 실행
- 필요한 기능만 허용
- 이미지 서명 검증
- 취약점 스캔 