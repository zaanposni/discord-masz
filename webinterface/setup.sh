echo $DEPLOY_MODE
if [[ -z "$DEPLOY_MODE" ]]; then
    echo "deploying local env file"
    cp .env.dev .env
else
    if [[ $DEPLOY_MODE == "prod" ]]; then
        echo "deploying prod env file"
        cp .env.prod .env
    else 
        echo "deploying local env file"
        cp .env.dev .env
    fi
fi
