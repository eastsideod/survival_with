# AWS 보안 설정

## 1. IAM 설정

### IAM 역할 생성
```bash
# 게임 서버 역할 생성
aws iam create-role \
  --role-name game-server-role \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [{
      "Effect": "Allow",
      "Principal": {
        "Service": "ec2.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }]
  }'

# 정책 연결
aws iam attach-role-policy \
  --role-name game-server-role \
  --policy-arn arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly
```

### IAM 사용자 생성
```bash
# 사용자 생성
aws iam create-user \
  --user-name game-server-admin

# 액세스 키 생성
aws iam create-access-key \
  --user-name game-server-admin

# 사용자 정책 연결
aws iam attach-user-policy \
  --user-name game-server-admin \
  --policy-arn arn:aws:iam::aws:policy/AdministratorAccess
```

## 2. 네트워크 보안

### 보안 그룹 설정
```bash
# 보안 그룹 생성
aws ec2 create-security-group \
  --group-name game-server-sg \
  --description "Game server security group" \
  --vpc-id $VPC_ID

# 인바운드 규칙 설정
aws ec2 authorize-security-group-ingress \
  --group-id $SG_ID \
  --protocol tcp \
  --port 7000-8000 \
  --cidr 0.0.0.0/0

# 아웃바운드 규칙 설정
aws ec2 authorize-security-group-egress \
  --group-id $SG_ID \
  --protocol all \
  --port all \
  --cidr 0.0.0.0/0
```

### 네트워크 ACL 설정
```bash
# 네트워크 ACL 생성
aws ec2 create-network-acl \
  --vpc-id $VPC_ID \
  --tag-specifications 'ResourceType=network-acl,Tags=[{Key=Name,Value=game-server-acl}]'

# 인바운드 규칙 설정
aws ec2 create-network-acl-entry \
  --network-acl-id $ACL_ID \
  --rule-number 100 \
  --protocol tcp \
  --rule-action allow \
  --ingress \
  --port-range From=7000,To=8000 \
  --cidr-block 0.0.0.0/0
```

## 3. 데이터 암호화

### KMS 키 생성
```bash
# KMS 키 생성
aws kms create-key \
  --description "Game server encryption key" \
  --key-usage ENCRYPT_DECRYPT \
  --customer-master-key-spec SYMMETRIC_DEFAULT

# 키 별칭 설정
aws kms create-alias \
  --alias-name alias/game-server-key \
  --target-key-id $KEY_ID
```

### EBS 볼륨 암호화
```yaml
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: game-server-encrypted
provisioner: ebs.csi.aws.com
parameters:
  type: gp3
  encrypted: "true"
  kmsKeyId: $KEY_ID
```

## 4. WAF 설정

### WAF 웹 ACL 생성
```bash
# 웹 ACL 생성
aws wafv2 create-web-acl \
  --name game-server-waf \
  --scope REGIONAL \
  --default-action Allow={} \
  --visibility-config SampledRequestsEnabled=true,CloudWatchMetricsEnabled=true,MetricName=game-server-waf

# 규칙 추가
aws wafv2 create-rule-group \
  --name game-server-rules \
  --scope REGIONAL \
  --capacity 100 \
  --visibility-config SampledRequestsEnabled=true,CloudWatchMetricsEnabled=true,MetricName=game-server-rules
```

## 5. 인증 및 권한 부여

### OIDC 공급자 설정
```bash
# OIDC 공급자 생성
eksctl utils associate-iam-oidc-provider \
  --cluster game-cluster \
  --approve

# 서비스 계정 생성
eksctl create iamserviceaccount \
  --name game-server-sa \
  --namespace default \
  --cluster game-cluster \
  --attach-policy-arn arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryReadOnly \
  --approve
```

## 6. 문제 해결

### IAM 권한 확인
```bash
# 역할 정책 확인
aws iam list-attached-role-policies \
  --role-name game-server-role

# 사용자 정책 확인
aws iam list-attached-user-policies \
  --user-name game-server-admin
```

### 보안 그룹 확인
```bash
# 보안 그룹 규칙 확인
aws ec2 describe-security-groups \
  --group-ids $SG_ID

# 네트워크 ACL 규칙 확인
aws ec2 describe-network-acls \
  --network-acl-ids $ACL_ID
```

### 암호화 상태 확인
```bash
# KMS 키 상태 확인
aws kms describe-key \
  --key-id $KEY_ID

# EBS 볼륨 암호화 상태 확인
aws ec2 describe-volumes \
  --filters "Name=encrypted,Values=true"
``` 