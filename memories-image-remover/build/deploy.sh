#!/usr/bin/env bash
set -euxo pipefail

: ${SLUG?"VERSION env variable is required"}
: ${BUILD_NUMBER?"BUILD_NUMBER env variable is required"}
: ${PROJECT?"PROJECT env variable is required"}
: ${ENV_SUFFIX?"ENV_SUFFIX env variable is required"}
: ${ENVIRONMENT?"ENV_SUFFIX env variable is required"}

stack_name="${PROJECT}-${ENV_SUFFIX}-${SLUG}"


echo "Deploying ${stack_name}"
aws s3 cp "s3://a4bird-memories-lambda-repo/${PROJECT}/${BUILD_NUMBER}/packaged.yaml" packaged.yaml
aws cloudformation deploy \
  --stack-name=${stack_name} \
  --template-file=packaged.yaml \
  --capabilities="CAPABILITY_NAMED_IAM" \
  --no-fail-on-empty-changeset \
  --parameter-overrides \
      SLUG="${SLUG}" \
      EnvironmentSuffix="${ENV_SUFFIX}" \
      Environment="${ENVIRONMENT}" \
      Project="${PROJECT}" \
  --tags billing=a4bird Enterprise=Memories Project=${PROJECT} Version=${BUILD_NUMBER} Branch=${SLUG}
