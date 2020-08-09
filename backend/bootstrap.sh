docker ps -a

docker stop $(docker ps -a -q --filter ancestor="discord-masz" --format="{{.ID}}") || echo "Failed to stop old container"
docker container rm $(docker ps -a -q --filter ancestor="discord-masz" --format="{{.ID}}") || echo "Failed to stop old container"
docker image rm discord-masz || echo "Failed to stop old container"

docker ps -a

docker build -t discord-masz --build-arg MYSQL_HOST=127.0.0.1 \
                             --build-arg MYSQL_PORT=3306 \
                             --build-arg MYSQL_DATABASE=masz \
                             --build-arg MYSQL_USER=mysqldummy \
                             --build-arg MYSQL_PASS=mysqldummy \
                             .
                                   
docker create -ti --restart=always --name discord-masz -p 5565:80 --network="host" discord-masz

docker start $(docker ps -a -q --filter ancestor="discord-masz" --format="{{.ID}}")

docker ps -a

echo "Done."