# Storage

## 1. 볼륨(Volume) 타입
### 1.1 emptyDir
- Pod와 생명주기를 함께하는 임시 볼륨
- Pod가 삭제되면 데이터도 삭제
- 컨테이너 간 데이터 공유에 사용

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: test-pd
spec:
  containers:
  - image: nginx
    name: test-container
    volumeMounts:
    - mountPath: /cache
      name: cache-volume
  volumes:
  - name: cache-volume
    emptyDir: {}
```

### 1.2 hostPath
- 노드의 파일시스템을 Pod에 마운트
- 노드의 데이터에 접근이 필요한 경우 사용
- 개발/테스트 환경에 적합

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: test-pd
spec:
  containers:
  - image: nginx
    name: test-container
    volumeMounts:
    - mountPath: /test-pd
      name: test-volume
  volumes:
  - name: test-volume
    hostPath:
      path: /data
      type: Directory
```

### 1.3 PersistentVolume (PV)
- 클러스터의 스토리지 리소스
- 관리자가 프로비저닝
- 다양한 스토리지 백엔드 지원

```yaml
apiVersion: v1
kind: PersistentVolume
metadata:
  name: pv-volume
spec:
  capacity:
    storage: 10Gi
  volumeMode: Filesystem
  accessModes:
    - ReadWriteOnce
  persistentVolumeReclaimPolicy: Retain
  storageClassName: slow
  hostPath:
    path: /mnt/data
```

### 1.4 PersistentVolumeClaim (PVC)
- 사용자의 스토리지 요청
- PV와 바인딩
- 동적 프로비저닝 지원

```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: pv-claim
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 3Gi
  storageClassName: slow
```

## 2. 스토리지 클래스(StorageClass)
### 2.1 기본 스토리지 클래스
```yaml
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: standard
provisioner: kubernetes.io/aws-ebs
parameters:
  type: gp2
reclaimPolicy: Retain
allowVolumeExpansion: true
```

### 2.2 동적 프로비저닝 예시
```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: dynamic-pvc
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 1Gi
  storageClassName: standard
```

## 3. 볼륨 사용 예시
### 3.1 PVC를 사용한 Pod
```yaml
apiVersion: v1
kind: Pod
metadata:
  name: pv-pod
spec:
  containers:
  - name: pv-container
    image: nginx
    volumeMounts:
    - mountPath: /usr/share/nginx/html
      name: pv-storage
  volumes:
  - name: pv-storage
    persistentVolumeClaim:
      claimName: pv-claim
```

### 3.2 StatefulSet에서의 볼륨 사용
```yaml
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: web
spec:
  serviceName: "nginx"
  replicas: 3
  selector:
    matchLabels:
      app: nginx
  template:
    metadata:
      labels:
        app: nginx
    spec:
      containers:
      - name: nginx
        image: nginx:1.14.2
        ports:
        - containerPort: 80
          name: web
        volumeMounts:
        - name: www
          mountPath: /usr/share/nginx/html
  volumeClaimTemplates:
  - metadata:
      name: www
    spec:
      accessModes: [ "ReadWriteOnce" ]
      resources:
        requests:
          storage: 1Gi
```

## 4. 관리 명령어
```bash
# PV 목록 확인
kubectl get pv

# PVC 목록 확인
kubectl get pvc

# 스토리지 클래스 목록 확인
kubectl get storageclass

# PV 상세 정보 확인
kubectl describe pv <pv-name>

# PVC 상세 정보 확인
kubectl describe pvc <pvc-name>
```

## 5. 문제 해결
### 5.1 일반적인 문제
- PVC가 바인딩되지 않는 경우
  - PV 용량 확인
  - 접근 모드 확인
  - 스토리지 클래스 확인

- 볼륨 마운트 실패
  - 노드 상태 확인
  - 스토리지 시스템 상태 확인
  - 권한 확인

### 5.2 디버깅 명령어
```bash
# Pod의 볼륨 마운트 상태 확인
kubectl describe pod <pod-name>

# PVC 이벤트 확인
kubectl describe pvc <pvc-name>

# PV 상태 확인
kubectl describe pv <pv-name>
```

## 6. 고급 기능
### 6.1 볼륨 스냅샷
```yaml
apiVersion: snapshot.storage.k8s.io/v1
kind: VolumeSnapshot
metadata:
  name: new-snapshot
spec:
  volumeSnapshotClassName: csi-hostpath-snapclass
  source:
    persistentVolumeClaimName: pvc-name
```

### 6.2 볼륨 확장
```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: pvc-name
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 10Gi
  storageClassName: standard
``` 