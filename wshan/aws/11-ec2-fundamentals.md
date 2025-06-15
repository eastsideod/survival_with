# EC2 기본 개념

## 1. EC2 개요

### EC2란?
- **정의**: Amazon Elastic Compute Cloud의 줄임말
- **기능**: 클라우드에서 크기 조정 가능한 가상 서버 제공
- **특징**: 
  - 다양한 인스턴스 타입 제공
  - 사용한 만큼만 비용 지불
  - 빠른 확장 및 축소 가능

### 주요 개념
- **인스턴스**: 가상 서버
- **AMI (Amazon Machine Image)**: 인스턴스 생성을 위한 템플릿
- **인스턴스 타입**: 인스턴스의 하드웨어 사양
- **키 페어**: SSH 접속을 위한 공개키/비공개키 쌍

## 2. 인스턴스 타입

### 베어메탈 인스턴스
- **정의**: 하이퍼바이저 없이 물리 서버에 직접 접근하는 인스턴스
- **특징**: 
  - 최고 성능 및 최저 지연시간
  - 라이센스 요구사항이 있는 소프트웨어에 적합
  - 예시: i3.metal, m5.metal, r5.metal

### 범용 인스턴스
- **T 시리즈**: 버스트 가능한 성능 인스턴스
  - t3.micro, t3.small, t3.medium
  - CPU 크레딧 기반으로 성능 조절
  - 기준 성능 이상 사용 시 크레딧 소모
- **M 시리즈**: 균형 잡힌 컴퓨팅, 메모리, 네트워킹
  - m5.large, m5.xlarge, m5.2xlarge
  - 웹 서버, 마이크로서비스에 적합

### 컴퓨팅 최적화
- **C 시리즈**: 고성능 프로세서
  - c5.large, c5.xlarge, c5.2xlarge
  - CPU 집약적 애플리케이션에 적합

### 메모리 최적화
- **R 시리즈**: 메모리 집약적 애플리케이션
  - r5.large, r5.xlarge, r5.2xlarge
  - 인메모리 데이터베이스에 적합

### 스토리지 최적화
- **I 시리즈**: 높은 순차 읽기/쓰기 성능
  - i3.large, i3.xlarge, i3.2xlarge
  - NoSQL 데이터베이스에 적합

## 3. AMI (Amazon Machine Image)

### AMI 유형
- **AWS 제공 AMI**: Amazon Linux, Ubuntu, Windows 등
- **AWS Marketplace AMI**: 서드파티에서 제공하는 AMI
- **커뮤니티 AMI**: 사용자가 공유한 AMI
- **사용자 지정 AMI**: 직접 생성한 AMI

### AMI 생성
```bash
# 인스턴스에서 AMI 생성
aws ec2 create-image \
  --instance-id i-1234567890abcdef0 \
  --name "My server AMI" \
  --description "Custom AMI for my application"
```

## 4. 보안 그룹

### 보안 그룹 개념
- **기능**: 가상 방화벽 역할
- **특징**: 상태 기반 방화벽 (Stateful)
- **적용 범위**: 인스턴스 레벨에서 적용

### 규칙 설정
```bash
# 보안 그룹 생성
aws ec2 create-security-group \
  --group-name web-server-sg \
  --description "Security group for web server"

# HTTP 트래픽 허용
aws ec2 authorize-security-group-ingress \
  --group-id sg-1234567890abcdef0 \
  --protocol tcp \
  --port 80 \
  --cidr 0.0.0.0/0

# SSH 트래픽 허용
aws ec2 authorize-security-group-ingress \
  --group-id sg-1234567890abcdef0 \
  --protocol tcp \
  --port 22 \
  --cidr 0.0.0.0/0
```

## 5. 인스턴스 생성 및 관리

### 인스턴스 생성
```bash
# EC2 인스턴스 생성
aws ec2 run-instances \
  --image-id ami-0abcdef1234567890 \
  --count 1 \
  --instance-type t3.micro \
  --key-name my-key-pair \
  --security-group-ids sg-1234567890abcdef0 \
  --subnet-id subnet-12345678
```

### 인스턴스 상태 관리
```bash
# 인스턴스 시작
aws ec2 start-instances --instance-ids i-1234567890abcdef0

# 인스턴스 중지
aws ec2 stop-instances --instance-ids i-1234567890abcdef0

# 인스턴스 재부팅
aws ec2 reboot-instances --instance-ids i-1234567890abcdef0

# 인스턴스 종료
aws ec2 terminate-instances --instance-ids i-1234567890abcdef0
```

### 인스턴스 연결
```bash
# SSH를 통한 Linux 인스턴스 연결
ssh -i /path/to/key.pem ec2-user@public-ip-address

# EC2 Instance Connect 사용
aws ec2-instance-connect send-ssh-public-key \
  --instance-id i-1234567890abcdef0 \
  --availability-zone us-west-2a \
  --instance-os-user ec2-user \
  --ssh-public-key file://~/.ssh/id_rsa.pub
```

## 6. 스토리지 옵션

### EBS (Elastic Block Store)
- **루트 볼륨**: 운영 체제가 설치된 기본 스토리지
- **추가 볼륨**: 데이터 저장을 위한 추가 스토리지
- **볼륨 타입**:
  - gp3: 범용 SSD (기본값)
  - io2: 프로비저닝된 IOPS SSD
  - st1: 처리량 최적화 HDD
  - sc1: 콜드 HDD

### 인스턴스 스토어
- **특징**: 인스턴스에 물리적으로 연결된 임시 스토리지
- **용도**: 
  - 캐시, 임시 파일, 버퍼링
  - 높은 순차 읽기/쓰기 성능이 필요한 경우
  - RAID 구성으로 성능 향상 가능
- **주의사항**: 
  - 인스턴스 중지/종료 시 데이터 손실
  - 하드웨어 장애 시 데이터 손실 위험

### EBS vs 인스턴스 스토어 비교
| 특성 | EBS | 인스턴스 스토어 |
|------|-----|----------------|
| 영속성 | 영구 저장 | 임시 저장 |
| 성능 | 네트워크 기반 | 로컬 디스크 |
| 백업 | 스냅샷 지원 | 수동 백업 필요 |
| 크기 조정 | 동적 조정 가능 | 고정 크기 |
| 비용 | 프로비저닝 기반 | 인스턴스 비용에 포함 |

### RAID 구성
- **RAID 0**: 성능 향상 (스트라이핑)
  - 여러 디스크에 데이터 분산 저장
  - 읽기/쓰기 성능 향상
  - 장애 시 모든 데이터 손실 위험
- **RAID 1**: 가용성 향상 (미러링)
  - 동일한 데이터를 여러 디스크에 복사
  - 한 디스크 장애 시에도 데이터 보존
  - 스토리지 용량 50% 감소

## 7. 네트워킹

### 퍼블릭 IP vs 프라이빗 IP
- **퍼블릭 IP**: 인터넷에서 접근 가능한 IP 주소
- **프라이빗 IP**: VPC 내부에서만 사용되는 IP 주소
- **Elastic IP**: 고정 퍼블릭 IP 주소

### 배치 그룹 (Placement Group)
- **클러스터**: 높은 네트워크 성능을 위한 배치
- **파티션**: 장애 격리를 위한 배치
- **분산**: 다양한 하드웨어에 분산 배치

## 8. 모니터링 및 로깅

### CloudWatch 메트릭
- **기본 메트릭**: CPU 사용률, 네트워크 I/O, 디스크 I/O
- **세부 모니터링**: 1분 간격 메트릭 수집
- **사용자 정의 메트릭**: 애플리케이션별 메트릭

### 로그 수집
```bash
# CloudWatch 에이전트 설치
sudo yum install -y amazon-cloudwatch-agent

# 에이전트 설정
sudo /opt/aws/amazon-cloudwatch-agent/bin/amazon-cloudwatch-agent-config-wizard
```

## 9. 비용 최적화

### 인스턴스 구매 옵션
- **온디맨드**: 시간 단위로 비용 지불
- **예약 인스턴스**: 1년 또는 3년 약정으로 할인
- **스팟 인스턴스**: 유휴 용량을 저렴하게 이용
- **Savings Plans**: 유연한 할인 계획

### 자동 스케일링
```bash
# Auto Scaling 그룹 생성
aws autoscaling create-auto-scaling-group \
  --auto-scaling-group-name my-asg \
  --launch-template LaunchTemplateName=my-template,Version=1 \
  --min-size 1 \
  --max-size 5 \
  --desired-capacity 2 \
  --availability-zones us-west-2a us-west-2b
```

## 10. 문제 해결

### 일반적인 문제
- **인스턴스 접속 불가**: 보안 그룹, 키 페어, 네트워크 설정 확인
- **성능 저하**: 인스턴스 타입, CPU 크레딧, 메모리 사용량 확인
- **스토리지 부족**: EBS 볼륨 확장 또는 추가 볼륨 연결

### 디버깅 도구
```bash
# 인스턴스 상태 확인
aws ec2 describe-instances --instance-ids i-1234567890abcdef0

# 시스템 로그 확인
aws ec2 get-console-output --instance-id i-1234567890abcdef0

# 인스턴스 스크린샷 확인
aws ec2 get-console-screenshot --instance-id i-1234567890abcdef0
``` 