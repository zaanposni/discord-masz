if ( -Not (Test-Path -Path ./.env)) {
    Write-Host "Failed to find .env file. Please execute setup.py first."
    exit 5
}

docker ps -a

Write-Host "Killing old containers"
docker-compose stop
Write-Host "Killed old containers"

docker ps -a

Write-Host "Removing old containers/images/volumes"
docker container rm masz_nginx
docker container rm masz_backend
docker container rm masz_discordbot
docker container rm masz_invitationtracker

docker image rm discord-masz_nginx
docker image rm discord-masz_backend
docker image rm discord-masz_discordbot
docker image rm discord-masz_invitationtracker


Write-Host "Removed old containers/images/volumes"

Write-Host "Starting up..."

docker-compose up --build --force-recreate -d

Write-Host "Started in background"
