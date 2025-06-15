# AWS 문제 해결

## 1. EKS 문제 해결

### 클러스터 상태 확인
```bash
# 클러스터 상태 확인
eksctl get cluster --name game-cluster

# 노드 상태 확인
kubectl get nodes

# 노드 상세 정보 확인
kubectl describe node <node-name>

# 클러스터 로그 확인
aws eks describe-cluster \
  --name game-cluster \
  --query "cluster.logging.clusterLogging[].enabled"
```

### 노드 문제 해결
```bash
# 노드 로그 확인
kubectl logs -n kube-system <node-problem-detector-pod>

# 노드 이벤트 확인
kubectl get events --field-selector involvedObject.kind=Node

# 노드 메트릭 확인
kubectl top node
```

## 2. 네트워크 문제 해결

### VPC 문제 해결
```bash
# VPC 흐름 로그 확인
aws ec2 describe-flow-logs \
  --filter "Name=resource-id,Values=$VPC_ID"

# 라우트 테이블 확인
aws ec2 describe-route-tables \
  --filters "Name=vpc-id,Values=$VPC_ID"

# 보안 그룹 확인
aws ec2 describe-security-groups \
  --filters "Name=vpc-id,Values=$VPC_ID"
```

### 로드 밸런서 문제 해결
```bash
# 로드 밸런서 상태 확인
aws elbv2 describe-load-balancers \
  --names game-server-lb

# 타겟 그룹 상태 확인
aws elbv2 describe-target-health \
  --target-group-arn $TG_ARN

# 리스너 확인
aws elbv2 describe-listeners \
  --load-balancer-arn $LB_ARN
```

## 3. 스토리지 문제 해결

### EBS 문제 해결
```bash
# 볼륨 상태 확인
aws ec2 describe-volumes \
  --volume-ids $VOLUME_ID

# 스냅샷 상태 확인
aws ec2 describe-snapshots \
  --snapshot-ids $SNAPSHOT_ID

# 볼륨 성능 확인
aws cloudwatch get-metric-statistics \
  --namespace AWS/EBS \
  --metric-name VolumeReadOps \
  --dimensions Name=VolumeId,Value=$VOLUME_ID \
  --start-time $(date -v-1H -u +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average
```

### S3 문제 해결
```bash
# 버킷 설정 확인
aws s3api get-bucket-versioning \
  --bucket game-server-data

# 버킷 정책 확인
aws s3api get-bucket-policy \
  --bucket game-server-data

# 버킷 로깅 확인
aws s3api get-bucket-logging \
  --bucket game-server-data
```

## 4. 모니터링 문제 해결

### CloudWatch 문제 해결
```bash
# 로그 그룹 확인
aws logs describe-log-groups \
  --log-group-name-prefix /aws/containerinsights/game-cluster

# 로그 스트림 확인
aws logs describe-log-streams \
  --log-group-name /aws/containerinsights/game-cluster/application

# 메트릭 확인
aws cloudwatch get-metric-statistics \
  --namespace AWS/EC2 \
  --metric-name CPUUtilization \
  --dimensions Name=InstanceId,Value=$INSTANCE_ID \
  --start-time $(date -v-1H -u +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average
```

### Prometheus 문제 해결
```bash
# Prometheus 서버 상태 확인
kubectl get pods -n monitoring -l app=prometheus-server

# ServiceMonitor 상태 확인
kubectl get servicemonitor -n monitoring

# Prometheus 로그 확인
kubectl logs -n monitoring -l app=prometheus-server
```

## 5. 보안 문제 해결

### IAM 문제 해결
```bash
# 역할 정책 확인
aws iam list-attached-role-policies \
  --role-name game-server-role

# 사용자 정책 확인
aws iam list-attached-user-policies \
  --user-name game-server-admin

# 액세스 키 확인
aws iam list-access-keys \
  --user-name game-server-admin
```

### 보안 그룹 문제 해결
```bash
# 보안 그룹 규칙 확인
aws ec2 describe-security-groups \
  --group-ids $SG_ID

# 네트워크 ACL 규칙 확인
aws ec2 describe-network-acls \
  --network-acl-ids $ACL_ID

# VPC 엔드포인트 확인
aws ec2 describe-vpc-endpoints \
  --filters "Name=vpc-id,Values=$VPC_ID"
```

## 6. CI/CD 문제 해결

### CodePipeline 문제 해결
```bash
# 파이프라인 상태 확인
aws codepipeline get-pipeline-state \
  --name game-server-pipeline

# 파이프라인 실행 기록 확인
aws codepipeline list-pipeline-executions \
  --pipeline-name game-server-pipeline

# 웹훅 상태 확인
aws codepipeline list-webhooks
```

### CodeBuild 문제 해결
```bash
# 빌드 상태 확인
aws codebuild batch-get-builds \
  --ids $BUILD_ID

# 빌드 로그 확인
aws codebuild batch-get-builds \
  --ids $BUILD_ID \
  --query "builds[].logs.{groupName:groupName,streamName:streamName}"

# 빌드 프로젝트 설정 확인
aws codebuild batch-get-projects \
  --names game-server-build
```

## 7. 지원 채널

### AWS 지원 계획
```bash
# 지원 계획 확인
aws support describe-severity-levels

# 지원 케이스 생성
aws support create-case \
  --subject "Game server issue" \
  --service-code "amazon-eks" \
  --severity-code "high" \
  --category-code "technical" \
  --communication-body "Issue description"
```

### 커뮤니티 지원
```bash
# AWS 포럼 검색
aws support search-forums \
  --query "Game server issue"

# AWS 문서 검색
aws support search-documents \
  --query "Game server issue"
``` 