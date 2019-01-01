#! /bin/bash
SERVERLESS_NAME=LuviesUrlShortener
S3_BUCKET=luvies-url-shortener-build

cd $(dirname "$0")/../src/UrlShortener

if [[ $1 == "local" ]]; then
    dotnet lambda package
elif [[ $1 == "run" ]]; then
    # Override the code URI path with the local archive.
    sam local start-api -t serverless.json --parameter-overrides ParameterKey=CodeArchiveUri,ParameterValue=./bin/Release/netcoreapp2.1/UrlShortener.zip
elif [[ $1 == "deploy" ]]; then
    dotnet lambda deploy-serverless $SERVERLESS_NAME --s3-bucket $S3_BUCKET
else
    echo "Build management script"
    echo -e "\tlocal\tBuilds and zips up the app for local development"
    echo -e "\trun\tRuns the app locally using the SAM CLI"
    echo -e "\tdeploy\tDeploys the app to AWS"
fi
