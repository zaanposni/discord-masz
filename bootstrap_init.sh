echo "================================================================="
echo "=                                                               ="
echo "=                         DEPRECATED                            ="
echo "=                                                               ="
echo "=    Usage of this script is deprecated and not recommended     ="
echo "=    Please checkout install.py for installation and setup.     ="
echo "================================================================="

echo "Copying config in subdirectories for docker"
if [ ! -f ./config.json ]; then
    cp default-config.json ./backend/config.json
    cp default-config.json ./discordbot/config.json
    file=default-config.json
else
    cp config.json ./backend/
    cp config.json ./discordbot/
    file=config.json
fi

jq --help > /dev/null || { echo "jq is not installed, please install it."; exit 1; }

mysql_port=$(jq  --raw-output '.mysql_database.port' $file)
mysql_database=$(jq --raw-output '.mysql_database.database' $file)
mysql_user=$(jq --raw-output '.mysql_database.user' $file)
mysql_pass=$(jq --raw-output '.mysql_database.pass' $file)
mysql_root_pass=$(jq --raw-output '.mysql_database.root_pass' $file)

service_domain=$(jq --raw-output '.meta.service_domain' $file)

cat> .env <<- EOM
MYSQL_PORT=$mysql_port
MYSQL_DATABASE=$mysql_database
MYSQL_USER=$mysql_user
MYSQL_PASSWORD=$mysql_pass
MYSQL_ROOT_PASSWORD=$mysql_root_pass
EOM

cat .env

echo "Using specified nginx config"
if [[ $(jq '.meta.nginx_mode' $file) = *prod* ]]; then
    echo "prod"
    cp ./nginx/nginx-prod.conf ./nginx/nginx.conf
    sed -i -e 's/{{placeholder}}/'$service_domain'/g' ./nginx/nginx.conf
    cp ./webinterface/app/.env.prod ./webinterface/app/.env
else
    echo "local"
    cp ./nginx/nginx-local.conf ./nginx/nginx.conf
    cp ./webinterface/app/.env.dev ./webinterface/app/.env
fi
