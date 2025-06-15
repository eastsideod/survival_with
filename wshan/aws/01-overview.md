# AWS 개요

## AWS 기본 개념

AWS는 Amazon에서 제공하는 클라우드 컴퓨팅 플랫폼으로, 다음과 같은 주요 특징이 있습니다:

1. **글로벌 인프라**
   - 33개 이상의 리전 (2024년 기준)
   - 105개 이상의 가용 영역
   - 450개 이상의 엣지 로케이션

2. **서비스 포트폴리오**
   - **컴퓨팅**: EC2, Lambda, ECS, EKS, Fargate
   - **스토리지**: S3, EBS, EFS, FSx, Glacier
   - **데이터베이스**: RDS, DynamoDB, ElastiCache, Redshift, Aurora
   - **네트워킹**: VPC, Route 53, CloudFront, Direct Connect, API Gateway
   - **보안**: IAM, KMS, WAF, Shield, GuardDuty, Security Hub
   - **분석**: Athena, EMR, Kinesis, QuickSight, Glue
   - **AI/ML**: SageMaker, Rekognition, Comprehend, Polly, Lex

3. **과금 모델**
   - **종량제 (Pay-as-you-go)**: 사용한 만큼만 결제
   - **예약 인스턴스**: 1년/3년 약정으로 최대 75% 할인
   - **스팟 인스턴스**: 유휴 용량 활용으로 최대 90% 할인
   - **세이빙 플랜**: 유연한 할인 계획으로 최대 72% 할인

4. **주요 특징**
   - **확장성**: 필요에 따라 자동으로 확장/축소
   - **안정성**: 99.99% 이상의 가용성 보장
   - **보안**: 업계 최고 수준의 보안 인증
   - **혁신**: 지속적인 새로운 서비스 출시

## GCP와 AWS의 주요 차이점

### 1. 네트워킹
- **AWS**: VPC가 기본 단위, 서브넷이 가용 영역에 바인딩
- **GCP**: 프로젝트가 기본 단위, 네트워크가 리전 전체에 걸쳐 있음

### 2. 컨테이너 서비스
- **AWS**: EKS (Elastic Kubernetes Service)
- **GCP**: GKE (Google Kubernetes Engine)
- 주요 차이:
  - EKS는 마스터 노드 비용이 별도로 발생
  - GKE는 마스터 노드 비용이 무료
  - EKS는 더 많은 커스터마이징 옵션 제공

### 3. 스토리지
- **AWS**: 
  - S3 (객체 스토리지)
  - EBS (블록 스토리지)
  - EFS (파일 스토리지)
- **GCP**:
  - Cloud Storage (객체 스토리지)
  - Persistent Disk (블록 스토리지)
  - Filestore (파일 스토리지)
- 주요 차이:
  - AWS는 더 세분화된 스토리지 옵션 제공
  - GCP는 통합된 인터페이스 제공

### 4. 모니터링
- **AWS**: CloudWatch
- **GCP**: Cloud Monitoring
- 주요 차이:
  - CloudWatch는 더 많은 커스텀 메트릭 지원
  - Cloud Monitoring은 더 직관적인 UI 제공

## AWS 주요 사용 사례

### 1. 웹 애플리케이션 호스팅
- **기본 구성**: EC2 + RDS + S3 + CloudFront
- **서버리스**: Lambda + API Gateway + DynamoDB
- **컨테이너**: ECS/EKS + ALB + RDS

### 2. 데이터 분석 및 빅데이터
- **데이터 레이크**: S3 + Glue + Athena + QuickSight
- **실시간 분석**: Kinesis + Lambda + ElastiSearch
- **기계학습**: SageMaker + S3 + EC2

### 3. 모바일 애플리케이션 백엔드
- **인증**: Cognito + IAM
- **API**: API Gateway + Lambda
- **푸시 알림**: SNS + SQS
- **파일 저장**: S3 + CloudFront

### 4. 기업 IT 인프라
- **하이브리드 클라우드**: Direct Connect + VPN
- **백업 및 복구**: S3 + Glacier + AWS Backup
- **디렉터리 서비스**: Directory Service + WorkSpaces

### 5. DevOps 및 CI/CD
- **소스 관리**: CodeCommit + CodeBuild + CodePipeline
- **배포**: CodeDeploy + CloudFormation + Systems Manager
- **모니터링**: CloudWatch + X-Ray + CloudTrail

## 게임 서버 운영을 위한 AWS 서비스

1. **컨테이너 오케스트레이션**
   - EKS (Elastic Kubernetes Service)
   - ECS (Elastic Container Service)

2. **네트워킹**
   - VPC (Virtual Private Cloud)
   - ALB/NLB (Application/Network Load Balancer)
   - Route 53 (DNS 서비스)

3. **스토리지**
   - EBS (Elastic Block Store)
   - S3 (Simple Storage Service)
   - EFS (Elastic File System)

4. **모니터링**
   - CloudWatch
   - X-Ray (분산 추적)
   - CloudTrail (감사 로그)

5. **보안**
   - IAM (Identity and Access Management)
   - KMS (Key Management Service)
   - WAF (Web Application Firewall)

## 마이그레이션 고려사항

1. **아키텍처 변경**
   - GCP의 글로벌 로드밸런서 → AWS의 ALB/NLB
   - GCP의 프로젝트 구조 → AWS의 계정/리전 구조

2. **네트워크 설계**
   - VPC 구조 설계
   - 서브넷 전략 수립
   - 보안 그룹 설정

3. **비용 최적화**
   - 예약 인스턴스 활용
   - 스팟 인스턴스 고려
   - 비용 알림 설정

4. **모니터링 전략**
   - CloudWatch 설정
   - Prometheus/Grafana 연동
   - 알림 설정 