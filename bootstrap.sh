# This script will deploy the whole masz application as long as docker and docker-compose are installed
# After running this script you may want to setup a reverse proxy to redirect http request to 5565

docker ps -a

docker-compose stop

docker ps -a

docker-compose build --build-arg MYSQL_HOST=db \
                     --build-arg MYSQL_PORT=3306 \
                     --build-arg MYSQL_DATABASE=masz \
                     --build-arg MYSQL_USER=mysqldummy \
                     --build-arg MYSQL_PASS=mysqldummy \
                     --build-arg DISCORD_BOT_TOKEN=<insert your value here> \
                     --build-arg DISCORD_CLIENT_ID=<insert your value here> \
                     --build-arg DISCORD_CLIENT_SECRET=<insert your value here> \
                     --build-arg SITEADMINS=<insert your value here>

docker-compose up -d

docker ps -a