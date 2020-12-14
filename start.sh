if [[ ! -f ./.deployment/.docker.env ]]; then
    echo "Failed to find .docker.env or .deployment directory. Please execute setup.py first."
    exit 5
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

echo "Starting up..."
docker-compose --env-file ./.deployment/.docker.env up --build --force-recreate -d
echo "Started in background"
