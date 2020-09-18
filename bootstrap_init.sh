echo "Copying config in subdirectories for docker"
if [ ! -f ./config.json ]; then
    cp default-config.json ./backend/
else
    cp config.json ./backend/
fi

echo "Using specified nginx config"
if [[ $(jq '.meta.nginx_mode' config.json) = *prod* ]]; then
    echo "prod"
    cp ./nginx/nginx-prod.conf ./nginx/nginx.conf
else
    echo "local"
    cp ./nginx/nginx-local.conf ./nginx/nginx.conf
fi
