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

bash bootstrap_init.sh

echo "Build docker-compose"
docker-compose build
echo "Build finished"

echo "removing config.json from subdirectories"
rm -f ./backend/config.json

echo "removing nginx.conf"
rm -f ./nginx/nginx.conf
rm -f ./webinterface/app/.env

echo "Starting up..."
docker-compose up -d
echo "Started in background"

docker ps -a

echo "Done."
