#!/usr/bin/env bash
set -euxo pipefail

: ${BUILD_NUMBER?"BUILD_NUMBER env variable is required"}
project="memories-image-ingestor-lambda"

npm ci
source node_modules/@ofx/bash-build-libraries/_pipeline_env.sh
source node_modules/@ofx/bash-build-libraries/_aws.sh
arn_role=$(env::get_pipeline_core_iam_role_arn Development)

echo "Running the tests and publishing..."

mkdir ./packages || true

image_name="${project}:${BUILD_NUMBER}"
docker build -f ../tests.dockerfile -t $image_name ../
docker run -e TEAMCITY_VERSION $image_name
container_id=$(docker create $image_name)
docker cp $container_id:./app/out/memories-image-ingestor-lambda-lambda ./packages/memories-image-ingestor-lambda-lambda
docker rm -v $container_id

echo "Publishing ${project}"
aws::sts_assume_role $arn_role $project
aws cloudformation package \
    --template-file="./cloudformation.yaml" \
    --output-template-file="./packaged.yaml" \
    --s3-prefix="${project}/${BUILD_NUMBER}" \
    --s3-bucket="ofx-shared-artifacts"

aws s3 cp "./packaged.yaml" "s3://ofx-shared-artifacts/${project}/${BUILD_NUMBER}/"
