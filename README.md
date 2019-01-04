# Serverless URL Shortener
This project provides an easy-to-set-up serverless URL shortener that can be run for little-to-no cost.

## Serverless
"Does serverless mean something beyond a good buzzword?"

Yes. Thanks to what AWS provides, serverless means that:

- If you have the AWS CLI set up (and AWS Serverless dotnet tool installed), deployment is ridiculously easy
  - This includes every single thing that the app requires
  - You can essentially set up a single config file, run a command, and have the whole app working
- It will cost about $0.01 a month to run with a medium-low trafficked site
  - Only S3 will cost something, since it has a minimum of 1 cent when hosting things
    - This only applies if you leave the deployment artefacts in S3 (these can be removed after deployment)
  - Both Lambda and DynamoDB (the only main dependencies of the app) have permanent free tiers

# Setup
To deploy this app, follow these steps:

## AWS CLI
Ensure you have the AWS CLI set up and working properly. You can test your config with `aws sts get-caller-identity`.

## AWS Lambda dotnet Tool
You will need to have the AWS Lambda tool installed (and by extension, the .NET Core 2.1 sdk), which can be done by `dotnet tool install -g Amazon.Lambda.Tools`.

## Config
You will need to define a config file at `scripts/.env.local`. This will contain all of the config you need to deploy the app. It looks something like this:

```conf
SERVERLESS_NAME=MyUrlShortener
S3_BUCKET=my-url-shortener-build
AUTH_KEY=secure-auth-key
CURRENT_ORIGIN=https://myshortener.com
```

- `SERVERLESS_NAME`
  - This is the name of the app used by AWS CloudFormation
  - In order to ensure that you don't redeploy the app multiple times, set this once and *never* change it
- `S3_BUCKET`
  - This is the bucket the build result is set to for use by CloudFormation when deploying
  - It has to exist already, but the default config works fine
- `AUTH_KEY`
  - This is the authentication key that will be used to log into the admin console
  - To make this secure, make it a long & complex enough
  - If you think someone has gained access to it, you will have to edit the lambda directly and change the key
    - CloudFormation doesn't yet support updating environment variables
  - If empty, then the admin console won't let anyone sign in at all
    - You can use this behaviour to disable logging in completely
- `CURRENT_ORIGIN`
  - This is the origin to use when displaying the short URLs in the admin console
  - Purely a graphical option, doesn't change behaviour

## Deployment
If you have set up all the other parts correctly, you can just call `scripts/build.sh deploy` and CloudFormation will deploy the app to AWS, along with all its dependencies.

### Custom URL
You can set up a custom URL like normal in the AWS API Gateway console. 
