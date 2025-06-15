# AWS 비용 최적화

## 1. 비용 분석

### Cost Explorer 설정
```bash
# Cost Explorer 활성화
aws ce enable-cost-explorer

# 비용 보고서 생성
aws ce create-cost-allocation-tags \
  --tags GameServer=true

# 비용 예측 설정
aws ce create-anomaly-monitor \
  --anomaly-monitor-name game-server-cost-monitor \
  --monitor-type CUSTOM \
  --monitor-specification '{
    "And": [{
      "Dimensions": {
        "Key": "SERVICE",
        "Values": ["Amazon Elastic Compute Cloud - Compute"]
      }
    }]
  }'
```

### 비용 알림 설정
```bash
# SNS 토픽 생성
aws sns create-topic \
  --name game-server-cost-alerts

# CloudWatch 알림 설정
aws cloudwatch put-metric-alarm \
  --alarm-name game-server-cost-alarm \
  --alarm-description "Game server cost exceeds threshold" \
  --metric-name EstimatedCharges \
  --namespace AWS/Billing \
  --statistic Maximum \
  --period 21600 \
  --threshold 1000 \
  --comparison-operator GreaterThanThreshold \
  --evaluation-periods 1 \
  --alarm-actions arn:aws:sns:region:account-id:game-server-cost-alerts
```

## 2. 리소스 최적화

### 인스턴스 크기 최적화
```bash
# Compute Optimizer 활성화
aws compute-optimizer update-enrollment-status \
  --status Active

# 최적화 권장사항 확인
aws compute-optimizer get-ec2-instance-recommendations \
  --instance-arns arn:aws:ec2:region:account-id:instance/instance-id
```

### 자동 스케일링 설정
```yaml
apiVersion: autoscaling.openshift.io/v1
kind: ClusterAutoscaler
metadata:
  name: default
spec:
  resourceLimits:
    maxNodesTotal: 10
  scaleDown:
    enabled: true
    delayAfterAdd: 10m
    delayAfterDelete: 10m
    delayAfterFailure: 3m
```

## 3. 예약 인스턴스 활용

### 예약 인스턴스 구매
```bash
# 예약 인스턴스 구매
aws ec2 purchase-reserved-instances-offering \
  --reserved-instances-offering-id $OFFERING_ID \
  --instance-count 3 \
  --limit-price Amount=1000,CurrencyCode=USD
```

### 예약 인스턴스 관리
```bash
# 예약 인스턴스 목록 확인
aws ec2 describe-reserved-instances \
  --filters "Name=state,Values=active"

# 예약 인스턴스 수정
aws ec2 modify-reserved-instances \
  --reserved-instances-ids $RI_ID \
  --target-configurations "AvailabilityZone=ap-northeast-2a,InstanceCount=3,Platform=Linux/UNIX,InstanceType=t3.large"
```

## 4. 스팟 인스턴스 활용

### 스팟 인스턴스 설정
```yaml
apiVersion: eksctl.io/v1alpha5
kind: ClusterConfig
metadata:
  name: game-cluster
  region: ap-northeast-2
nodeGroups:
  - name: spot-nodes
    instanceType: t3.large
    desiredCapacity: 3
    minSize: 3
    maxSize: 5
    spot: true
    labels:
      role: game-server
    taints:
      game-server: "true:NoSchedule"
```

### 스팟 인스턴스 관리
```bash
# 스팟 인스턴스 요청 확인
aws ec2 describe-spot-instance-requests \
  --filters "Name=state,Values=active"

# 스팟 인스턴스 요청 취소
aws ec2 cancel-spot-instance-requests \
  --spot-instance-request-ids $SPOT_REQUEST_ID
```

## 5. 스토리지 최적화

### EBS 볼륨 최적화
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

### S3 수명 주기 설정
```bash
# 수명 주기 정책 설정
aws s3api put-bucket-lifecycle-configuration \
  --bucket game-server-data \
  --lifecycle-configuration '{
    "Rules": [{
      "ID": "StorageOptimization",
      "Status": "Enabled",
      "Transitions": [{
        "Days": 30,
        "StorageClass": "STANDARD_IA"
      }],
      "Expiration": {
        "Days": 365
      }
    }]
  }'
```

## 6. 문제 해결

### 비용 모니터링
```bash
# 비용 및 사용량 보고서 확인
aws ce get-cost-and-usage \
  --time-period Start=2024-01-01,End=2024-01-31 \
  --granularity MONTHLY \
  --metrics "BlendedCost" "UnblendedCost" "UsageQuantity"

# 예약 인스턴스 사용률 확인
aws ce get-reservation-utilization \
  --time-period Start=2024-01-01,End=2024-01-31 \
  --granularity MONTHLY
```

### 리소스 사용량 확인
```bash
# 인스턴스 사용량 확인
aws cloudwatch get-metric-statistics \
  --namespace AWS/EC2 \
  --metric-name CPUUtilization \
  --dimensions Name=InstanceId,Value=$INSTANCE_ID \
  --start-time $(date -v-1H -u +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average

# 스토리지 사용량 확인
aws cloudwatch get-metric-statistics \
  --namespace AWS/EBS \
  --metric-name VolumeReadOps \
  --dimensions Name=VolumeId,Value=$VOLUME_ID \
  --start-time $(date -v-1H -u +%Y-%m-%dT%H:%M:%S) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%S) \
  --period 300 \
  --statistics Average
``` 