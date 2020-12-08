if ( -Not (Test-Path -Path ./config.json) -or -Not (Test-Path -Path ./.deployment/)) {
    Write-Host "Failed to find config.json or .deployment directory. Please execute install.py first."
    exit 5
} else {
    cp config.json ./backend/
}

docker ps -a

Write-Host "Killing old containers"
docker-compose stop
Write-Host "Killed old containers"

docker ps -a

Write-Host "Removing old containers/images/volumes"
docker container rm masz_nginx
docker container rm masz_backend
docker container rm masz_sf4_apache
docker container rm masz_sf4_php

docker image rm discord-masz_nginx
docker image rm discord-masz_sf4_apache
docker image rm discord-discord-masz_php
docker image rm discord-discord-masz_backend

docker volume rm discord-masz_php_share
Write-Host "Removed old containers/images/volumes"

cp ./.deployment/nginx.conf ./nginx/nginx.conf
cp ./.deployment/.env ./webinterface/app/.env

if ( Test-Path -Path `./.deployment/.domain` ) {
    $domain = [IO.File]::ReadAllText(".\.deployment\.domain")
    (Get-Content ./nginx/nginx.conf).replace('{{placeholder}}', $domain) | Set-Content ./nginx/nginx.conf
}

Write-Host "Starting up..."
docker-compose --env-file .\.deployment\.docker.env up --build --force-recreate -d
Write-Host "Started in background"

Write-Host "removing config.json from subdirectories"
rm -Force ./backend/config.json

Write-Host "removing nginx.conf"
rm -Force ./nginx/nginx.conf
rm -Force ./webinterface/app/.env
