echo "Copying config in subdirectories for docker"
if [ ! -f ./config.json ]; then
    cp default-config.json ./backend/config.json
    file=default-config.json
else
    cp config.json ./backend/
    file=config.json
fi

mysql_port=$(jq '.mysql_database.port')
mysql_database=$(jq '.mysql_database.database')
mysql_user=$(jq '.mysql_database.user')
mysql_pass=$(jq '.mysql_database.pass')
mysql_root_pass=$(jq '.mysql_database.root_pass')

echo "\n " \
"MYSQL_PORT=$mysql_port" \
"MYSQL_DATABASE=$mysql_database" \
"MYSQL_USER=$mysql_user" \
"MYSQL_PASSWORD=$mysql_pass" \
"MYSQL_ROOT_PASSWORD=$mysql_root_pass" \
"" > .env

cat .env

echo "Using specified nginx config"
if [[ $(jq '.meta.nginx_mode' $file) = *prod* ]]; then
    echo "prod"
    cp ./nginx/nginx-prod.conf ./nginx/nginx.conf
    cp ./webinterface/app/.env.prod ./webinterface/app/.env
else
    echo "local"
    cp ./nginx/nginx-local.conf ./nginx/nginx.conf
    cp ./webinterface/app/.env.dev ./webinterface/app/.env
fi
