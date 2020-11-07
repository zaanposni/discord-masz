Write-Host "#############################################################################################################"
Write-Host "This script copies files for a development deployment, do not use it in production, use bootstrap.sh instead."
Write-Host "#############################################################################################################"

docker-compose stop

docker container rm masz_nginx
docker container rm masz_backend
docker container rm masz_sf4_apache
docker container rm masz_sf4_php

docker image rm discord-masz_nginx
docker image rm discord-masz_sf4_apache
docker image rm discord-discord-masz_php
docker image rm discord-discord-masz_backend

docker volume rm discord-masz_php_share

cp config.json ./backend/config.json
cp ./nginx/nginx-local.conf ./nginx/nginx.conf
cp ./webinterface/app/.env.dev ./webinterface/app/.env
cp .env.example .env

docker-compose --env-file .env up --build --force-recreate -d

rm -Force ./backend/config.json
rm -Force ./nginx/nginx.conf
# rm -Force ./webinterface/app/.env
rm -Force .env
