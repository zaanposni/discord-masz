echo $DEPLOY_DOMAIN

if [[ -z "$DEPLOY_DOMAIN" ]]; then
    echo "deploying local nginx"
    cp nginx-local.conf nginx.conf
else
if [[ $DEPLOY_MODE == "prod" ]]; then
        echo "deploying prod nginx with domain $DEPLOY_DOMAIN"
        sed -i -e 's/{{placeholder}}/'$DEPLOY_DOMAIN'/g' ./nginx-prod.conf
        cp nginx-prod.conf nginx.conf
    else 
        echo "deploying local nginx"
        cp nginx-local.conf nginx.conf
    fi
fi
