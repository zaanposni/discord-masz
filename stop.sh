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

docker ps -a
