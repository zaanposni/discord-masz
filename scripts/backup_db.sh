/usr/bin/docker exec masz_db sh -c 'exec mysqldump --databases masz -uroot -p"root"' > $(pwd)/$( date '+%F_%H_%M_%S').sql
