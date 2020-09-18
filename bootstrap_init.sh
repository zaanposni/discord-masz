echo "Copying config in subdirectories for docker"
if [ ! -f ./config.json ]; then
    cp default-config.json ./backend/config.json
    file=default-config.json
else
    cp config.json ./backend/
    file=config.json
fi

echo "Using specified nginx config"
if [[ $(jq '.meta.nginx_mode' $file) = *prod* ]]; then
    echo "prod"
    cp ./nginx/nginx-prod.conf ./nginx/nginx.conf
else
    echo "local"
    cp ./nginx/nginx-local.conf ./nginx/nginx.conf
fi
