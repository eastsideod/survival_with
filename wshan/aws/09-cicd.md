# AWS CI/CD 설정

## 1. CodePipeline 설정

### 파이프라인 생성
```bash
# 파이프라인 생성
aws codepipeline create-pipeline \
  --pipeline-name game-server-pipeline \
  --pipeline '{
    "name": "game-server-pipeline",
    "roleArn": "arn:aws:iam::account-id:role/service-role/AWSCodePipelineServiceRole",
    "artifactStore": {
      "type": "S3",
      "location": "game-server-artifacts"
    },
    "stages": [
      {
        "name": "Source",
        "actions": [{
          "name": "Source",
          "actionTypeId": {
            "category": "Source",
            "owner": "AWS",
            "provider": "CodeCommit",
            "version": "1"
          },
          "configuration": {
            "RepositoryName": "game-server",
            "BranchName": "main"
          },
          "outputArtifacts": [{
            "name": "SourceArtifact"
          }]
        }]
      }
    ]
  }'
```

### 웹훅 설정
```bash
# 웹훅 생성
aws codepipeline create-webhook \
  --webhook-name game-server-webhook \
  --pipeline-name game-server-pipeline \
  --authentication GITHUB_HMAC \
  --authentication-configuration '{
    "SecretToken": "secret-token"
  }' \
  --filters '[
    {
      "jsonPath": "$.ref",
      "matchEquals": "refs/heads/{Branch}"
    }
  ]'
```

## 2. CodeBuild 설정

### 빌드 프로젝트 생성
```bash
# 빌드 프로젝트 생성
aws codebuild create-project \
  --name game-server-build \
  --source '{
    "type": "CODECOMMIT",
    "location": "https://git-codecommit.region.amazonaws.com/v1/repos/game-server",
    "buildspec": "buildspec.yml"
  }' \
  --artifacts '{
    "type": "S3",
    "location": "game-server-artifacts",
    "path": "build",
    "name": "game-server-build",
    "packaging": "ZIP"
  }' \
  --environment '{
    "type": "LINUX_CONTAINER",
    "image": "aws/codebuild/standard:5.0",
    "computeType": "BUILD_GENERAL1_SMALL"
  }'
```

### 빌드 스펙 설정
```yaml
version: 0.2

phases:
  install:
    runtime-versions:
      golang: 1.18
    commands:
      - go version
  pre_build:
    commands:
      - go mod download
  build:
    commands:
      - go build -o game-server
  post_build:
    commands:
      - aws s3 cp game-server s3://game-server-artifacts/build/

artifacts:
  files:
    - game-server
  name: game-server-build
```

## 3. 배포 자동화

### ECR 설정
```bash
# ECR 리포지토리 생성
aws ecr create-repository \
  --repository-name game-server \
  --image-scanning-configuration scanOnPush=true

# 이미지 태그 설정
aws ecr put-image-tag-mutability \
  --repository-name game-server \
  --image-tag-mutability IMMUTABLE
```

### 배포 스크립트
```bash
#!/bin/bash

# 이미지 빌드
docker build -t game-server:latest .

# ECR 로그인
aws ecr get-login-password --region region | docker login --username AWS --password-stdin account-id.dkr.ecr.region.amazonaws.com

# 이미지 태그 및 푸시
docker tag game-server:latest account-id.dkr.ecr.region.amazonaws.com/game-server:latest
docker push account-id.dkr.ecr.region.amazonaws.com/game-server:latest

# Kubernetes 배포 업데이트
kubectl set image deployment/game-server game-server=account-id.dkr.ecr.region.amazonaws.com/game-server:latest
```

## 4. 테스트 자동화

### 테스트 스크립트
```bash
#!/bin/bash

# 단위 테스트 실행
go test -v ./...

# 통합 테스트 실행
go test -v -tags=integration ./...

# 성능 테스트 실행
go test -v -bench=. ./...
```

### 테스트 결과 보고
```yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: test-results
data:
  test-results.json: |
    {
      "unit_tests": {
        "passed": 100,
        "failed": 0,
        "coverage": 85
      },
      "integration_tests": {
        "passed": 50,
        "failed": 0,
        "coverage": 75
      }
    }
```

## 5. 문제 해결

### 파이프라인 상태 확인
```bash
# 파이프라인 상태 확인
aws codepipeline get-pipeline-state \
  --name game-server-pipeline

# 빌드 상태 확인
aws codebuild batch-get-builds \
  --ids $BUILD_ID
```

### 배포 로그 확인
```bash
# 배포 로그 확인
kubectl logs -l app=game-server

# 이벤트 확인
kubectl get events --sort-by='.lastTimestamp'
```

### 테스트 결과 확인
```bash
# 테스트 결과 확인
aws s3 ls s3://game-server-artifacts/test-results/

# 테스트 보고서 다운로드
aws s3 cp s3://game-server-artifacts/test-results/test-report.html .
``` 