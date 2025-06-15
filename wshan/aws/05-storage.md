# AWS 스토리지 설정

## 1. EBS 볼륨 설정

### EBS CSI 드라이버 설치
```bash
# EBS CSI 드라이버 설치
kubectl apply -k "github.com/kubernetes-sigs/aws-ebs-csi-driver/deploy/kubernetes/overlays/stable/?ref=master"

# IAM 역할 설정
eksctl create iamserviceaccount \
  --name ebs-csi-controller-sa \
  --namespace kube-system \
  --cluster game-cluster \
  --attach-policy-arn arn:aws:iam::aws:policy/service-role/AmazonEBSCSIDriverPolicy \
  --approve
```

### 스토리지 클래스 설정
```yaml
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: game-server-storage
provisioner: ebs.csi.aws.com
parameters:
  type: gp3
  iopsPerGB: "3000"
  throughput: "125"
  encrypted: "true"
volumeBindingMode: WaitForFirstConsumer
```

### 퍼시스턴트 볼륨 클레임 설정
```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: game-data-pvc
spec:
  accessModes:
    - ReadWriteOnce
  storageClassName: game-server-storage
  resources:
    requests:
      storage: 100Gi
```

## 2. S3 버킷 설정

### S3 버킷 생성
```bash
# 버킷 생성
aws s3api create-bucket \
  --bucket game-server-data \
  --region ap-northeast-2 \
  --create-bucket-configuration LocationConstraint=ap-northeast-2

# 버킷 정책 설정
aws s3api put-bucket-policy \
  --bucket game-server-data \
  --policy '{
    "Version": "2012-10-17",
    "Statement": [{
      "Sid": "AllowGameServerAccess",
      "Effect": "Allow",
      "Principal": {
        "AWS": "arn:aws:iam::ACCOUNT_ID:role/game-server-role"
      },
      "Action": [
        "s3:GetObject",
        "s3:PutObject",
        "s3:ListBucket"
      ],
      "Resource": [
        "arn:aws:s3:::game-server-data",
        "arn:aws:s3:::game-server-data/*"
      ]
    }]
  }'
```

### 버킷 암호화 설정
```bash
# 버킷 암호화 설정
aws s3api put-bucket-encryption \
  --bucket game-server-data \
  --server-side-encryption-configuration '{
    "Rules": [{
      "ApplyServerSideEncryptionByDefault": {
        "SSEAlgorithm": "AES256"
      }
    }]
  }'
```

## 3. EFS 설정

### EFS 파일 시스템 생성
```bash
# EFS 파일 시스템 생성
aws efs create-file-system \
  --creation-token game-server-efs \
  --performance-mode generalPurpose \
  --throughput-mode bursting \
  --tags Key=Name,Value=game-server-efs

# 마운트 타겟 생성
aws efs create-mount-target \
  --file-system-id $EFS_ID \
  --subnet-id $SUBNET_ID \
  --security-groups $SG_ID
```

### EFS CSI 드라이버 설치
```bash
# EFS CSI 드라이버 설치
kubectl apply -k "github.com/kubernetes-sigs/aws-efs-csi-driver/deploy/kubernetes/overlays/stable/?ref=master"

# IAM 역할 설정
eksctl create iamserviceaccount \
  --name efs-csi-controller-sa \
  --namespace kube-system \
  --cluster game-cluster \
  --attach-policy-arn arn:aws:iam::aws:policy/service-role/AmazonEFSCSIDriverPolicy \
  --approve
```

### EFS 스토리지 클래스 설정
```yaml
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: game-server-efs
provisioner: efs.csi.aws.com
parameters:
  provisioningMode: efs-ap
  fileSystemId: $EFS_ID
  directoryPerms: "700"
```

## 4. 백업 전략

### EBS 스냅샷 설정
```bash
# 스냅샷 생성
aws ec2 create-snapshot \
  --volume-id $VOLUME_ID \
  --description "Game server data backup" \
  --tag-specifications 'ResourceType=snapshot,Tags=[{Key=Name,Value=game-server-backup}]'

# 스냅샷 복사
aws ec2 copy-snapshot \
  --source-region ap-northeast-2 \
  --source-snapshot-id $SNAPSHOT_ID \
  --destination-region ap-northeast-1 \
  --description "Cross-region backup"
```

### S3 버전 관리 설정
```bash
# 버전 관리 활성화
aws s3api put-bucket-versioning \
  --bucket game-server-data \
  --versioning-configuration Status=Enabled

# 수명 주기 정책 설정
aws s3api put-bucket-lifecycle-configuration \
  --bucket game-server-data \
  --lifecycle-configuration '{
    "Rules": [{
      "ID": "BackupRetention",
      "Status": "Enabled",
      "NoncurrentVersionExpiration": {
        "NoncurrentDays": 30
      }
    }]
  }'
```

## 5. 문제 해결

### EBS 볼륨 문제 해결
```bash
# 볼륨 상태 확인
aws ec2 describe-volumes \
  --volume-ids $VOLUME_ID

# 스냅샷 상태 확인
aws ec2 describe-snapshots \
  --snapshot-ids $SNAPSHOT_ID
```

### S3 버킷 문제 해결
```bash
# 버킷 설정 확인
aws s3api get-bucket-versioning \
  --bucket game-server-data

# 버킷 정책 확인
aws s3api get-bucket-policy \
  --bucket game-server-data
```

### EFS 문제 해결
```bash
# 파일 시스템 상태 확인
aws efs describe-file-systems \
  --file-system-id $EFS_ID

# 마운트 타겟 상태 확인
aws efs describe-mount-targets \
  --file-system-id $EFS_ID
``` 