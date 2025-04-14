좋아요. 쿠버네티스를 처음 접하는 개발자와 DevOps 엔지니어를 위한 3개월 이상 학습용 한국어 자료를 Markdown 형식으로 정리해드릴게요. 예제 코드도 포함해서 초급부터 고급까지 자연스럽게 실력을 쌓을 수 있도록 구성하겠습니다.

곧 정리된 학습 문서를 보여드릴게요!

# 쿠버네티스 3개월 학습 커리큘럼

## 1. 학습 개요 및 목표 소개

쿠버네티스(Kubernetes)는 컨테이너화된 애플리케이션을 **오케스트레이션**하기 위한 강력한 플랫폼입니다. 이 커리큘럼은 **초급**부터 **고급**까지 3개월 이상의 기간 동안 단계적으로 학습할 수 있도록 구성되었습니다. 개발자 및 DevOps 엔지니어를 대상으로 하며, 각 단계마다 핵심 개념 요약과 실습 예제를 포함하여 **로컬 환경에서도 쉽게 따라할 수 있게** 설계되었습니다. 학습 완료 시 다음과 같은 목표를 달성할 수 있습니다:

- **쿠버네티스 기본 개념 이해:** 컨테이너, Pod, 서비스, 디플로이먼트 등의 핵심 개념을 습득합니다.
- **쿠버네티스 클러스터 사용 능력 함양:** Minikube 등의 도구로 로컬 클러스터를 구축하고 애플리케이션을 배포/관리할 수 있습니다.
- **운영 및 확장 기술 습득:** Helm 패키지 매니저 활용, 설정 관리(ConfigMap/Secret), 모니터링/로그 수집 및 CI/CD 파이프라인 구성 능력을 기릅니다.
- **고급 패턴 적용:** StatefulSet, Operator(오퍼레이터), 네트워크 정책 등을 활용하여 **상태 저장 애플리케이션**이나 **고가용성** 아키텍처를 구현하고, 쿠버네티스 디자인 패턴을 실제 시나리오에 적용해봅니다.
- **종합 실전 능력:** 마이크로서비스 기반의 애플리케이션을 설계부터 **쿠버네티스에 배포**하고 운영하는 최종 프로젝트 경험을 쌓습니다.

본 커리큘럼은 이론과 실습을 병행하여 진행됩니다. 각 장에서는 개념 설명 후 관련 **YAML 구성 예제나 명령어 실행 예제**를 제시하여, 독자가 직접 실행하며 학습 내용을 검증할 수 있도록 합니다. 이제 단계별 커리큘럼을 살펴보겠습니다.

## 2. 초급 과정: Kubernetes 기본 개념 및 실습

초급 과정에서는 쿠버네티스의 전반적인 구조와 핵심 리소스에 대한 이해를 다집니다. **클러스터 설치**부터 **간단한 애플리케이션 배포**까지 실습을 포함하며, 이를 통해 쿠버네티스의 기본 사용 방법을 익힙니다. 주요 학습 주제는 다음과 같습니다:

- 쿠버네티스란 무엇인가 – **아키텍처 개요** (마스터/노드 구성 요소)
- 로컬 환경에 **Minikube**로 쿠버네티스 클러스터 설치 및 사용
- **Pod, Service, Deployment** 등의 기본 리소스 개념과 YAML 작성법
- `kubectl` 커맨드 라인을 통한 리소스 생성, 조회, 삭제 실습

### 2.1 쿠버네티스와 컨테이너 오케스트레이션 이해

쿠버네티스는 구글에서 시작된 Borg 시스템에 뿌리를 둔 **컨테이너 오케스트레이션 플랫폼**입니다. 컨테이너 기술(Docker 등)을 사용하면 애플리케이션의 **환경 이식성**이 높아지지만, 다수의 컨테이너 운영에는 스케줄링, 클러스터 자원 관리, 서비스 디스커버리 등 많은 과제가 있습니다. 쿠버네티스는 이러한 문제를 해결하기 위해 태어났으며, **컨테이너의 배치, 확장, 연결 및 복구**를 자동화함으로써 마이크로서비스 아키텍처를 효과적으로 관리하게 해줍니다. 예를 들어 쿠버네티스는 **노드 장애 시 자동으로 컨테이너를 재시작**하고, **명령 한 줄로 애플리케이션을 확장(스케일링)**할 수 있는 기능을 제공합니다.

쿠버네티스 **클러스터 아키텍처**는 **마스터 노드**(제어 플레인)와 **워커 노드**로 구성됩니다. 마스터는 클러스터 상태를 관리하고 스케줄링을 담당하며, etcd(키-값 저장소), API 서버, 컨트롤러 매니저, 스케줄러 등의 구성 요소로 이루어져 있습니다. 워커 노드는 실제 컨테이너(Pod)가 동작하는 곳으로, **kubelet** (노드 에이전트)과 **kube-proxy** (네트워킹) 등을 실행합니다. 이러한 구조를 이해하면 이후에 등장하는 오브젝트들의 동작 맥락을 파악하는 데 도움이 됩니다.

### 2.2 Minikube를 이용한 로컬 쿠버네티스 설치

학습을 위해서는 손쉽게 쿠버네티스 클러스터를 만들 수 있는 **Minikube**를 사용합니다. Minikube는 로컬 환경(개발 PC)에서 경량 쿠버네티스 클러스터(싱글 노드)를 실행할 수 있는 도구입니다. 다음은 Minikube 설치 및 기본 사용 방법입니다:

1. **Minikube 설치:** 운영 체제에 맞는 바이너리를 다운로드하거나 패키지 매니저를 이용합니다. 또한 쿠버네티스 CLI 도구인 `kubectl`도 설치해야 합니다.
2. **클러스터 시작:** 터미널에서 `minikube start` 명령을 실행하면 VM 또는 컨테이너 드라이버로 쿠버네티스 노드가 생성됩니다. 설치가 완료되면 `kubectl`을 통해 클러스터와 통신할 수 있습니다.
3. **클러스터 동작 확인:** `kubectl cluster-info` 명령으로 쿠버네티스 마스터와 서비스들이 정상 기동되었는지 확인합니다. (`minikube dashboard` 명령으로 웹 대시보드를 켜 시각적으로 확인할 수도 있습니다.)
4. **kubectl 구성:** `kubectl get nodes` 등을 실행하여 노드와 클러스터 정보를 조회해 봅니다. Minikube 노드가 **Ready** 상태로 표시되면 클러스터가 준비된 것입니다.

이제 간단한 애플리케이션을 배포하면서 쿠버네티스 객체들을 배워보겠습니다.

### 2.3 Pod: 컨테이너의 기본 배포 단위

**Pod(파드)** 는 쿠버네티스에서 **가장 작은 배포 단위**입니다. 하나의 Pod은 한 개 이상의 컨테이너와 스토리지 볼륨을 함께 묶어주는 **원자적 단위**이며, 항상 동일한 노드에서 스케줄링됩니다. 컨테이너 오케스트레이션에서 Pod을 사용하는 이유는, 여러 컨테이너가 긴밀히 협력해야 할 경우(예: 로컬 파일 공유, localhost 통신) 하나의 Pod에 담아 **공동 배치 및 스케줄링** 하기 위함입니다. Pod 내부의 컨테이너들은 **네트워크 네임스페이스(IP와 포트 공간)** 를 공유하여 localhost로 통신할 수 있고, **호스트네임**도 공유합니다. 반면 Pod 서로 간에는 격리되어 각각 다른 IP를 가지므로, **Pod간 통신은 네트워크를 통해서만** 이뤄집니다.

- **비유:** Docker에서 개별 컨테이너가 **고래(whale)** 라면, 쿠버네티스의 Pod은 여러 고래를 태운 **고래 무리(Pod)** 라고 할 수 있습니다. 이러한 **Pod 개념** 덕분에 워드프레스(웹)와 MySQL(데이터베이스)처럼 별도로 확장되어야 할 구성요소는 각각 Pod으로 나누고, 컨테이너 간에 꼭 붙어있어야 할 경우(예: 로그 수집 에이전트와 애플리케이션)는 하나의 Pod로 묶는 등의 **설계 전략**을 취할 수 있습니다.

**실습 예제: Hello World Pod 생성** – 간단한 Nginx 웹서버를 실행하는 Pod을 만들어보겠습니다. 다음은 Nginx 이미지를 사용하는 Pod의 매니페스트(YAML) 예시입니다:

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: hello-pod
spec:
  containers:
  - name: hello-container
    image: nginx:latest
    ports:
    - containerPort: 80
```

위 YAML을 `pod-hello.yaml`로 저장한 후, `kubectl apply -f pod-hello.yaml` 명령을 실행하면 쿠버네티스 클러스터에 Pod이 생성됩니다. `kubectl get pods`로 생성된 Pod의 상태가 **Running** 인지 확인하고, `kubectl port-forward pod/hello-pod 8080:80`으로 로컬 포트(`8080`)를 Pod의 Nginx 포트(`80`)에 연결한 뒤 브라우저에서 `localhost:8080`에 접속해보세요. Nginx 환영 페이지가 보이면 **성공적으로 Pod이 배포** 된 것입니다.

이 예제를 통해 **쿠버네티스에 컨테이너(Pod) 배포**를 경험해보았습니다. 하지만 일반적으로 Pod은 위와 같이 직접 생성하기보다는 **디플로이먼트(Deployment)** 와 같은 상위 리소스에 의해 관리됩니다. 다음으로 Deployment와 서비스에 대해 알아보겠습니다.

### 2.4 Deployment 및 ReplicaSet: 애플리케이션 배포와 확장

**Deployment(디플로이먼트)** 는 애플리케이션 배포의 **선언적 관리**를 담당하는 쿠버네티스 객체입니다. Deployment를 생성하면 쿠버네티스는 내부적으로 **ReplicaSet** 을 만들고, ReplicaSet이 다시 다수의 Pod을 관리합니다. Deployment에 원하는 **Pod 개수(replicas)** 와 **Pod 템플릿** 을 명시하면, 시스템은 현재 실행 중인 Pod 수를 지속적으로 감시하면서 Desired 상태를 유지하게 됩니다.

- **ReplicaSet**: ReplicaSet은 **클러스터 전체에서 Pod의 개수를 조절**하는 컨트롤러로, 항상 올바른 종류와 개수의 Pod이 동작하도록 보장합니다. 이를테면 ReplicaSet은 “쿠키 커터+쿠키 수량”에 비유할 수 있는데, 주형(template)에 따라 똑같은 Pod을 원하는 개수만큼 찍어내는 역할을 합니다. ReplicaSet은 Deployment에 의해 자동 생성되거나 독립적으로도 만들 수 있지만, 현재는 Deployment를 통해 관리하는 것이 일반적입니다.

- **Deployment의 롤링 업데이트**: Deployment는 **롤링 업데이트 전략**을 기본으로 하여, 새 버전의 Pod을 점진적으로 생성하고 구 버전의 Pod을 제거하는 방식으로 **무중단 배포**를 제공합니다. 이 과정에서 Deployment는 새로운 ReplicaSet을 생성하고 이전 ReplicaSet과 교체해나가며, `maxSurge`나 `maxUnavailable` 등의 파라미터를 통해 한번에 몇 개의 Pod을 교체할지 제어할 수 있습니다. 만약 문제가 발생하면 Deployment 리소스를 통해 **롤백**도 가능합니다.

**실습 예제: Deployment로 애플리케이션 배포** – 앞서 만든 Nginx Pod를 Deployment로 관리되도록 설정해보겠습니다. Deployment YAML은 다음과 같습니다:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: hello-deployment
spec:
  replicas: 3
  selector:
    matchLabels:
      app: hello
  template:  # 새 ReplicaSet이 생성할 Pod 템플릿
    metadata:
      labels:
        app: hello
    spec:
      containers:
      - name: hello-container
        image: nginx:1.21  # 특정 버전 이미지
        ports:
        - containerPort: 80
```

위 매니페스트(`deploy-hello.yaml`)를 적용하면 (`kubectl apply -f deploy-hello.yaml`), 쿠버네티스는 `hello-deployment` Deployment를 만들고 자동으로 ReplicaSet 및 Pod 3개를 생성합니다. `kubectl get deployments`와 `kubectl get rs`로 Deployment와 ReplicaSet이 생성된 것을 확인할 수 있습니다. 또한 `kubectl describe deployment hello-deployment` 명령어로 Deployment의 상태를 살펴보고, 새로운 ReplicaSet이 3개의 Pod을 실행하여 **레플리카 수가 충족되었는지** 확인해보세요.

이렇게 Deployment를 이용하면 수동으로 Pod을 관리하지 않고도 **애플리케이션의 상태를 선언적으로 관리**할 수 있습니다. Pod의 수를 늘리거나 줄일 때는 `kubectl scale deployment/hello-deployment --replicas=5` 처럼 명령하거나, YAML의 `replicas` 필드를 수정 후 apply하는 방식으로 손쉽게 **수평 확장(스케일 아웃)**을 할 수 있습니다.

### 2.5 Service: Pod에 안정적인 네트워크 연결 제공

쿠버네티스의 **Service(서비스)** 는 **동적인 Pod들 위에 안정적인 네트워크 엔드포인트(IP와 포트)를 제공하는 추상화**입니다. 서비스는 지정된 셀렉터(selector)에 매칭되는 Pod들을 묶어주고, 그 앞에 가상 IP(**ClusterIP**)와 DNS 이름을 부여하여 클러스터 내 다른 구성요소들이 해당 서비스 이름으로 접근할 수 있게 합니다. 이를 통해 **Pod의 생성/삭제로 IP가 바뀌어도** 클라이언트는 항상 동일한 서비스 IP로 통신할 수 있어 **서비스 디스커버리**가 용이해지고, 다수의 Pod에 대한 **로드 밸런싱**도 자동으로 처리됩니다.

- **서비스 셀렉터와 레이블:** 서비스는 **레이블(label)** 을 사용하여 대상 Pod을 선택합니다. 예를 들어 `app: hello` 레이블을 가진 Pod 3개를 앞서 배포했다면, 이들을 모아주는 서비스를 다음과 같이 정의할 수 있습니다:

```yaml
apiVersion: v1
kind: Service
metadata:
  name: hello-service
spec:
  selector:
    app: hello   # 레이블이 "app=hello"인 Pod들을 대상으로
  ports:
  - port: 80            # 서비스 포트 (클러스터 내부 접속용)
    targetPort: 80      # Pod의 컨테이너 포트
    protocol: TCP
  type: ClusterIP       # 클러스터 내부 IP를 통해만 접근 (기본값)
```

이 YAML(`svc-hello.yaml`)을 적용하면 (`kubectl apply -f svc-hello.yaml`), `hello-service`라는 이름으로 서비스가 생성됩니다. 쿠버네티스는 자동으로 해당 서비스에 **클러스터IP**를 할당하고, DNS 서버(kube-dns)가 `hello-service.default.svc.cluster.local` 같은 도메인 이름을 지정합니다. 결과적으로 동일 네임스페이스 내 다른 Pod에서 `hello-service:80`으로 접근하면 3개의 Nginx Pod 중 하나로 트래픽이 라우팅됩니다.

- **서비스 타입:** 기본 타입인 ClusterIP 외에도 **NodePort**, **LoadBalancer** 등의 타입이 있습니다. NodePort를 사용하면 서비스가 각 노드의 특정 포트를 통해 외부에 노출되어 (`<노드IP>:NodePort`) 클러스터 밖에서도 접근할 수 있습니다. 예를 들어 Minikube에서는 `minikube service hello-service` 명령으로 NodePort URL을 쉽게 확인하고 브라우저로 열 수 있습니다. Cloud 환경에서는 LoadBalancer 타입을 사용해 클라우드 제공자의 LB 서비스와 연계하여 퍼블릭 IP를 받을 수도 있습니다.

서비스를 통해 **동적으로 변화하는 Pod 집합을 하나의 논리적인 서비스 단위**로 다룰 수 있게 되었으며, 이는 마이크로서비스 아키텍처에서 서비스 간 통신을 단순화하고 유연하게 만들어줍니다. 또한 서비스는 **백엔드 구현을 교체하거나 Pod 수를 변경하더라도** 프론트엔드에는 영향을 주지 않는 계층을 형성하여, 시스템의 탄력성과 확장성을 높여줍니다.

### 2.6 초급 과정 마무리

지금까지 초급 단계에서 다룬 내용은 다음과 같습니다:

- **쿠버네티스 기본 개념:** 컨테이너 오케스트레이션의 필요성과 쿠버네티스의 역할.
- **클러스터 설치:** Minikube를 사용한 로컬 클러스터 구축 및 kubectl 사용법.
- **핵심 리소스 실습:** Pod 생성, Deployment를 통한 관리형 Pod 배포, Service를 통한 네트워크 연결.

이 단계의 학습을 통해 **쿠버네티스의 기본 사용 방법과 동작 원리**를 습득했습니다. 이제 클러스터에 애플리케이션을 배포하고 노출시키는 기본 사이클을 이해했으므로, 다음 **중급 과정**에서 이를 확장하여 보다 효율적으로 관리하고 운영하는 방법들을 배워보겠습니다.

## 3. 중급 과정: Helm, 설정 관리, 모니터링 및 CI/CD

중급 과정에서는 쿠버네티스를 실제 **운영 환경에 활용**하기 위한 기술과 도구들을 다룹니다. 애플리케이션의 복잡도가 올라갈수록 필요한 **패키지 매니지먼트(Helm)**, 설정관리(ConfigMap/Secret), 로그와 모니터링, 그리고 지속적 통합/배포(CI/CD) 파이프라인 구성 등을 학습합니다. 주요 학습 주제는 다음과 같습니다:

- **Helm 패키지 매니저**를 통한 쿠버네티스 자원 배포 및 관리
- **ReplicaSet**과 **Deployment**의 관계 및 수평 확장/축소(HPA는 고급에서 언급 가능)
- **ConfigMap/Secret**으로 애플리케이션 설정 분리하기
- **로깅 및 모니터링:** 쿠버네티스에서 로그 수집과 모니터링 스택 구성
- **CI/CD 파이프라인 구성:** 빌드부터 배포까지 자동화된 워크플로우 개요

### 3.1 Helm을 이용한 패키지 관리와 배포 자동화

쿠버네티스에서는 애플리케이션 배포 시 여러 리소스(Pod, Service, Deployment, ConfigMap 등)를 한꺼번에 정의해야 하는데, 이를 효율적으로 관리하기 위해 **Helm**이라는 패키지 매니저를 사용합니다. Helm은 다양한 쿠버네티스 객체들을 하나의 **차트(chart)** 로 묶어버전 관리하고 배포할 수 있게 해줍니다. **Chart**는 쿠버네티스 리소스들의 템플릿 집합과 기본 값들(values)을 포함하며, 이를 통해 복잡한 애플리케이션도 손쉽게 패키징할 수 있습니다. Helm을 사용함으로써 얻는 이점은 다음과 같습니다:

- **복잡성 관리:** 다수의 마이크로서비스와 구성 요소로 이루어진 앱도 하나의 차트로 묶어서 일관되게 배포 가능 (예: LAMP 스택, ELK 스택 등).
- **업그레이드/롤백 용이:** `helm upgrade` 명령으로 애플리케이션 업데이트를 적용하고, 문제가 있을 경우 `helm rollback`으로 손쉽게 이전 버전으로 돌아갈 수 있음.
- **차트 공유 및 재사용:** 커뮤니티 또는 사내에서 작성된 차트를 저장소에 올려 공유하고, 설치 시 `helm install <차트>`를 통해 재사용할 수 있음.
- **설정 커스터마이징:** Chart의 기본 값을 사용자의 필요에 맞게 오버라이드하여 (`--set` 플래그나 values.yaml 편집) 하나의 차트를 다양한 환경에 맞게 배포할 수 있음.

**Helm 구성 요소:** Helm은 **클라이언트/서버 아키텍처** 를 가집니다. Helm 클라이언트는 사용자의 로컬에서 명령을 실행하고, 쿠버네티스 클러스터 내에는 Helm v3 기준으로 **서버 사이드 컴포넌트(Tiller)**가 사라지고 **쿠버네티스 API를 통해 직접** 릴리스가 관리됩니다 (Helm v2까지 존재했던 Tiller는 v3부터 제거됨). Chart를 설치하면 해당 릴리스 정보가 쿠버네티스 리소스(Secret 등)로 저장되어 Helm이 상태를 추적합니다.

**실습 예제: Helm으로 Nginx 배포** – Helm이 제대로 설치되었다면, 간단한 예제로 Nginx를 Helm Chart로 배포해봅시다.

1. **차트 검색:** Helm Hub 또는 ArtifactHUB에서 공식 **nginx** 차트를 검색하거나, `helm search repo nginx` 명령으로 차트를 찾습니다.
2. **차트 설치:** `helm install my-nginx bitnami/nginx` 명령을 실행하면 `bitnami` 리포지토리의 nginx 차트를 `my-nginx`라는 릴리스 이름으로 설치합니다. Helm은 필요한 Deployment, Service 등을 자동 생성합니다.
3. **릴리스 확인:** `helm list` 명령으로 설치된 릴리스 목록을 확인하고, `kubectl get pods,svc`로 Nginx Pod과 Service가 생성되었는지 봅니다. (NodePort로 노출되었다면 `minikube service my-nginx`로 바로 접속 가능)
4. **차트 제거:** 실습 후 `helm uninstall my-nginx`로 릴리스를 제거하면 관련 리소스가 모두 삭제됩니다.

이 예제를 통해 Helm이 쿠버네티스 리소스 배포를 **얼마나 간편하게** 만들어주는지 경험했을 것입니다. Helm은 이후의 고급 과정이나 실전 프로젝트에서도 반복 활용하게 되므로, 익숙해져 두는 것이 좋습니다.

### 3.2 ReplicaSet과 Auto-scaling 이해 (중급)

이전 단계에서 Deployment와 ReplicaSet을 다루었지만, 이번에는 **수평 확장**과 **자동 스케일링**에 대해 조금 더 알아보겠습니다. 쿠버네티스는 워크로드의 부하에 따라 Pod 개수를 자동으로 조절해주는 **Horizontal Pod Autoscaler (HPA)** 기능을 제공합니다. (HPA는 CPU 사용률 등의 메트릭을 기준으로 작동하며, 실제 구성은 모니터링 스택이 필요하므로 여기서는 개념만 다룹니다.)

ReplicaSet은 수동 또는 상위 Deployment에 의해 지정된 **replicas 수를 유지**하는 역할을 하지만, HPA를 설정하면 이 replicas 수가 동적으로 변경되도록 허용합니다. 예를 들어 Nginx Deployment에 HPA를 붙여 CPU 사용률 50%를 타겟으로 설정하면, 부하 증가 시 Deployment의 replicas 수가 증가하여 더 많은 Pod이 생성되고, 부하 감소 시 줄어들게 됩니다. 이러한 자동 확장 메커니즘을 통해 **운영자는 효율적으로 자원을 사용**하면서도 **트래픽 피크에 유연하게 대응**할 수 있습니다.

**팁:** Minikube 환경에서는 메트릭 서버(metrics-server)를 추가로 설치해야 HPA를 테스트해볼 수 있습니다. (예: `minikube addons enable metrics-server`).

### 3.3 ConfigMap과 Secret: 설정과 시크릿 관리

실제 애플리케이션을 쿠버네티스에 배포할 때, **환경별 설정값**이나 **민감 정보(비밀번호 등)** 를 코드나 컨테이너 이미지에 하드코딩하지 않고 외부로 분리해야 합니다. 쿠버네티스에서는 이를 위해 **ConfigMap** 과 **Secret** 리소스를 제공합니다.

- **ConfigMap:** 환경 설정 등의 일반 텍스트 데이터를 저장하기 위한 객체입니다. 예를 들어 애플리케이션의 설정 파일, 설정 값(key-value) 등을 ConfigMap에 미리 담아두고 Pod에서 이를 **환경변수**  또는 **볼륨 마운트** 형태로 불러와서 사용합니다. ConfigMap을 사용하면 설정 변경 시 이미지를 다시 빌드하지 않고도 값을 교체할 수 있고, 여러 Pod에서 공통 ConfigMap을 참조하도록 하여 설정을 중앙관리할 수 있습니다.

- **Secret:** API 키, DB 비밀번호 같은 **민감 정보** 를 취급하기 위한 객체로, ConfigMap과 사용 방법은 거의 같지만 내용이 base64로 인코딩되어 저장되고 접근 권한도 좀 더 엄격하게 제어됩니다. Secret은 노출을 방지하기 위해 주로 **환경 변수**로 마운트하거나, 또는 볼륨으로 마운트하여 파일로 접근하게 합니다. (쿠버네티스 시크릿은 완벽한 보안을 보장하지는 않지만, 평문으로 값을 두는 것보다 안전합니다.)

**실습 예제: ConfigMap 및 Secret 적용** – 간단한 Node.js 애플리케이션을 쿠버네티스에 배포하면서 ConfigMap과 Secret을 적용해보겠습니다. 이 앱은 환경변수로 `WELCOME_MESSAGE`를 받아 해당 메시지를 출력한다고 가정합니다.

1. **ConfigMap 생성:** 먼저 환영 메시지를 담은 ConfigMap을 만듭니다. YAML 파일 `config.yaml`:
   ```yaml
   apiVersion: v1
   kind: ConfigMap
   metadata:
     name: hello-config
   data:
     WELCOME_MESSAGE: "쿠버네티스에 오신 것을 환영합니다!"
   ```
   생성: `kubectl apply -f config.yaml`

2. **Secret 생성:** DB 접속 비밀번호 같은 민감 정보를 Secret에 저장합니다 (`db-password: mypass` 예시). `secret.yaml`:
   ```yaml
   apiVersion: v1
   kind: Secret
   metadata:
     name: hello-secret
   type: Opaque
   data:
     DB_PASSWORD: bXlwYXNz  # "mypass"의 base64 인코딩 값
   ```
   생성: `kubectl apply -f secret.yaml`

3. **Deployment에서 사용:** Node.js 앱을 실행하는 Deployment를 작성할 때, 위 ConfigMap과 Secret을 참조합니다. `deployment.yaml` (발췌):
   ```yaml
   spec:
     containers:
     - name: hello-app
       image: myrepo/hello-node:latest
       env:
       - name: WELCOME_MESSAGE
         valueFrom:
           configMapKeyRef:
             name: hello-config
             key: WELCOME_MESSAGE
       - name: DB_PASSWORD
         valueFrom:
           secretKeyRef:
             name: hello-secret
             key: DB_PASSWORD
   ```
   이처럼 `env` 섹션에 `...KeyRef`를 사용하여 ConfigMap과 Secret의 값을 환경변수로 주입할 수 있습니다. (`kubectl apply -f deployment.yaml`로 배포)

4. **확인:** Pod가 뜬 후 `kubectl exec -it <pod명> -- printenv | grep WELCOME_MESSAGE`로 환경변수가 제대로 설정되었는지 확인합니다. 또는 애플리케이션 로그를 통해 메시지가 출력되는지 검증합니다.

이 과정을 통해 설정과 민감정보를 이미지에서 분리하여 쿠버네티스 리소스로 관리하는 방법을 배웠습니다. **베스트 프랙티스**로, ConfigMap과 Secret을 잘 활용하면 애플리케이션의 **환경 구성 변경을 유연하게 처리**할 수 있고 시크릿 노출을 최소화할 수 있습니다.

### 3.4 로깅과 모니터링

컨테이너화 된 마이크로서비스 환경에서는 **중앙화된 로깅 및 모니터링** 체계가 필수적입니다. 쿠버네티스는 기본적으로 표준 출력(stdout/stderr) 로그를 수집하며, `kubectl logs` 명령으로 각 Pod의 로그를 확인할 수 있습니다. 그러나 프로덕션 수준에서는 **모든 Pod의 로그를 한곳에 모아 검색**하고, **메트릭을 수집해 대시보드로 시각화**하는 것이 중요합니다.

- **로그 수집 (EFK 스택 등):** 쿠버네티스 환경에서 흔히 사용되는 로그 수집 스택은 **EFK**(Elasticsearch + Fluentd + Kibana)입니다. 각 노드에 **Fluentd(DaemonSet으로 구성)** 에이전트를 배치하여 노드 상의 모든 컨테이너 로그를 수집하고, 이를 Elasticsearch에 저장한 후 Kibana를 통해 조회/시각화합니다. 최근에는 **Grafana Loki**나 **Elastic Cloud** 등의 대안도 각광받고 있습니다. 중급 단계에서는 Fluentd가 설치되어 있다고 가정하고 Kibana에서 네임스페이스별 로그를 검색하는 정도를 실습해볼 수 있습니다.

- **모니터링 (Prometheus 등):** 쿠버네티스의 메트릭 수집에는 **Prometheus**가 사실상 표준입니다. Prometheus는 쿠버네티스와 연동하여 클러스터 상태(Pod CPU/메모리 사용률 등)와 애플리케이션 메트릭(HTTP 요청율, 에러율 등)을 스크랩하고, Grafana로 대시보드를 구성해 시각화합니다. Minikube에서는 애드온으로 간단히 **Metrics Server**를 활성화해 기본 리소스 모니터링을 해볼 수 있고, 더 나아가 Prometheus Operator를 사용해 풀 스택을 구성할 수 있습니다.

쿠버네티스 1.x 버전에서는 **Heapster**와 **Dashboard**가 기본 모니터링 도구였으나, 현재는 Heapster가 더 이상 사용되지 않고 Prometheus 기반으로 전환되었습니다. 예를 들어, **cAdvisor**(모든 노드에 내장된 컨테이너 모니터링 에이전트)가 노드별 메트릭을 수집하면, **metrics-server**나 **Prometheus**가 이를 모아 HPA나 모니터링 UI에 활용하는 식입니다.

**실습 예제: 간단한 모니터링 구성** – 이 단계에서는 Minikube에 간략히 모니터링 툴을 구성해보겠습니다.

1. **Metrics Server 활성화:** `minikube addons enable metrics-server` 실행. 몇 분 후 `kubectl get deployment metrics-server -n kube-system` 등으로 배포 상태 확인.
2. **Dashboard 확인:** `minikube addons enable dashboard`로 쿠버네티스 대시보드 활성화 후, `minikube dashboard` 명령으로 웹 UI 접속. (여기서 각 리소스의 상태, 기본 메트릭 정보를 볼 수 있음)
3. **Grafana 설치 (선택):** Helm으로 Prometheus와 Grafana를 설치하는 것은 고급 주제이나, **Kube Prometheus Stack** 차트를 사용하면 쉽게 배포 가능. `helm install monitoring prometheus-community/kube-prometheus-stack` 등으로 설치한 뒤 Grafana 대시보드에서 클러스터 메트릭을 관찰해볼 수 있습니다.

**로그 확인:** 애플리케이션 Pod 여러 개를 배포한 후 고의로 에러를 발생시켜 봅니다. 그런 다음 Kibana나 Grafana Loki(설치했다면)의 검색창에서 해당 에러 로그를 찾아보세요. 이를 통해 중앙 로그의 이점을 체감할 수 있습니다 (모든 Pod의 로그를 한눈에 확인하고 필터링 가능).

로그와 모니터링은 운영 환경에서 **문제 감지와 성능 튜닝의 핵심**입니다. 중급 단계에서는 이론과 경량 실습을 통해 개념을 잡고, 필요에 따라 더 깊이 있는 도구 사용법은 고급이나 실전 프로젝트에서 다룹니다.

### 3.5 간단한 CI/CD 파이프라인 구성

쿠버네티스를 활용하면 애플리케이션의 빌드부터 배포까지 **CI/CD(지속적 통합 및 전달)** 파이프라인을 구현하기 좋습니다. 중급 과정의 마지막으로, 쿠버네티스에 배포하는 CI/CD 파이프라인의 개념과 예시를 알아보겠습니다.

**CI/CD 파이프라인 이해:** CI/CD 파이프라인은 개발자가 코드에 변경을 가하면 자동으로 빌드, 테스트, 컨테이너 이미지화, 배포까지 일련의 단계가 수행되도록 구성된 워크플로우입니다. 일반적인 단계는 다음과 같습니다:
1. **소스 코드 푸시** -> CI 시스템이 감지 (예: GitHub Actions, Jenkins, GitLab CI 등)
2. **컨테이너 이미지 빌드 및 푸시:** Docker를 이용해 새로운 애플리케이션 이미지를 빌드하고 Registry(ECR, Docker Hub 등)에 업로드.
3. **쿠버네티스 배포 업데이트:** 새 이미지로 Deployment를 업데이트. 이는 `kubectl apply`를 CI 툴에서 스크립트로 실행하거나, **Helm 차트를 사용**한다면 `helm upgrade`를 실행함. (또는 Argo CD 같은 GitOps 도구를 사용하기도 함)
4. **배포 후 테스트:** 배포된 새 버전이 정상 동작하는지 헬스체크나 통합 테스트 수행.
5. **완료 및 피드백:** 성공 여부를 팀에 알리고 모니터링을 통해 이상 없는지 확인.

쿠버네티스는 컨테이너 중심이므로, **CI 단계에서 이미 컨테이너 이미지를 산출**한다는 점이 전통적인 VM 배포와 차이입니다. 그리고 kubelet이 이미지 교체 및 롤백을 지원하므로 배포 단계를 간소화하고 신뢰성을 높일 수 있습니다.

**예시 시나리오:** 예를 들어, 애플리케이션 코드를 Git 저장소에 푸시하면 Jenkins가 트리거되어 `docker build`로 이미지를 만들고 레지스트리에 태그를 푸시합니다. 이후 `kubectl set image deployment/hello-deployment hello-container=myimage:새태그` 명령을 통해 쿠버네티스 Deployment의 이미지를 업데이트하면, 롤링 업데이트가 시작되어 새로운 버전으로 교체됩니다. Jenkins 파이프라인 코드는 다음과 유사할 것입니다:

```groovy
stage('Build') {
    sh 'docker build -t myrepo/hello-node:$BUILD_NUMBER .'
    sh 'docker push myrepo/hello-node:$BUILD_NUMBER'
}
stage('Deploy') {
    sh 'kubectl set image deploy/hello-deployment hello-container=myrepo/hello-node:$BUILD_NUMBER'
}
```

물론 실제로는 각 단계에 대한 에러 처리를 하고, Canary 배포나 Blue-Green 배포 전략을 적용하기도 합니다. 그러나 위의 간략한 파이프라인만으로도 코드 푸시 -> 배포 업데이트가 자동화되어 **개발 생산성**과 **배포 신뢰성**이 크게 향상됩니다.

중급 과정의 내용을 정리하면, 우리는 쿠버네티스 사용을 더욱 **효율화**하고 **현실화**하는 방법들을 배웠습니다. Helm으로 복잡한 배포를 관리하고, ConfigMap/Secret으로 구성 관리를 개선했으며, 로깅/모니터링 체계를 이해하고, CI/CD를 통해 **지속적 배포**로 가는 길을 살펴보았습니다. 다음은 **고급 과정**으로, 보다 깊은 쿠버네티스 활용법과 패턴을 다루겠습니다.

## 4. 고급 과정: 오퍼레이터, 상태 저장 서비스, 고가용성 및 네트워크 심화

고급 과정에서는 쿠버네티스의 내부 동작과 확장 기능을 활용하여 **복잡한 시나리오**를 처리하는 방법을 학습합니다. Operator 패턴으로 대표되는 **쿠버네티스 API 확장**, **StatefulSet**을 이용한 상태 저장 애플리케이션 운영, 멀티 노드 **고가용성(HA)** 클러스터 구성, 그리고 **네트워크 보안/정책** 등을 다룹니다. 또한 앞서 언급된 내용들을 아우르는 **쿠버네티스 패턴** 개념도 소개합니다. 주요 학습 항목은 다음과 같습니다:

- **오퍼레이터(Operator)** 를 통한 쿠버네티스 기능 확장 및 자동화
- **StatefulSet**과 볼륨을 이용한 상태ful 애플리케이션 배포
- 쿠버네티스 클러스터 및 애플리케이션의 **고가용성 구성 전략**
- **네트워크 정책(NetworkPolicy)**을 통한 미세한 트래픽 제어와 보안
- **쿠버네티스 패턴 활용** (사이드카, 앰배서더 등 구조적 패턴의 심화 활용)

### 4.1 오퍼레이터 (Operator) 패턴과 쿠버네티스 API 확장

쿠버네티스 **오퍼레이터(Operator)** 는 쿠버네티스 기능을 **사용자 정의 리소스**로 확장하여, 특정 애플리케이션의 배포와 운영을 자동화하는 패턴입니다. **Custom Resource Definition (CRD)** 를 통해 새로운 리소스 종류를 정의하고, 그 리소스의 생명주기를 관리하는 **컨트롤러(Controller)** 를 구현함으로써 마치 쿠버네티스 자체가 해당 애플리케이션을 “특별 지원”하는 것처럼 동작하게 할 수 있습니다. 오퍼레이터 패턴의 핵심은 **도메인 전문 지식의 자동화**입니다:

- 사람 운영자(Human operator)가 특정 애플리케이션을 관리할 때 하는 복잡한 절차(예: 백업, 복구, 스케일 아웃, 업그레이드 등)를 코드로 옮겨서 **쿠버네티스 컨트롤 루프**로 실행되게 함으로써, 수동 개입 없이도 애플리케이션이 바람직한 상태를 유지하도록 합니다.
- 예를 들어 **Etcd Operator**는 etcd 클러스터의 백업/복제를 자동화하고, **Prometheus Operator**는 Prometheus 및 Alertmanager의 배포를 쉽게 해줍니다. 이러한 오퍼레이터들은 쿠버네티스 CRD를 통해 `EtcdCluster`나 `Prometheus` 같은 새로운 객체를 도입하고, 이를 읽어들여 필요한 Deployment, Service, ConfigMap 등을 생성/관리합니다.

**오퍼레이터의 동작:** 오퍼레이터는 본질적으로 쿠버네티스 **컨트롤러**의 특수한 경우입니다. 컨트롤러는 실제 상태를 원하는 상태와 맞추는 역할을 하는데, 쿠버네티스 기본 컨트롤러들은 ReplicaSet, Deployment, StatefulSet 등에 대해 이미 존재합니다. 오퍼레이터를 만들면 우리가 원하는 대상(예: 데이터베이스 클러스터)에 대한 컨트롤러를 추가로 작성하게 되는 것입니다. 컨트롤러 러닝 루프는 다음과 같이 동작합니다:
1. Custom Resource(예: `MyDatabase`)를 사용자가 생성하거나 수정.
2. 해당 CR을 감시(watch)하고 있던 오퍼레이터 컨트롤러가 이벤트를 수신.
3. CR 명세에 정의된 Desired 상태에 맞게 관련 리소스(Pod, Service, Volume 등)를 생성 또는 조정.
4. 지속적으로 실제 상태를 모니터링하여, 원하는 상태와 다르면 다시 조치.

**학습 예제:** 직접 오퍼레이터를 개발하는 것은 고급 주제이므로, 여기서는 **기존 오퍼레이터 배포**를 통해 개념을 익혀봅니다. 대표적으로 **MongoDB Community Operator**를 설치하여 MongoDB Replica Set 클러스터를 구성해보겠습니다.

- **Operator 설치:** 쿠버네티스 1.16+ 환경에서, YAML 매니페스트 (또는 OLM - Operator Lifecycle Manager 사용)로 MongoDB Operator를 설치합니다. 이를 통해 `MongoDBCommunity`라는 CRD가 추가됩니다.
- **CR 생성:** 다음과 같은 YAML로 MongoDBCommunity 객체를 생성합니다:
  ```yaml
  apiVersion: mongodb.com/v1
  kind: MongoDBCommunity
  metadata:
    name: my-mongo
  spec:
    members: 3
    type: ReplicaSet
    version: "4.2.6"
    persistent: true
  ```
  Operator가 이 리소스를 감지하여 MongoDB ReplicaSet에 필요한 StatefulSet 3개 및 관련 Service 등을 자동으로 생성합니다.
- **상태 확인:** `kubectl get pods`로 `my-mongo-0`, `my-mongo-1`, `my-mongo-2` Pod가 running 중인지 확인하고, MongoDB에 접속하여 3개 복제본이 구성되었는지 확인합니다.
- **자동 치유 실험:** 임의로 `kubectl delete pod my-mongo-0`를 실행하면 Operator가 이를 감지하고 새로운 Pod을 생성하여 다시 ReplicaSet 크기를 맞춥니다. 이처럼 Operator는 **특정 애플리케이션에 특화된 복구/확장 논리**를 가지고 동작합니다.

이를 통해 쿠버네티스 Operator가 얼마나 강력하게 애플리케이션 운영을 자동화할 수 있는지 경험했습니다. Operator 패턴은 쿠버네티스의 경계를 넓혀, 데이터베이스, 메시지 큐 등의 **상태ful 시스템도 쿠버네티스에서 원활히 운영**할 수 있게 해주며, 최근에는 많은 클라우드 네이티브 소프트웨어들이 오퍼레이터를 통해 배포되고 있습니다.

### 4.2 StatefulSet과 상태 저장 애플리케이션

쿠버네티스 초기 버전은 **스테이트리스(stateless)** 애플리케이션 (예: 웹 서버 팜) 운영에 적합했지만, **데이터베이스나 로그 처리기** 같은 **상태 저장(stateful)** 애플리케이션 관리에는 어려움이 있었습니다. 이를 개선하기 위해 도입된 것이 **StatefulSet** 리소스입니다. StatefulSet은 **고유한 아이덴티티와 영구 스토리지를 필요로 하는 Pod들을 관리**하기 위한 컨트롤러입니다. 주요 특징은 다음과 같습니다:

- **고정된 네트워크 ID:** StatefulSet이 생성한 각 Pod에는 순차적인 이름(예: `mysql-0`, `mysql-1`...)과 함께 **헤드리스 서비스**를 통한 도메인 이름이 부여되어, Pod 개체 하나하나가 네트워크에서 **고유한 정체성**을 갖습니다. 예를 들어 `statefulset 이름-인덱스` 형식으로 DNS가 등록되므로, 클러스터 내에서 `mysql-0.mysql.default.svc.cluster.local`처럼 특정 노드에 항상 접근 가능합니다.
- **순서 보장 배포/종료:** Pod을 하나씩 순서대로 생성하고, 종료할 때도 역순으로 하나씩 처리합니다. 이를 통해 **애플리케이션에 따라 정해진 부트 순서**(예: Zookeeper처럼 노드0이 마스터 역할을 함)가 필요한 시나리오를 지원합니다.
- **PersistentVolume 연계:** StatefulSet과 함께 쓰이는 **PersistentVolumeClaim (PVC)**은 각 Pod에 1:1로 매칭되어, Pod이 재시작되거나 노드가 바뀌어도 **동일한 저장소를 재마운트**합니다. 이로써 Pod가 재Scheduled되더라도 데이터를 잃지 않고 연속성을 가질 수 있습니다.

**실습 예제: StatefulSet으로 MySQL 배포** – 간단한 예로 MySQL 데이터베이스를 StatefulSet으로 배포해보겠습니다.

1. **스토리지 클래스 준비:** 로컬 환경에서는 hostPath를 사용하거나, Minikube의 `standard` 스토리지 클래스를 사용할 수 있습니다. 이미 기본 StorageClass가 있다면 그대로 진행합니다.
2. **StatefulSet YAML 작성:** `mysql-statefulset.yaml`:
   ```yaml
   apiVersion: apps/v1
   kind: StatefulSet
   metadata:
     name: mysql
   spec:
     serviceName: "mysql"              # Headless Service 이름
     replicas: 2
     selector:
       matchLabels:
         app: mysql
     template:
       metadata:
         labels:
           app: mysql
       spec:
         containers:
         - name: mysql
           image: mysql:5.7
           env:
           - name: MYSQL_ROOT_PASSWORD
             value: example
           volumeMounts:
           - name: data
             mountPath: /var/lib/mysql
     volumeClaimTemplates:
     - metadata:
         name: data
       spec:
         accessModes: [ "ReadWriteOnce" ]
         resources:
           requests:
             storage: 1Gi
   ```
   여기서 `serviceName: "mysql"`은 Headless Service로, 반드시 별도로 `kind: Service`에 clusterIP: None 설정으로 만들어주어야 합니다. `volumeClaimTemplates` 섹션은 각 Pod에 `data`라는 PVC를 1Gi씩 생성하도록 합니다.
3. **적용 및 확인:** `kubectl apply -f mysql-statefulset.yaml` 및 Headless Service 생성 YAML도 적용합니다. 생성 후 `kubectl get pods`를 보면 `mysql-0`부터 순서대로 생성되는 것을 볼 수 있습니다. `kubectl get pvc`로 `data-mysql-0`, `data-mysql-1` PVC가 만들어졌는지 확인합니다.
4. **네트워크 아이덴티티 확인:** 동일 네임스페이스에서 `nslookup mysql-0.mysql` 등을 해보면 DNS 응답이 오는 것을 확인할 수 있습니다. 만약 `mysql-0` Pod을 삭제한 후 다시 생기면, 동일한 이름과 볼륨으로 복원되어 데이터 일관성이 유지됩니다.

StatefulSet은 이러한 특성 때문에 **분산 데이터베이스, 키-밸류 스토어, 메시지 브로커** 등 상태정보와 멤버별 고유함이 필요한 시스템에 적합합니다. 다만, StatefulSet 자체가 복제를 보장하거나 데이터를 동기화해주지는 않으므로, 애플리케이션 레벨에서의 **복제/샤딩 메커니즘**은 별도로 고려해야 합니다.

### 4.3 고가용성 구성: 쿠버네티스 클러스터와 애플리케이션

**고가용성(High Availability, HA)** 이란 시스템이 일부 구성 요소에 장애가 발생하더라도 전체 서비스는 지속 운영되는 능력을 뜻합니다. 쿠버네티스 환경에서 HA를 논할 때 두 가지 측면이 있습니다: **쿠버네티스 자체의 HA**와 **애플리케이션 레벨의 HA**.

- **쿠버네티스 제어 플레인 HA:** 프로덕션 환경에서는 쿠버네티스 마스터 노드가 단일 장애점이 되지 않도록 여러 대로 구성합니다. etcd도 클러스터링하여 (일반적으로 3개 이상의 홀수 개) 복제본을 두고, API 서버 역시 여러 노드에서 동작시키며 로드 밸런서를 앞단에 둡니다. 이러한 구성을 통해 마스터 노드 한두 대가 죽어도 클러스터가 동작을 이어갈 수 있습니다. (노드 장애 시 새 선출이 일어날 때까지 잠깐 조정이 중단될 순 있지만, 워커 노드의 기존 Pod들은 영향 없이 움직입니다.)
- **애플리케이션 HA:** 쿠버네티스 Deployment나 StatefulSet을 활용하면 이미 애플리케이션 레벨 HA의 기본이 마련됩니다. 동일한 애플리케이션 Pod을 다중 실행하여 한 Pod이 죽어도 서비스가 지속되게 하고(Replica= n), 쿠버네티스 서비스가 트래픽을 자동 재분배합니다. 또한 Pod을 다른 노드들에 **반반** 배치하여 (예: **Anti-affinity** 설정으로 동일한 호스트에 몰리지 않게) **노드 장애 시 피해 범위를 줄이는** 전략도 사용할 수 있습니다. 예를 들어 3개의 Pod이 각각 별도 노드에서 돌고 있고, 그 중 한 노드가 다운되면, 남은 2개의 Pod이 서비스하며, 쿠버네티스는 자동으로 다른 정상 노드에 새 Pod을 스케줄링하여 복구합니다.

**고가용성 실습 포인트:** Minikube로는 멀티 노드 환경을 구성하기 어렵지만, 개념적인 실습으로 Deployment의 replica를 1과 3으로 바꿔가며 가용성 차이를 비교해볼 수 있습니다. `replicas: 1`로 두고 Pod을 강제 종료하면 (`kubectl delete pod`), 새 Pod이 뜨는 동안 서비스 중단이 발생하지만, `replicas: 3`인 경우에는 하나를 삭제해도 다른 Pod들이 즉시 응답하여 서비스 중단이 없는 것을 확인할 수 있습니다.

고급 과정에서는 HA를 달성하기 위한 **설계 원칙** 도 함께 짚고 넘어갑니다:
- **무상태 서비스 우선:** 가능하다면 상태 공유를 없애고 각 인스턴스를 독립적으로 만들어 필요한 수만큼 띄워두기.
- **stateful 서비스 이중화:** 반드시 상태를 가져야 한다면 StatefulSet 등을 활용해 복제본을 유지하거나, 외부 관리형 DB 서비스로 위임.
- **다중 가용 영역 고려:** 클라우드 환경에서는 노드를 여러 가용 영역(AZ)에 걸쳐 배치하여 자연재해나 네트워크 단절에도 한 영역의 실패로 전체 장애가 나지 않게 구성.

### 4.4 네트워크 정책(Network Policy)을 통한 보안 강화

쿠버네티스 클러스터 내 모든 Pod들은 기본적으로 **평등하게 서로 통신 가능** 합니다. 마이크로서비스 환경에서는 수십 수백 개의 Pod가 존재하므로, **서비스 간 트래픽 제어**와 **격리**가 필요할 수 있습니다. **네트워크 정책(NetworkPolicy)** 은 쿠버네티스에서 Pod 레벨에서 적용하는 **방화벽 규칙**으로, 특정 Pod 그룹에 대한 인그레스/이그레스 트래픽 허용/차단 규칙을 정의합니다.

- **정책의 동작 방식:** 네트워크 정책은 **기본 Deny, 선별 Allow** 개념으로 동작합니다. 아무 정책도 없는 네임스페이스에서는 모든 Pod 통신이 허용되지만, 일단 어떤 Pod에 네트워크 정책이 적용되면 해당 정책에서 명시적으로 허용한 트래픽 외에는 거부됩니다. 정책은 **레이블 셀렉터**로 대상 Pod(예: role=db 라벨이 붙은 Pod들)을 지정하고, 허용할 트래픽의 조건을 정의합니다 (어떤 **namespace** 혹은 **Pod 라벨**에서 오는 트래픽만 허용할지, 포트/프로토콜 제한 등).
- **실습 예시:** 다음은 네임스페이스 `prod`에 적용할 네트워크 정책 예시입니다. "role=db 라벨을 가진 Pod들은 같은 네임스페이스의 role=frontend 라벨 가진 Pod로부터 오는 TCP 3306(MySQL) 포트 트래픽만 허용하고, 그 외에는 거부"하는 정책입니다.
  ```yaml
  apiVersion: networking.k8s.io/v1
  kind: NetworkPolicy
  metadata:
    name: db-allow-mysql
    namespace: prod
  spec:
    podSelector:
      matchLabels:
        role: db
    policyTypes: ["Ingress"]
    ingress:
    - from:
      - podSelector:
          matchLabels:
            role: frontend
      ports:
      - protocol: TCP
        port: 3306
  ```
  이 정책을 적용하면, 만약 `prod` 네임스페이스의 `role=db`인 Pod (예: MySQL Pod)에 다른 라벨을 가진 Pod이 접속 시도하거나, 혹은 port 3306이 아닌 포트로 접근하면 모두 차단됩니다. 오직 role=frontend 라벨의 Pod들만 MySQL 포트로 접근 가능하죠.
- **CNI 플러그인 유의:** 네트워크 정책 기능은 쿠버네티스 네이티브 기능이지만, 실제 시행(enforcement)은 네트워크 플러그인(CNI)에 의해 이뤄집니다. Calico, Weave Net, Cilium 등 대부분의 CNI는 네트워크 정책을 지원하지만, 기본 Kubenet 드라이버나 일부 간단한 플러그인은 지원하지 않을 수 있습니다. Minikube 사용 시 `--network-plugin=cni` 및 적절한 애드온 설정을 통해 Calico 등을 활성화하면 정책 테스트를 할 수 있습니다.

네트워크 정책을 잘 활용하면, 동일 클러스터 내에서도 **환경(개발/운영)별 격리**나 **서비스 그룹 간 접근 통제**를 구현할 수 있습니다. 이는 **제로 트러스트 보안 모델**에 부합하며, 특히 멀티테넌시나 보안이 중요한 워크로드에서 필수적으로 고려됩니다.

### 4.5 고급 과정 마무리 및 패턴 소개

고급 과정에서는 쿠버네티스의 **확장성과 유연성**을 최대한 활용하는 방법들을 살펴보았습니다. Operator를 통해 쿠버네티스의 **자체 한계를 넘어서는 자동화**를 구현하고, StatefulSet으로 **데이터를 가진 애플리케이션**도 쿠버네티스에서 안정적으로 운영할 수 있음을 배웠습니다. 또한, 고가용성 개념과 네트워크 정책을 통해 **운영 환경에서의 안정성**과 **보안**을 강화할 수 있음을 확인했습니다.

이제 마지막으로, **"Kubernetes Patterns" 책에 소개된 패턴들**을 검토하면서 우리가 다룬 개념들을 한층 추상화되고 구조화된 시각에서 재정리해보겠습니다. 이는 실전 프로젝트에 앞서 **베스트 프랙티스와 디자인 패턴**을 살펴보는 단계입니다.

## 5. 패턴 및 실전 예제: Kubernetes Patterns 활용

쿠버네티스 패턴은 쿠버네티스 상에서 자주 등장하는 **설계 과제들을 해결한 모범 사례**들을 일관된 용어로 정리한 것입니다. 이 절에서는 *Kubernetes Patterns* 책의 구성을 참고하여, 중요한 패턴들을 소개하고 간단한 실습 예제를 제시합니다. 패턴들은 크게 **기초 패턴**, **행위 패턴**, **구조 패턴**, **설정 패턴**, **고급 패턴**으로 분류할 수 있습니다:

- **Foundational Patterns (기초 패턴):** 쿠버네티스의 기본 철학과 원리를 담은 패턴 (예: **자동화 가능한 단위(Automatable Unit)** 로서의 Pod, **선언적 배포(Declarative Deployment)** 등).
- **Behavioral Patterns (행위 패턴):** 작업(Job), 스케줄(CronJob), 데몬(DaemonSet) 등 **특정 동작 형태**의 패턴.
- **Structural Patterns (구조 패턴):** 컨테이너들의 **구성 및 협력 구조**에 관한 패턴 (예: **사이드카(Sidecar)**, **앰배서더(Ambassador)**, **어댑터(Adapter)**).
- **Configuration Patterns (설정 패턴):** 애플리케이션 설정을 다루는 방법 (예: **EnvVar 기반 설정**, **ConfigMap/Secret 리소스 활용**, **Immutable configuration** 등).
- **Advanced Patterns (고급 패턴):** 쿠버네티스의 확장과 전문화된 활용 (예: **상태ful 서비스 패턴(Stateful Service)**, **커스텀 컨트롤러/오퍼레이터 패턴**).

여기서는 주요 패턴 몇 가지를 선정하여 설명하고, 필요시 간단한 실습 예제를 추가합니다.

### 5.1 자동화 가능한 단위 (Automatable Unit)와 선언적 배포

**자동화 가능한 단위 패턴**은 쿠버네티스에서 모든 것을 기계적으로 처리하고 자동화할 수 있는 단위로 보는 관점을 말합니다. 예를 들어, 쿠버네티스는 애플리케이션 컨테이너 하나가 아닌 **Pod 단위로 스케줄링**함으로써, Pod을 **원자적(atomic)** 으로 생성/삭제/재시작할 수 있게 합니다. 이는 앞서 학습한 Pod 개념과 일맥상통하며, **Pod은 하나 이상의 협력 컨테이너를 포함하는 배포 단위**라는 원칙이죠. 이러한 기초 패턴 덕분에 쿠버네티스는 복잡한 시스템도 일관된 방법으로 자동화할 수 있습니다.

**선언적 배포(Declarative Deployment) 패턴**은 애플리케이션을 배포할 때 **절차**를 기술하는 대신 최종 **상태**를 선언함으로써 쿠버네티스가 알아서 그 상태에 도달하도록 하는 접근을 말합니다. Deployment 객체를 사용하여 “Replicas 5개, 이미지 버전 v2”와 같이 원하는 상태만 명시하면, 쿠버네티스가 내부적으로 ReplicaSet을 조정하고 롤링 업데이트를 수행하여 그 상태를 달성합니다. 이 패턴은 IaC(Infrastructure as Code)의 중요 요소로, 시스템의 현재 상태를 한눈에 파악하고 재현 가능하게 해줍니다.

**실습 예제:** 선언적 배포의 이점을 체감하기 위해, 의도적으로 Deployment의 상태를 변경해보는 실습을 합니다. 예를 들어, 프론트엔드 Deployment를 `replicas: 4`에서 `replicas: 2`로 YAML 수정 후 적용하면 (`kubectl apply`), 쿠버네티스는 자동으로 2개의 Pod를 제거하여 새로운 선언 상태에 맞춥니다. 반대로 imperative하게 Pod를 수동 제거해도, Deployment 컨트롤러가 이를 감지하고 다시 생성(자동 self-healing)하는 것을 확인할 수 있습니다. 이를 통해 **선언형 관리의 자동 복구력**을 직접 확인할 수 있습니다.

### 5.2 배치 작업 패턴 (Batch Job)과 예약 작업 패턴 (Scheduled Job)

일반적인 서비스와 달리, **짧은 수명**을 가지고 한번 실행 후 끝나는 작업들도 있습니다. 쿠버네티스에서는 **Job** 리소스로 **배치 작업(batch job)**을 표현하며, 필요한 개수의 Pod를 생성하여 주어진 작업을 완료하면 자동으로 종료합니다. 예를 들어 데이터베이스 마이그레이션 작업이나, 1회성 대용량 연산 작업 등을 Job으로 실행할 수 있습니다.

- **Job 패턴:** Job은 **재시도와 보장 실행**에 초점을 둡니다. 지정한 Pod 컨테이너가 성공(exit 0)할 때까지 재시도하며, 정해진 횟수만큼 성공 상태에 도달하면 완료됩니다. spec 안에 `completions: N`과 `parallelism: M`을 지정하여 **N개의 성공 완료**를 목표로 병렬로 M개씩 작업을 진행할 수도 있습니다 (예: 작업을 10번 처리해야 하면 completions=10, parallelism=2로 두 개씩 동시에 처리).

- **CronJob 패턴:** 주기적으로 반복되는 작업의 경우 **CronJob** 리소스를 사용합니다. CronJob은 Cron 스케줄 표현식에 따라 특정 시간에 Job을 생성하여 실행합니다. 백업 작업을 매일 새벽 1시에 돌린다거나, 주기적 리포트를 매주 생성하는 등의 시나리오를 CronJob으로 구현할 수 있습니다. CronJob spec에는 `schedule: "0 1 * * *"` 같은 크론표현식과, 실패 시 다음 일정 다루기(`concurrencyPolicy`, `startingDeadlineSeconds` 등) 옵션을 지정합니다.

**실습 예제: CronJob으로 정기 작업 실행** – 간단한 CronJob을 만들어보겠습니다. 1분마다 실행되며 현재 시간을 출력하는 작업입니다.
```yaml
apiVersion: batch/v1
kind: CronJob
metadata:
  name: hello-cron
spec:
  schedule: "*/1 * * * *"       # 매 1분마다
  jobTemplate:
    spec:
      template:
        spec:
          containers:
          - name: hello
            image: busybox
            args:
            - /bin/sh
            - -c
            - date; echo "Hello from CronJob"
          restartPolicy: OnFailure
```
`kubectl apply -f cron.yaml`로 생성하고 2~3분 기다린 후 `kubectl get jobs`를 보면 매 분 생성된 Job들이 기록되는 것을 볼 수 있습니다. `kubectl logs job/<job이름>`으로 각 작업의 출력을 확인해보세요. CronJob을 제거할 때는 `kubectl delete cronjob hello-cron`을 실행하면 연결된 현재 Job들도 함께 제거됩니다 (`successfulJobsHistoryLimit` 등의 설정에 따라 보존될 수도 있음).

이처럼 Job과 CronJob 패턴은 쿠버네티스에서 **일회성 작업**과 **정기 작업**을 관리하기 위한 표준 방법을 제시합니다. 기존에 외부 스케줄러로 관리하던 Cron 작업을 모두 클러스터 내에서 통합 관리할 수 있다는 장점이 있습니다.

### 5.3 데몬 서비스 패턴 (Daemon Set)

**데몬 서비스(DaemonSet)** 패턴은 **모든 (혹은 선택된) 노드에서 하나씩 실행되어야 하는 Pod**를 관리하는 방법입니다. 시스템 모니터 에이전트, 로그 수집기(앞서 언급한 Fluentd), 노드 로컬 DNS 캐시 등은 클러스터 내 **각 노드마다** 반드시 한 개씩 돌려야 효과적입니다. DaemonSet 리소스는 새로운 노드가 추가되면 자동으로 Pod을 하나 스케줄링하고, 노드가 제거되면 해당 Pod도 제거하여 이러한 요구 사항을 만족합니다.

DaemonSet의 spec은 Deployment와 유사하지만 `spec.template`만 있고 `replicas` 대신 모든 노드에 1개씩이라는 암묵적 규칙을 갖습니다 (특정 레이블을 가진 노드에만 배치하도록 nodeSelector나 affinity를 사용할 수 있음). DaemonSet으로 실행된 Pod은 ReplicaSet 없이 DaemonSet 컨트롤러에 의해 직접 관리됩니다.

**예시:** `kubectl get ds -A`로 쿠버네티스 기본 DaemonSet들을 살펴보면, kube-system 네임스페이스에 `kube-proxy`, `metrics-server`(옵션), 혹은 CNI 플러그인의 DaemonSet들이 보일 것입니다. 이러한 컴포넌트들은 **모든 노드**에서 구동되어야 하므로 DaemonSet으로 구현되어 있습니다.

**실습:** 개발자는 DaemonSet을 직접 만드는 일이 흔하진 않지만, 커스텀 노드 에이전트를 배포해야 할 경우 DaemonSet을 작성할 수 있습니다. 예를 들어, 각 노드의 특정 디렉토리를 주기적으로 정리하는 간단한 쉘 스크립트를 Pod으로 만들고 DaemonSet으로 배포해볼 수 있습니다 (노드 간 하나씩 실행되는지 확인).

DaemonSet 패턴을 통해 **쿠버네티스 클러스터 전역에 걸친 작업/서비스의 배포**를 일관되게 수행할 수 있으며, 이는 시스템 수준 기능을 구현할 때 매우 유용합니다.

### 5.4 사이드카 패턴 (Sidecar)

**사이드카 패턴**은 쿠버네티스 **구조 패턴** 중 가장 널리 알려진 개념으로, 한 Pod 안에 **메인 애플리케이션 컨테이너**와 **보조 컨테이너**를 함께 넣어 동작시키는 것을 말합니다. 보조 컨테이너는 주 컨테이너에 새로운 기능을 추가하거나 기존 기능을 보완하는 역할을 합니다. 사이드카 패턴을 적용하면 다음과 같은 장점이 있습니다:

- 각 컨테이너는 **관심사의 분리**(Separation of Concerns)를 통해 자기 역할에만 집중합니다. 예를 들어, 메인 컨테이너는 애플리케이션 로직만 담당하고, 사이드카 컨테이너는 로그 수집이나 설정 파일 동기화 등을 전담할 수 있습니다.
- 컨테이너별로 **개발 주기**와 **스택**을 분리할 수 있습니다. 서로 다른 팀이 각 컨테이너를 관리하거나, 언어/런타임이 달라도 한 Pod에서 협력 가능하므로 유연성이 증가합니다.
- 사이드카를 모듈처럼 재활용할 수 있습니다. 예를 들어, 표준화된 로그 수집 사이드카나 프록시 사이드카(Envoy 등)는 여러 애플리케이션 Pod에 공통 적용하여 기능을 쉽게 추가할 수 있습니다.

**예시:** 대표적인 사이드카 활용 예로 **로그 에이전트**를 들 수 있습니다. 애플리케이션 컨테이너는 파일이나 stdout으로 로그를 남기고, 같은 Pod의 사이드카인 Fluentd 컨테이너가 이를 실시간 수집하여 외부 로그 시스템으로 전송합니다. 둘은 `emptyDir` 볼륨을 공유하여 파일을 교환합니다. 이러한 구조를 사용하면 애플리케이션 코드를 수정하지 않고도 로그 전송 기능을 추가할 수 있습니다.

또 다른 예는 **프록시 사이드카**로, **서비스 메쉬**(예: Istio)의 Envoy 프록시가 각 Pod에 사이드카로 주입되어 트래픽 가로채기, 관찰, 보안을 담당합니다. 메인 컨테이너는 순수 비즈니스 로직만 처리하고, 공통 통신 로직은 사이드카가 맡는 구조입니다.

**실습 예제: 사이드카로 간단한 파일 동기화** – 한 컨테이너가 주기적으로 파일을 생성하고, 다른 컨테이너가 이를 모니터링해 로그로 출력하는 사이드카 구성을 만들어봅니다:

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: sidecar-example
spec:
  volumes:
  - name: shared-data
    emptyDir: {}
  containers:
  - name: writer
    image: busybox
    args:
    - /bin/sh
    - -c
    - "i=0; while true; do echo \"Sidecar test $i\" > /output/data.txt; i=$((i+1)); sleep 5; done"
    volumeMounts:
    - name: shared-data
      mountPath: /output
  - name: reader
    image: busybox
    args:
    - /bin/sh
    - -c
    - "tail -f /input/data.txt"
    volumeMounts:
    - name: shared-data
      mountPath: /input
```

이 Pod YAML에는 `writer` 컨테이너와 `reader` 컨테이너가 `emptyDir` 볼륨(`/output`과 `/input` 경로)로 연결되어 있습니다. writer는 5초마다 파일을 쓰고, reader는 그 파일을 지속적으로 읽어 터미널에 출력합니다. `kubectl apply`로 Pod 생성 후 `kubectl logs sidecar-example -c reader -f`로 로그를 팔로우해 보면, writer가 쓰는 내용을 reader가 잘 읽어오는 것을 확인할 수 있습니다. 이처럼 사이드카 컨테이너를 활용하여 서로 협업하는 작업을 구현할 수 있습니다.

### 5.5 앰배서더 패턴 (Ambassador)과 어댑터 패턴 (Adapter)

사이드카 패턴의 변형으로, **Ambassador(대사) 패턴**과 **Adapter(적응자) 패턴**이 있습니다. 이들 또한 Pod 내의 여러 컨테이너 협업 모델이지만, 목적이 약간 다릅니다:

- **앰배서더 패턴:** 앰배서더 컨테이너는 주 컨테이너를 대신해 **외부 세계와 통신을 담당하는 프록시** 역할을 합니다. 예를 들어, 주 컨테이너는 외부 DB에 직접 연결하는 대신 localhost의 앰배서더 컨테이너에 요청을 보내고, 앰배서더가 외부 DB와 통신합니다. 이렇게 하면 주 컨테이너는 외부 주소나 인증 등을 몰라도 되고, 앰배서더가 복잡한 통신 세부사항(프로토콜 변환, 보안 등)을 처리할 수 있습니다. 즉, **외부 서비스에 대한 어댑터**를 Pod 안에 두는 셈입니다. 이 패턴의 이점은 개발자가 단순한 인터페이스(localhost와 통신)에만 신경 쓰면 되고, 외부 연동 로직은 앰배서더가 맡아 **의존성 분리**와 **테스트 용이성**을 높인다는 것입니다.

- **어댑터 패턴:** 어댑터 컨테이너는 주 컨테이너가 제공하는 출력이나 인터페이스를 외부 시스템이 기대하는 형식으로 **변환**해주는 역할을 합니다. 가령, 애플리케이션은 자체 포맷의 메트릭을 노출하지만 어댑터 컨테이너가 이를 표준 Prometheus 포맷으로 변환하여 `/metrics` 엔드포인트로 재노출할 수 있습니다. 이렇게 하면 애플리케이션 코드를 수정하지 않고도 기존 시스템에 통합할 수 있습니다. 어댑터 패턴은 레거시 시스템 통합이나, 서로 다른 두 컴포넌트 간 **인터페이스 mismatch를 해결**할 때 주로 사용됩니다.

앰배서더와 어댑터 패턴은 사이드카의 특수한 경우로 볼 수 있으며, **모든 컨테이너가 동등한 협업**을 하는 사이드카와 달리 **주-보조** 관계가 더 뚜렷합니다. Kubernetes Patterns 책에서는 이들을 별도 패턴으로 명명하여, 의도를 명확히 구분하고 있습니다.

**예시:** 앰배서더 패턴의 실 예로 Envoy를 DB 프록시로 사용하는 케이스를 들 수 있습니다. 애플리케이션은 localhost:5432(PostgreSQL 표준 포트)로 연결하지만, 실제 Envoy 사이드카가 이를 받아 회사 내부 VPC의 실제 DB로 TLS 연결을 맺어주는 식입니다. 이러면 앱은 **DB 연결 방식에 무관**하게 동작하고, 보안이나 라우팅 정책은 사이드카가 처리합니다.

어댑터 패턴의 예로는, 기존 애플리케이션이 출력하는 로그를 JSON으로 변환해주는 사이드카를 붙여서 ELK 스택에 바로 읽히도록 만드는 경우를 생각할 수 있습니다. 또는 애플리케이션이 JMX로 내보내는 메트릭을 Prometheus exporter 사이드카가 읽어 `/metrics` HTTP로 변환해주는 JMX Exporter 사례 등이 있습니다.

**실습:** 앰배서더/어댑터는 특정 상황에 따라 구현이 다르므로 간단 실습으로 체감하기는 어렵습니다. 대신 학습자는 위 개념을 이해하고, 자신이 만든 Pod에 적용할 수 있는 보조 컨테이너 아이디어를 떠올려 보는 연습을 합니다. (예: “내 웹서버 컨테이너 앞에 Nginx를 프록시로 붙이면 어떤 이점이 있을까?” 등)

### 5.6 설정 패턴: ConfigMap, Secret 및 Immutable 설정

이 부분은 이미 3.3절에서 ConfigMap과 Secret 사용을 다루었으므로 간략히 패턴의 맥락만 언급합니다. **Configuration Patterns**에서는 애플리케이션 설정을 쿠버네티스 리소스로 외부화하는 방법과, 이를 활용한 다양한 전략을 소개합니다. 핵심 내용:

- **EnvVar Configuration Pattern:** 모든 설정을 환경 변수로 주입하는 방식. 12-Factor App 원칙을 따르는 앱에 적합하며, ConfigMap/Secret을 환경 변수로 매핑해서 사용합니다.
- **Configuration Resource Pattern:** 설정을 파일이나 복잡한 구조로 관리해야 할 경우 ConfigMap을 **볼륨으로 마운트**하여 애플리케이션이 설정 파일을 읽게 하거나, 혹은 쿠버네티스 CRD를 통해 설정 자체를 커스텀 리소스로 다루는 접근입니다.
- **Configuration Template Pattern:** 애플리케이션이 필요로 하는 동적 설정 파일(예: 템플릿)을 ConfigMap에 템플릿 형태로 저장해두고, 실제 실행 시기에 사이드카나 초기화 컨테이너가 이를 렌더링(예: 환경 변수값 채워넣기)하여 사용할 수 있게 하는 패턴입니다.
- **Immutable Configuration Pattern:** 설정이 한 번 정해지면 가급적 불변으로 두고, 변경이 필요할 때는 새 ConfigMap을 만들어 참조를 바꾸는 식으로 처리하는 것입니다. 이를 통해 설정 변경이력 관리와 롤백을 용이하게 합니다.

실전에서는 **ConfigMap/Secret의 버전관리**가 과제인데, Kubernetes Patterns 책은 이를 패턴으로 정리하여 제안하고 있습니다. 예를 들어, Deployement spec의 `configMapKeyRef`에 ConfigMap 이름을 직접 쓰는 대신, 이름에 버전을 넣어(`my-config-v2` 등) 업데이트 시 참조명을 바꾸고 Pod을 재배포하는 방법 등이 있습니다.

### 5.7 고급 패턴: 상태 서비스와 커스텀 컨트롤러

고급 패턴으로 **Stateful Service**와 **Custom Controller**가 책에 소개되어 있습니다. Stateful Service는 앞서 StatefulSet과 연관된 내용으로, 분산 상태를 다루는 디자인 원칙들을 다룹니다. **Custom Controller**는 우리가 4.1절에서 다룬 Operator 패턴과 같은 개념으로, 쿠버네티스의 컨트롤러를 커스텀하게 작성하여 도메인별 요구사항을 구현하는 것입니다. 이러한 패턴들은 이미 실습과 함께 개념을 설명했으므로 여기서 구체 예제를 반복하지는 않겠습니다.

정리하면, **패턴 학습의 의의**는 우리가 쿠버네티스를 활용하면서 접하게 되는 문제들을 **공식화**하여 솔루션을 패턴화한 것을 배우는 것입니다. 이를 통해 새로운 상황에 직면했을 때 과거의 유사 사례를 떠올려 해결책을 도출하기 쉽습니다. 예컨대 "여러 컨테이너가 필요한데 배포 주기가 다르면? -> 사이드카 패턴 적용", "특정 노드에서만 동작해야 하면? -> DaemonSet 사용", "X 시스템을 쿠버네티스에서 운용하려면? -> 혹시 이미 Operator가 있는지 조사" 등의 사고를 할 수 있게 됩니다.

## 6. 마무리 프로젝트: 마이크로서비스 애플리케이션 쿠버네티스 배포

이제 마지막 단계로, 앞서 배운 모든 지식을 활용하여 **마이크로서비스 기반 애플리케이션을 쿠버네티스에 설계 및 배포**하는 종합 실습을 진행합니다. 이 프로젝트를 통해 쿠버네티스 학습 커리큘럼을 마무리하며, 실전 적용 감각을 익히게 됩니다.

### 6.1 프로젝트 개요

가상의 시나리오로, **간단한 블로그 플랫폼**을 마이크로서비스 아키텍처로 개발했다고 가정합니다. 구성은 아래와 같습니다:

- **프론트엔드 서비스 (frontend)**: React 또는 Vue.js 등으로 작성된 SPA, 정적인 파일을 Nginx로 서빙하거나 Node.js 서버가 지원.
- **게시글 서비스 (posts-service)**: 백엔드 API, Node.js (Express) 또는 Python (Flask/FastAPI) 등으로 구현, REST API 제공.
- **사용자 서비스 (user-service)**: 별도 마이크로서비스로 회원 관리 API 제공.
- **데이터베이스 (MongoDB)**: 게시글과 사용자 정보를 저장하는 NoSQL DB.
- **관리용 도구 (admin-tool)**: 예를 들어 백업/통계를 위한 배치 작업 (CronJob으로 실행).

각 서비스는 독립적인 컨테이너 이미지로 패키징되어 있고, 쿠버네티스 상에서 서로 통신하며 동작한다고 가정합니다. 우리의 목표는 이 모든 컴포넌트를 쿠버네티스 클러스터에 배포하고 서로 연동되도록 구성하는 것입니다.

### 6.2 아키텍처 설계 및 Kubernetes 리소스 구성

프로젝트를 시작하기 전에, 큰 그림에서 쿠버네티스 상에 어떤 리소스들이 필요할지 설계해보겠습니다:

- 각 마이크로서비스(프론트엔드, posts, user)는 **Deployment**로 배포하고 여러 Pod 복제본으로 구동하여 가용성과 성능을 확보.
- 내부 통신을 위해 서비스 간 **Service** 객체를 생성 (예: posts-service Deployment 앞에 `posts-service` ClusterIP 서비스 생성).
- 외부에서 접속해야 하는 프론트엔드와 (백엔드 API도 외부 공개로 할 경우) posts-service에 **Ingress** 리소스나 LoadBalancer Service를 사용하여 접근 경로 제공. (개발 환경에서는 NodePort로 대체 가능)
- MongoDB 데이터베이스는 **StatefulSet**으로 배포하고, PersistentVolumeClaim을 통해 영구 스토리지 확보. 또는, 운영 환경에서는 RDS같은 외부 관리형 DB를 사용한다고 가정하고 해당 연결 정보만 Kubernetes에 설정(Secret)으로 저장.
- 환경 설정: 각 서비스별 필요한 환경 변수 (예: DB 연결 문자열, 외부 API 키 등)를 ConfigMap/Secret으로 관리.
- CI/CD: 애플리케이션 코드는 이미 컨테이너 이미지로 빌드/푸시되며, 우리는 배포 manifest만 적용하면 최신 이미지를 배포한다고 가정. (고급 단계로 실제 CI 파이프라인을 구성해볼 수도 있음)
- 모니터링/로깅: 프로젝트 범위를 벗어나지만, kube-prometheus-stack이나 EFK 스택이 설치되어 있다고 간주하고 애플리케이션 Pod에 필요한 메트릭/로그 주입 설정 (예: stdout 로그 사용) 정도만 언급.

### 6.3 구현 단계

이제 실제로 쿠버네티스 매니페스트를 작성하고 배포해봅시다. (규모가 큰 프로젝트이므로 여기선 개략적인 절차와 예시만 기술합니다.)

1. **네임스페이스 설정:** `blog-platform`이라는 별도 네임스페이스를 만들어 모든 리소스를 격리합니다.
   ```bash
   kubectl create namespace blog-platform
   ```

2. **백엔드 서비스 배포 (posts, user):** 각 서비스에 대해 Deployment와 Service YAML 작성.
   - *posts-service.yaml*:
     ```yaml
     apiVersion: apps/v1
     kind: Deployment
     metadata:
       name: posts-service
       namespace: blog-platform
     spec:
       replicas: 3
       selector:
         matchLabels:
           app: posts
       template:
         metadata:
           labels:
             app: posts
         spec:
           containers:
           - name: posts-app
             image: myregistry/posts-service:1.0.0
             ports:
             - containerPort: 3000  # 예: Express 앱이 3000 포트 사용
             env:
             - name: MONGO_URL
               valueFrom:
                 secretKeyRef:
                   name: blog-secrets
                   key: mongo_url
     ---
     apiVersion: v1
     kind: Service
     metadata:
       name: posts-service
       namespace: blog-platform
     spec:
       selector:
         app: posts
       ports:
       - port: 3000
         targetPort: 3000
       type: ClusterIP
     ```
     (user-service도 유사하게 작성, `app: user` 레이블로 구분)
   - 여기서 데이터베이스 연결 문자열 `MONGO_URL`은 미리 생성한 Secret `blog-secrets`에 key로 저장되어 있다고 가정합니다. 예를 들어 `mongo_url: "mongodb://mongo-0.mongo:27017/blog"` 형태로, StatefulSet으로 배포될 MongoDB의 Headless Service DNS를 가리킵니다.

3. **데이터베이스(StatefulSet) 배포:** MongoDB를 ReplicaSet (3개 노드)로 구성해보겠습니다.
   - *mongo-stateful.yaml*:
     ```yaml
     apiVersion: v1
     kind: Service
     metadata:
       name: mongo
       namespace: blog-platform
     spec:
       ports:
       - port: 27017
         name: db
       clusterIP: None  # Headless 서비스
       selector:
         app: mongo
     ---
     apiVersion: apps/v1
     kind: StatefulSet
     metadata:
       name: mongo
       namespace: blog-platform
     spec:
       serviceName: "mongo"
       replicas: 3
       selector:
         matchLabels:
           app: mongo
       template:
         metadata:
           labels:
             app: mongo
         spec:
           containers:
           - name: mongod
             image: mongo:4.4
             args: ["--replSet", "rs0", "--bind_ip_all"]
             ports:
             - containerPort: 27017
               name: db
             volumeMounts:
             - name: mongo-data
               mountPath: /data/db
       volumeClaimTemplates:
       - metadata:
           name: mongo-data
         spec:
           accessModes: [ "ReadWriteOnce" ]
           resources:
             requests:
               storage: 5Gi
     ```
     - 이 매니페스트는 3개의 MongoDB Pod (`mongo-0`...`mongo-2`)을 생성하고, 각각 5Gi 영구볼륨을 claim합니다. MongoDB를 ReplicaSet 모드로 띄우기 위해 args로 replSet 설정을 주었습니다. (별도의 initContainer를 사용해 `rs.initiate()` 명령으로 초기화하는 로직이 실제로는 필요하지만, 지면상 생략합니다.)
     - posts-service와 user-service는 위 Headless Service `mongo`를 통해 MongoDB에 접근하게 됩니다. (예: `mongo-0.mongo.blog-platform.svc.cluster.local`)

4. **프론트엔드 배포:** 프론트엔드는 정적 파일을 서빙한다고 가정하고, ConfigMap을 이용해 HTML 파일을 제공해보겠습니다 (실제론 이미지화되어 있을 수도 있음).
   - *frontend-deploy.yaml*:
     ```yaml
     apiVersion: apps/v1
     kind: Deployment
     metadata:
       name: frontend
       namespace: blog-platform
     spec:
       replicas: 2
       selector:
         matchLabels:
           app: frontend
       template:
         metadata:
           labels:
             app: frontend
         spec:
           containers:
           - name: nginx
             image: nginx:alpine
             ports:
             - containerPort: 80
             volumeMounts:
             - name: html
               mountPath: /usr/share/nginx/html
           volumes:
           - name: html
             configMap:
               name: frontend-html
     ---
     apiVersion: v1
     kind: ConfigMap
     metadata:
       name: frontend-html
       namespace: blog-platform
     data:
       index.html: |
         <!DOCTYPE html>
         <html><head><title>Blog</title></head>
         <body>
           <h1>Welcome to the Blog</h1>
           <div id="posts"></div>
           <script src="app.js"></script>
         </body></html>
       app.js: |
         // JavaScript to fetch posts from API and display
         fetch('/api/posts').then(res => res.json()).then(data => {
           const container = document.getElementById('posts');
           data.forEach(post => {
             const div = document.createElement('div');
             div.innerText = post.title;
             container.appendChild(div);
           });
         });
     ---
     apiVersion: v1
     kind: Service
     metadata:
       name: frontend
       namespace: blog-platform
     spec:
       selector:
         app: frontend
       ports:
       - port: 80
         targetPort: 80
       type: NodePort  # 간단히 NodePort로 외부 노출 (Minikube 터널 또는 LoadBalancer 환경 가정)
     ```
     - 이 예에서는 ConfigMap에 간단한 index.html과 app.js를 저장하고 Nginx 컨테이너에 볼륨으로 마운트했습니다. (실제 애플리케이션이라기보다 구성 예제를 보여주기 위함)
     - 프론트엔드 JS는 `/api/posts` 경로로 요청을 보낼텐데, 이를 백엔드 `posts-service`로 보내기 위해 **Ingress**를 구성하거나 프록시 설정이 필요합니다. 간단히 하기 위해 NodePort를 사용했으므로, Minikube 환경에서는 `minikube service frontend -n blog-platform`으로 프론트엔드를 띄우고, 프론트엔드 컨테이너의 Nginx 설정에 프록시 패스가 없다면 `/api/posts` 요청은 그대로 NodePort로 들어와 프론트엔드 컨테이너가 404를 줄 것입니다. 따라서 Nginx 설정을 ConfigMap으로 override하여 `/api/` prefix를 `posts-service`로 프록시하는 설정을 추가해야 할 수 있습니다. (이 부분은 심화되므로 개념적으로 언급만 합니다.)
     - 또는, 더 표준적으로 **Ingress 리소스**를 만들어 프론트엔드와 백엔드 경로를 분기할 수 있습니다. 예: `host: blog.local`에 대해 `/ -> frontend Service`, `/api/posts -> posts-service`로 Path 분기. Minikube에서는 ingress 애드온과 `/etc/hosts` 설정을 통해 테스트 가능합니다.

5. **애플리케이션 연동 및 검증:** 모든 컴포넌트가 배포되었다면, 이제 각각 잘 통신하는지 테스트합니다.
   - `kubectl get all -n blog-platform`으로 Deployment, Pod, Service, StatefulSet 등이 모두 정상 (`Running`/`Ready`) 상태인지 확인.
   - 프론트엔드 접속 테스트: NodePort URL 또는 Ingress를 통해 브라우저 접속, 게시글 리스트가 보이는지 확인.
   - API 테스트: `kubectl port-forward svc/posts-service -n blog-platform 8080:3000` 후 `curl localhost:8080/posts` 등으로 API 응답 확인.
   - DB 연결 테스트: MongoDB Pod에 접속하여 (`kubectl exec -it mongo-0 -n blog-platform -- mongosh`) 데이터베이스에 posts, users 데이터가 쓰였는지 확인.
   - 장애 상황 테스트: posts-service의 Pod을 일부러 삭제하거나 (`kubectl delete pod ...`), MongoDB Pod을 삭제하여 자동 복구(재생성)되는지 관찰. 또한 posts-service Deployment의 replicas를 0으로 스케일 다운했다가 다시 3으로 올려 서비스 중단 없이 복구되는지 확인.

6. **CI/CD 및 운영 고려:** (선택 사항) 프로젝트를 마무리하며, CI/CD 파이프라인에서 이 쿠버네티스 배포를 활용하려면 어떻게 해야 할지 토론합니다. 예를 들어:
   - 코드가 변경되면 새 이미지 빌드 & 푸시, `kubectl set image` 또는 `helm upgrade`로 배포 업데이트.
   - DB 마이그레이션이 필요하면 Job으로 migration 작업 실행.
   - 모니터링(alert) 설정하여 posts-service 에러율이 높아지면 슬랙 알림 등.

### 6.4 마무리 및 회고

3개월여의 커리큘럼 여정을 통해, 우리는 쿠버네티스의 기초부터 고급까지 폭넓은 개념과 기술을 학습했습니다. 마지막 프로젝트에서는 이 모든 요소를 실전 시나리오에 적용하여 **통합적인 이해**를 증진시켰습니다.

쿠버네티스는 학습 범위도 넓고 일찍 진화하고 있지만, **핵심 개념(Pod, 서비스, 컨트롤러)**은 비교적 안정적이고 그 위에 생태계가 확장되는 형태입니다. 학습을 지속하면서 **공식 문서**와 **커뮤니티**를 참고하면 최신 동향 (예: 새 리소스나 베스트 프랙티스 변화)에 적응할 수 있을 것입니다.

이 커리큘럼을 끝마친 시점에서 독자는 다음과 같은 역량을 갖추었을 것입니다:

- 쿠버네티스 클러스터를 직접 구성하고 관리할 수 있는 능력 (Minikube, kubeadm 등 활용).
- 컨테이너화된 애플리케이션을 쿠버네티스에 배포하고 서비스로 노출시키는 **운영 스킬**.
- Helm 등 도구를 통한 **복잡한 배포 관리**, 그리고 ConfigMap/Secret/Ingress 등의 리소스를 활용한 **환경 구성**.
- 클라우드 네이티브 애플리케이션 개발 시 고려해야 할 **패턴**들을 인지하고, 상황에 맞게 적용하는 설계 마인드.
- 실제 현업 수준의 마이크로서비스를 쿠버네티스 위에 올리고, CI/CD와 연계하여 **개발에서 운영(DevOps)까지** 일련의 프로세스를 이해.

마지막으로, 기술은 실제로 활용해보면서 체득하는 것이 가장 중요합니다. 독자는 마무리 프로젝트 이후에도 자신의 응용 시나리오에 쿠버네티스를 적용해 보거나, 오픈소스 쿠버네티스 프로젝트에 기여하는 등 실습을 이어나가길 권장합니다. 쿠버네티스의 원리를 깊게 이해하고 활용할수록 **개발 생산성**과 **서비스 신뢰성** 모두를 향상시킬 수 있을 것입니다.

쿠버네티스 학습을 마친 것을 축하드리며, 이제 **클라우드 네이티브 여정**에 힘찬 발걸음을 내딛으시길 바랍니다!