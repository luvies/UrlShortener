#! /bin/bash

# Config parameters.
# All of these should be given in a .env.local file located
# next to this script.
SERVERLESS_NAME=
S3_BUCKET=
AUTH_KEY=
CURRENT_ORIGIN=

# Script env.
SCRIPT_DIR=$(dirname "$0")
ENV_PATH=$SCRIPT_DIR/.env.local
APP_PATH=$SCRIPT_DIR/../src/UrlShortener

if [[ -f $ENV_PATH ]]; then
    source $ENV_PATH
else
    echo "Missing .env.local file"
    exit 1
fi

if [[ $1 == "local" ]]; then
    dotnet lambda package
elif [[ $1 == "run" ]]; then
    cd $APP_PATH

    # Override the code URI path with the local archive.
    sam local start-api -t serverless.json --parameter-overrides ParameterKey=CodeArchiveUri,ParameterValue=./bin/Release/netcoreapp2.1/UrlShortener.zip
elif [[ $1 == "deploy" ]]; then
    cd $APP_PATH

    dotnet lambda deploy-serverless $SERVERLESS_NAME --s3-bucket $S3_BUCKET --template-parameters "AuthKey=$AUTH_KEY;CurrentOrigin=$CURRENT_ORIGIN"
else
    echo "Build management script"
    echo -e "\tlocal\t\tBuilds and zips up the app for local development"
    echo -e "\trun\t\tRuns the app locally using the SAM CLI"
    echo -e "\tdeploy\t\tDeploys the app to AWS"
fi
