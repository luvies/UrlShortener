#! /bin/bash

# Config parameters.
# All of these should be given in a .env.local file located
# next to this script.
SERVERLESS_NAME=
S3_BUCKET=
AUTH_KEY=

# Script env.
SCRIPT_DIR=$(dirname "$0")
ENV_PATH=$SCRIPT_DIR/.env.local

if [[ -f $ENV_PATH ]]; then
    source $ENV_PATH
else
    echo "Missing .env.local file"
    exit 1
fi

cd $SCRIPT_DIR/../src/UrlShortener

if [[ $1 == "local" ]]; then
    dotnet lambda package
elif [[ $1 == "run" ]]; then
    # Override the code URI path with the local archive.
    sam local start-api -t serverless.json --parameter-overrides ParameterKey=CodeArchiveUri,ParameterValue=./bin/Release/netcoreapp2.1/UrlShortener.zip
elif [[ $1 == "deploy" ]]; then
    dotnet lambda deploy-serverless $SERVERLESS_NAME --s3-bucket $S3_BUCKET --template-parameters AuthKey=$AUTH_KEY
else
    echo "Build management script"
    echo -e "\tlocal\tBuilds and zips up the app for local development"
    echo -e "\trun\tRuns the app locally using the SAM CLI"
    echo -e "\tdeploy\tDeploys the app to AWS"
fi
