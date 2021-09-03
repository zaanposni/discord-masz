docker ps -a

echo "Killing old containers"
docker-compose stop
echo "Killed old containers"

docker ps -a

echo "Removing old containers/images/volumes"
docker container rm masz_nginx
docker container rm masz_backend

docker image rm discord-masz_nginx
docker image rm discord-masz_backend

echo "Removed old containers/images/volumes"

docker ps -a
