docker ps -a

Write-Host "Killing old containers"
docker-compose stop
Write-Host "Killed old containers"

docker ps -a

Write-Host "Removing old containers/images/volumes"
docker container rm masz_nginx
docker container rm masz_backend

docker image rm discord-masz_nginx
docker image rm discord-masz_backend
Write-Host "Removed old containers/images/volumes"

docker ps -a
