# AWS 고급 개념

## 1. AWS Nitro System

### Nitro System 개요
- **정의**: AWS의 차세대 EC2 인스턴스를 위한 하드웨어 및 소프트웨어 플랫폼
- **구성 요소**:
  - **Nitro Cards**: 네트워킹, 스토리지, 보안 기능을 하드웨어로 오프로드
  - **Nitro Security Chip**: 하드웨어 루트 오브 트러스트
  - **Nitro Hypervisor**: 경량화된 하이퍼바이저

### Nitro System의 장점
- **성능 향상**: 네트워킹과 스토리지를 하드웨어로 처리
- **보안 강화**: 하드웨어 기반 보안 및 격리
- **가상화 오버헤드 최소화**: 베어메탈에 가까운 성능
- **더 많은 인스턴스 크기**: 물리 서버 리소스를 최대한 활용

### Nitro 기반 인스턴스
```bash
# Nitro 기반 인스턴스 예시
M5, M5a, M5ad, M5d, M5dn, M5n, M5zn
C5, C5a, C5ad, C5d, C5n
R5, R5a, R5ad, R5d, R5dn, R5n
T3, T3a, T4g
```

## 2. AWS Graviton 프로세서

### Graviton 개요
- **정의**: AWS가 설계한 ARM 기반 64비트 프로세서
- **세대**:
  - **Graviton**: 1세대 (A1 인스턴스)
  - **Graviton2**: 2세대 (M6g, C6g, R6g 등)
  - **Graviton3**: 3세대 (C7g, M7g, R7g 등)

### Graviton의 장점
- **가격 대비 성능**: x86 대비 최대 40% 향상된 가격 대비 성능
- **에너지 효율성**: 최대 60% 적은 에너지 소비
- **AWS 서비스 통합**: RDS, ElastiCache, Lambda 등에서 지원

```bash
# Graviton 기반 인스턴스 생성
aws ec2 run-instances \
  --image-id ami-0abcdef1234567890 \
  --instance-type m6g.large \
  --key-name my-key-pair \
  --subnet-id subnet-12345678
```

## 3. 가상화 기술

### CPU 가상화
- **하이퍼바이저**: 물리 CPU를 가상 CPU로 분할
- **CPU 스케줄링**: 시간 분할로 여러 VM에 CPU 할당
- **CPU 어피니티**: 특정 VM을 특정 물리 CPU 코어에 고정

### 메모리 가상화
- **메모리 오버커밋**: 물리 메모리보다 많은 가상 메모리 할당
- **메모리 스와핑**: 사용하지 않는 메모리를 디스크로 이동
- **Ballooning**: 게스트 OS가 메모리를 반납하도록 유도

### 컨테이너 가상화
- **네임스페이스**: 프로세스, 네트워크, 파일시스템 격리
- **cgroups**: CPU, 메모리, I/O 리소스 제한
- **Union 파일시스템**: 레이어 기반 파일시스템

## 4. Fargate와 데이터플레인

### AWS Fargate
- **정의**: 서버리스 컨테이너 실행 환경
- **특징**:
  - 인프라 관리 불필요
  - 작업별 과금
  - 자동 스케일링
  - VPC 네트워킹 지원

```yaml
# Fargate 태스크 정의
apiVersion: v1
kind: Pod
spec:
  containers:
  - name: app
    image: nginx
    resources:
      requests:
        memory: "512Mi"
        cpu: "0.25"
      limits:
        memory: "1Gi"
        cpu: "0.5"
```

### 데이터플레인 vs 컨트롤플레인
- **컨트롤플레인**: 관리 기능 (API 서버, 스케줄러)
- **데이터플레인**: 실제 워크로드 실행 (kubelet, 컨테이너 런타임)
- **Fargate**: AWS가 데이터플레인을 관리하는 서비스

## 5. 로드 밸런서 비교

### Application Load Balancer (ALB)
- **레이어**: Layer 7 (HTTP/HTTPS)
- **기능**:
  - 경로 기반 라우팅
  - 호스트 기반 라우팅
  - WebSocket 지원
  - HTTP/2 지원
- **대상**: EC2, IP, Lambda

### Network Load Balancer (NLB)
- **레이어**: Layer 4 (TCP/UDP)
- **기능**:
  - 고성능 (수백만 요청/초)
  - 정적 IP 지원
  - 소스 IP 보존
  - 극저지연
- **대상**: EC2, IP, ALB

### Gateway Load Balancer (GWLB)
- **레이어**: Layer 3 (IP)
- **용도**: 보안 어플라이언스, 방화벽 배포
- **기능**: GENEVE 프로토콜 사용

```bash
# ALB 생성
aws elbv2 create-load-balancer \
  --name my-application-load-balancer \
  --subnets subnet-12345678 subnet-87654321 \
  --security-groups sg-12345678

# NLB 생성
aws elbv2 create-load-balancer \
  --name my-network-load-balancer \
  --scheme internal \
  --type network \
  --subnets subnet-12345678 subnet-87654321
```

## 6. Launch Template

### Launch Template vs Launch Configuration
- **Launch Template**: 새로운 기능 지원, 버전 관리
- **Launch Configuration**: 레거시, 제한된 기능

### Launch Template 구성
```json
{
  "LaunchTemplateName": "my-launch-template",
  "LaunchTemplateData": {
    "ImageId": "ami-0abcdef1234567890",
    "InstanceType": "t3.micro",
    "KeyName": "my-key-pair",
    "SecurityGroupIds": ["sg-12345678"],
    "UserData": "IyEvYmluL2Jhc2gKZWNobyAiSGVsbG8gV29ybGQi",
    "IamInstanceProfile": {
      "Name": "my-instance-profile"
    },
    "BlockDeviceMappings": [
      {
        "DeviceName": "/dev/xvda",
        "Ebs": {
          "VolumeSize": 20,
          "VolumeType": "gp3",
          "DeleteOnTermination": true
        }
      }
    ]
  }
}
```

## 7. 인스턴스 연결 방법

### SSH Key Pair
- **생성**: RSA 2048비트 키 쌍
- **보안**: 프라이빗 키는 안전하게 보관
- **접근**: `ssh -i key.pem ec2-user@ip`

### EC2 Instance Connect
- **정의**: 브라우저 기반 SSH 연결
- **장점**: 키 관리 불필요, 임시 SSH 키 사용
- **제한**: Amazon Linux 2, Ubuntu 16.04+ 지원

```bash
# EC2 Instance Connect 사용
aws ec2-instance-connect send-ssh-public-key \
  --instance-id i-1234567890abcdef0 \
  --availability-zone us-west-2a \
  --instance-os-user ec2-user \
  --ssh-public-key file://~/.ssh/id_rsa.pub
```

### Session Manager
- **정의**: IAM 기반 인스턴스 액세스
- **장점**:
  - SSH 키 불필요
  - 보안 그룹에 22번 포트 열지 않아도 됨
  - 세션 로깅 및 감사
- **요구사항**: SSM Agent 설치

```bash
# Session Manager로 연결
aws ssm start-session --target i-1234567890abcdef0
```

## 8. CloudWatch 모니터링

### CloudWatch 메트릭 유형
- **기본 메트릭**: 5분 간격, 무료
- **세부 메트릭**: 1분 간격, 유료
- **사용자 정의 메트릭**: 애플리케이션에서 전송

### CloudWatch Logs
- **로그 그룹**: 로그 스트림의 컨테이너
- **로그 스트림**: 동일한 소스의 로그 이벤트 시퀀스
- **로그 보존**: 1일 ~ 영구 보관

```bash
# CloudWatch Logs 에이전트 설정
{
  "logs": {
    "logs_collected": {
      "files": {
        "collect_list": [
          {
            "file_path": "/var/log/httpd/access_log",
            "log_group_name": "httpd-access-log",
            "log_stream_name": "{instance_id}"
          }
        ]
      }
    }
  }
}
```

### CloudWatch 알람
- **임계값 알람**: 메트릭이 임계값을 초과할 때
- **복합 알람**: 여러 알람의 조합
- **액션**: SNS, Auto Scaling, EC2 작업

## 9. 스토리지 비교

### EBS vs EFS
| 특성 | EBS | EFS |
|------|-----|-----|
| 유형 | 블록 스토리지 | 파일 스토리지 |
| 접근 | 단일 인스턴스 | 다중 인스턴스 |
| 성능 | 높은 IOPS | 높은 처리량 |
| 가용성 | 단일 AZ | 다중 AZ |
| 백업 | 스냅샷 | 자동 백업 |
| 프로토콜 | - | NFS v4.1 |

```bash
# EFS 파일시스템 마운트
sudo mount -t efs fs-12345678 /mnt/efs
```

## 10. 보안 고급 개념

### IAM 역할 vs 사용자
- **IAM 사용자**: 영구적인 자격 증명
- **IAM 역할**: 임시 자격 증명, 서비스 간 권한 위임

### 인스턴스 프로파일
```bash
# IAM 역할을 인스턴스에 연결
aws ec2 associate-iam-instance-profile \
  --instance-id i-1234567890abcdef0 \
  --iam-instance-profile Name=MyInstanceProfile
```

### 메타데이터 서비스 v2 (IMDSv2)
- **세션 토큰 기반**: PUT 요청으로 토큰 획득
- **홉 제한**: 메타데이터 접근 범위 제한
- **보안 향상**: SSRF 공격 방지

```bash
# IMDSv2 사용 예시
TOKEN=$(curl -X PUT -H "X-aws-ec2-metadata-token-ttl-seconds: 21600" \
  http://169.254.169.254/latest/api/token)
  
curl -H "X-aws-ec2-metadata-token: $TOKEN" \
  http://169.254.169.254/latest/meta-data/instance-id
``` 