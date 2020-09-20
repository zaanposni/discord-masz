docker-compose stop

cp default-config.json ./backend/config.json
cp ./nginx/nginx-local.conf ./nginx/nginx.conf
cp ./webinterface/app/.env.dev ./webinterface/app/.env

docker-compose build

rm -Force ./backend/config.json
rm -Force ./nginx/nginx.conf
rm -Force ./webinterface/app/.env

docker-compose up
