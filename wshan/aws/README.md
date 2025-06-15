# AWS 클라우드 서비스

AWS(Amazon Web Services)는 Amazon에서 제공하는 클라우드 컴퓨팅 플랫폼입니다. 이 문서에서는 AWS의 기본 개념부터 게임 서버 운영에 필요한 서비스와 설정 방법까지 체계적으로 설명합니다.

## 목차

0. [AWS 기본 개념](00-basic-concepts.md)
   - 클라우드 컴퓨팅 기초
   - AWS 글로벌 인프라
   - 계정 및 결제 관리
   - 보안 기본 개념

1. [AWS 개요](01-overview.md)
   - AWS 기본 개념
   - 주요 서비스 소개
   - GCP와 AWS의 차이점

1-2. [EC2 기본 개념](11-ec2-fundamentals.md)
   - EC2 인스턴스 관리
   - AMI와 보안 그룹
   - 스토리지 옵션 (EBS vs 인스턴스 스토어)
   - 네트워킹 및 모니터링

1-3. [네트워킹 기본 개념](13-networking-concepts.md)
   - RFC 1918, CIDR, 서브넷
   - 라우팅 테이블과 인터넷 연결
   - 보안 그룹 vs NACL
   - VPC 연결 옵션 (Peering, Transit Gateway)
   - VPC Endpoint와 Flow Logs

1-4. [고급 개념](14-advanced-concepts.md)
   - AWS Nitro System과 Graviton
   - 가상화 기술과 Fargate
   - ELB 종류별 비교
   - Launch Template과 인스턴스 연결 방법
   - CloudWatch와 보안 고급 개념

2. [EKS 설정](02-eks.md)
   - EKS 클러스터 생성
   - 노드 그룹 설정
   - 네트워크 구성

3. [Agones 설정](03-agones.md)
   - EKS에 Agones 설치
   - GameServer 설정
   - Fleet 설정
   - AWS 환경에서의 게임 서버 관리

4. [네트워크 설정](04-network.md)
   - VPC 구성
   - 서브넷 설정
   - 보안 그룹 설정
   - 로드 밸런서 설정

5. [스토리지 설정](05-storage.md)
   - EBS 볼륨 설정
   - S3 버킷 설정
   - 백업 전략

6. [모니터링 설정](06-monitoring.md)
   - CloudWatch 설정
   - Prometheus 연동
   - Grafana 대시보드

7. [보안 설정](07-security.md)
   - IAM 설정
   - 네트워크 보안
   - 데이터 암호화

8. [비용 최적화](08-cost-optimization.md)
   - 비용 분석
   - 리소스 최적화
   - 예약 인스턴스 활용

9. [CI/CD 설정](09-cicd.md)
   - CodePipeline 설정
   - CodeBuild 설정
   - 배포 자동화

10. [문제 해결](10-troubleshooting.md)
    - 일반적인 문제 해결
    - 디버깅 방법
    - 지원 채널 