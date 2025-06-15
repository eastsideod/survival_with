# AWS 네트워킹 기본 개념

## 1. IP 주소 체계

### RFC 1918 (Private IP 주소 범위)
- **정의**: 인터넷에서 라우팅되지 않는 사설 IP 주소 범위를 정의한 표준
- **사설 IP 주소 범위**:
  - **Class A**: 10.0.0.0 ~ 10.255.255.255 (10.0.0.0/8)
  - **Class B**: 172.16.0.0 ~ 172.31.255.255 (172.16.0.0/12)
  - **Class C**: 192.168.0.0 ~ 192.168.255.255 (192.168.0.0/16)
- **AWS VPC에서 사용**: VPC 생성 시 이 범위 내에서 CIDR 블록 설정

### CIDR (Classless Inter-Domain Routing)
- **정의**: IP 주소와 서브넷 마스크를 함께 표기하는 방식
- **표기법**: IP주소/프리픽스 길이 (예: 10.0.0.0/16)
- **서브넷 마스킹**: 
  - /16 = 255.255.0.0 (65,536개 IP)
  - /24 = 255.255.255.0 (256개 IP)
  - /28 = 255.255.255.240 (16개 IP)

### AWS에서의 IP 주소 할당
```bash
# VPC CIDR 블록 예시
10.0.0.0/16    # 전체 VPC (65,536개 IP)
├── 10.0.1.0/24    # 퍼블릭 서브넷 1 (256개 IP)
├── 10.0.2.0/24    # 퍼블릭 서브넷 2 (256개 IP)
├── 10.0.11.0/24   # 프라이빗 서브넷 1 (256개 IP)
└── 10.0.12.0/24   # 프라이빗 서브넷 2 (256개 IP)
```

## 2. 서브넷과 라우팅

### 서브넷의 기본 특성
- **기본 상태**: 모든 서브넷은 기본적으로 **프라이빗(Private)**
- **퍼블릭 서브넷이 되는 조건**:
  1. 인터넷 게이트웨이(IGW)가 VPC에 연결
  2. 라우팅 테이블에 인터넷 게이트웨이로의 라우트 추가
  3. 인스턴스에 퍼블릭 IP 할당

### 라우팅 테이블 설정
```bash
# 퍼블릭 서브넷 라우팅 테이블
Destination     Target
10.0.0.0/16     Local
0.0.0.0/0       igw-12345678

# 프라이빗 서브넷 라우팅 테이블
Destination     Target
10.0.0.0/16     Local
0.0.0.0/0       nat-12345678
```

## 3. 인터넷 연결

### 인터넷 게이트웨이 (Internet Gateway, IGW)
- **기능**: VPC와 인터넷 간의 통신 제공
- **특징**: 
  - VPC당 하나만 연결 가능
  - 고가용성 및 확장성 보장
  - NAT 역할 수행 (프라이빗 IP ↔ 퍼블릭 IP)

### NAT Gateway
- **목적**: 프라이빗 서브넷의 인스턴스가 아웃바운드 인터넷 액세스
- **특징**:
  - 관리형 서비스 (AWS가 관리)
  - 특정 가용 영역에 생성
  - 고가용성을 위해 여러 AZ에 생성 권장
  - NAT 인스턴스보다 성능 우수

```bash
# NAT Gateway 생성
aws ec2 create-nat-gateway \
  --subnet-id subnet-12345678 \
  --allocation-id eipalloc-12345678
```

## 4. 보안 그룹 vs NACL

### 보안 그룹 (Security Group)
- **레벨**: 인스턴스 레벨
- **상태**: Stateful (연결 추적)
- **규칙**: Allow 규칙만 가능
- **적용**: 인스턴스 ENI에 적용
- **기본 동작**: 모든 아웃바운드 허용, 인바운드 거부

### NACL (Network Access Control List)
- **레벨**: 서브넷 레벨
- **상태**: Stateless (연결 추적 안함)
- **규칙**: Allow/Deny 규칙 모두 가능
- **처리**: 규칙 번호 순서대로 처리
- **기본 동작**: 기본 NACL은 모든 트래픽 허용

```bash
# NACL 규칙 예시
Rule #    Type        Protocol    Port Range    Source      Allow/Deny
100       HTTP        TCP         80            0.0.0.0/0   ALLOW
200       HTTPS       TCP         443           0.0.0.0/0   ALLOW
300       SSH         TCP         22            10.0.0.0/8  ALLOW
32767     ALL         ALL         ALL           0.0.0.0/0   DENY
```

## 5. DDoS 보호

### AWS Shield
- **AWS Shield Standard**:
  - 모든 AWS 고객에게 무료 제공
  - Layer 3/4 DDoS 공격 보호
  - SYN/UDP 플러드, 반사 공격 등 차단

### AWS Shield Advanced
- **비용**: 월 $3,000 + 데이터 전송 비용
- **기능**:
  - 고급 DDoS 보호
  - 24/7 DRT(DDoS Response Team) 지원
  - DDoS 비용 보호
  - 실시간 공격 알림

```bash
# Shield Advanced 활성화
aws shield subscribe-to-proactive-engagement \
  --proactive-engagement-status ENABLED
```

## 6. VPC 연결 옵션

### VPC Peering
- **정의**: 두 VPC 간의 프라이빗 연결
- **특징**:
  - 1:1 연결만 가능
  - 전이적 라우팅 불가
  - CIDR 블록 겹치면 안됨
- **제한**: Full Mesh 구성 시 연결 수 기하급수적 증가

### Transit Gateway
- **정의**: 여러 VPC와 온프레미스를 연결하는 허브
- **장점**:
  - 중앙 집중식 연결 관리
  - 전이적 라우팅 지원
  - 확장성 우수
- **비용**: 연결당 시간당 요금 + 데이터 처리 요금

```bash
# Transit Gateway 생성
aws ec2 create-transit-gateway \
  --description "Main TGW" \
  --options DefaultRouteTableAssociation=enable,DefaultRouteTablePropagation=enable
```

## 7. 하이브리드 연결

### Site-to-Site VPN
- **용도**: 온프레미스와 VPC 간 VPN 연결
- **구성 요소**:
  - Virtual Private Gateway (VGW)
  - Customer Gateway (CGW)
  - VPN Connection
- **대역폭**: 최대 1.25 Gbps per tunnel

### AWS Direct Connect
- **정의**: 온프레미스와 AWS 간 전용 네트워크 연결
- **장점**:
  - 일관된 네트워크 성능
  - 대역폭 비용 절감
  - 보안성 향상
- **대역폭**: 1Gbps ~ 100Gbps

```bash
# Direct Connect Gateway 생성
aws directconnect create-direct-connect-gateway \
  --name "main-dxgw"
```

## 8. VPC Endpoint

### 인터페이스 엔드포인트 (Interface Endpoint)
- **기술**: AWS PrivateLink 사용
- **대상**: 대부분의 AWS 서비스
- **비용**: 시간당 요금 + 데이터 처리 요금

### 게이트웨이 엔드포인트 (Gateway Endpoint)
- **대상**: S3, DynamoDB만 지원
- **비용**: 무료 (데이터 전송 비용만)
- **라우팅**: 라우팅 테이블을 통해 설정

```bash
# S3 Gateway Endpoint 생성
aws ec2 create-vpc-endpoint \
  --vpc-id vpc-12345678 \
  --service-name com.amazonaws.ap-northeast-2.s3 \
  --route-table-ids rtb-12345678
```

## 9. VPC Flow Logs

### 정의 및 용도
- **정의**: VPC 내 네트워크 인터페이스의 IP 트래픽 정보 캡처
- **용도**:
  - 네트워크 트러블슈팅
  - 보안 분석
  - 트래픽 패턴 분석

### Flow Logs 레벨
- **VPC 레벨**: VPC 내 모든 ENI
- **서브넷 레벨**: 서브넷 내 모든 ENI  
- **ENI 레벨**: 특정 네트워크 인터페이스

### Flow Logs 형식
```
account-id interface-id srcaddr dstaddr srcport dstport protocol packets bytes windowstart windowend action flowlogstatus
```

```bash
# VPC Flow Logs 생성
aws ec2 create-flow-logs \
  --resource-type VPC \
  --resource-ids vpc-12345678 \
  --traffic-type ALL \
  --log-destination-type cloud-watch-logs \
  --log-group-name VPCFlowLogs
```

## 10. 문제 해결

### 일반적인 네트워킹 문제
1. **인터넷 연결 안됨**:
   - 라우팅 테이블 확인
   - 보안 그룹/NACL 확인
   - 퍼블릭 IP 할당 확인

2. **VPC 간 통신 안됨**:
   - VPC Peering 상태 확인
   - 라우팅 테이블 확인
   - CIDR 블록 겹침 확인

3. **온프레미스 연결 안됨**:
   - VPN 터널 상태 확인
   - BGP 라우팅 확인
   - 방화벽 설정 확인

### 디버깅 도구
```bash
# 연결성 테스트
aws ec2 describe-vpc-peering-connections
aws ec2 describe-route-tables
aws ec2 describe-network-acls
aws logs filter-log-events --log-group-name VPCFlowLogs
``` 