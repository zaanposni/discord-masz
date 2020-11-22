# This script will deploy the whole masz application as long as docker and docker-compose are installed
# After running this script you may want to setup a reverse proxy to redirect http request to 5565

if [ ! -f ./config.json ]; then
    >&2 echo "Config file not found."
    >&2 echo "Please create a config.json based on ./default-config.json and try again."
    exit 1
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

docker image rm discord-masz_nginx
docker image rm discord-masz_sf4_apache
docker image rm discord-discord-masz_php
docker image rm discord-discord-masz_backend

docker volume rm discord-masz_php_share
echo "Removed old containers/images/volumes"

bash bootstrap_init.sh || exit 5;

echo "Starting up..."
docker-compose --env-file .env up --build --force-recreate -d
echo "Started in background"

echo "removing config.json from subdirectories"
rm -f ./backend/config.json

echo "removing nginx.conf"
rm -f ./nginx/nginx.conf
rm -f ./webinterface/app/.env

echo "removing .env file"
rm -f .env

docker ps -a

echo "Done."
