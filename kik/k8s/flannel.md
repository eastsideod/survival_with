알겠습니다! Flannel의 공식 문서를 바탕으로, 쿠버네티스나 네트워크 지식이 부족한 분들도 단계별로 따라가며 학습할 수 있도록 초보자부터 고급자까지 아우르는 내용을 정리해 드릴게요. 각 단계는 개념 설명, 실습 가이드, 주요 설정 옵션 등을 포함하고, Markdown 형식으로 제공될 예정입니다.

완성되면 바로 공유드릴게요.

# Flannel 단계별 학습 가이드 (초급/중급/고급)

쿠버네티스(Kubernetes) 클러스터에서 파드(Pod)들이 원활하게 통신하려면 **네트워크 플러그인**이 필요합니다. **Flannel**은 쿠버네티스 환경에서 가장 널리 사용되는 네트워크 플러그인 중 하나로, **단순하고 설정이 쉬운** L3 (Layer 3) 네트워크 패브릭을 제공하여 클러스터 내 모든 파드들이 IP를 통해 서로 통신할 수 있게 해줍니다 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=Platforms%20like%20Kubernetes%20assume%20that,sharing%20a%20single%20host%20IP)). 이 가이드에서는 **Flannel 공식 문서**를 기반으로, Flannel을 처음 접하는 **초급자**부터 심화 활용이 가능한 **고급 사용자**까지 단계별로 학습할 수 있도록 내용을 정리했습니다. 각 단계별로 **개념 설명**, **관련 기능 및 설정**, **실습 예제**, **유의사항(자주 겪는 문제)**를 다룹니다. 쿠버네티스나 컴퓨터 네트워크에 익숙하지 않은 독자도 이해할 수 있도록 쉬운 언어로 풀어 설명하며, 필요한 경우 그림과 표, 코드 블록을 활용하여 최대한 **읽기 쉽게** 작성했습니다.

## 초급 단계

### 개념 설명 (기본 개념 및 동작 원리)
**쿠버네티스 네트워킹 모델:** 쿠버네티스에서는 **모든 파드가 클러스터 내의 모든 다른 파드와 IP로 직접 통신할 수 있어야** 한다는 모델을 채택합니다. 각 파드는 자체 **고유 IP**를 가지며, 통신 시 별도의 NAT(Network Address Translation) 없이 **직접 라우팅**됩니다. 이러한 통신 모델은 하나의 호스트 IP를 여러 컨테이너가 공유하면서 포트를 나누는 복잡성을 제거해주지만, 이를 구현하려면 추가적인 네트워크 구성 요소가 필요합니다 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=Platforms%20like%20Kubernetes%20assume%20that,sharing%20a%20single%20host%20IP)). 즉, 쿠버네티스 자체에는 내장된 클러스터 네트워킹 기능이 없고, **별도의 CNI(Container Network Interface) 플러그인**이 각 노드에 설치되어야 파드 간 통신이 가능해집니다.

**Flannel이란?:** Flannel은 CoreOS에서 시작된 **오픈 소스 쿠버네티스 네트워크 플러그인(CNI)**으로, **오버레이 네트워크** 방식을 통해 여러 노드에 걸친 **평탄한(Flat) L3 네트워크**를 구성합니다 ([Understanding the Basics of Kubernetes Flannel Networking - WafaTech Blogs](https://wafatech.sa/blog/devops/kubernetes/understanding-the-basics-of-kubernetes-flannel-networking/#:~:text=Flannel%20creates%20a%20virtual%20Layer,ease%20of%20use%20are%20paramount)) ([Understanding the Basics of Kubernetes Flannel Networking - WafaTech Blogs](https://wafatech.sa/blog/devops/kubernetes/understanding-the-basics-of-kubernetes-flannel-networking/#:~:text=2,pod%20can%20be%20addressed%20independently)). 간단히 말해, **여러 노드를 하나의 거대한 가상 네트워크 스위치로 묶어준다**고 이해할 수 있습니다. Flannel은 각 쿠버네티스 노드마다 작은 데몬(`flanneld`)을 실행시켜, **클러스터 전체 IP 주소 범위 중 일부를 각 노드에 할당**하고, 노드 간에 패킷을 전달해주는 역할을 합니다 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=Flannel%20runs%20a%20small%2C%20single,the%20network%20configuration%2C%20the%20allocated)). 예를 들어 클러스터 전체 네트워크를 `10.244.0.0/16`으로 정했다면, 노드 A에는 `10.244.1.0/24`, 노드 B에는 `10.244.2.0/24` 식으로 **노드별 파드 서브넷**을 떼어주는 식입니다. 이렇게 하면 각 노드의 파드들은 자신에게 할당된 작은 서브넷 안에서 IP를 받으며, 클러스터 전체적으로는 **IP 주소가 겹치지 않는 통합 네트워크**를 형성하게 됩니다.

**오버레이 네트워크와 VXLAN:** Flannel의 기본 동작 방식은 **오버레이 네트워크**입니다. 즉, **노드 간 통신을 위해 실제 물리 네트워크 위에 가상의 Layer 3 네트워크를 얹어서** 패킷을 주고받습니다. 기본 설정에서 Flannel은 **VXLAN**이라는 기술을 사용하는데, 이는 **레이어 2 프레임을 UDP로 캡슐화하여 전송**하는 표준 프로토콜입니다. 쉽게 말해, **파드 IP 패킷을 노드 간 전달할 때 UDP로 한 번 더 감싸서 보내고, 받는 쪽에서 이를 풀어** 원래 패킷을 해당 파드로 전달합니다. VXLAN은 리눅스 커널에 내장된 기능을 사용하므로 성능과 안정성이 뛰어나며, 클라우드 환경 등 **L2 네트워크가 공유되지 않는 상황에서도 동작**할 수 있다는 장점이 있습니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=VXLAN%20is%20the%20recommended%20choice,kernels%20that%20don%27t%20support%20VXLAN)) ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=VXLAN)). 이러한 캡슐화를 통해 **각 파드는 동일한 클러스터 내라면 상대 노드에 상관없이 자신의 IP를 가지고 통신**할 수 있게 됩니다.

**Flannel의 구성 요소:** 쿠버네티스에서 Flannel은 일반적으로 **DaemonSet**으로 배포되어 각 노드에서 `flanneld` 데몬이 실행됩니다. 이 데몬은 **쿠버네티스 API 서버**를 통해 클러스터 네트워크 정보를 공유받거나 (혹은 별도의 Etcd를 사용할 수도 있습니다) **자신에게 할당된 파드용 서브넷 대역**을 확보합니다. 그리고 `flanneld`는 해당 서브넷을 사용할 수 있도록 노드에 **가상 인터페이스**(`flannel.1` 등)와 **라우팅 규칙**을 설정합니다 ([flannel/Documentation/running.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/running.md#:~:text=Flannel%20will%20acquire%20a%20subnet,network%20and%20start%20routing%20packets)) ([flannel/Documentation/running.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/running.md#:~:text=docker%20run%20,vxlan)). 또한 Flannel은 쿠버네티스에서 CNI 플러그인으로 동작하기 위해, 각 노드에서 **CNI 설정 파일**과 **CNI 플러그인 바이너리**를 배치하여 kubelet이 파드를 만들 때 Flannel CNI를 통해 가상 이더넷(veth) 페어 생성 및 브리지 연결 등을 수행하도록 합니다. 이러한 과정은 보통 **InitContainer** 등을 통해 자동으로 이뤄지므로, 사용자는 Flannel 데몬셋만 배포하면 추가적인 수작업 없이 파드 네트워킹이 설정됩니다.

### 관련 Flannel 기능 및 설정 소개 (초급 단계)
초급 단계에서는 Flannel의 **기본 기능과 설정** 위주로 살펴보겠습니다. Flannel은 기본 설정만으로도 동작하도록 설계되어 있어, 초보자가 깊이 있는 설정을 바꾸지 않고도 **곧바로 사용할 수 있는 환경**을 제공합니다. 다음은 주요 기본 기능과 설정 사항입니다:

- **쿠버네티스 API 연동 (Subnet Manager):** Flannel을 쿠버네티스 클러스터에서 사용할 때는 일반적으로 **쿠버네티스 API를 백엔드 데이터베이스로 사용**합니다. 이를 Flannel에서는 "*kube subnet manager*" 모드라고 부르며, `--kube-subnet-mgr` 옵션을 통해 활성화됩니다. 이 모드에서는 별도로 Etcd를 구성하지 않고, **노드의 파드 CIDR 정보를 쿠버네티스 노드 객체**를 통해 얻습니다 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=Though%20not%20required%2C%20it%27s%20recommended,as%20the%20kube%20subnet%20manager)) ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=If%20you%20use%20custom%20,network%20to%20match%20your%20one)). 쿠버네티스 컨트롤 플레인이 각 노드에 고유한 파드 CIDR 대역을 미리 할당해놓았다면, Flannel 데몬은 기동 시 그 정보를 읽어와 해당 노드의 서브넷으로 사용합니다. *(참고: 쿠버네티스 클러스터를 생성할 때 `--pod-network-cidr` 옵션으로 전체 파드 대역(예: 10.244.0.0/16)을 지정하면, 노드별 CIDR이 자동 할당됩니다.)*

- **Network/Subnet 설정:** Flannel은 **클러스터 전체 네트워크 범위(Network)**와 **노드당 서브넷 크기(SubnetLen)** 등을 설정할 수 있습니다. 기본 제공되는 매니페스트 (`kube-flannel.yml`)에서는 `10.244.0.0/16` 대역을 사용하도록 설정되어 있습니다 ([flannel/Documentation/kube-flannel.yml at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/kube-flannel.yml#:~:text=)). 이 뜻은 클러스터 내 모든 파드 IP는 10.244.X.X 형태로 할당되며, 각 노드는 그 중 /24 크기(256개 IP)의 서브넷을 받게 됩니다. 이러한 기본값은 kubeadm 등 많은 배포 도구에서 사용하므로 별 문제가 없지만, **이미 이 대역이 다른 용도로 쓰이거나 충돌하는 경우** 변경이 필요합니다. **다른 네트워크 대역을 사용하려면**, Flannel 배포 전에 매니페스트의 ConfigMap 내 `net-conf.json`에서 `"Network": "<새로운 CIDR>"` 값을 수정해야 합니다 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=kubectl%20apply%20)) ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=If%20you%20use%20custom%20,network%20to%20match%20your%20one)). (예: `10.100.0.0/16` 등) 초급 단계에서는 기본값을 그대로 사용하길 권장하지만, 이러한 설정이 있다는 것을 알아두세요.

- **백엔드(Backend) 종류:** Flannel은 내부적으로 패킷을 어떻게 전달할지에 대해 몇 가지 **백엔드 옵션**을 제공합니다. 기본값은 위에서 언급한 **VXLAN** (`"Type": "vxlan"`)이며, 별도 설정하지 않으면 VXLAN 모드로 동작합니다 ([flannel/Documentation/kube-flannel.yml at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/kube-flannel.yml#:~:text=)). 초급 단계에서는 VXLAN만 알아도 충분합니다. VXLAN 외에도 **host-gw, wireguard, udp** 등의 백엔드가 있는데, 각각 특성이 조금씩 다릅니다. 이러한 내용은 중급 단계에서 자세히 다룰 예정입니다. 

- **IP Masquerade(IP Masq):** Flannel은 **클러스터 외부로 나가는 트래픽 처리**를 위해 **IP Masquerade** 설정을 제공하며, 매니페스트에서 `--ip-masq` 플래그로 제어됩니다 ([flannel/Documentation/kube-flannel.yml at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/kube-flannel.yml#:~:text=)). 기본 매니페스트에는 `--ip-masq` 옵션이 활성화되어 있어, **파드가 외부 인터넷과 통신할 때** 출발 IP를 노드의 IP로 **마스커레이드(NAT)** 해줍니다. 이를 통해 파드들이 인터넷에 접근하거나 호스트 네트워크와 통신할 때 반환 패킷이 경로를 제대로 찾아올 수 있게 합니다. (쿠버네티스 **Service를 통한 외부 통신**에서도 이 masquerade 설정이 영향을 줍니다.) 초급 단계에서는 이 옵션을 기본값대로 두면 되며, 보통 특별한 경우가 아니라면 활성화된 상태를 유지합니다.

- **기본 MTU 설정:** Flannel은 VXLAN 등의 encapsulation으로 인한 **MTU 감소**를 자동으로 계산하여 적용합니다 ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=MTU%20is%20calculated%20and%20set,be%20changed%20as%20backend%20config)). 예컨대 물리 인터페이스 MTU가 1500바이트인 환경에서 VXLAN을 사용하면, Flannel은 약 50바이트의 오버헤드를 고려하여 **파드 네트워크의 MTU를 1450**으로 설정합니다. 이 값은 `/run/flannel/subnet.env` 파일에 기록되어 Docker 데몬이나 CNI 플러그인이 컨테이너 인터페이스 설정에 참고하게 됩니다 ([flannel/Documentation/running.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/running.md#:~:text=After%20flannel%20has%20acquired%20the,and%20MTU%20that%20it%20supports)) ([flannel/Documentation/running.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/running.md#:~:text=Now%20,written%20a%20subnet%20config%20file)). 초급 사용자는 MTU를 따로 수정할 필요는 없지만, 환경에 따라 큰 MTU를 지원한다면 Flannel이 자동 조정할 것임을 알고 있으면 됩니다.

### 실습 예제 (Flannel 배포 및 기본 동작 확인)
이번 섹션에서는 실제로 Flannel을 쿠버네티스 클러스터에 배포하고 **파드 간 통신이 되는지 확인하는 간단한 실습**을 해보겠습니다. 클러스터가 이미 준비되어 있다는 가정 하에 진행하며, **쿠버네티스가 설치되어 있고 `kubectl`을 사용할 수 있어야** 합니다. (만약 아직 클러스터가 없다면 Minikube 등을 이용해 멀티노드 환경을 구성할 수 있습니다.)

**1단계: Flannel 배포**  
공식 문서에서는 Flannel을 배포하는 가장 쉬운 방법으로 준비된 YAML 매니페스트를 적용하는 것을 권장합니다 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=)). 다음 명령을 실행하여 Flannel을 설치하세요:

```bash
kubectl apply -f https://raw.githubusercontent.com/flannel-io/flannel/master/Documentation/kube-flannel.yml
``` 

위 명령은 Flannel 관련 리소스(네임스페이스 `kube-flannel`, 서비스어카운트, ClusterRole 등과 DaemonSet)를 클러스터에 생성합니다. 배포가 완료되면 `kube-flannel` 네임스페이스에서 **DaemonSet `kube-flannel-ds`**가 실행되며, 각 노드마다 하나씩 **`kube-flannel` Pod**가 생성됩니다. 

적용 후, 다음과 같이 Flannel Pod들이 정상적으로 실행 중인지 확인합니다:

```bash
kubectl get pods -n kube-flannel -o wide
```

예상 출력:
```
NAME                 READY   STATUS    RESTARTS   AGE   IP           NODE    ...
kube-flannel-ds-abcde 1/1     Running   0          1m    10.244.1.1   node1   ...
kube-flannel-ds-fghij 1/1     Running   0          1m    10.244.2.1   node2   ...
```

위 출력 예시는 노드 **node1**과 **node2**에 각각 Flannel Pod가 올라가 있고, 각 Pod에 할당된 IP(`10.244.1.1`, `10.244.2.1`)를 보여줍니다. 이 IP들은 Flannel이 각 노드에 할당한 파드 서브넷 내에서 첫 번째 IP로 주어진 것입니다. (`10.244.1.0/24`의 .1, `10.244.2.0/24`의 .1)

**2단계: 네트워크 연결 확인**  
Flannel 설치 후, 간단한 방법으로 **파드 네트워크가 정상 동작하는지** 확인할 수 있습니다. 두 개의 노드에 각각 파드를 하나씩 띄우고 통신해보겠습니다. 우선 각 노드에 하나씩 배치되도록 파드를 생성합니다. 예를 들어, `ping-test1`과 `ping-test2`라는 파드를 Node 간에 띄워보겠습니다:

```yaml
# ping-test-pods.yaml
apiVersion: v1
kind: Pod
metadata:
  name: ping-test1
spec:
  containers:
  - name: ping1
    image: busybox
    command: ["sleep", "3600"]
  nodeSelector:
    kubernetes.io/hostname: node1  # node1에 스케줄링
---
apiVersion: v1
kind: Pod
metadata:
  name: ping-test2
spec:
  containers:
  - name: ping2
    image: busybox
    command: ["sleep", "3600"]
  nodeSelector:
    kubernetes.io/hostname: node2  # node2에 스케줄링
```

위 매니페스트에서 `nodeSelector`를 통해 각 파드가 특정 노드에 올라가도록 했습니다. 해당 YAML 파일을 적용합니다:

```bash
kubectl apply -f ping-test-pods.yaml
```

이제 파드가 뜨면, `ping-test1` 파드에서 `ping-test2` 파드로 **ping**을 보내 보겠습니다. `ping-test1` 파드의 셸에 들어갑니다:

```bash
kubectl exec -it ping-test1 -- sh
```

셸에 진입한 뒤, `ping-test2` 파드의 IP로 ping을 수행합니다. `ping-test2`의 IP는 `kubectl get pod ping-test2 -o wide` 명령으로 확인할 수 있습니다. 예를 들어 `ping-test2`의 IP가 `10.244.2.5`로 나온다면:

```bash
/ # ping -c 4 10.244.2.5
```

예상 출력:
```
PING 10.244.2.5 (10.244.2.5): 56 data bytes
64 bytes from 10.244.2.5: seq=0 ttl=62 time=0.789 ms
64 bytes from 10.244.2.5: seq=1 ttl=62 time=0.634 ms
64 bytes from 10.244.2.5: seq=2 ttl=62 time=0.712 ms
64 bytes from 10.244.2.5: seq=3 ttl=62 time=0.650 ms

--- 10.244.2.5 ping statistics ---
4 packets transmitted, 4 packets received, 0% packet loss
```

`0% packet loss`와 함께 응답이 오면 **Flannel 네트워크가 정상적으로 동작**하고 있다는 뜻입니다. 서로 다른 노드에 있는 파드 간에도 사설 IP로 직접 통신이 가능해졌습니다. 만약 ping이 실패한다면, Flannel이 제대로 설치되었는지, 파드들이 Running 상태인지, 방화벽 설정은 올바른지 등을 점검해야 합니다.

**3단계: 간단한 서비스 통신 테스트 (선택)**  
추가로, 하나의 파드에서 간단한 **HTTP 서버**를 열고 다른 파드에서 접속해보는 예제를 해볼 수 있습니다. 예를 들어 `ping-test2` 파드에서 웹 서버를 실행하고 `ping-test1`에서 접속해봅니다:

- `ping-test2` 파드 안에서 BusyBox의 httpd를 실행:
  ```bash
  kubectl exec -it ping-test2 -- sh -c "httpd -f -p 8080 &"
  ```
  (간단히 8080 포트로 파일 서비스를 시작합니다. 백그라운드로 실행)

- `ping-test1` 파드에서 `wget`으로 접속 테스트:
  ```bash
  kubectl exec -it ping-test1 -- wget -qO- 10.244.2.5:8080
  ```
  만약 `index.html`이 있었다면 그 내용이 출력될 것이고, 없었다면 빈 응답이지만 **연결 자체는 성공**할 것입니다. 이로써 Flannel을 통한 **L3 네트워크 통신이 정상 동작**함을 확인했습니다.

### 유의할 점 및 자주 겪는 문제 (초급 단계)
Flannel을 처음 사용하는 단계에서 흔히 겪는 문제들과 주의사항을 정리합니다. 아래 사항들을 체크하면 **초기 설정 오류를 상당 부분 예방**할 수 있습니다.

- **쿠버네티스 버전 및 설정:** Flannel은 대부분의 쿠버네티스 버전에서 잘 동작하지만, 클러스터를 만들 때 **파드 네트워크 CIDR을 지정**하지 않았다면 문제가 생길 수 있습니다. 예를 들어 kubeadm으로 클러스터 생성 시 `--pod-network-cidr=10.244.0.0/16` 옵션을 주지 않았다면, **노드 객체에 파드 CIDR 정보가 없어서 Flannel이 할당을 못하는 문제**가 발생할 수 있습니다. 이 경우 Flannel Pod 로그에 "failed to acquire subnet" 등의 오류가 뜰 수 있습니다. **해결:** 클러스터를 재설정하거나, Flannel 대신 수동 etcd 모드를 사용해야 하는 복잡함이 있으므로, **처음부터 클러스터 설정에 올바른 CIDR을 지정**하는 것이 좋습니다.

- **커널 모듈 및 시스템 설정:** Flannel은 **리눅스 커널의 br_netfilter 모듈**을 필요로 합니다. 이 모듈이 로드되지 않은 상태에서는 Flannel이 네트워크 브리지를 통한 패킷 전달을 처리하지 못해 동작에 문제가 생길 수 있습니다. 최근 kubeadm (v1.30 기준)에서는 `br_netfilter` 로드 여부를 자동으로 체크하지 않으므로, Flannel 사용 전에 사용자가 수동으로 `modprobe br_netfilter`를 실행하거나 `/etc/modules-load.d/`에 추가하여 **해당 모듈이 항상 로드되도록** 해야 합니다 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=tar%20)). 또한 `/proc/sys/net/bridge/bridge-nf-call-iptables` 값을 `1`로 설정하여 브리지된 트래픽에 iptables 룰이 적용되도록 시스템 설정을 조정해야 합니다. (`kubeadm init` 실행 시 이 값은 자동으로 설정됨)

- **방화벽 설정:** Flannel(특히 VXLAN 모드)은 **노드 간 UDP 통신을 위한 포트**를 사용합니다. VXLAN의 경우 리눅스 커널 기본 포트인 UDP **8472번 포트**를, 그리고 UDP 백엔드를 사용할 경우 UDP **8285번 포트**를 사용합니다 ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=When%20using%20,8285%20for%20sending%20encapsulated%20packets)). 만약 노드 방화벽(예: `ufw`, `firewalld`)이나 클라우드 보안 그룹에서 이 포트를 막고 있다면, **노드 간 캡슐화 트래픽이 차단되어 파드 통신이 되지 않습니다**. **해결:** 모든 노드 간에 해당 UDP 포트(8472)를 **허용**합니다. 예를 들어 `firewalld`를 쓰는 경우:
  ```bash
  firewall-cmd --permanent --zone=public --add-port=8472/udp
  firewall-cmd --reload
  ```
  를 실행합니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=In%20case%20,cmd)). 방화벽 설정 변경 후에는 Flannel Pod를 재시작하거나 몇 초 기다린 뒤 파드 통신을 다시 테스트합니다. 또한, 쿠버네티스 마스터 노드(API 서버)에 대해 **파드 네트워크 CIDR에서 오는 트래픽을 허용**해야 합니다. (Flannel이 IP Masq을 켠 경우에는 파드->마스터 통신이 노드 IP로 NAT되어 문제가 없지만, Masquerade가 꺼진 설정이라면 마스터는 파드 IP를 SRC로 가진 패킷을 받게 되므로 이를 허용해야 함)

- **Docker 데몬 iptables 정책:** Docker를 사용중인 환경에서는 Docker가 기본 체인의 iptables 정책을 변경해서 예기치 않은 문제가 생길 수 있습니다. 예를 들어 Docker 1.13+ 버전부터 기본 `FORWARD` 체인 정책이 `DROP`으로 설정되어, **쿠버네티스 파드 트래픽이 iptables에서 드롭**되는 현상이 보고되었습니다 ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=Connectivity)). 만약 파드 간 통신이 안 되고 Flannel 설정이 정상이라면, `iptables -L FORWARD`로 정책을 확인해보세요. 정책이 DROP이라면 다음과 같이 수정합니다:
  ```bash
  iptables -P FORWARD ACCEPT
  ```
  그리고 Docker 설정 파일(`/etc/docker/daemon.json`)에 `"iptables": false`를 설정하거나, Docker 서비스 시작 시 `--iptables=false` 옵션을 주어 Docker가 FORWARD 정책을 건드리지 않도록 할 수 있습니다. (단, `"iptables": false`를 하면 Docker가 컨테이너용 iptables 규칙 추가를 안 하므로 환경에 따라 주의가 필요)

- **Flannel Pod 로그 확인:** Flannel이 의도대로 동작하지 않을 때는 우선 각 노드의 `kube-flannel-ds` Pod 로그를 확인해야 합니다:
  ```bash
  kubectl logs -n kube-flannel <flannel-pod-name>
  ```
  대표적인 오류 메시지로 "*failed to read net config: open /etc/kube-flannel/net-conf.json: no such file or directory*" 등이 있는데, 이는 ConfigMap 마운트 문제가 있을 때 나옵니다 ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=match%20at%20L367%20Log%20messages)). 또는 "*Error registering network: permission denied*"와 같은 메시지는 Pod의 권한 문제일 수 있습니다. Flannel DaemonSet은 `NET_ADMIN` 등의 권한이 필요하며, 매니페스트에서 이를 설정하지만, 만약 수정하다가 이 부분을 누락하면 이러한 에러가 날 수 있습니다 ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=Permissions)). 이러한 로그들을 분석하면 설정 미스나 환경 문제를 파악하는 데 도움이 됩니다.

---

초급 단계에서는 Flannel의 개념과 기본 사용법, 그리고 설치 직후 발생할 수 있는 흔한 문제 해결까지 다루었습니다. 다음 **중급 단계**에서는 Flannel의 다양한 설정과 최적화, 그리고 백엔드 교체와 같은 좀 더 심화된 주제를 설명합니다.

## 중급 단계

### 개념 설명 (심화 개념 및 동작 이해)
중급 단계에서는 Flannel의 **내부 동작과 다양한 옵션**에 대해 좀 더 깊이 알아보겠습니다. 초급 단계에서 Flannel이 "노드별 서브넷을 할당하고 VXLAN으로 패킷을 캡슐화한다"는 큰 그림을 배웠다면, 이제는 **Flannel의 백엔드 종류와 동작 차이**, **구성 변경 방법**, **성능 고려사항** 등을 다룹니다.

**Flannel 백엔드란?:** Flannel의 "*백엔드(Backend)*"는 **노드 간 통신에 사용되는 메커니즘**을 뜻합니다. 쉽게 말해, **파드 트래픽을 어떤 방식으로 다른 노드에 전달할지**를 결정하는 설정입니다. Flannel은 여러 종류의 백엔드를 지원하며, 공식 문서에서는 **VXLAN을 기본이자 권장 옵션**으로, 그 외에 **host-gw**, **WireGuard**, **UDP**, **IPIP** 등을 선택적으로 제공합니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=VXLAN%20is%20the%20recommended%20choice,kernels%20that%20don%27t%20support%20VXLAN)) ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=VXLAN)). 각 백엔드는 장단점과 요구 사항이 다르므로, 클러스터 환경에 맞게 선택할 수 있습니다. 아래는 주요 백엔드들의 개념적인 설명입니다:

- **VXLAN** – *기본값*. 커널 레벨 VXLAN 장치를 사용하여 **L2 over L3** 캡슐화를 수행합니다. **장점:** 설정이 간편하고 (별도 요구사항 거의 없음) 대부분 환경에서 동작, Kubernetes 클러스터에서 가장 광범위하게 사용됨. **단점:** 패킷을 캡슐화하므로 약간의 **오버헤드**가 있으며, 캡슐화된 패킷은 결국 UDP로 전송되기 때문에 매우 고성능이 요구되는 환경에서는 CPU 부하가 생길 수 있음. 그러나 전반적으로 VXLAN은 최신 리눅스 커널에서 잘 최적화되어 있고 하드웨어 오프로드도 지원하는 경우가 있어 **일반적인 선택지로 충분한 성능**을 발휘합니다.

- **host-gw** – 호스트 게이트웨이 모드. **캡슐화를 전혀 사용하지 않고**, 각 노드 간에 **IP 경로(route)**를 직접 설정하여 통신합니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=host)). 예를 들어 노드 A의 파드 대역 `10.244.1.0/24`에 대해 노드 B가 경로를 갖고, 그 다음 홉을 노드 A의 IP로 하는 식입니다. **장점:** 캡슐화가 없으므로 **오버헤드가 최소화**되고, 패킷이 직접 전달되므로 **지연시간이 낮고 네이티브 IP 라우팅 성능**을 기대할 수 있습니다. **단점:** 노드들이 서로 **L2 네트워크로 직접 연결**되어 있거나, 최소한 각 노드가 다른 노드들의 IP로 라우팅할 수 있는 환경이어야 합니다. 일반적인 클라우드(VPC) 환경에서는 인스턴스들이 서로 L3 라우팅만 가능하고, 또한 AWS같은 경우 인스턴스에 *소스/데스트 체크*가 기본 활성화되어 임의의 경로전달 패킷을 드롭할 수 있기 때문에 host-gw 사용이 까다롭습니다. 주로 **베어메탈 환경**이나 **동일 스위치망**에서, 높은 성능이 요구될 때 사용합니다. (OpenShift Origin 등 일부 배포판에서는 기본 네트워크로 host-gw 모드의 Flannel을 사용하기도 했습니다 ([Flannel - Additional Concepts | Architecture | OpenShift Origin Branch Build](https://people.redhat.com/aweiteka/docs/preview/20170510/architecture/additional_concepts/flannel.html#:~:text=OpenShift%20Origin%20runs%20flannel%20in,flanneld%2C%20which%20is%20responsibile%20for)) ([Flannel - Additional Concepts | Architecture | OpenShift Origin Branch Build](https://people.redhat.com/aweiteka/docs/preview/20170510/architecture/additional_concepts/flannel.html#:~:text=default%20via%20192,200%20dev%20eth0)).)

- **WireGuard** – 비교적 새로운 옵션으로, **WireGuard VPN** 기술을 이용하여 노드 간 터널을 만들고 **암호화된** overlay 네트워크를 구성합니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=WireGuard)). **장점:** 노드 간 트래픽이 암호화되므로 **보안성이 높아** 부외부 네트워크를 통과하거나 여러 조직이 공유하는 인프라에서 클러스터를 운영할 때 **트래픽 기밀성**을 보장할 수 있습니다. 또한 WireGuard는 커널 모듈로 동작하여 비교적 성능도 우수한 편입니다. **단점:** WireGuard 키 관리가 필요하고, VXLAN 대비 설정이 조금 복잡합니다. 또한 **커널 5.6 이상** 또는 별도 모듈 설치가 필요하며, Windows 노드는 지원하지 않습니다. 주로 **보안을 중시하는 환경**에서 사용합니다.

- **UDP** – Flannel 초기 기본 백엔드였던 방법으로, **VXLAN 없이 UDP로 직접 캡슐화**하여 패킷을 전송합니다. 현재는 **디버깅 용도 또는 매우 구형 커널(VXLAN 미지원)** 환경에서만 사용을 권장합니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=VXLAN%20is%20the%20recommended%20choice,kernels%20that%20don%27t%20support%20VXLAN)). UDP모드는 VXLAN처럼 커널 수준 최적화가 없고 userspace에서 처리하는 부분이 있어 성능이 떨어집니다. 따라서 실서비스에는 잘 쓰지 않습니다.

- **IPIP (Alpha)** – 리눅스 커널의 **IP-in-IP 터널** 기능을 이용하여 L3 패킷을 직접 L3로 캡슐화하는 실험적 모드입니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=IPIP)). IPIP는 encapsulation overhead가 낮고 구성도 단순하지만, **multicast나 IPv6 트래픽은 지원하지 못하는** 제약이 있습니다. 아직 실험적이므로 일반적으로 권장되진 않습니다.

위 주요 백엔드들의 특성을 정리하면 다음과 같습니다:

| **백엔드 유형** | **전달 방식**                 | **장점**                         | **주의사항**                  |
|-----------------|------------------------------|----------------------------------|-------------------------------|
| VXLAN (기본)    | L2 Over UDP 캡슐화            | 설정 간편, 어느 환경이나 지원    | 약간의 성능 오버헤드          |
| host-gw         | L3 라우팅 (캡슐화 없음)       | 낮은 지연, 높은 처리량          | L2 네트워크 필수, 클라우드 제약|
| WireGuard       | 암호화 터널 (UDP)             | 트래픽 암호화 (보안)            | 키 관리 필요, 커널 모듈 필요  |
| UDP             | L3 UDP 캡슐화 (비표준)        | 구형 커널 대응 (예비 옵션)      | 낮은 성능, 디버그 용도        |
| IPIP (실험적)   | L3 IP-in-IP 캡슐화           | 낮은 오버헤드                   | IPv4 유니캐스트만 지원        |

**Flannel 설정 파일 (net-conf.json):** Flannel의 설정은 ConfigMap으로 제공된 JSON 파일로 관리됩니다. 이 파일 (`/etc/kube-flannel/net-conf.json`)에는 앞서 언급한 **Network** 대역과 **Backend 타입** 등이 정의되어 있습니다. 예시를 보면: 

```json
{
  "Network": "10.244.0.0/16",
  "EnableIPv6": false,
  "Backend": {
    "Type": "vxlan"
  }
}
``` 

이렇게 기본설정이 있고, 필요한 경우 여기에 **추가 옵션**들을 넣어줄 수 있습니다. 예를 들어 VXLAN의 `Port`(포트 번호)나 WireGuard의 `PSK`(프리쉐어키) 같은 것이 이에 해당합니다. 공식 문서에 따르면, 백엔드별로 사용 가능한 키와 값들은 정해져 있습니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=,DirectRouting%20is%20not%20supported)) ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=WireGuard)). 중급 사용자라면 ConfigMap의 내용을 편집하여 Flannel 동작을 커스터마이징할 수 있습니다.

**노드 별 설정 (명령행 옵션):** 반면, **Flannel의 실행 옵션** 중에는 전체 ConfigMap이 아닌 **노드별로 개별 지정해야 하는 것들도** 있습니다. 예를 들어 **다중 NIC가 있는 환경에서 어느 인터페이스를 쓸지 지정**하는 `--iface` 옵션이나, 특정 IP를 public IP로 사용하도록 하는 `--public-ip` 옵션 등이 있습니다 ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=,This%20can%20be%20specified%20multiple)) ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=,subnet)). 이러한 플래그들은 ConfigMap이 아니라 Flannel 데몬셋의 컨테이너 명령줄에 설정됩니다. 기본 매니페스트에서는 `--iface`를 지정하지 않는데, 이는 **Flannel이 자동으로 적절한 인터페이스(기본 라우트가 설정된 인터페이스)**를 선택하도록 하기 위함입니다. 하지만 특정 상황에서는 수동 지정이 필요합니다. 이에 대해서는 아래 유의사항에서 추가 설명합니다.

### Flannel 기능 및 설정 변경 (중급 단계)
이번에는 Flannel의 설정을 변경하거나 기능을 활용하는 방법들을 알아보겠습니다. **백엔드 변경**, **파드 네트워크 변경**, **노드 인터페이스 지정** 등의 시나리오를 다뤄보겠습니다.

1. **백엔드 변경 (VXLAN -> host-gw 등):** 기본 VXLAN 대신 다른 백엔드를 쓰고 싶다면, **ConfigMap의 `Backend` 섹션을 수정**하면 됩니다. 예를 들어 host-gw 모드를 사용하려면, ConfigMap (`kube-flannel-cfg`)의 `net-conf.json` 내용을 다음과 같이 바꿉니다:
   ```json
   {
     "Network": "10.244.0.0/16",
     "Backend": {
       "Type": "host-gw"
     }
   }
   ```
   변경 후에는 **Flannel Pod들을 재시작**하여(new config 적용을 위해) 다시 기동해야 합니다. (예: `kubectl delete pod -n kube-flannel -l app=flannel`을 실행하면 DaemonSet이 다시 생성) 백엔드 변경 시 주의할 점은, **이미 파드들이 통신 중인 상황에서 변경하면 순간적으로 네트워크 단절이 발생**할 수 있고, 또한 VXLAN->host-gw처럼 방식이 완전히 달라지면 기존 설정이 남아서 문제를 일으킬 수 있습니다. 가능하면 **클러스터 초기** 또는 **새로 구성할 때** 원하는 백엔드를 선택하는 것이 좋습니다. 또한 host-gw나 WireGuard 등은 환경 요건(L2 연결, 커널 모듈 등)이 충족되는지 먼저 확인해야 합니다. Host-gw의 경우 모든 노드가 같은 서브넷에 있어 서로 직접 ARP/통신 가능해야 하며, 클라우드인 경우 **각 VM의 소스/데스티네이션 체크를 비활성화**하고 (AWS EC2의 인스턴스 속성) **VPC 라우팅 테이블에 각 파드 서브넷 경로를 추가**해야 할 수 있습니다. WireGuard의 경우 **모든 노드에 최신 WireGuard가 동작 가능한 커널/모듈이 있어야** 하고, 51820/UDP 등의 포트를 허용해야 합니다.

2. **Pod 네트워크 대역 변경:** 이미 Flannel을 적용한 후 **파드 CIDR을 변경**하는 것은 복잡한 작업입니다. 일반적으로는 **클러스터를 다시 구성**해야 합니다. 그러나 만약 부득이하게 변경해야 한다면, **모든 파드를 삭제**하고, Flannel ConfigMap의 `"Network"`를 수정한 뒤 Flannel을 재시작하고, 파드를 재배포하는 절차가 필요합니다. 이 과정에서 기존 IP를 가진 파드와 신규 IP 파드 간 충돌 등이 있을 수 있으므로, 프로덕션 환경에서는 권장되지 않습니다. 따라서 **초기에 네트워크 대역을 올바르게 정하는 것**이 중요합니다. (예: 회사 내부망과 겹치지 않는 대역으로 선택)

3. **노드 인터페이스 지정:** Flannel이 기본 인터페이스를 잘못 선택하는 경우가 있습니다. 예를 들어, **노드에 이중 인터페이스**가 있고, 기본 게이트웨이가 외부망을 향해 있지만 실제 노드 간 통신은 내부망 인터페이스로 해야 더 빠른 환경이라면, Flannel이 엉뚱하게 외부망 NIC를 잡을 수 있습니다. 이런 경우 DaemonSet의 명령행 옵션에 `--iface=<인터페이스명>` 또는 `--iface-regex` 등을 추가해줘야 합니다 ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=,This%20can%20be%20specified%20multiple)) ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=times%20to%20check%20each%20regex,Defaults)). 변경 방법은 DaemonSet의 spec.containers.args 부분에 해당 플래그를 넣고 재배포하면 됩니다. 예를 들어 노드 내부망 인터페이스 이름이 eth1이라면:
   ```yaml
   spec:
     containers:
     - name: kube-flannel
       image: ...
       args:
         - --ip-masq
         - --kube-subnet-mgr
         - --iface=eth1    # 추가
   ```
   이렇게 하고 Flannel Pod들을 재시작하면 모든 노드에서 eth1 IP를 서로 **Public IP(노드 간 통신 IP)**로 사용하게 됩니다 ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=Interface%20selection%20and%20the%20public,IP)) ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=interface%20on%20a%20host,the%20second%20interface%20is%20chosen)). 만약 특정 노드만 다르게 지정해야 한다면, DaemonSet이 아닌 개별 Pod 설정을 바꿔야 하므로 현재 구조에서는 어렵습니다. 보통은 모든 노드가 동일한 네트워크 환경(동일한 인터페이스 명 등)을 가진 경우가 많으므로, 전역 설정으로 충분합니다.

4. **Dual-stack (IPv6) 사용:** Flannel은 **이중 스택(dual-stack)**을 지원하여 파드에 IPv4와 IPv6를 모두 할당할 수 있습니다 ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=Flannel%20supports%20dual,gw%28linux%29%20backends)). 이를 사용하려면 ConfigMap 설정에 `"EnableIPv6": true`와 `"IPv6Network": "<IPv6대역>"` 등을 추가하고, 쿠버네티스 클러스터도 IPv6를 활성화해야 합니다. Dual-stack 모드는 아직 비교적 새로운 기능이므로 고급 설정에 속하지만, 중급 단계에서 알아둘 것은 Flannel로 IPv6도 다룰 수 있다는 점입니다. 예를 들어:
   ```json
   {
     "Network": "10.244.0.0/16",
     "IPv6Network": "fd00:10:244::/48",
     "EnableIPv6": true,
     "Backend": { "Type": "vxlan" }
   }
   ```
   위와 같이 설정하면 각 노드는 IPv4 /24 서브넷과 IPv6 /64 서브넷을 하나씩 받게 되고, 파드들도 IPv4/IPv6 이중 IP를 갖게 됩니다. 이 경우 환경의 IPv6 라우팅 설정, 노드 OS의 IPv6 forwarding 허용 등의 추가 작업이 필요합니다. Dual-stack은 **VXLAN, host-gw, WireGuard 백엔드에서만** 지원됩니다 ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=Flannel%20supports%20dual,gw%28linux%29%20backends)).

5. **Flannel과 Network Policy:** 기본 Flannel은 **네트워크 폴리시(NetworkPolicy)**를 직접 지원하지 않습니다 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=Flannel%20is%20responsible%20for%20providing,guidance%20on%20integrating%20with%20Docker)). 즉, Kubernetes `NetworkPolicy` 리소스를 생성해도 Flannel만으로는 통신 제한이 적용되지 않습니다. 이를 구현하려면 **다른 솔루션과 연동**해야 합니다. 대표적으로 **Canal**이라고 불리는 조합이 있는데, **Calico의 네트워크 정책 엔진**을 Flannel과 함께 사용하는 방식입니다. Calico의 policy 전용 모드를 설치하면, Pod간 통신 경로는 Flannel이 처리하고 정책 제어는 Calico가 담당하게 됩니다 ([Install Calico for policy and flannel (aka Canal) for networking](https://docs.tigera.io/calico/latest/getting-started/kubernetes/flannel/install-for-flannel#:~:text=Install%20Calico%20for%20policy%20and,policy%20to%20secure%20cluster%20communications)) ([Install Calico for policy and flannel (aka Canal) for networking](https://docs.tigera.io/calico/latest/getting-started/kubernetes/flannel/install-for-flannel#:~:text=Install%20Calico%20for%20policy%20and,policy%20to%20secure%20cluster%20communications)). 최근에는 Flannel 환경에서 동작하는 경량 네트워크 정책 컨트롤러도 등장하여 (`kubernetes-sigs/kube-network-policies` 프로젝트) Flannel 위에 간단한 정책 적용이 가능해졌습니다 ([flannel/Documentation/netpol.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/netpol.md#:~:text=Network%20policy%20controller)). 중급 사용자는 "Flannel 단독으로는 모든 Pod가 모든 Pod에 통신 허용"이 기본임을 이해하고, 보안이 필요하면 이러한 추가 도구를 고려해야 합니다.

### 실습 예제 (구성 변경 및 문제 해결)
이 섹션에서는 중급 단계에서 다룬 설정들을 실험해보는 예제를 소개합니다. **주의:** 실습은 실환경이 아닌 테스트 클러스터에서 진행하세요. 잘못된 네트워크 설정 변경은 클러스터 통신 장애를 유발할 수 있습니다.

**예제 1: VXLAN에서 host-gw 모드로 전환**  
가정: 클러스터 노드들이 모두 같은 2계층 네트워크상에 있고(예: 동일 스위치에 연결된 베어메탈 서버들), 더 높은 성능을 위해 host-gw를 쓰려 한다.

- *1단계:* Flannel ConfigMap 편집:
  ```bash
  kubectl -n kube-flannel edit configmap kube-flannel-cfg
  ```
  편집기에서 `net-conf.json`의 `"Backend": { "Type": "vxlan" }` 부분을 `"Type": "host-gw"`로 수정하고 저장합니다.

- *2단계:* 노드 OS 설정 점검:
  - 각 노드에서 Linux 커널의 IP Forwarding이 활성화되어 있는지 (`/proc/sys/net/ipv4/ip_forward` = 1) 확인합니다.
  - (AWS EC2일 경우 각 인스턴스의 **Src/Dst Check**를 Disable해야 하며, VPC 라우팅 테이블에 각 파드 서브넷 경로가 존재하는지 확인합니다.)
  - 방화벽에 특별히 열어야 할 포트는 host-gw 모드에는 없습니다 (캡슐화 없음).

- *3단계:* Flannel 데몬셋 재시작:
  ```bash
  kubectl rollout restart daemonset kube-flannel-ds -n kube-flannel
  ```
  이 명령으로 각 노드의 Flannel Pod가 순차 재시작됩니다. 모든 Pod가 Running이 되면 전환 완료입니다.

- *4단계:* 확인:
  - **라우팅 테이블 확인:** 임의의 노드에 접속하여 `ip route` 명령을 보면, 다른 노드의 파드 서브넷에 대한 경로가 추가된 것을 볼 수 있습니다. 예: `10.244.2.0/24 via 192.168.0.102 dev eth0` (node2의 파드CIDR이 node2의 호스트 IP를 경유하도록 node1에 경로 설정됨) ([Flannel - Additional Concepts | Architecture | OpenShift Origin Branch Build](https://people.redhat.com/aweiteka/docs/preview/20170510/architecture/additional_concepts/flannel.html#:~:text=default%20via%20192,200%20dev%20eth0)).
  - 파드 간 ping을 다시 시험하여 통신이 잘 되는지 확인합니다. 이상이 없다면 host-gw 전환 성공입니다. 통신 지연이나 처리량도 VXLAN 때보다 개선되었는지 `iperf` 등을 사용해 측정해볼 수 있습니다.

**예제 2: WireGuard 백엔드 적용**  
가정: 노드 간 트래픽 암호화를 위해 WireGuard를 적용하려 한다. (리눅스 노드 커널 5.6+, UDP 51820 포트 허용 가정)

- *1단계:* ConfigMap 편집 (`kube-flannel-cfg`):
  ```json
  {
    "Network": "10.244.0.0/16",
    "Backend": {
      "Type": "wireguard"
    }
  }
  ```
  필요한 경우 프리쉐어 키(PSK)를 사용하려면 `"PSK": "<base64로 인코딩된 키>"`를 추가합니다. 키는 `wg genpsk`로 생성 가능 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=Use%20in,and%20encrypt%20the%20packets)).

- *2단계:* 각 노드에 WireGuard 커널 모듈 설치/활성 확인:
  - 최신 리눅스 배포판은 이미 WireGuard 모듈이 포함돼 있으나, 그렇지 않다면 `apt install wireguard` 등을 통해 설치합니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=WireGuard%20tools%20like%20,to%20debug%20interfaces%20and%20peers)).
  - NodeSecurityGroup/방화벽에 UDP 51820(및 IPv6의 경우 51821) 포트를 열어둡니다.

- *3단계:* Flannel 데몬셋 재시작 (rollout restart).

- *4단계:* 확인:
  - 각 노드에서 `wg` 명령 (WireGuard 툴)으로 WireGuard 인터페이스 설정을 조회할 수 있습니다 (`wg show`). Flannel은 `flannel-wg`라는 인터페이스를 만들고, 노드 간 피어(peer)를 자동 구성합니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=this%20path)) ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=The%20static%20names%20of%20the,to%20debug%20interfaces%20and%20peers)). 예를 들어 Node1에서 Node2에 대한 피어가 설정되어 있고 PublicKey, AllowedIPs (Node2의 파드CIDR) 등이 출력됩니다.
  - 파드 통신을 테스트하여 문제없음과, 노드 간 패킷이 캡쳐 시 암호화되어 나가는지 (`tcpdump` 등으로 확인) 점검합니다.

**예제 3: Flannel 문제 상황 해결 - 인터페이스 선택 오류**  
가정: 노드들이 두 개의 네트워크(내부망 `eth1`, 외부망 `eth0`)에 연결되어 있고, Flannel이 잘못 외부망으로 통신하려고 해서 패킷 손실이 발생한다.

- *증상:* Flannel Pod 로그에 `Subnet added: 10.244.2.0/24 via <외부망 IP>` 같은 로그가 찍혀있고, 그 IP가 다른 노드에서 접근 불가하거나, ping 시 응답이 없다.
- *해결:* DaemonSet의 `--iface` 옵션 추가. (`kubectl edit daemonset kube-flannel-ds -n kube-flannel`)
  - `args` 섹션에 `--iface=eth1` 추가. 저장 후 DaemonSet이 자동으로 rolling update됨.
  - 적용 후 Flannel Pod 로그에 이번엔 `<내부망 IP>`를 경유지로 하여 경로를 설정했다는 메시지가 보일 것입니다.
  - 파드 통신을 다시 테스트해서 정상화되었는지 확인합니다.

이러한 실습을 통해 **Flannel 설정을 커스터마이징하고 문제를 해결하는 방법**을 익혔습니다. 중급 단계에서는 백엔드 변경이나 네트워크 환경 조정을 다뤘는데, 이를 통해 Flannel을 다양한 시나리오에 맞게 활용할 수 있게 됩니다.

### 유의할 점 및 자주 겪는 문제 (중급 단계)
중급 단계의 설정 변화 시에 흔히 맞닥뜨리는 이슈들과, 알아두면 좋은 추가 사항들을 정리합니다:

- **백엔드 변경 시 일관성:** Flannel의 백엔드는 **클러스터 전체에서 통일**되어야 합니다. 일부 노드만 다른 백엔드를 쓰는 것은 지원되지 않으며, 그렇게 되면 노드 간 통신이 불가능해집니다. 또한 한번 네트워크가 운영 중인 클러스터에서 백엔드를 변경하는 것은 위험할 수 있으므로, 가급적 클러스터를 초기화하거나 **Maintenance 시간대를 확보**하고 진행해야 합니다. 공식 문서에서도 "백엔드는 런타임 중 변경하지 않는 것이 좋다"고 명시하고 있습니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=Flannel%20may%20be%20paired%20with,not%20be%20changed%20at%20runtime)).

- **host-gw 사용 시 클라우드 환경:** 클라우드에서 host-gw를 쓰려면 각 클라우드의 특성을 고려해야 합니다. 예를 들어 **AWS**에서는 각 EC2 인스턴스에 다른 호스트를 경유지로 하는 라우팅을 허용하려면 **Src/Dst Check**를 끄고, VPC 라우팅 테이블에 각 파드 서브넷(노드당 /24)을 대상지로 추가해주어야 합니다. (Terraform이나 Kubernetes용 Cloud Controller Manager의 기능을 이용해 자동화할 수도 있음) **GCP**의 경우 VPC가 자동으로 모든 인스턴스 /32 경로만 갖기 때문에, host-gw로는 Pod CIDR를 전파할 방법이 마땅치 않습니다. 이러한 이유로 클라우드에서는 대부분 VXLAN을 사용하는 것입니다.  
  또한 AWS에서 host-gw를 쓰려 할 때 **노드의 보안 그룹**이 파드 CIDR에서 오는 트래픽을 인식하지 못해 막는 경우도 있습니다. 해결책으로는 **AWS VPC CNI** 플러그인을 함께 쓰거나, 그냥 host-gw를 포기하고 VXLAN을 쓰는 것이 일반적입니다. (Flannel 자체에는 AWS나 GCE와 직접 연동하여 라우트 테이블에 경로를 추가하는 기능은 없습니다; 관련 내용은 *Integrations 문서*에 언급되어 있지만 수동 단계가 필요합니다.)

- **WireGuard 키 관리:** WireGuard 백엔드 사용 시 키 관리에 유의해야 합니다. 기본적으로 Flannel은 각 노드에 자동으로 키를 생성하지만, **재시작 시 키가 바뀌면 터널이 단절**될 수 있습니다. 이를 막으려면 생성된 개인 키(`/run/flannel/wgkey`)를 백업/복원하거나, **Static Private Key** 설정을 고려해야 합니다. (Flannel은 환경변수 `WIREGUARD_KEY_FILE`을 통해 키 경로를 변경할 수 있습니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=If%20no%20private%20key%20was,to%20change%20this%20path)).)

- **로그 수준 및 모니터링:** Flannel은 기본 로그 레벨이 INFO이며, `-v=1` 정도로 올리면 디버그에 도움이 되는 추가 메시지가 표시됩니다 ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=flannel%20network,version%3A%20print%20version%20and%20exit)). 지나치게 높이면(log level 5 등) 성능에 영향을 줄 수 있으니 주의합니다. 또한 Flannel 자체에는 Prometheus 메트릭 노출 기능은 없으므로, 노드나 CNI 레벨의 메트릭으로 모니터링을 해야 합니다. (ex: 노드의 network throughput, or CNI plugin latency from kubelet logs)

- **성능 튜닝:** 중급 사용자는 Flannel 네트워크의 성능에 관심을 갖기 시작합니다. **두 가지 주요 요소는 백엔드와 MTU**입니다 ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=There%20are%20two%20flannel%20specific,a%20big%20impact%20on%20performance)) ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=1,have%20the%20correct%20MTU%20on)). 앞서 백엔드 선택으로 성능을 조절할 수 있다는 것을 다뤘고, MTU 역시 중요합니다. 가능하면 **기본 물리망의 MTU를 크게** 해서 (예: 9000바이트, Jumbo Frame) Flannel overlay에 여유를 주면 파드 통신에도 더 높은 대역폭을 확보할 수 있습니다. Flannel은 자동으로 최적 MTU를 계산하지만, 환경에 따라 수동으로 `Backend` 설정에 `"MTU": <숫자>`를 넣어 원하는 MTU로 강제할 수도 있습니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=,2A)). 단, 파드 MTU를 너무 크게 설정하면 물리 네트워크에서 fragmentation이 발생할 수 있으니, **물리망 MTU보다 약간 작게** 설정해야 합니다.

- **여러 네트워크(Multiple networks):** 하나의 쿠버네티스 클러스터에 **두 개 이상의 독립적인 Pod 네트워크**를 운영해야 하는 특수한 경우가 있을 수 있습니다 (예: 일부 노드끼리는 별도 격리된 네트워크 사용 등). Flannel은 과거에 experimental하게 하나의 데몬으로 다중 네트워크를 지원한 적 있으나, 현재는 **한 데몬/한 DaemonSet당 하나의 네트워크만** 지원합니다 ([flannel/Documentation/running.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/running.md#:~:text=Multiple%20networks)). 만약 두 개의 Flannel 네트워크가 필요하다면, **별도의 ConfigMap, DaemonSet을 만들어 두 번째 Flannel 인스턴스**를 띄우는 방식으로 가능은 하나(예: `kube-flannel-2`), 이는 고급 사용자 영역이며 일반적으로 잘 하지 않습니다.

- **업그레이드:** Flannel 업그레이드는 새로운 버전의 이미지로 DaemonSet을 교체하면 됩니다. 다만 **버전 차이가 클 경우 설정파일 호환성**을 확인해야 합니다. 공식 문서의 업그레이드 가이드를 참고하여, 주요 변경사항(예: ConfigMap 스키마 변경)이 있는지 확인하세요. 예를 들어 v0.13 -> v0.14 업 시에 SubnetLen 기본값 변경 등의 이슈가 있었습니다. 업그레이드 시에도 약간의 네트워크 플랩(flapping)이 있을 수 있으니, 장애 허용 시간에 하거나 노드별 순차 drain 등의 방법을 사용합니다.

이상으로 중급 단계에서 주로 고려해야 할 사항들을 살펴봤습니다. 이제 Flannel의 작동 원리와 설정 변경에 대해 어느 정도 숙련되었으므로, 마지막 **고급 단계**에서는 더욱 내부적인 원리나 특수한 상황, 그리고 성능 최적화 및 트러블슈팅 심화 내용을 다루겠습니다.

## 고급 단계

### 개념 설명 (고급 개념 및 심화 내용)
고급 단계에서는 Flannel을 **심층적으로 이해**하고, 대규모 환경이나 특수한 경우에 필요한 지식, 그리고 문제 발생 시 **근본적인 원인 파악 및 해결**을 목표로 합니다. 또한 Flannel만으로 해결하기 어려운 요구사항에 대한 대안도 함께 살펴보겠습니다.

**Flannel의 동작 흐름 깊게 보기:** Flannel 데몬(`flanneld`)은 시작되면 다음과 같은 순서로 동작합니다:
1. **백엔드 초기화:** 먼저 지정된 백엔드 종류에 따라 필요한 커널 리소스(예: VXLAN 디바이스 생성, WireGuard 터널 설정 등)를 준비합니다. VXLAN의 경우 `flannel.1` 이라는 인터페이스를 만들고, WireGuard는 `flannel-wg` 인터페이스를 만듭니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=The%20static%20names%20of%20the,to%20debug%20interfaces%20and%20peers)).
2. **네트워크 구성 수신:** Flannel은 **데이터스토어**(쿠버네티스 API 또는 etcd)를 통해 전체 클러스터의 네트워크 설정을 가져옵니다. kube subnet manager 모드에서는 Kubernetes API를 통해 자신의 Node 객체 `.spec.podCIDR`를 확인합니다. etcd 모드라면 `/coreos.com/network/config` 키에 저장된 설정(JSON)을 읽어옵니다 ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=If%20the%20,conf.json)) ([flannel/Documentation/configuration.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/configuration.md#:~:text=The%20value%20of%20the%20config,dictionary%20with%20the%20following%20keys)).
3. **서브넷 할당:** 만약 **노드에 할당된 서브넷 정보가 없다면** Flannel이 직접 새로운 서브넷을 할당하거나 (etcd 모드일 때) Kubernetes 모드에서는 API서버가 이미 할당했어야 합니다. etcd 모드에서는 `/coreos.com/network/subnets/` 경로 아래에 각 노드의 서브넷 임대 정보를 작성하는데, Flannel은 etcd에서 **분산 락**을 활용하여 동시에 두 노드에 같은 서브넷을 주지 않도록 조정합니다. 이렇게 확보한 서브넷을 해당 노드의 `FLANNEL_SUBNET`으로 설정하고, 앞서 만든 인터페이스(`flannel.1` 등)에 IP를 할당합니다.
4. **라우팅 테이블 설정:** 백엔드에 따라 노드의 라우팅 테이블을 조정합니다. VXLAN의 경우 모든 Pod CIDR 대역에 대해서 `flannel.1` 인터페이스로 라우팅하도록 한 개의 규칙을 넣고(`10.244.0.0/16 dev flannel.1`), ARP 테이블 혹은 FDB(Forwarding Database)를 통해 **각 목적지 노드의 MAC 주소**를 VXLAN 디바이스에 알려줍니다. host-gw의 경우 각 노드별로 라우트 엔트리를 추가합니다 (ex: `10.244.2.0/24 via 192.168.0.102 dev eth0`). WireGuard는 자동으로 피어(peer) 설정 (AllowedIPs 등)을 하여 커버합니다.
5. **환경 변수 파일 기록:** Flannel은 결과적으로 확보한 서브넷과 계산된 MTU 등을 `/run/flannel/subnet.env` 파일에 저장합니다 ([flannel/Documentation/running.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/running.md#:~:text=After%20flannel%20has%20acquired%20the,and%20MTU%20that%20it%20supports)). 이 파일에는 `FLANNEL_SUBNET=10.244.1.1/24`, `FLANNEL_MTU=1450` 같은 내용이 담깁니다. Docker 데몬이 이를 읽어서 docker0 브리지 MTU 설정에 쓰거나, CNI 플러그인이 이 값을 참고하여 veth MTU를 설정합니다.
6. **실시간 모니터링:** Flanneld는 백그라운드로 **데이터스토어의 변화를 watch**합니다. 새로운 노드가 추가되어 서브넷을 할당받으면 라우팅 테이블에 추가하고, 노드가 내려가서 서브넷이 반환되면 경로를 제거합니다 ([flannel/Documentation/running.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/running.md#:~:text=Flannel%20will%20acquire%20a%20subnet,network%20and%20start%20routing%20packets)) ([flannel/Documentation/running.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/running.md#:~:text=It%20will%20also%20monitor%20,and%20adjust%20the%20routes%20accordingly)). Kubernetes API 모드에서는 Node 객체나 신설된 CRD(예: Lease) 등을 감시하여 비슷하게 처리합니다. 또한 일정 주기로(기본 24시간, 옵션 조정 가능) etcd 임대 갱신을 합니다.

이러한 내부 흐름을 이해하면, 예를 들어 **특정 노드만 통신 안 될 때** 무엇을 점검해야 하는지 감을 잡을 수 있습니다. (그 노드의 flanneld가 서브넷을 못 받았는지, 인터페이스 설정이 안 되었는지 등)

**고급 네트워크 최적화:** Flannel 자체는 심플한 구조지만, **대규모/고성능 환경**에서는 몇 가지 한계에 부딪힐 수 있습니다:
- 대량의 노드(수백~수천 대)에서 VXLAN 브로드캐스트 트래픽이나 ARP 부하 등이 커질 수 있습니다. VXLAN은 기본적으로 각 패킷 전송 시 대상 노드의 MAC을 FDB에서 찾아 encapsulate 하지만, 초기에는 FF:FF:FF:FF 브로드캐스트로 ARP 질의를 VXLAN으로 보내는 과정 등이 있습니다. 노드 수가 매우 많다면 이러한 오버헤드를 줄이기 위해 **DirectRouting** 옵션을 켤 수 있습니다. Flannel VXLAN 백엔드의 `DirectRouting: true`는 **같은 L2에 있는 노드들끼리는 VXLAN 대신 직접 라우팅(host-gw)으로 통신**하게 해주는 기능입니다 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=,used%20to%20encapsulate%20packets%20to)) ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=,Defaults)). 이를 활성화하면 같은 랙이나 서브넷 내 노드 간에는 성능 향상을 얻고, 그렇지 않은 경우만 VXLAN 캡슐화를 사용합니다.
- **팟 수 증가에 따른 ARP 테이블**: host-gw 모드에서는 각 노드가 모든 다른 노드의 IP를 다음 홉으로 가지는 경로를 갖습니다. 노드가 N이라면 N-1개의 경로가 있고, 각 경로에 대해 ARP를 관리해야 합니다. 대규모 환경에서는 ARP 캐시 크기를 늘리거나, 라우팅 테이블 사이즈 튜닝이 필요할 수 있습니다. (리눅스 기본 ARP 캐시나 라우팅 캐시 파라미터 조정 등)
- **패킷 처리 경로 튜닝:** Flannel이 overlay를 구성해 주지만, 실제 **컨테이너에서 나가는 패킷 경로**는 여러 단계를 거칩니다: veth -> 브리지(cni0) -> (iptables NAT/MASQ) -> flannel.1(VXLAN) -> 물리NIC. 이 경로에 있는 iptables rule 최적화 (특히 NAT table), CPU 바인딩, IRQ 조정 등은 고급 최적화 영역입니다. Flannel 자체 설정은 아니지만, 만약 수십 Gbps 트래픽을 처리해야 한다면 이러한 커널 튜닝이 필요하게 됩니다.

**트러블슈팅 심화:** 고급 사용자는 Flannel에 문제가 생겼을 때 원인을 찾기 위해 **다각도의 진단**을 할 수 있어야 합니다. 몇 가지 가이드:
- `flannel.1` 등의 **가상 인터페이스 상태**: `ip addr show flannel.1`로 IP가 제대로 붙어 있는지, `ip link show flannel.1`로 인터페이스가 UP 상태인지 확인합니다. (DOWN이면 flanneld가 뭔가 실패한 것)
- `ip route`로 라우팅 테이블 확인: 특정 파드 CIDR에 대한 경로가 없으면 그 노드와 통신 안 됩니다. 경로가 있는데도 통신 불가면 다음 단계로.
- `arp -n` 또는 VXLAN FDB 조회: `bridge fdb show dev flannel.1` 명령으로 VXLAN의 FDB를 볼 수 있습니다. 각 노드의 VTEP (각 노드의 flannel.1에 해당하는 MAC)이 보여야 하고, 없다면 flanneld가 peer 정보를 못 받은 것입니다.
- **etcd 사용 시**: etcdctl로 `/coreos.com/network/subnets/` 키 들을 열람하여 서브넷 임대 현황을 파악합니다. 혹은 etcd와의 통신 문제로 flannel이 갱신을 못할 수도 있으니, flannel 로그에 etcd 관련 에러가 없는지 확인합니다.
- **쿠버네티스 API 모드**: kube subnet manager 모드에서는 `kubectl get node -o yaml`로 각 Node의 `.spec.podCIDR` 값을 확인합니다. 비어 있으면 controller-manager 설정 문제입니다. 또한 Flannel은 Node의 annotation으로 상태를 남기기도 합니다 (예: flannel.alpha.coreos.com/public-ip). `kubectl describe node <name>`에서 Flannel 관련 annotation이나 이벤트를 보며 정보가 일관된지 점검합니다.
- **로그 메시지 해석:** 몇 가지 흔한 로그:
  - `"evicting reservation for <CIDR>"`: 노드가 종료되어 서브넷 회수 작업을 했다는 의미입니다.
  - `"Subnet added: <CIDR> via <IP>"`: 새로운 노드 경로 추가 (via IP가 해당 노드의 "Public IP").
  - `"Failed to register network: <err>"`: 주로 권한 문제 ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=Depending%20on%20the%20backend%20being,sudo)). Flannel Pod의 SecurityContext나 Capabilities 문제.
  - `"failed to acquire lease"`: etcd모드에서 임대 못 받았을 때. (다른 노드와 경합 혹은 etcd 권한 문제)
  - `"vxlan device already exists"`: 이미 flannel.1 인터페이스가 존재해서 충돌날 때. (이전에 종료 안 된 flanneld 프로세스가 남아있는 경우 등)
- **NAT 체크섬 이슈:** NAT 뒤에 있는 노드에서 VXLAN 쓸 때 UDP 체크섬 문제로 통신이 불안정할 수 있습니다 ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=When%20the%20public%20IP%20is,commands%20to%20avoid%20corrupted%20checksums)) ([flannel/Documentation/troubleshooting.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/troubleshooting.md#:~:text=%2Fusr%2Fsbin%2Fethtool%20)). 이런 경우 `ethtool -K flannel.1 tx-checksum-ip-generic off` 명령으로 offloading 끄는 해결법이 문서에 나와 있습니다. (VXLAN UDP checksum offloading이 NAT환경에서 문제될 때)

**대안 및 추가 도구:** 고급 단계에서는 "언제 Flannel만으로 부족한가?"를 고려해야 합니다. 예를 들어 **네트워크 정책이 필수**인 환경이라면 앞서 언급한 Canal 혹은 Calico를 써야 합니다. **고성능/저지연**이 절대적으로 중요하면, VXLAN같은 오버레이 대신 **BGP 기반**의 Calico나, eBPF 기반의 Cilium 등이 대안입니다. 또한 멀티클러스터 네트워킹이 필요하면 Submariner 같은 솔루션이 필요할 수도 있습니다. Flannel은 **설계 목표가 단순한 Kubernetes 네트워킹 구현**이므로, 고급 기능(정책, 서비스 메시 최적화, eBPF 가속 등)은 제공하지 않습니다. 따라서 Flannel로 시작해서 요구사항이 늘어나면 **다른 CNI로 전환**을 검토하게 될 수 있다는 점도 알고 있어야 합니다.

### 고급 활용 예제 및 시나리오
이번에는 고급 사용자가 접할만한 특수 시나리오나 문제 상황을 들어보고, Flannel을 활용하거나 개선하는 방법을 제시합니다.

**시나리오 1: 대규모 클러스터에서 초기 노드 부팅 지연**  
- *상황:* 500노드 규모 클러스터에서 새 노드가 추가되면, 다른 노드의 Flannel이 해당 노드의 라우트 정보를 적용하기까지 시간이 꽤 걸린다. (예: 수초 이상) 그 동안 새 노드 파드가 네트워크 통신에 지연을 겪음.
- *원인:* etcd(또는 API 서버) 이벤트 및 각 flanneld 처리에 **부하**가 걸리는 현상. Flannel은 기본 이벤트 큐 깊이가 5000으로 설정되어 있는데 (환경변수 `EVENT_QUEUE_DEPTH`) ([flannel/Documentation/kube-flannel.yml at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/kube-flannel.yml#:~:text=fieldPath%3A%20metadata)) ([flannel/Documentation/kube-flannel.yml at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/kube-flannel.yml#:~:text=)), 노드가 매우 많으면 초기에 처리해야 할 이벤트(경로 추가 등)가 몰릴 수 있습니다.
- *대응:* Flannel Manifest에 `EVENT_QUEUE_DEPTH`를 더 늘려준다 (예: 10000). 그리고 etcd/API 서버 성능도 점검하여, 특히 etcd의 IOPS나 CPU가 충분한지 확인합니다. 또한 VXLAN FDB 전파에 시간이 걸릴 수 있으므로, **DirectRouting** 옵션을 켜서 같은 L2 영역 내 노드들은 ARP로 바로 통신하게 하거나, VXLAN 조정 프로토콜을 활용하는 방안을 고려합니다. (Flannel은 기본적으로 **수동 FDB 관리**를 하나, `neigh`를 통해 미리 ARP를 리졸브해 둘 수도 있음)

**시나리오 2: 특정 노드의 Pod만 타 노드와 통신 불가**  
- *상황:* node5의 파드와 다른 노드 파드 간 ping이 안 된다. node5 <-> node6 사이만 문제이고, node5 <-> node1 등 대부분은 된다.
- *진단:* node6에서 `ip route`를 보니 `10.244.5.0/24` (node5 파드 대역)에 대한 경로가 **없다**. 다른 노드들엔 경로가 있는데 node6에만 누락. 이는 node6의 flanneld가 etcd/API의 해당 키 업데이트를 못 받았거나 반영을 못한 상태일 수 있다. node6 Flannel 로그에 `"Subnet added: 10.244.5.0/24"` 메시지가 안 보일 가능성이 높다.
- *해결:* node6의 flanneld 프로세스를 재시작 (Pod 재시작)하여 정보를 다시 싱크한다. 그리고 **버그 가능성**을 염두에 두고 Flannel 버전을 최신으로 올린다. 예전 버전에서 비슷한 증상이 이따금 보고되었고 (특정 race condition), 업그레이드로 해결된 경우가 있습니다. 만약 업그레이드로도 반복된다면, etcd와 node6 사이 네트워크 문제나, node6의 clock skrew 등에 의한 etcd watch 문제 등을 의심해야 합니다.

**시나리오 3: Flannel 사용 중 Calico로 전환** (외부 요구로 인한 CNI 교체)  
- *상황:* 현재 Flannel로 잘 동작 중인 클러스터에서, 보안 정책을 적용하기 위해 Calico로 교체해야 한다.
- *고려사항:* CNI 교체는 클러스터 전체 재구성이 필요한 큰 작업입니다. 일반적으로는 **노드 단위로 점진 교체** 전략을 취합니다. Calico 문서나 Tigera 공식 가이드에는 flannel->calico 마이그레이션 방법이 나와 있으니 참고합니다. 간략히:
  - 새 Calico DaemonSet을 적용하되, 아직 Flannel은 유지.
  - Calico가 Flannel의 etcd 데이터를 읽어와 동기화하며, 동시에 Flannel이 만든 인터페이스도 Calico가 인계받아 사용 (Canal과 유사하게).
  - 모두 준비되면 Flannel DaemonSet을 제거.
  - 이런 복잡한 단계가 필요하므로, 가능하면 처음부터 네트워크 정책 필요하면 Calico를 썼으면 더 좋았을 상황.

### 유의할 점 및 팁 (고급 단계)
마지막으로, 고급 사용자를 위한 추가 팁과 Flannel 운용상 주의사항들입니다:

- **Etcd 모드 고려:** 만약 쿠버네티스 API 서버의 부하를 줄이고 싶거나, 쿠버네티스 외부에서도 네트워크를 공유해야 한다면 Flannel을 **etcd 백엔드 모드**로 쓸 수 있습니다. 이런 경우 independent etcd 클러스터가 필요하며, `--etcd-endpoints`, `--etcd-prefix` 등의 옵션과 함께 `--kube-subnet-mgr` 옵션을 끄면 됩니다 ([github.com](https://github.com/flannel-io/flannel/raw/refs/heads/master/Documentation/configuration.md#:~:text=Configuration%20If%20the%20,%28Mandatory%20if)) ([github.com](https://github.com/flannel-io/flannel/raw/refs/heads/master/Documentation/configuration.md#:~:text=overridden%20using%20%60,%60SubnetLen)). etcd 모드는 쿠버네티스 없이 Docker Swarm 등과도 Flannel 네트워크를 공유할 수 있지만, 현대 쿠버네티스 환경에서는 Kubernetes API를 쓰는 편이 훨씬 간편합니다.

- **Prometheus나 ipamctl 등의 도구:** Flannel은 자체 모니터링 기능이 부족하므로, 오픈소스 커뮤니티에서 Flannel 환경을 관찰하기 위한 도구들이 나왔습니다. `flannel-prometheus-exporter` 같은 비공식 익스포터가 etcd 정보를 읽어 서브넷할당 현황 등을 메트릭으로 노출하기도 합니다. 또한 `kubectl get cm -n kube-flannel kube-flannel-cfg -o jsonpath='{.data.net-conf\.json}'` 명령으로 현재 설정을 스크립트로 확인하거나, etcd 모드일 때 `etcdctl ls /coreos.com/network/subnets` 등으로 현황을 살펴보는 방식도 있습니다.

- **최신 문서 및 이슈 추적:** Flannel은 비교적 단순한 프로젝트지만, 커뮤니티 이슈 트래커를 가끔 살펴보면 유용한 정보를 얻을 수 있습니다. 특히 커널/배포판 버전 업데이트에 따른 버그 (예: Ubuntu 21.10부터 RPi의 VXLAN 모듈 경로 문제 ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=to%20%600E))) 등이 있으므로, GitHub의 Issue나 Release 노트를 참고하면 문제 발생 시 빠르게 원인을 찾는데 도움이 됩니다. 또한 Flannel의 새 버전이 릴리스되면 changelog를 읽어보길 권장합니다.

---

이상으로 Flannel에 대한 **초급-중급-고급 단계별 학습 가이드**를 모두 마치겠습니다. 지금까지 살펴본 내용을 정리하면 다음과 같습니다:

- **초급:** Flannel의 목적과 기본 개념 (쿠버네티스 네트워킹, 오버레이), 설치 방법과 기본 사용, 초기 문제 해결(방화벽, 설정 누락 등).
- **중급:** Flannel의 다양한 백엔드 옵션과 설정 변경 방법, 환경에 맞춘 최적화 (인터페이스 지정, dual-stack 등) 그리고 부분적인 문제 해결과 네트워크 정책 연동 방안.
- **고급:** Flannel의 내부 작동 원리에 대한 깊은 이해, 대규모/특수 환경에서의 고려사항, 트러블슈팅 스킬과 성능 튜닝, 그리고 한계를 인지하고 다른 솔루션과의 비교까지.

Flannel은 **설치해서 큰 수정 없이 쓰기엔 매우 편리한 솔루션**이며, 쿠버네티스 입문자도 손쉽게 클러스터 네트워킹을 구축할 수 있게 해줍니다. 반면 고급 기능은 최소한으로 제공하기 때문에, 사용자가 요구에 맞게 적절한 설정을 하고 한계를 넘어서는 상황에서는 다른 대안을 병행하는 지혜가 필요합니다. 이 가이드를 통해 독자 여러분이 Flannel을 처음 접한 뒤 실전 운영까지 차근차근 나아가는 데 도움이 되었기를 바랍니다. 마지막으로, 실제 환경에 변화를 주기 전에는 **테스트 클러스터**에서 충분히 검증하고, 장애 대비책을 마련한 후 진행하는 것을 잊지 마세요. Flannel 공식 문서와 커뮤니티도 늘 참고하여 최신 정보를 얻으면 큰 도움이 될 것입니다. 

즐거운 쿠버네티스 네트워킹 실험 되세요! 🚀

**참고 문헌:** Flannel 공식 GitHub 문서 일 ([GitHub - flannel-io/flannel: flannel is a network fabric for containers, designed for Kubernetes](https://github.com/flannel-io/flannel#:~:text=Networking%20details)) ([flannel/Documentation/backends.md at master · flannel-io/flannel · GitHub](https://github.com/flannel-io/flannel/blob/master/Documentation/backends.md#:~:text=VXLAN))】 및 Kubernetes Documentation, 기타 커뮤니티 자료.