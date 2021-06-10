#!/usr/bin/env bash
set -euxo pipefail

: ${BUILD_NUMBER?"BUILD_NUMBER env variable is required"}
: ${ENVIRONMENT?"ENVIRONMENT env variable is required"}

npm ci
source node_modules/@ofx/bash-build-libraries/_pipeline_env.sh
source node_modules/@ofx/bash-build-libraries/_aws.sh

project="memories-image-ingestor-lambda"

resource_suffix=$(env::get_resource_suffix)
environment_suffix=$(env::get_environment_suffix $ENVIRONMENT)
arn_role=$(env::get_pipeline_core_iam_role_arn $ENVIRONMENT)
stack_name="${project}${environment_suffix}${resource_suffix}"

echo "Deploying ${project}"
aws::sts_assume_role $arn_role $project
aws s3 cp "s3://ofx-shared-artifacts/${project}/${BUILD_NUMBER}/packaged.yaml" packaged.yaml
aws cloudformation deploy \
  --stack-name=${stack_name} \
  --template-file=packaged.yaml \
  --capabilities="CAPABILITY_NAMED_IAM" \
  --no-fail-on-empty-changeset \
  --parameter-overrides \
      Environment="${ENVIRONMENT}" \
      ResourceSuffix="${resource_suffix}" \
      EnvironmentSuffix="${environment_suffix}" \
      Project="${project}" \
  --tags billing1=stp Team=stp Project=${project} Version=${BUILD_NUMBER} Branch=$(env::get_git_branch)

aws::encrypt_log_groups $stack_name "$environment_suffix"