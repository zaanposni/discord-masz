if [[ ! -f ./config.json || ! -d ./.deployment/ ]]; then
    echo "Failed to find config.json or .deployment directory. Please execute setup.py first."
    exit 5
else
    cp config.json ./backend/
    cp config.json ./discordbot/
fi

docker ps -a

echo "Killing old containers"
docker-compose stop
echo "Killed old containers"

docker ps -a

echo "Removing old containers/images/volumes"
docker container rm masz_nginx
docker container rm masz_backend
docker container rm masz_sf4_apache
docker container rm masz_sf4_php
docker container rm masz_discordbot

docker image rm discord-masz_nginx
docker image rm discord-masz_sf4_apache
docker image rm discord-masz_php
docker image rm discord-masz_backend
docker image rm discord-masz_discordbot

docker volume rm discord-masz_php_share
echo "Removed old containers/images/volumes"

cp ./.deployment/nginx.conf ./nginx/nginx.conf
cp ./.deployment/.env ./webinterface/app/.env

if [ -f ./.deployment/.domain ]; then
    domain=`cat ./.deployment/.domain`
    sed -i -e 's/{{placeholder}}/'$domain'/g' ./nginx/nginx.conf
fi

echo "Starting up..."
docker-compose --env-file ./.deployment/.docker.env up --build --force-recreate -d
echo "Started in background"

echo "removing config.json from subdirectories"
rm -f ./backend/config.json
rm -f ./discordbot/config.json

echo "removing nginx.conf"
rm -f ./nginx/nginx.conf
rm -f ./webinterface/app/.env
