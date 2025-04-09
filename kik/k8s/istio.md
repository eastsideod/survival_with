좋아요. Istio 공식 문서를 기반으로, 서비스 메시 아키텍처에 중점을 두고 마이크로서비스 운영을 위한 Kubernetes 환경에서의 Istio 활용에 대한 내용을 초급자에서 고급자로 성장할 수 있도록 정리해드릴게요. 마크다운 파일로 구성하여 학습 경로, 개념 설명, 실습 예제 등도 포함하겠습니다.
잠시만 기다려 주세요. 정리되는 대로 보여드릴게요.

# 초보자부터 고급 사용자까지 Istio 서비스 메쉬 학습 가이드

**Istio 소개**: Istio는 Kubernetes 환경에서 널리 사용되는 오픈 소스 서비스 메쉬로, 애플리케이션의 코드 변경 없이도 **보안**, **관측성(모니터링)**, **트래픽 관리** 등의 기능을 인프라 계층에서 제공해줍니다 ([Istio / The Istio service mesh](https://istio.io/latest/about/service-mesh/#:~:text=A%20service%20mesh%20is%20an,is%20a%20graduated%20project%20in)). 마이크로서비스 아키텍처에서 발생하는 통신상의 도전과제를 해결하며, Kubernetes나 가상 머신 등 다양한 환경에 걸쳐 일관된 서비스 네트워크를 구축할 수 있습니다 ([Istio / The Istio service mesh](https://istio.io/latest/about/service-mesh/#:~:text=Istio%20is%20not%20confined%20to,included%20within%20a%20single%20mesh)). 이 가이드는 Istio 공식 문서를 기반으로 **초급**, **중급**, **고급** 단계별로 Istio의 개념과 기능을 설명하고, 각 단계에서 실습할 과제와 실제 활용 사례, 참고 자료를 제공합니다.

## 초급 단계 (Beginner)

초급 단계에서는 **Istio 서비스 메쉬의 기본 개념**과 **아키텍처**에 초점을 맞춥니다. Istio를 처음 설치하고 간단한 애플리케이션에 적용해 보면서, 사이드카 프록시를 통한 기본 트래픽 처리와 기본적인 모니터링 기능을 체험합니다.

### 개념 및 기능

- **서비스 메쉬 아키텍처**: Istio의 서비스 메쉬는 **데이터 플레인**(Data Plane)과 **컨트롤 플레인**(Control Plane)으로 논리적으로 구분됩니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=An%20Istio%20service%20mesh%20is,plane%20and%20a%20control%20plane)). 데이터 플레인은 각 마이크로서비스에 사이드카로 배치된 **Envoy 프록시**들의 집합으로 구성되며, 서비스들 간 모든 네트워크 통신을 중재하고 트래픽 데이터를 수집합니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=An%20Istio%20service%20mesh%20is,plane%20and%20a%20control%20plane)). 컨트롤 플레인은 이러한 프록시들을 제어하고 구성(config)하여 트래픽 라우팅 등을 관리하는 역할을 합니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=An%20Istio%20service%20mesh%20is,plane%20and%20a%20control%20plane)). 즉, Istio에서는 **Istiod**라는 컨트롤 플레인 컴포넌트가 환경의 서비스 정보를 수집하고, 사용자의 설정을 Envoy 사이드카들에게 전파함으로써 서비스 메쉬 전체를 조율합니다.

- **사이드카 프록시 (Envoy)**: Istio는 각 서비스 인스턴스 옆에 **Envoy** 프록시 사이드카를 자동으로 주입하여(**사이드카 패턴**) 트래픽을 가로채고 제어합니다. Envoy 프록시는 고성능 C++로 구현된 프록시로, 들어오고 나가는 서비스 트래픽을 투명하게 처리합니다. 이를 통해 애플리케이션을 수정하지 않고도 **동적 서비스 검색**, **로드 밸런싱**, **TLS 종단 처리**, **HTTP/2 및 gRPC 지원**, **회로 차단기**, **헬스 체크** 등의 네트워크 기능과 **단계적 트래픽 배포(퍼센트 기반 트래픽 분할)**, **장애 주입**, **풍부한 모니터링 지표 수집** 등을 즉시 활용할 수 있습니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=,Rich%20metrics)). 이러한 사이드카 모델 덕분에 기존 애플리케이션 코드를 재작성하거나 아키텍처를 변경하지 않고도 Istio의 다양한 기능을 추가할 수 있습니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=This%20sidecar%20deployment%20allows%20Istio,behavior%20of%20the%20entire%20mesh)). 사이드카 프록시는 메쉬 트래픽에 대한 **정책 적용**과 **세밀한 모니터링(텔레메트리)** 데이터를 추출할 수 있게 해주며, 메쉬 전반의 동작을 관찰하는 기반을 제공합니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=This%20sidecar%20deployment%20allows%20Istio,behavior%20of%20the%20entire%20mesh)).

- **기본 트래픽 처리**: Istio를 설치하면 클러스터 내 서비스들의 DNS 이름과 엔드포인트 정보가 Istiod를 통해 자동으로 수집되고 서비스 레지스트리에 등록됩니다 ([Istio / Traffic Management](https://istio.io/latest/docs/concepts/traffic-management/#:~:text=In%20order%20to%20direct%20traffic,and%20endpoints%20in%20that%20cluster)). Envoy 사이드카들은 이 정보를 토대로 메쉬 내 목적지 서비스를 인식하고, 각 서비스의 여러 인스턴스(다수의 Pod)에 대한 **로드 밸런싱**을 수행합니다 ([Istio / Traffic Management](https://istio.io/latest/docs/concepts/traffic-management/#:~:text=Using%20this%20service%20registry%2C%20the,loaded%20than%20any%20other%20host)). 기본적으로 Envoy 사이드카들은 **최소활성요청(Least Request)** 알고리즘 등을 사용하여 부하를 분산하고, Kubernetes 서비스의 클러스터IP 대신 자체 프록시를 통해 트래픽을 라우팅합니다 ([Istio / Traffic Management](https://istio.io/latest/docs/concepts/traffic-management/#:~:text=Using%20this%20service%20registry%2C%20the,loaded%20than%20any%20other%20host)). 이처럼 Istio를 적용한 후에도 별도의 설정 없이 서비스 간 통신은 기존과 같이 동작하지만, 모든 통신이 프록시를 거치게 되어 일관된 트래픽 제어 지점이 생깁니다.

- **기본 보안**: 초급 단계에서는 Istio 설치 시 제공되는 **기본 보안 기능**을 이해하는 것이 중요합니다. Istio는 설치 시 기본적으로 각 서비스 간 통신에 대해 **Mutual TLS**(상호 TLS 인증)를 **옵션**으로 활성화할 수 있는 준비가 되어 있습니다. Istio의 인증 체계는 서비스 간에 고유한 신원(아이덴티티)을 부여하고 X.509 인증서를 활용하여 투명한 mTLS 통신을 가능하게 합니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20security%20%20enables%20strong,who%20can%20access%20your%20services)). 컨트롤 플레인인 Istiod는 인증 기관(CA) 역할을 하여 데이터 플레인의 사이드카들에게 인증서를 발급함으로써 서비스 간 **암호화 통신**을 지원합니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20acts%20as%20a%20Certificate,communication%20in%20the%20data%20plane)). (초기 설정에 따라 mTLS가 **Permissive 모드**로 동작하여, 양측 모두 Istio 사이드카가 있을 경우에만 암호화를 사용하고 그렇지 않은 경우 평문 통신을 허용합니다. 이는 점진적 도입을 용이하게 합니다.)

- **기본 관측성(Observability)**: Istio에 포함된 Envoy 사이드카들은 서비스 트래픽에 대한 **텔레메트리 데이터**를 자동으로 수집합니다. 각 요청에 대한 **메트릭(metrics)**과 **로그**, **분산 추적(trace)** 정보가 사이드카를 통해 수집되며, 별도의 애플리케이션 코드 변경 없이 체계적인 모니터링이 가능합니다. Istio는 이러한 텔레메트리를 Prometheus, Grafana, Zipkin/Jaeger와 같은 모니터링/분석 도구와 연계하여 활용할 수 있도록 기본 설정을 제공합니다 ([Istio / The Istio service mesh](https://istio.io/latest/about/service-mesh/#:~:text=Istio%20generates%20telemetry%20within%20the,troubleshoot%2C%20maintain%2C%20and%20optimize%20applications)). 예를 들어, **Prometheus**로 수집된 메트릭을 **Grafana** 대시보드로 시각화하거나, **Jaeger**로 분산 추적 정보를 조회할 수 있습니다. 초급 단계에서는 Istio를 설치하는 것만으로도 각 서비스 간 호출에 대한 지연 시간, 성공/에러 비율 등의 지표가 수집되는 것을 확인할 수 있습니다.

- **Ingress 게이트웨이(진입 게이트웨이)**: 서비스 메쉬 내부뿐만 아니라 **외부에서 메쉬로 들어오는 트래픽**도 Istio로 관리할 수 있습니다. Istio는 기본적으로 클러스터 엣지에 **Ingress Gateway**(진입 게이트웨이) 역할을 하는 Envoy 프록시를 배포하며, 이를 통해 외부 요청을 mesh 내부 서비스로 라우팅할 수 있습니다. Istio의 **Gateway** 리소스는 메쉬 경계에서 동작하는 로드 밸런서로서, 수신할 **포트와 프로토콜 (HTTP/TCP)**, **호스트명** 및 **TLS 설정** 등을 정의합니다 ([Istio / Gateway](https://istio.io/latest/docs/reference/config/networking/gateway/#:~:text=,for%20the%20load%20balancer%2C%20etc)). 이를 이용해 사용자는 특정 호스트/경로로 들어오는 외부 트래픽을 어떤 내부 서비스로 보낼지 지정할 수 있습니다. (예: `myservice.mydomain.com`으로 들어오는 요청을 클러스터 내 `myservice`로 연결) 초급 단계에서는 게이트웨이를 통해 외부에서 샘플 애플리케이션에 접속하는 기본 시나리오를 실습하게 됩니다.

### 실습 과제 (Hands-On Practice)

- **Istio 설치 및 기본 실행**: 공식 문서의 지침에 따라 Istio를 설치합니다 (예: `istioctl` CLI 사용). 설치 후 `istio-system` 네임스페이스에 Istiod(컨트롤 플레인)와 기본 Ingress/Egress 게이트웨이 등이 배포됩니다. Istio가 제대로 설치되었는지 확인한 뒤, **네임스페이스 레이블링**을 통해 사이드카 자동 주입을 활성화합니다 (예: `kubectl label namespace default istio-injection=enabled`). 

- **Bookinfo 샘플 애플리케이션 배포**: Istio 공식 예제인 [**Bookinfo**](https://istio.io/latest/docs/examples/bookinfo/) 애플리케이션을 배포합니다. Bookinfo는 제품 정보 페이지(`productpage`)와 이를 구성하는 여러 마이크로서비스(`details`, `reviews`, `ratings`)로 이루어진 예제입니다. `kubectl apply -f bookinfo.yaml`을 실행하면 각 서비스가 배포되고, **Istio 사이드카가 자동으로 주입**되어 실행됩니다 ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=The%20application%20will%20start,the%20Istio%20sidecar%20will%20be)) ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=The%20application%20will%20start,the%20Istio%20sidecar%20will%20be)). 배포가 완료되면 **Istio Ingress Gateway**를 통해 외부에서 접속할 수 있도록 Gateway 및 VirtualService 리소스를 적용합니다 ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=1,for%20the%20Bookinfo%20application)). (예: `kubectl apply -f bookinfo-gateway.yaml` 명령으로 게이트웨이 생성). 그런 다음 `kubectl port-forward` 또는 클라우드 환경의 LoadBalancer IP를 통해 브라우저에서 `http://<gateway-url>/productpage`에 접속하여 Bookinfo 웹페이지가 보이는지 확인합니다.

- **기본 동작 확인**: Bookinfo 웹 애플리케이션에 여러 번 접속해보고, Istio를 통해 **서비스 간 통신이 정상적으로 이루어지는지** 확인합니다. 예를 들어, `productpage` 서비스가 `reviews` 서비스를 호출하고, `reviews` 서비스는 `ratings` 서비스를 호출하는 체인이 사이드카 프록시를 통해 잘 동작하는지 살펴봅니다. 초기 설정에서는 `reviews` 서비스의 **v1 버전**만 배포되어 있으므로, productpage에 표시되는 리뷰에는 별점(rating)이 보이지 않습니다 (v1은 ratings 서비스와 연동하지 않음). 이는 Istio **기본 라우팅 규칙**이 모든 트래픽을 `reviews:v1`로 보내도록 설정되어 있기 때문입니다 ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=destination%20to%20another)) ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=In%20this%20task%2C%20you%20will,of%20traffic%20to%20%60reviews%3Av3)). 이러한 기본 라우팅 동작은 이후 단계를 통해 변경해볼 수 있습니다.

- **기본 모니터링 툴 활용**: 초급 단계의 추가 학습으로, Istio와 함께 제공되는 **모니터링 애드온**을 사용해볼 수 있습니다. 예를 들어 **Kiali 대시보드**를 설치하고 (`kubectl apply -f samples/addons`으로 Kiali, Grafana, Jaeger 등을 배포) `istioctl dashboard kiali` 명령으로 대시보드에 접속합니다 ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=Istio%20integrates%20with%20several%20different,the%20health%20of%20your%20mesh)) ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=2)). Kiali에서 **서비스 그래프**를 확인하면 Bookinfo 애플리케이션의 서비스 관계도와 트래픽 흐름을 시각화할 수 있습니다 ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=The%20Kiali%20dashboard%20shows%20an,to%20visualize%20the%20traffic%20flow)). 또, Grafana 대시보드에서 Istio의 기본 메트릭(서비스별 요청량, 에러율, 응답시간 등)이 실시간으로 수집되어 그래프로 나타나는 것을 확인하고, Jaeger를 통해 `productpage` 요청의 분산 트레이싱 정보를 살펴볼 수 있습니다. 이러한 도구를 사용하면 Istio가 **코드 수정 없이도** 얼마나 풍부한 운영 정보를 제공하는지 체감할 수 있습니다.

### 사용 사례 (Use Cases)

- **기본 트래픽 라우팅 시나리오**: 예를 들어, 전자상거래 애플리케이션을 모놀리식에서 마이크로서비스로 분리한 팀을 생각해봅시다. Istio를 도입한 후, **사용자 요청이 Ingress Gateway를 통해 제품 페이지 서비스로 전달**되고, 제품 페이지 서비스는 **Istio 사이드카**를 통해 상세정보 서비스나 리뷰 서비스 등 관련 마이크로서비스들을 호출합니다. 이때 Istio는 자동으로 각 서비스 호출에 **로드 밸런싱**을 적용하고, 장애 발생 시 **투명한 재시도 및 폴백**을 수행하여 애플리케이션의 신뢰성을 높여줍니다. 또한 서비스 호출마다 **메트릭과 추적 정보**를 수집하므로, 운영자는 Grafana/Kiali 등을 통해 어느 경로로 트래픽이 흐르고 있는지, 응답 시간은 어떤지 한눈에 파악할 수 있습니다. 이 **기본적인 서비스 메쉬 구성**만으로도 마이크로서비스 아키텍처 운영에 큰 가치를 제공하며, 이후 단계에서 보다 고급 기능을 순차적으로 활성화할 수 있는 토대를 마련합니다.

- **서비스 메쉬 도입 초기**: 기업 내에 여러 마이크로서비스가 존재하는 상황에서, Istio를 **점진적으로 도입**하는 사례를 들 수 있습니다. 우선 몇 개 서비스에만 사이드카 프록시를 주입하고 **Permissive mTLS** 모드로 설정하여, Istio 메쉬 내의 통신은 자동으로 암호화하되 메쉬에 포함되지 않은 서비스와도 통신이 호환되도록 운영합니다. 이러한 설정으로 **기존 시스템에 영향 없이** 메쉬의 **가시성**을 확보하고, 이후 점차 모든 서비스로 사이드카 주입을 확대하면서 **전면적인 mTLS 보안**을 적용해 나갈 수 있습니다. 초급 단계에서는 이처럼 **큰 변경 없이도** Istio를 도입해볼 수 있으며, 작은 시작으로부터 서비스 메쉬의 이점을 경험하는 것이 목표입니다.

### 참고 자료 (Resources)

- **Istio 개요** – [What is Istio?](https://istio.io/latest/docs/overview/what-is-istio/): 서비스 메쉬와 Istio의 기본 개념을 소개하는 공식 문서 ([Istio / The Istio service mesh](https://istio.io/latest/about/service-mesh/#:~:text=A%20service%20mesh%20is%20an,projects%20like%20Kubernetes%20and%20Prometheus)). Istio가 제공하는 기능(보안, 트래픽 관리, 관측성 등)과 탄생 배경, CNCF 프로젝트로서의 위상 등을 설명합니다.

- **Istio 아키텍처** – [Architecture](https://istio.io/latest/docs/ops/deployment/architecture/): Istio의 데이터 플레인(Envoy 사이드카)과 컨트롤 플레인(Istiod) 구조를 도식과 함께 설명하는 문서 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=An%20Istio%20service%20mesh%20is,plane%20and%20a%20control%20plane)) ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20provides%20service%20discovery%2C%20configuration,and%20certificate%20management)). 각 컴포넌트의 역할 (예: Envoy 프록시의 기능 목록, Istiod의 서비스 발견 및 설정 전파, 인증서 관리 등)을 확인할 수 있습니다.

- **Bookinfo 예제** – [Bookinfo Application](https://istio.io/latest/docs/examples/bookinfo/): Bookinfo 샘플 애플리케이션을 배포하고 Istio의 기본 기능을 체험해볼 수 있는 자습서. 서비스 메쉬에 애플리케이션을 배포하는 절차와, Gateway를 통한 외부 트래픽 유입 설정, Kiali를 통한 시각화 등의 **튜토리얼**이 포함되어 있습니다 ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=Congratulations%20on%20completing%20the%20evaluation,installation)).

- **초보자용 실습 가이드** – [Learn Microservices using Kubernetes and Istio](https://istio.io/latest/docs/examples/microservices-istio/): 새로운 Istio 사용자를 위한 모듈식 튜토리얼로, Bookinfo 애플리케이션을 활용하여 단계별로 기능을 실습합니다. (예: **제품 페이지 서비스에 Istio 사이드카 주입** → **트래픽 테스트** → **리뷰 서비스 새로운 버전 추가** → **트래픽 분할** 등 일련의 과정을 안내합니다.)

## 중급 단계 (Intermediate)

중급 단계에서는 Istio의 **고급 트래픽 관리**, **서비스 보안 강화**, **심화된 모니터링 기능**에 대해 학습합니다. 여러 가지 Istio 리소스(VirtualService, DestinationRule, Gateway, Policy 등)를 직접 정의하여, 트래픽을 세밀하게 제어하고 서비스 간 **Zero-Trust 보안**을 구현하며 **관찰 가능성(Observability)**을 높이는 방법을 다룹니다. 또한 이러한 기능들을 활용한 실용적인 시나리오 (예: 카나리 배포, 서킷 브레이커, mTLS 적용 등)를 살펴봅니다.

### 개념 및 기능

- **고급 트래픽 관리**: Istio의 **트래픽 관리 API**를 사용하면 서비스 간 트래픽 흐름을 세밀하게 제어할 수 있습니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=You%20can%20use%20Istio%E2%80%99s%20Traffic,traffic%20in%20your%20service%20mesh)). 핵심 리소스로는 **VirtualService**와 **DestinationRule**이 있습니다. 
  - *VirtualService*는 어떤 **호스트(서비스)**로 들어온 트래픽을 어떻게 라우팅할지 규칙을 정의하는 객체로, **요청 경로/헤더 등에 따른 라우팅**, **트래픽 분할(가중치 기반)**, **타임아웃/재시도**, **Fault Injection(장애 주입)** 등의 규칙을 설정할 수 있습니다. 예를 들어 VirtualService를 사용하여 전체 트래픽의 10%는 서비스의 **v2 버전**으로 보내고 나머지는 v1으로 보내는 **Canary(카나리) 배포**가 가능합니다. 
  - *DestinationRule*은 대상 서비스의 **서브셋(subset)**을 정의하고 해당 서브셋에 대한 설정(예: **서킷 브레이커**나 **고급 로드밸런싱 정책**)을 적용하는 객체입니다. 예를 들어 `reviews` 서비스의 `v1`, `v2` 버전을 각각 하나의 서브셋으로 정의하고, `v2`에만 특정 연결 제한이나 서로 다른 LB 알고리즘을 적용할 수 있습니다. 
  - 이러한 Istio 트래픽 관리 규칙을 활용하면 **A/B 테스팅**, **카나리 릴리즈**, **블루/그린 배포**, **지연 주입 및 장애 테스트**, **지역 기반 라우팅** 등 다양한 시나리오를 구현할 수 있습니다. 실제로 Istio는 **가중치 기반 라우팅**을 손쉽게 구성할 수 있게 해주므로, 운영자는 새 버전 서비스로 트래픽을 점진적으로 이동시키며 안전하게 배포할 수 있습니다 ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=This%20task%20shows%20you%20how,of%20a%20microservice%20to%20another)). 또한 VirtualService를 통해 특정 사용자나 지역의 트래픽만 별도 버전으로 라우팅하는 등 섬세한 제어도 가능합니다. Istio의 이러한 라우팅 규칙들은 모두 Envoy 사이드카 프록시 설정으로 실시간 변환되어 적용되므로, **서비스를 재배포하지 않고도** 트래픽 정책을 변경할 수 있다는 장점이 있습니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20converts%20high%20level%20routing,the%20Envoy%20API%20can%20consume)).

- **트래픽 관리 주요 기능 요약**: Istio의 트래픽 관리 기능은 다음과 같은 **시나리오별 특징**을 갖습니다 ([Istio / Traffic Management](https://istio.io/latest/docs/concepts/traffic-management/#:~:text=Istio%E2%80%99s%20traffic%20routing%20rules%20let,dependent%20services%20or%20the%20network)) ([Istio / Traffic Management](https://istio.io/latest/docs/concepts/traffic-management/#:~:text=While%20Istio%E2%80%99s%20basic%20service%20discovery,also%20want%20to%20apply%20special)):
  - **트래픽 분할 및 버전별 라우팅**: 요청의 일부 비율을 새 버전으로 보내 카나리 배포를 실행하거나, 특정 사용자 그룹에만 새로운 기능을 시험적으로 노출(A/B 테스트)할 수 있습니다. 예를 들어, `reviews` 서비스의 v1과 v3 버전 사이에 트래픽을 50/50으로 분배하고 점진적으로 100%까지 올리는 전략을 구현할 수 있습니다.
  - **네트워크 견고성 (Resiliency)**: **시간초과(Timeout)**와 **재시도(Retry)** 정책을 적용하여 불안정한 네트워크 환경에서도 안정성을 높입니다. 또한 **회로 차단기(Circuit Breaker)** 설정으로 특정 서비스 인스턴스에 오류가 발생했을 때 해당 인스턴스로의 과부하를 방지하고, **폴백(fallback)** 시나리오를 구성할 수 있습니다.
  - **장애 주입(Fault Injection)**: 테스트를 위해 인위적으로 지연(Time delay)이나 오류를 주입함으로써 **카오스 엔지니어링**을 수행할 수 있습니다. 이를 통해 장애 상황에서 시스템이 어떻게 반응하는지 미리 점검하고, 장애 내성(fault tolerance)을 개선할 수 있습니다.
  - **Ingress/Egress 제어**: Istio **게이트웨이(Gateway)**와 **ServiceEntry**를 활용하여 **메쉬 경계의 트래픽**도 제어합니다. Ingress Gateway를 통해 외부 트래픽의 수용 및 TLS 종료를 제어하고, Egress 설정을 통해 외부 API 호출에 대한 정책(예: 외부 서비스 도메인 whitelisting, TLS origination)을 관리할 수 있습니다. 예를 들어, 메쉬 내부 서비스가 외부의 REST API를 호출해야 한다면 **Egress Gateway**를 경유하도록 하여 모니터링 및 통제를 일원화할 수 있습니다.

- **서비스 보안 (Zero-Trust)**: 중급 단계에서는 Istio를 활용한 **서비스 보안 강화**에 집중합니다. Istio는 기본적으로 **서비스 신원 기반**의 보안 모델을 제공합니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20security%20%20enables%20strong,who%20can%20access%20your%20services)). 이는 IP나 포트보다는 **서비스 계정(Service Account)** 등 워크로드의 ID에 따라 정책을 적용하는 방식입니다.
  - **Mutual TLS 설정**: Istio의 **PeerAuthentication** 정책을 사용하여 서비스 간 통신에 **상호 TLS**를 강제할 수 있습니다. 예를 들어 mesh 전체에 PeerAuthentication `mode: STRICT`를 적용하면, Istio 사이드카들 사이의 통신은 자동으로 mTLS로 암호화되고 사이드카가 없는 워크로드와는 통신이 차단됩니다. 이렇게 하면 클러스터 내 서비스 트래픽이 모두 암호화되어 **제로트러스트** 네트워크를 구현할 수 있습니다. Istio는 인증서를 자동 관리하므로 운영자는 정책 몇 줄 설정만으로 이 기능을 켤 수 있습니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20security%20%20enables%20strong,who%20can%20access%20your%20services)). (Istio 1.5+ 버전부터 보안 정책은 *PeerAuthentication* 및 *AuthorizationPolicy* 등의 CRD로 관리됩니다.)
  - **인증(Authentication)과 인가(Authorization)**: Istio는 서비스 또는 사용자에 대한 **인증**을 수행하고, 그 결과를 바탕으로 **인가 정책**을 적용할 수 있습니다. 예를 들어, JSON 웹 토큰(**JWT**) 기반의 엔드유저 인증을 Istio 레벨에서 처리하여 특정 권한을 가진 사용자만 서비스에 접근하도록 하거나, 특정 서비스 간 호출을 허용/차단하는 규칙을 적용할 수 있습니다. Istio의 **AuthorizationPolicy**를 이용하면 서비스 별로 세부적인 ACL을 설정 가능하며, HTTP 경로, 메소드, 요청 헤더 등에 기반한 세밀한 접근 통제도 구현할 수 있습니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20security%20%20enables%20strong,who%20can%20access%20your%20services)). 이러한 정책은 사이드카 프록시에서 요청을 허용하거나 거부하는 방식으로 enforcement되므로 애플리케이션 코드에 영향을 주지 않습니다.
  - **기타 보안 기능**: Istio는 이외에도 **유휴 상태의 인증서 자동 회전**, **인증서 갱신 자동화**, **외부 CA 연동** 등의 기능을 통해 보안 운영을 단순화합니다. 또한 대시보드나 istioctl 명령을 통해 **인증 상태 검증**(예: 어떤 서비스 간에 mTLS가 적용되고 있는지 확인)과 **정책 시뮬레이션**(dry-run) 등을 할 수 있어, 안전하게 정책을 배포할 수 있습니다.

- **관측성 강화(Observability)**: Istio의 기본 텔레메트리 외에도, 중급 단계에서는 **추가적인 모니터링 설정**과 **커스텀 텔레메트리**에 대해 배웁니다.
  - **커스텀 메트릭 수집**: Istio **Telemetry API**를 사용하면 수집되는 메트릭을 커스터마이징할 수 있습니다. 예를 들어 특정 HTTP 헤더나 응답 코드를 기준으로 메트릭 레이블을 추가하는 등 고급 설정이 가능합니다. 이를 통해 도메인 특화된 지표(예: 멀티테넌트 시스템에서 테넌트 ID별 요청량 등)를 추가로 수집할 수 있습니다.
  - **로그 및 추적**: Envoy 사이드카의 **액세스 로그**를 활성화하거나 로그 형식을 조정할 수 있습니다 ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=,access%20logs%20with%20Telemetry%20API)). 또한 **분산 추적**을 위해 샘플링 비율을 조정하거나 Zipkin/Jaeger와의 연계를 세밀하게 설정할 수 있습니다. Istio는 기본 샘플링 비율이 1%이므로, 필요에 따라 이를 조절하여 트래픽 패턴에 맞게 추적 데이터를 수집해야 합니다. 중급 단계에서는 애플리케이션에 Trace 헤더를 전달하여 **엔드투엔드 추적**을 완성하거나, **OpenTelemetry**와의 통합을 통해 표준화된 방식으로 모니터링 스택을 구성하는 방법도 고려할 수 있습니다.
  - **대시보드 활용**: Istio는 Grafana에 Istio 전용 대시보드를 제공하며 ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=Istio%20integrates%20with%20several%20different,the%20health%20of%20your%20mesh)), Kiali를 통해 트래픽 라우팅 현황, 지연 시간 분포, 에러 발생률 등을 종합적으로 파악할 수 있습니다. 특히 Kiali의 **서비스 그래프**는 특정 버전으로의 트래픽 비율, 활성화된 정책 등을 시각화해주어, 예를 들어 카나리 배포 진행 중에 **버전별 트래픽 분포**를 모니터링하기에 유용합니다. 또한 Kiali를 통해 Istio 구성을 검사하고 잠재적인 설정 충돌이나 문제를 미리 알려주는 기능도 활용할 수 있습니다.

### 실습 과제 (Hands-On Practice)

- **가중치 기반 트래픽 분할 (카나리 배포)**: `VirtualService`를 정의하여 **새 버전 서비스로의 트래픽 이동**을 실습합니다. 예를 들어 `reviews` 서비스의 v1과 v3 버전이 배포되어 있다고 가정하고, 아래와 같이 VirtualService에서 두 버전에 **50%씩 트래픽을 분배**한 후 점진적으로 비율을 변경해 봅니다 (Istio 공식 Bookinfo 예제 기반):

  ```yaml
  apiVersion: networking.istio.io/v1alpha3
  kind: VirtualService
  metadata:
    name: reviews
  spec:
    hosts:
      - reviews
    http:
      - route:
          - destination:
              host: reviews
              subset: v1
            weight: 50
          - destination:
              host: reviews
              subset: v3
            weight: 50
  ```

  위 설정을 적용하면, `reviews` 서비스로 향하는 트래픽의 50%는 subset `v1`으로, 50%는 subset `v3`으로 전달됩니다 ([Getting started with Istio Service Mesh - Part 4 Traffic management | HPE Express Containers](https://hewlettpackard.github.io/Docker-SimpliVity/blog/istio-getting-started-4.html#:~:text=apiVersion%3A%20networking,route)) ([Getting started with Istio Service Mesh - Part 4 Traffic management | HPE Express Containers](https://hewlettpackard.github.io/Docker-SimpliVity/blog/istio-getting-started-4.html#:~:text=,reviews%20subset%3A%20v3%20weight%3A%2050)). 실제로 `productpage`를 새로고침하면 검은 별(별점 없음)과 붉은 별(별점 있음)이 번갈아 보이게 되는데, 이는 `reviews:v1` (별점 서비스 미연동)과 `reviews:v3` (별점 서비스 연동) 응답이 절반씩 나타나기 때문입니다. 그런 다음 VirtualService의 weight 값을 조정하여 80/20, 100/0으로 변경 적용함으로써 **트래픽을 새 버전으로 서서히 이동**해보세요 ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=destination%20to%20another)) ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=In%20this%20task%2C%20you%20will,of%20traffic%20to%20%60reviews%3Av3)). 이 과정에서 **Istio의 점진적 릴리스** 기능을 체험하고, 문제가 없을 경우 최종적으로 모든 트래픽을 `v3`으로 전환하여 배포를 완료할 수 있습니다.

- **트래픽 견고성 시나리오**: Istio의 **네트워크 장애 대응 기능**을 실습합니다. 먼저 VirtualService에 **Fault Injection** 규칙을 추가하여, `reviews` 서비스 호출에 인위적으로 지연을 발생시키거나 오류 코드를 반환하도록 설정해봅니다 (예: 5초 지연 후 503 오류 10% 확률로 주입). 그런 다음 `productpage`에서의 사용자 경험 변화를 관찰하고, Istio가 retry나 timeouts으로 대응하도록 DestinationRule에 **재시도 정책**(예: 3번 재시도, 타임아웃 2초)을 적용해 볼 수 있습니다. 또한 DestinationRule에 **Circuit Breaker**(예: 연결 최대 1개, 초과 시 차단) 설정을 적용한 뒤 부하를 주어, **회로 차단 동작**이 발생하면 요청 일부가 즉시 실패하도록 테스트해봅니다. 이 과정에서 **Kiali 대시보드**의 **Error Rate**나 **Traffic** 그래프를 보면 장애 주입 전후의 변화를 시각적으로 확인할 수 있습니다. 이러한 실습을 통해 Istio가 **서비스 신뢰성**을 높이기 위해 어떤 기법들을 제공하는지 이해하게 됩니다.

- **서비스 간 mTLS 및 보안 정책 적용**: **보안 기능**을 활성화하는 실습을 진행합니다. 먼저, **PeerAuthentication** 리소스를 사용하여 **전역 mTLS 활성화** 정책을 배포합니다. 예를 들어 다음과 같이 mesh 전역(default 네임스페이스 등)에 `mode: STRICT`인 PeerAuthentication을 적용하면 모든 서비스 통신이 자동으로 암호화됩니다. 적용 후 Istio **Authentiction Policy** 가이드의 지침대로 실제 패킷이 암호화(TLS)되는지 확인하고 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20acts%20as%20a%20Certificate,communication%20in%20the%20data%20plane)), 검증을 위해 mTLS가 필요한 환경에서 사이드카가 없는 서비스로의 통신이 차단되는지도 테스트합니다. 
  이어서, **AuthorizationPolicy**를 하나 만들어 **서비스 접근 제어**를 적용해봅니다. 예를 들어 `ratings` 서비스에 대해 `"authenticated"` 된 요청만 허용하는 정책이나, `reviews` 서비스가 `ratings` 서비스에 호출할 때 특정 경로만 허용하는 정책 등을 정의해볼 수 있습니다. 정책 적용 후 의도한 대로 호출이 차단/허용되는지 확인하여 Istio의 **세분화된 보안 정책** 동작을 이해합니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20security%20%20enables%20strong,who%20can%20access%20your%20services)). 
  마지막으로 **JWT 인증** 연동을 시험해보려면, Istio 공식 문서의 *JWT 인증 예제*에 따라 Istio Ingress Gateway에 JWT 검증 필터를 설정하고, 올바른 토큰을 가진 요청만 내부 서비스로 전달되도록 구성합니다. 이런 일련의 보안 실습을 통해 Istio를 사용한 **Zero-Trust 접근 제어**의 구현 방법을 익힐 수 있습니다.

- **모니터링 및 로그 확인**: 중급 실습에서는 Istio의 **텔레메트리 데이터를 직접 확인하고 활용**해봅니다. 우선 **Prometheus**에 쿼리를 실행하여 서비스 지표를 확인하는 연습을 합니다 (예: `istio_requests_total{destination_service="reviews.default.svc.cluster.local"}` 같은 메트릭으로 `reviews` 서비스에 대한 요청량을 조회). 그리고 **Grafana**의 Istio Dashboards를 살펴보면서, 전체 success rate이나 p95 지연 시간 등의 지표를 모니터링합니다 ([Istio / The Istio service mesh](https://istio.io/latest/about/service-mesh/#:~:text=Istio%20generates%20telemetry%20within%20the,troubleshoot%2C%20maintain%2C%20and%20optimize%20applications)). 
  한편, **Envoy 사이드카 로그**를 활성화하여 각 요청별 액세스 로그를 살펴보는 것도 중요합니다. Istio 공식 가이드의 지침에 따라 `Telemetry API` 또는 `EnvoyFilter`를 사용해 **Envoy Access Log**를 포맷 지정하여 활성화한 뒤, 실제 Pod 로그에서 사이드카의 로그를 확인합니다. 이를 통해 어느 요청이 어느 서비스로 라우팅되었고, 응답 코드가 무엇이며, 소요 시간은 얼마였는지 등의 세부 정보를 얻을 수 있습니다. 
  아울러 **분산 추적(Distributed Tracing)**을 위해 `productpage` 서비스 등을 여러 번 호출하여 **Jaeger** UI에서 트레이스를 조회해 봅니다. Istio가 투명하게 전파한 `x-b3-*` 헤더에 의해 각 서비스의 호출이 하나의 Trace로 연결되어 나타나며, 마이크로서비스 호출 흐름과 각 단계의 지연 시간을 직관적으로 파악할 수 있습니다. 
  마지막으로, 이렇게 수집된 데이터를 활용해 **서비스 메쉬 튜닝**에 나서봅니다. 예를 들어, 특정 서비스의 에러율 증가가 관찰되면 Istio의 대시보드를 통해 문제 지점을 식별하고, 필요 시 **트래픽를 임시 차단**하거나 **라우팅 우회**를 설정하는 등의 조치를 취해볼 수 있습니다. 이는 실제 운영 상황에서 Istio를 활용해 문제를 완화하는 시나리오를 미리 경험해 보는 것입니다.

### 사용 사례 (Use Cases)

- **카나리 배포**: 대규모 사용자를 둔 서비스에서 새로운 버전을 출시할 때 **카나리 배포** 전략을 Istio로 구현할 수 있습니다. 예를 들어 A/B 테스트를 겸하여 **일부 사용자(또는 트래픽의 일정 비율)**만 신규 기능을 사용하게 하고, 나머지는 기존 버전을 사용하도록 할 수 있습니다. 현실 시나리오로, 쇼핑몰의 추천 엔진을 개선한 새로운 버전을 배포하면서 Istio **VirtualService**로 트래픽의 5%만 새 엔진으로 보내고 95%는 기존 엔진으로 보내도록 설정합니다. 운영팀은 **Kiali**를 통해 두 버전으로 나뉘는 트래픽 상황을 모니터링하고, **Grafana**로 각 버전에서의 전환율, 오류율 등을 비교합니다. 새 버전에서 문제가 발견되면 VirtualService 설정을 즉시 0%로 조정해 모든 트래픽을 구버전으로 롤백하고, 문제가 없으면 단계적으로 비율을 높여나가 최종 전환합니다 ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=A%20common%20use%20case%20is,from%20one%20destination%20to%20another)). Istio의 **즉각적인 정책 적용**과 **모니터링 통합** 덕분에 이러한 카나리 배포가 **안전하고 유연하게** 이루어집니다.

- **서비스 간 완전 암호화 통신 (Zero-Trust)**: 금융 또는 보안이 중요한 환경에서는 **서비스 간 모든 통신을 암호화**해야 할 요구가 있습니다. Istio를 도입하면 비교적 적은 노력으로 이를 구현 가능합니다. 실제 사례로, 한 금융기업은 Kubernetes 상의 수십 개 마이크로서비스에 Istio를 설치하고 **전 구간 mTLS**를 적용했습니다. Istio **PeerAuthentication**을 STRICT 모드로 설정한 후, 애플리케이션 코드 변경 없이도 마이크로서비스 호출이 모두 TLS로 보호되었고, 각 서비스의 아이덴티티가 검증되었습니다. 또한 Istio **AuthorizationPolicy**를 활용해 서비스별 **허용된 호출 매트릭스**를 만들어, 예를 들어 결제 서비스는 주문 서비스만 호출 가능하고 그 외에는 차단하는 식의 **동서(Lateral) 트래픽 보안**을 달성했습니다. 이처럼 Istio를 활용하면 마이크로서비스 아키텍처에서 흔히 어려운 **세분화 보안(segmentation)**을 중앙 정책으로 일관되게 관리할 수 있어 **제로트러스트 보안 모델**을 실현할 수 있습니다.

- **고급 트래픽 정책을 통한 안정성 강화**: 중급 기능을 종합적으로 활용한 사례로, **한 SaaS 업체가 Istio를 통해 마이크로서비스의 안정성을 높인 경우**를 들 수 있습니다. 이 업체는 서비스 간 호출에서 간헐적으로 지연이 발생해 장애로 이어지는 문제가 있었습니다. Istio 도입 후, **타임아웃/재시도** 값을 조정하고 **서킷 브레이커**를 구성하여 문제를 완화했습니다. 구체적으로, 마이크로서비스 A가 B를 호출할 때 응답 지연이 2초를 넘으면 Istio가 자동으로 연결을 끊고 B 서비스의 오류로 간주하도록 설정했습니다. 동시에 B 서비스에 대한 연결 수를 제한해 B에 장애가 발생해도 A에 연쇄적으로 부하가 전이되지 않도록 했습니다. 그 결과 B 서비스 문제 발생 시 A 서비스는 즉시 **fallback 로직**을 수행하여 일부 기능만 제한하고 전체 시스템 다운은 피할 수 있었습니다. 또한 이 과정에서 Istio의 **메트릭**을 통해 서킷 브레이커 발생 빈도를 모니터링하고, 임계값을 튜닝하여 **안정성과 성능 균형**을 맞추었습니다. 이러한 사례는 Istio의 트래픽 제어 기능이 실전 환경에서 **장애 격리와 복원력 향상**에 어떻게 기여하는지를 보여줍니다.

### 참고 자료 (Resources)

- **트래픽 관리 개념** – [Traffic Management Concepts](https://istio.io/latest/docs/concepts/traffic-management/): Istio의 트래픽 관리 모델과 VirtualService, DestinationRule, Gateway 등의 리소스 개념을 상세히 설명하는 문서 ([Istio / Traffic Management](https://istio.io/latest/docs/concepts/traffic-management/#:~:text=Istio%E2%80%99s%20traffic%20routing%20rules%20let,dependent%20services%20or%20the%20network)) ([Istio / Traffic Management](https://istio.io/latest/docs/concepts/traffic-management/#:~:text=Istio%E2%80%99s%20traffic%20management%20model%20relies,any%20changes%20to%20your%20services)). 서비스 레지스트리, 기본 라우팅부터 고급 정책(시간초과, 재시도, 장애 주입, 지역 LB 등)까지 포괄적으로 다룹니다. 특히 **왜 VirtualService가 필요한지**, **여러 규칙이 충돌하면 어떤 우선순위로 적용되는지** 등 운영자가 알아야 할 사항이 정리되어 있습니다.

- **트래픽 관리 태스크** – Istio 공식 **Traffic Management Tasks** 모음: 다음은 중급 단계에서 실습하기 좋은 공식 가이드들입니다.
  - [Request Routing](https://istio.io/latest/docs/tasks/traffic-management/request-routing/): 요청 경로나 콘텐츠에 따라 다른 서비스 버전으로 라우팅하는 방법.
  - [Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/): 퍼센트 기반으로 트래픽을 분배하여 카나리 배포를 구현하는 방법 ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=This%20task%20shows%20you%20how,of%20a%20microservice%20to%20another)).
  - [Fault Injection](https://istio.io/latest/docs/tasks/traffic-management/fault-injection/): 인위적으로 오류/지연을 발생시켜 서비스의 내구성을 테스트.
  - [Circuit Breaking](https://istio.io/latest/docs/tasks/traffic-management/circuit-breaking/): 서킷 브레이커 설정으로 과부하를 방지하는 방법.
  - [Ingress Gateway](https://istio.io/latest/docs/tasks/traffic-management/ingress/ingress-gateway/): Istio 게이트웨이를 이용해 외부 트래픽을 받아들이는 설정 (HTTP 및 TLS).
  - [Egress](https://istio.io/latest/docs/tasks/traffic-management/egress/egress-control/): 메쉬 외부로 나가는 트래픽을 제어하고 외부 서비스와 통신하는 방법.

- **보안 개념 및 태스크** – Istio **Security** 관련 공식 문서:
  - [Security Concepts](https://istio.io/latest/docs/concepts/security/): Istio의 보안 모델, mTLS 인증 구조, 인증/인가 정책의 개념을 정리 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=Istiod%20security%20%20enables%20strong,who%20can%20access%20your%20services)).
  - [Authentication Policy (PeerAuthentication)](https://istio.io/latest/docs/tasks/security/authentication/authn-policy/): 서비스 간 mTLS를 설정하고 기존 서비스와 **점진적 mTLS 이행**(migration)하는 방법에 대한 가이드.
  - [Authorization Policy](https://istio.io/latest/docs/tasks/security/authorization/): 서비스에 대한 접근 제어 정책을 생성하고 적용하는 실습 가이드.
  - [Mutual TLS Migration](https://istio.io/latest/docs/tasks/security/authentication/mtls-migration/): 운영 중인 클러스터에 mTLS를 점진적으로 적용하여 **완전한 암호화 통신**을 달성하는 시나리오를 다룹니다 ([Mutual TLS Migration - Istio](https://istio.io/latest/docs/tasks/security/authentication/mtls-migration/#:~:text=This%20task%20shows%20how%20to,they%20are%20migrated%20to%20Istio)).

- **관측성(모니터링) 가이드** – Istio **Observability** 관련 자료:
  - [Observability Concepts](https://istio.io/latest/docs/concepts/observability/): Istio가 수집하는 텔레메트리의 종류와 원리를 설명 (metrics, logs, tracing).
  - [Metrics and Grafana](https://istio.io/latest/docs/tasks/observability/metrics/visualize-metrics/): Grafana를 통해 Istio 메트릭을 시각화하는 방법 ([Istio / Getting Started](https://istio.io/latest/docs/setup/getting-started/#:~:text=using%20this%20)).
  - [Logs (Enable Envoy Access Logs)](https://istio.io/latest/docs/tasks/observability/logs/): Envoy 사이드카의 액세스 로그를 구성하고 수집된 로그를 보는 방법 ([Istio / Traffic Shifting](https://istio.io/latest/docs/tasks/traffic-management/traffic-shifting/#:~:text=,Visualizing%20Metrics%20with%20Grafana)).
  - [Distributed Tracing](https://istio.io/latest/docs/tasks/observability/distributed-tracing/): Jaeger나 Zipkin을 사용하여 Istio 분산 추적 데이터를 활용하는 예제.
  - [Kiali Tutorial](https://istio.io/latest/docs/tasks/observability/kiali/): Kiali 대시보드 설치 및 사용법, 서비스 그래프 분석 등의 내용.

## 고급 단계 (Advanced)

고급 단계에서는 Istio 서비스 메쉬의 **확장성과 고가용성**에 대해 다룹니다. 멀티 클러스터 환경에서 하나의 메쉬를 운영하거나, Istio의 기능을 심화 확장하기 위해 **Envoy 필터**나 **WebAssembly 플러그인**을 사용하는 방법, 그리고 서비스 메쉬 운영상의 모범 사례를 학습합니다. 또한 Istio의 최신 동향인 **Ambient Mesh(사이드카 없는 메쉬)** 모드 등에 대해서도 간략히 언급합니다. 이 단계에서는 Istio를 대규모 환경이나 특수한 요구사항에 맞게 **튜닝 및 확장**하는 능력을 키우는 것이 목표입니다.

### 개념 및 기능

- **멀티 클러스터 메쉬**: Istio는 단일 Kubernetes 클러스터를 넘어 **여러 클러스터에 걸친 하나의 서비스 메쉬**를 구성할 수 있습니다. 이는 예를 들어 **다중 리전/다중 AZ** 환경에서 트래픽을 지리적으로 분산하거나, 온프레미스-클라우드 **하이브리드** 환경을 통합하는 데 유용합니다 ([Istio / The Istio service mesh](https://istio.io/latest/about/service-mesh/#:~:text=Istio%20is%20not%20confined%20to,included%20within%20a%20single%20mesh)). 멀티 클러스터를 구성하는 방식에는 크게 두 가지 모델이 있습니다:
  - *공동 컨트롤 플레인(Multi-primary)*: 각 클러스터마다 Istiod 컨트롤 플레인이 실행되지만, 서로 **루트 인증서(CA)** 등을 공유하여 하나의 mesh로 동작합니다. 모든 클러스터가 동등한(primary) 역할을 하며, 서비스 레지스트리 정보도 서로 교환되어 **클러스터 간 서비스 검색**이 가능합니다.
  - *주-종 컨트롤 플레인(Primary-remote)*: 한 클러스터에만 Istiod를 두고, 다른 클러스터들은 **원격(remote)** 형태로 그 Istiod를 공용으로 사용합니다. Remote 클러스터의 사이드카들은 네트워크를 통해 primary 클러스터의 Istiod와 통신하며, 중앙에서 정책이 배포됩니다.
  - Istio 공식 문서에서는 이러한 모델별로 **동일 네트워크 vs 서로 다른 네트워크** 상황까지 포함해 구성 방법을 제시합니다. 멀티 클러스터 환경에서는 각 클러스터 사이의 **서비스 DNS 도메인**, **서비스 디스커버리**와 **인증서 신뢰**를 설정하는 것이 핵심입니다. 올바르게 설정하면, 예를 들어 **Cluster-A**의 서비스가 **Cluster-B**의 서비스에게도 마치 한 클러스터 내 서비스처럼 호출할 수 있고, mTLS 인증도 상호 인정되어 **보안 통신**이 이루어집니다. 고급 단계에서는 이러한 멀티 클러스터 메쉬를 구축하고, **페일오버(Failover)**나 **지역 기반 라우팅** 등을 설정함으로써 **재해 복구** 시나리오를 실습할 수 있습니다.
  - 추가로, Istio는 Kubernetes 외에 **가상머신(VM) 워크로드**도 메쉬에 포함시킬 수 있습니다. VM에 사이드카 에이전트를 설치하고 적절한 Workload Entry를 설정하면, VM에서 동작하는 서비스도 Kubernetes Pod와 동일하게 메쉬의 트래픽 관리와 보안 정책의 영향을 받습니다. 이것은 레거시 시스템과 클라우드 네이티브 환경을 하나의 메쉬로 아우르려는 고급 활용 사례입니다.

- **Envoy 필터 (EnvoyFilter)**: 기본 Istio API로 제어할 수 없는 특수한 프록시 동작을 실현하기 위해, Istio는 **EnvoyFilter**라는 확장 기능을 제공합니다. EnvoyFilter는 Istiod가 생성하는 Envoy 구성에 사용자가 정의한 변경 사항을 **주입(patch)**할 수 있는 메커니즘입니다 ([Istio / Envoy Filter](https://istio.io/latest/docs/reference/config/networking/envoy-filter/#:~:text=,EnvoyFilters%20in%20the%20workload%E2%80%99s%20namespace)). 이를 통해 Envoy 설정의 특정 필드를 수정하거나, **새로운 리스너/클러스터/필터** 등을 추가할 수 있습니다 ([Istio / Envoy Filter](https://istio.io/latest/docs/reference/config/networking/envoy-filter/#:~:text=,EnvoyFilters%20in%20the%20workload%E2%80%99s%20namespace)). 예를 들면:
  - 요청을 외부로 보내기 전에 특정 HTTP 헤더를 강제로 삽입하거나, 응답을 가로채서 내용 수정/필터링을 하는 **맞춤 필터**를 추가할 수 있습니다.
  - Envoy가 지원하지만 Istio API로 노출되지 않은 기능(예: 특정 프로토콜 프록시 설정이나 TCP 레벨 옵션)을 활성화할 수 있습니다.
  - 사이드카뿐 아니라 Ingress/Egress 게이트웨이의 Envoy에 대해서도 EnvoyFilter를 적용해 특별한 동작을 추가할 수 있습니다.
  - 다만 EnvoyFilter를 남용하면 Envoy와 Istio 내부 동작에 대한 깊은 이해가 필요하고, 버전 업그레이드 시 구성 호환성이 깨질 수 있어 **주의해서 사용**해야 합니다 ([Istio / Envoy Filter](https://istio.io/latest/docs/reference/config/networking/envoy-filter/#:~:text=NOTE%201%3A%20Some%20aspects%20of,are%20removed%20and%20replaced%20appropriately)). Istio API로 제공되는 기능들(앞서 언급된 VirtualService 등)로 거의 모든 시나리오를 커버할 수 있으나, 기업 현장에서 **아주 특수한 요구**가 있을 경우에 한해 EnvoyFilter를 고려합니다. 예를 들어, 한 금융사에서는 모든 서비스 통신에 대해 자체 개발한 **DLP(데이터 유출 방지)** 필터를 삽입하기 위해 EnvoyFilter를 사용하였고, 이를 통해 통신 내용에 특정 패턴이 있을 경우 차단하는 기능을 메쉬에 구현했습니다. 고급 단계에서는 간단한 EnvoyFilter 예제를 적용해보고, istio-proxy의 구성(`istioctl proxy-config`)이 어떻게 바뀌는지 확인해 보는 실습을 할 수 있습니다.

- **WebAssembly 확장 (WASM 플러그인)**: EnvoyFilter보다 한 단계 높은 수준의 확장성을 제공하는 방법으로 **WebAssembly(WASM)** 플러그인이 있습니다. Istio는 Envoy에 **WebAssembly 모듈**을 동적 로딩하여 사용자 정의 필터를 실행할 수 있는 기능을 갖추고 있습니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=,telemetry%20generation%20for%20mesh%20traffic)). WASM을 사용하면 RUST나 C++, Go (Proxy-Wasm SDK) 등을 이용해 Envoy에서 동작할 코드를 작성하고, 이 모듈을 Istio **WASM Plugin**(Extension) API를 통해 배포할 수 있습니다. EnvoyFilter가 Envoy 설정을 직접 편집하는 것이라면, WASM 플러그인은 **실행 가능한 코드**를 주입하여 **런타임에 새로운 기능**을 추가한다는 차이가 있습니다. 예를 들면:
  - 복잡한 **사용자 정의 인증/인가** 로직을 WASM으로 구현하여 Envoy가 매 요청마다 실행하도록 만들 수 있습니다.
  - 트래픽 데이터를 실시간으로 가공하여 **특정 외부 시스템으로 전송하는 로직**(예: custom 로그 포맷으로 출력)을 주입할 수 있습니다.
  - Istio 공식 기능으로 지원되지 않는 프로토콜에 대한 **프록시 지원**을 임시로 추가할 수도 있습니다.
  - WASM의 장점은 EnvoyFilter보다 **안전하게 기능을 확장**할 수 있고, 필요한 논리를 고수준 언어로 작성 가능하다는 것입니다. 다만 러닝커브가 존재하고 성능에 미치는 영향도 고려해야 합니다. 고급 단계의 학습자로서는 간단한 WASM 예제를 참고하여 Envoy가 “Hello World” 형태의 헤더 변환을 수행하도록 플러그인을 삽입해보는 식으로 실습해볼 수 있습니다. Istio **Extensibility** 문서 및 SDK 튜토리얼을 참고하면 WASM 기반 플러그인 개발 및 배포 과정을 상세히 알 수 있습니다.

- **운영 및 모범 사례**: Istio를 생산 환경에 활용하려면 고려해야 할 고급 주제들이 있습니다. 
  - **성능 및 튜닝**: 사이드카 프록시를 전 서비스에 붙이면 생기는 **오버헤드**에 대비해 리소스 요청/제한을 조정하고, 필요 없는 기능(Mixer Telemetry v1은 예전 버전에서 삭제됨)을 비활성화하여 경량화할 수 있습니다. 또한 대용량 트래픽 환경에서는 **concurrency**나 **outlier detection** 등의 Envoy 설정을 조정해 성능을 높일 수 있습니다. Istio는 **프로파일(Profile)** 개념으로 기본, 고성능, 최소 기능 세트를 미리 제공하므로, 환경에 맞는 프로파일로 설치하는 것도 중요합니다.
  - **업그레이드 전략**: Istio는 자주 업데이트되므로 **메이저 업그레이드** 시 호환성에 신경써야 합니다. **Canary 업그ade** 방식으로 새로운 Istio 컨트롤 플레인을 기존 버전과 함께 운영하며 점진적으로 사이드카를 교체하는 접근이 권장됩니다. Istio 공식 문서의 Upgrade 가이드를 따라가며 **Istioctl 업그레이드**나 **Helm 업그레이드** 절차를 미리 연습해두는 것이 좋습니다.
  - **멀티테넌시**: 하나의 Istio 메쉬에 여러 팀/프로젝트의 서비스가 혼재하는 경우 네임스페이스 단위로 **네트워크 격리**를 설정하고, 각 팀별로 **Istio 정책**을 관리하도록 **RBAC 권한**을 부여하는 방법을 고려해야 합니다. 예를 들어, 팀 A의 네임스페이스에는 팀 A만 수정 가능한 **네임스페이스-스코프 Istio 권한**을 주고, 공통 인프라팀만 전체 Mesh 설정을 바꿀 수 있도록 권한을 제한하는 식입니다. 또 필요하다면 하나의 클러스터 내에 여러 개 Istio control plane을 설치해 (각각 별도 메쉬) 팀별로 독립적인 메쉬를 운영하는 방안도 있습니다.
  - **Ambient Mesh**: 사이드카 없이도 트래픽 제어를 가능하게 하는 Istio의 실험적 모드인 Ambient Mesh도 언급할 가치가 있습니다. Ambient Mesh에서는 **ztunnel**과 **waypoint**라는 경량 프록시 데몬셋을 사용하여, 애플리케이션 Pod 안에 사이드카를 넣지 않고도 트래픽을 투명하게 가로챕니다. 이는 사이드카 운영으로 인한 오버헤드를 줄이고 보안 컨텍스트 등 이슈를 완화하려는 새로운 접근입니다. 다만 현재(2025년 초 기준) Ambient 모드는 제한적인 기능만 지원하므로, **고급 연구 주제**로서 동향을 파악하는 수준이면 충분합니다. 장기적으로 Istio의 발전 방향을 이해하고 싶다면 Ambient Mesh의 개념과 구성 방식을 공식 블로그나 문서를 통해 살펴보세요.

### 실습 과제 (Hands-On Practice)

- **멀티 클러스터 Istio 설치**: 두 개 이상의 Kubernetes 클러스터에 걸쳐 Istio 메쉬를 구성하는 실습을 합니다. 공식 [멀티클러스터 설치 가이드](https://istio.io/latest/docs/setup/install/multicluster/)에 따라, 예를 들어 Cluster-1과 Cluster-2에 모두 Istio를 설치하되 **공통의 루트 인증서**를 사용하도록 설정합니다 ([Istio / Install Multicluster](https://istio.io/latest/docs/setup/install/multicluster/#:~:text=Follow%20this%20guide%20to%20install,mesh%20that%20spans%20multiple%20clusters)). 그런 다음 Cluster-1의 Bookinfo 서비스들을 Cluster-2에서도 접근할 수 있게 네트워크 통합을 수행합니다. Istio **Gateway**를 이용해 클러스터 간 트래픽이 서로 통신하도록 설정하고, **ServiceEntry**로 원격 클러스터의 서비스들을 로컬처럼 인식시키는 작업 등을 진행합니다. 설정 완료 후 Cluster-2의 `productpage`가 Cluster-1의 `reviews` 서비스에 요청을 보내 별점이 표시되는지를 테스트해봅니다. 또한 **failover**를 시험하기 위해 Cluster-1의 `reviews`를 일시 중지하고, Istio가 자동으로 Cluster-2의 백업 서비스로 트래픽을 넘기는지 확인합니다. 이 멀티클러스터 실습을 통해 지리적으로 분산된 서비스 메쉬 운영과 교차 클러스터 호출, 장애 조치 등에 대한 이해를 높입니다.

- **Mesh 확장: VM 워크로드 추가**: Kubernetes 외부의 VM을 Istio 메쉬에 참여시켜 봅니다. 예를 들어, VM 상에서 동작하는 오래된 데이터베이스 서비스에 사이드카 프록시를 설치하고 Istio 메쉬로 편입시킵니다. Istio 공식 문서의 [가상 머신 연결](https://istio.io/latest/docs/setup/install/virtual-machine/) 가이드를 참고하여, VM에 필요한 토큰과 인증서를 배포하고, VM에서 Istio **VM 설치 스크립트**를 실행해 사이드카(Ebpf 또는 일반 Envoy)를 구동시킵니다. 그런 다음 Kubernetes의 서비스에서 이 VM 서비스로 호출해 보고, Istio를 통해 트래픽이 암호화되고 제어되는지 확인합니다. 이 실습을 통해 **하이브리드 메쉬** 운용 방법과 잠재적인 문제(예: DNS 해석, 방화벽 설정)를 다뤄볼 수 있습니다.

- **EnvoyFilter 적용**: 간단한 **EnvoyFilter** 예제를 통해 Istio 구성 확장을 실습합니다. 케이스 스터디로, **모든 출향(outbound) HTTP 트래픽에 커스텀 헤더 추가** 요구사항이 있다고 합시다. 이를 구현하기 위해 `EnvoyFilter` 리소스를 생성하여 Envoy의 HTTP Connection Manager 필터 체인에 envoy.lua 또는 wasm 필터를 삽입합니다. (Lua 스크립트를 이용해 모든 응답 헤더에 `X-Envoy-Test: true`를 추가하는 예제가 쉬운 시작점입니다.) 해당 EnvoyFilter를 `istio-system` 등의 mesh 전역에 적용한 뒤, Bookinfo 애플리케이션의 응답 헤더를 확인해 기대한 헤더가 추가되었는지 검증합니다. 또한 `istioctl proxy-config listeners/pods` 등을 사용해 Envoy 설정이 패치된 것을 확인해봅니다. 잘 동작했다면 EnvoyFilter를 제거하고 원상복구를 확인합니다. 추가로, EnvoyFilter 설정에 잘못된 값(Envoy 버전과 호환되지 않는 설정 등)을 넣었을 때 Envoy가 해당 구성(**NACK**)을 거부하여 적용되지 않음을 관찰해 보는 것도 도움이 됩니다 (이런 경우 Istiod 로그에 경고가 나타납니다). 이 실습으로 **Istio 기능의 한계를 넘어서는 커스텀 제어** 방법을 익히되, 동시에 이러한 방법의 위험성도 이해합니다 ([Istio / Envoy Filter](https://istio.io/latest/docs/reference/config/networking/envoy-filter/#:~:text=NOTE%201%3A%20Some%20aspects%20of,are%20removed%20and%20replaced%20appropriately)).

- **WASM 플러그인 체험**: 준비가 되었다면, 간단한 **WebAssembly 플러그인**을 Istio에 배포해봅니다. 예를 들어 Proxy-Wasm 기본 튜토리얼에 있는 **HTTP 헤더 변환** 모듈을 빌드하여 사용합니다. Rust 또는 Go로 "Hello Istio" 문자열을 응답 헤더에 추가하는 WASM 모듈을 작성하고, 이를 컴파일하여 `.wasm` 파일을 얻습니다. Istio의 `WasmPlugin` 리소스를 생성하여 해당 모듈을 특정 프록시에 주입하도록 설정합니다. (예: `productpage` 사이드카의 요청 처리 시 동작하도록 설정) 설정 적용 후 `productpage` 서비스를 호출하여 응답 헤더에 "Hello Istio"가 포함되는지 확인합니다. WASM 모듈의 로그를 출력하도록 만든 경우 `kubectl logs`로 사이드카 로그를 확인해 모듈이 실행된 흔적을 볼 수도 있습니다. 이 실습은 매우 고급 주제이므로 필수는 아니지만, Istio의 **무한한 확장 가능성**을 보여주는 좋은 예시입니다 ([Istio / Architecture](https://istio.io/latest/docs/ops/deployment/architecture/#:~:text=,telemetry%20generation%20for%20mesh%20traffic)). (주의: WASM 플러그인은 실험적인 영역이므로, 실제 운영보다는 R&D 측면에서 접근해야 합니다.)

- **Ambient Mesh 탐구 (선택 사항)**: 실습 클러스터가 충분하다면, Istio의 Ambient Mesh 모드를 시험해볼 수 있습니다. 공식 [Ambient 모드 가이드](https://istio.io/latest/docs/ambient/)에 따라 사이드카가 없는 데이터 플레인을 구성해보고, 기존 사이드카 모드와의 차이를 비교합니다. 예를 들어 Ambient 모드에서 `nginx` 같은 Pod을 배포하고, `ztunnel`이 해당 Pod의 트래픽을 자동 처리하는지를 확인합니다. `waypoint` 프록시를 배치하여 L7 정책(예: HTTP 경로별 라우팅)이 작동하게 한 후, VirtualService가 아닌 Kubernetes Gateway API를 통해 트래픽을 조정해 봅니다. Ambient 모드는 현재 제한적 기능만 지원하므로 깊게 실습하기는 어렵지만, **Istio의 미래 방향**을 살펴보고 사이드카 방식과의 트레이드오프를 이해하는 데 도움이 됩니다.

### 사용 사례 (Use Cases)

- **다중 클러스터/다중 지역 서비스 메쉬**: 글로벌 서비스를 운영하는 기업에서, **지역별로 분산된 클러스터들을 하나의 Istio 메쉬로 연결**하여 사용한 사례가 있습니다. 북미와 유럽에 각각 Kubernetes 클러스터를 운영하던 한 SaaS 업체는 Istio 멀티클러스터를 도입해, 사용자 위치에 따라 **가장 가까운 지역의 서비스로 트래픽을 유도**하고 장애 시 다른 지역으로 **자동 페일오버**되도록 구성했습니다. 예를 들어 미국 사용자의 요청은 기본적으로 미국 클러스터의 서비스들이 처리하지만, 만약 해당 서비스에 문제가 생기면 Istio가 유럽 클러스터의 동일한 서비스로 트래픽을 넘겨주었습니다. 또한 두 클러스터 모두에 공통 인증서를 써서 mTLS를 구현, 지역 간 통신도 암호화하였습니다. 이 결과 하나의 통합된 **글로벌 서비스 메쉬**가 만들어져, 지역 단위 장애에도 서비스 가용성을 높이고 복잡한 DNS 페일오버 없이 애플리케이션 레벨에서 빠른 전환이 가능했습니다. 이와 같이 Istio의 **멀티클러스터 기능**은 높은 **신뢰성**과 **지연 최소화**를 요구하는 워크로드에 활용됩니다.

- **EnvoyFilter를 통한 특수 기능 구현**: 한 스타트업에서는 Istio를 사용하면서, API 호출마다 **고객의 API 키를 헤더에 자동 첨부**해야 하는 요구사항이 있었습니다. 애플리케이션 코드를 모두 수정하기 어려웠기 때문에, 대신 Istio **EnvoyFilter**를 활용하는 방법을 택했습니다. 팀은 Envoy의 Lua 필터를 이용해 들어오는 요청에 특정 헤더(`X-API-Key`)를 주입하는 스크립트를 작성하고 EnvoyFilter로 배포했습니다. 그 결과 모든 서비스로 향하는 요청은 Istio 사이드카에서 자동으로 API 키 헤더가 추가된 후 백엔드로 전달되었습니다. 이 방법을 통해 **레거시 애플리케이션을 수정하지 않고도 공통 기능을 적용**할 수 있었지만, 이후 Istio 업그레이드 시 Lua 스크립트 호환성 문제를 겪어 유지보수의 어려움도 겪었습니다. 이 사례는 EnvoyFilter를 사용해 **Istio 기본 기능을 확장**할 수 있음을 보여주지만, 동시에 **관리 부담**도 따를 수 있음을 시사합니다 ([Istio / Envoy Filter](https://istio.io/latest/docs/reference/config/networking/envoy-filter/#:~:text=NOTE%201%3A%20Some%20aspects%20of,are%20removed%20and%20replaced%20appropriately)). 고급 사용자는 이러한 trade-off를 고려하여 EnvoyFilter 사용 여부를 판단해야 합니다 (가능하면 Istio 정책으로 풀 수 있는지 재검토하는 것이 권장됨).

- **WebAssembly로 정책 오프로드**: 대기업의 내부 플랫폼 팀이 Istio WASM 플러그인을 활용한 사례가 있습니다. 이 팀은 서비스 코드에 흩어져 있던 **공통 검증 로직**(예: 특정 요청 헤더 검증 후 없으면 거절)을 제거하고 플랫폼 레벨에서 일괄 적용하기 원했습니다. 이에 해당 로직을 WebAssembly 모듈로 구현하여 Istio 사이드카에 탑재하였고, 결과적으로 애플리케이션 코드에서 중복 검증을 없애는 데 성공했습니다. WASM 모듈은 Istio **WasmPlugin**으로 배포되어, 각 서비스 사이드카 Envoy에 **동적으로 로드**되었습니다. 이 방식의 이점은, 새로운 정책이 필요할 때마다 모듈만 업데이트하면 모든 서비스에 즉시 반영된다는 것입니다. 다만 WASM 모듈의 성능과 안정성 검증에 많은 시간을 투자해야 했고, 일부 모듈은 Envoy 버전 업 시 재컴파일이 필요했습니다. 이 사례는 Istio WASM을 통해 **플랫폼 기능을 서비스 메쉬에 녹여낼 수 있음**을 보여주며, 향후 서비스 메쉬의 역할이 단순 트래픽 관리에서 **애플리케이션 레벨의 단일화된 정책 엔진**으로 확장될 가능성을 시사합니다.

### 참고 자료 (Resources)

- **멀티클러스터 가이드** – [Install Multicluster](https://istio.io/latest/docs/setup/install/multicluster/): 다중 클러스터에 Istio 메쉬를 설치하는 공식 가이드 ([Istio / Install Multicluster](https://istio.io/latest/docs/setup/install/multicluster/#:~:text=Follow%20this%20guide%20to%20install,mesh%20that%20spans%20multiple%20clusters)). 네트워크 토폴로지(단일/이중 네트워크)와 컨트롤 플레인 토폴로지(멀티프라이머리 vs 프라이머리-리모트)별로 설치 절차를 제시합니다. 또한 [Deployment Models](https://istio.io/latest/docs/ops/deployment/deployment-models/) 문서에서는 멀티클러스터 구성 시 고려사항과 옵션들을 자세히 설명하므로 함께 참고하면 좋습니다.

- **운영 모범 사례** – [Deployment Best Practices](https://istio.io/latest/docs/ops/best-practices/): Istio 메쉬 운영 시 리소스 사용, 보안, 모니터링, 업그레이드 등에 대한 권장 사항을 정리한 문서. 사이드카 자원 최적화, 컨피그 병합 전략, 대규모 환경에서의 설계 팁 등이 포함되어 있습니다. 또한 [Canary Upgrades](https://istio.io/latest/docs/setup/upgrade/canary/) 가이드는 Istio를 안전하게 업그레이드하는 방법을 안내합니다.

- **EnvoyFilter 레퍼런스** – [EnvoyFilter API Reference](https://istio.io/latest/docs/reference/config/networking/envoy-filter/): EnvoyFilter 리소스의 구조와 사용 가능한 필드에 대한 상세 문서 ([Istio / Envoy Filter](https://istio.io/latest/docs/reference/config/networking/envoy-filter/#:~:text=,EnvoyFilters%20in%20the%20workload%E2%80%99s%20namespace)). EnvoyFilter가 적용되는 대상(워크로드 셀렉터, Listener/Cluster 등)과 패치 동작(ADD, REMOVE, MERGE 등)에 대한 설명을 제공합니다. 공식 위키의 [EnvoyFilter Samples](https://github.com/istio/istio/wiki/EnvoyFilter-Samples)에는 몇 가지 유용한 EnvoyFilter 예제가 있으므로, 실제 작성 시 참고할 수 있습니다.

- **Extensibility (WASM)** – [Extensibility and WASM](https://istio.io/latest/docs/concepts/wasm/): Istio에서 WebAssembly를 통해 기능을 확장하는 개념을 다룬 문서. WASM의 장단점, Istio 환경에서 WASM 플러그인을 배포하는 방법, Sandbox 제약 등을 설명합니다. 실습을 위해 Istio 공식 블로그의 *WASM 확장 튜토리얼*이나 GitHub의 Proxy-Wasm 예제 프로젝트를 참고하면 구체적인 구현 방법을 배울 수 있습니다.

- **Ambient Mesh** – [Ambient Mesh 소개](https://istio.io/latest/blog/2022/introducing-ambient-mesh/): Istio 개발팀이 발표한 Ambient Mesh 개념 소개 블로그. 사이드카 없는 메쉬의 동작 방식, ztunnel/waypoint 아키텍처, 현재 지원 기능과 향후 로드맵을 다룹니다. Ambient Mesh 관련 [공식 문서](https://istio.io/latest/docs/ambient/)도 있으며, 사이드카 모드와의 설정 비교, 제약사항 등이 정리되어 있으므로 관심 있는 독자는 참고하면 좋습니다.

