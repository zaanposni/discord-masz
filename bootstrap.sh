# This script will deploy the whole masz application as long as docker and docker-compose are installed
# After running this script you may want to setup a reverse proxy to redirect http request to 5565

docker ps -a

docker-compose stop

docker ps -a

docker-compose build --build-arg MYSQL_HOST=127.0.0.1 \
                     --build-arg MYSQL_PORT=33061 \
                     --build-arg MYSQL_DATABASE=masz \
                     --build-arg MYSQL_USER=mysqldummy \
                     --build-arg MYSQL_PASS=mysqldummy

docker-compose up -d

docker ps -a