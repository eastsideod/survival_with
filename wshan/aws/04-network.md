# AWS 네트워크 설정

## 1. VPC 구성

### VPC 생성
```bash
# VPC 생성
aws ec2 create-vpc \
  --cidr-block 10.0.0.0/16 \
  --tag-specifications 'ResourceType=vpc,Tags=[{Key=Name,Value=game-vpc}]'

# VPC ID 저장
VPC_ID=$(aws ec2 describe-vpcs --filters "Name=tag:Name,Values=game-vpc" --query "Vpcs[0].VpcId" --output text)
```

### 서브넷 생성
```bash
# 퍼블릭 서브넷 생성
aws ec2 create-subnet \
  --vpc-id $VPC_ID \
  --cidr-block 10.0.1.0/24 \
  --availability-zone ap-northeast-2a \
  --tag-specifications 'ResourceType=subnet,Tags=[{Key=Name,Value=game-public-2a}]'

# 프라이빗 서브넷 생성
aws ec2 create-subnet \
  --vpc-id $VPC_ID \
  --cidr-block 10.0.2.0/24 \
  --availability-zone ap-northeast-2a \
  --tag-specifications 'ResourceType=subnet,Tags=[{Key=Name,Value=game-private-2a}]'
```

## 2. 인터넷 게이트웨이 설정

### 인터넷 게이트웨이 생성 및 연결
```bash
# 인터넷 게이트웨이 생성
aws ec2 create-internet-gateway \
  --tag-specifications 'ResourceType=internet-gateway,Tags=[{Key=Name,Value=game-igw}]'

# VPC에 연결
aws ec2 attach-internet-gateway \
  --vpc-id $VPC_ID \
  --internet-gateway-id $IGW_ID
```

### 라우트 테이블 설정
```bash
# 퍼블릭 라우트 테이블 생성
aws ec2 create-route-table \
  --vpc-id $VPC_ID \
  --tag-specifications 'ResourceType=route-table,Tags=[{Key=Name,Value=game-public-rt}]'

# 인터넷 게이트웨이 라우트 추가
aws ec2 create-route \
  --route-table-id $PUBLIC_RT_ID \
  --destination-cidr-block 0.0.0.0/0 \
  --gateway-id $IGW_ID
```

## 3. NAT 게이트웨이 설정

### NAT 게이트웨이 생성
```bash
# EIP 할당
aws ec2 allocate-address \
  --domain vpc \
  --tag-specifications 'ResourceType=elastic-ip,Tags=[{Key=Name,Value=game-nat-eip}]'

# NAT 게이트웨이 생성
aws ec2 create-nat-gateway \
  --subnet-id $PUBLIC_SUBNET_ID \
  --allocation-id $EIP_ALLOC_ID \
  --tag-specifications 'ResourceType=natgateway,Tags=[{Key=Name,Value=game-nat}]'
```

### 프라이빗 라우트 테이블 설정
```bash
# 프라이빗 라우트 테이블 생성
aws ec2 create-route-table \
  --vpc-id $VPC_ID \
  --tag-specifications 'ResourceType=route-table,Tags=[{Key=Name,Value=game-private-rt}]'

# NAT 게이트웨이 라우트 추가
aws ec2 create-route \
  --route-table-id $PRIVATE_RT_ID \
  --destination-cidr-block 0.0.0.0/0 \
  --nat-gateway-id $NAT_GW_ID
```

## 4. 보안 그룹 설정

### 게임 서버 보안 그룹
```bash
# 보안 그룹 생성
aws ec2 create-security-group \
  --group-name game-server-sg \
  --description "Game server security group" \
  --vpc-id $VPC_ID

# 인바운드 규칙 추가
aws ec2 authorize-security-group-ingress \
  --group-id $SG_ID \
  --protocol tcp \
  --port 7000-8000 \
  --cidr 0.0.0.0/0

# 아웃바운드 규칙 추가
aws ec2 authorize-security-group-egress \
  --group-id $SG_ID \
  --protocol all \
  --port all \
  --cidr 0.0.0.0/0
```

## 5. 로드 밸런서 설정

### Network Load Balancer 생성
```bash
# NLB 생성
aws elbv2 create-load-balancer \
  --name game-nlb \
  --subnets $PUBLIC_SUBNET_ID_1 $PUBLIC_SUBNET_ID_2 \
  --scheme internet-facing \
  --type network \
  --tags Key=Name,Value=game-nlb
```

### 타겟 그룹 생성
```bash
# 타겟 그룹 생성
aws elbv2 create-target-group \
  --name game-servers \
  --protocol TCP \
  --port 7654 \
  --vpc-id $VPC_ID \
  --target-type ip \
  --health-check-protocol TCP \
  --health-check-port 7654
```

### 리스너 설정
```bash
# 리스너 생성
aws elbv2 create-listener \
  --load-balancer-arn $NLB_ARN \
  --protocol TCP \
  --port 80 \
  --default-actions Type=forward,TargetGroupArn=$TG_ARN
```

## 6. DNS 설정

### Route 53 호스팅 영역 생성
```bash
# 호스팅 영역 생성
aws route53 create-hosted-zone \
  --name gameserver.example.com \
  --caller-reference $(date +%s) \
  --hosted-zone-config Comment="Game server hosted zone"
```

### DNS 레코드 설정
```bash
# A 레코드 생성
aws route53 change-resource-record-sets \
  --hosted-zone-id $HOSTED_ZONE_ID \
  --change-batch '{
    "Changes": [{
      "Action": "CREATE",
      "ResourceRecordSet": {
        "Name": "gameserver.example.com",
        "Type": "A",
        "AliasTarget": {
          "HostedZoneId": "Z2IFOLAFXWLO4F",
          "DNSName": "'$NLB_DNS'",
          "EvaluateTargetHealth": false
        }
      }
    }]
  }'
```

## 7. 문제 해결

### 네트워크 연결 확인
```bash
# VPC 흐름 로그 확인
aws ec2 describe-flow-logs \
  --filter "Name=resource-id,Values=$VPC_ID"

# 보안 그룹 규칙 확인
aws ec2 describe-security-groups \
  --group-ids $SG_ID

# 라우트 테이블 확인
aws ec2 describe-route-tables \
  --route-table-ids $PUBLIC_RT_ID $PRIVATE_RT_ID
```

### 로드 밸런서 상태 확인
```bash
# NLB 상태 확인
aws elbv2 describe-load-balancers \
  --load-balancer-arns $NLB_ARN

# 타겟 그룹 상태 확인
aws elbv2 describe-target-health \
  --target-group-arn $TG_ARN
``` 