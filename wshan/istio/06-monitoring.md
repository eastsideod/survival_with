# Istio 모니터링 및 관찰성

## 1. ASP.NET 메트릭 수집

### Prometheus 설정

```yaml
# prometheus-config.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: aspnet-prometheus
data:
  prometheus.yml: |
    global:
      scrape_interval: 15s
    scrape_configs:
    - job_name: 'aspnet-mesh'
      kubernetes_sd_configs:
      - role: endpoints
        namespaces:
          names:
          - default
      relabel_configs:
      - source_labels: [__meta_kubernetes_service_name]
        action: keep
        regex: aspnet-service
```

### ASP.NET 메트릭 구성

```yaml
# metrics-config.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-metrics-config
spec:
  metrics:
  - providers:
    - name: prometheus
    overrides:
    - match:
        metric: REQUEST_COUNT
        mode: CLIENT_AND_SERVER
      tagOverrides:
        source_service:
          value: "source.workload.namespace"
        destination_service:
          value: "destination.workload.namespace"
```

## 2. ASP.NET 로깅

### 로그 구성

```yaml
# logging-config.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-logging-config
spec:
  logging:
  - providers:
    - name: stackdriver
    overrides:
    - match:
        mode: CLIENT_AND_SERVER
      tagOverrides:
        source_service:
          value: "source.workload.namespace"
        destination_service:
          value: "destination.workload.namespace"
```

### ASP.NET 로그 필터링

```yaml
# log-filter.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-log-filter
spec:
  logging:
  - providers:
    - name: stackdriver
    filter:
      expression: "response.code >= 400"
```

## 3. ASP.NET 추적

### Jaeger 설정

```yaml
# jaeger-config.yaml
apiVersion: jaegertracing.io/v1
kind: Jaeger
metadata:
  name: aspnet-jaeger
spec:
  strategy: production
  storage:
    type: elasticsearch
    options:
      es:
        server-urls: http://elasticsearch:9200
```

### ASP.NET 추적 구성

```yaml
# tracing-config.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-tracing-config
spec:
  tracing:
  - randomSamplingPercentage: 100.0
    customTags:
      source_service:
        literal:
          value: "source.workload.namespace"
      destination_service:
        literal:
          value: "destination.workload.namespace"
```

## 4. ASP.NET 대시보드

### Grafana 설정

```yaml
# grafana-config.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: aspnet-grafana-datasources
data:
  datasources.yaml: |
    apiVersion: 1
    datasources:
    - name: Prometheus
      type: prometheus
      url: http://prometheus:9090
      access: proxy
      isDefault: true
```

### Kiali 설정

```yaml
# kiali-config.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: aspnet-kiali
data:
  config.yaml: |
    auth:
      strategy: anonymous
    server:
      port: 20001
    external_services:
      prometheus:
        url: http://prometheus:9090
      grafana:
        url: http://grafana:3000
      tracing:
        url: http://jaeger-query:16686
```

## 5. ASP.NET 알림

### Alertmanager 설정

```yaml
# alertmanager-config.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: aspnet-alertmanager
data:
  config.yml: |
    global:
      resolve_timeout: 5m
    route:
      group_by: ['alertname']
      group_wait: 30s
      group_interval: 5m
      repeat_interval: 12h
      receiver: 'slack'
    receivers:
    - name: 'slack'
      slack_configs:
      - api_url: 'https://hooks.slack.com/services/...'
        channel: '#aspnet-alerts'
```

### ASP.NET 경고 규칙

```yaml
# alert-rules.yaml
apiVersion: monitoring.coreos.com/v1
kind: PrometheusRule
metadata:
  name: aspnet-alerts
spec:
  groups:
  - name: aspnet.rules
    rules:
    - alert: ASPNetHighRequestLatency
      expr: histogram_quantile(0.95, sum(rate(istio_request_duration_milliseconds_bucket[1m])) by (le)) > 1000
      for: 5m
      labels:
        severity: warning
      annotations:
        summary: ASP.NET High request latency
```

## 6. ASP.NET 사용자 정의 메트릭

### 메트릭 정의

```yaml
# custom-metrics.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-custom-metrics
spec:
  metrics:
  - providers:
    - name: prometheus
    overrides:
    - match:
        metric: CUSTOM_METRIC
        mode: CLIENT_AND_SERVER
      tagOverrides:
        custom_tag:
          value: "custom.value"
```

### ASP.NET 메트릭 수집

```yaml
# metric-collection.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-metric-collection
spec:
  metrics:
  - providers:
    - name: prometheus
    overrides:
    - match:
        metric: REQUEST_COUNT
        mode: CLIENT_AND_SERVER
      tagOverrides:
        custom_tag:
          value: "custom.value"
```

## 7. ASP.NET 성능 모니터링

### 성능 메트릭

```yaml
# performance-metrics.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-performance-metrics
spec:
  metrics:
  - providers:
    - name: prometheus
    overrides:
    - match:
        metric: REQUEST_DURATION
        mode: CLIENT_AND_SERVER
      tagOverrides:
        percentile:
          value: "95"
```

### ASP.NET 리소스 모니터링

```yaml
# resource-metrics.yaml
apiVersion: telemetry.istio.io/v1alpha1
kind: Telemetry
metadata:
  name: aspnet-resource-metrics
spec:
  metrics:
  - providers:
    - name: prometheus
    overrides:
    - match:
        metric: CPU_USAGE
        mode: CLIENT_AND_SERVER
      tagOverrides:
        resource_type:
          value: "cpu"
``` 