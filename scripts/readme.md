# Scripts

## Migrate from dynobot to masz

To migrate your warnings from dynobots database to masz follow this guide:

- Visit the warnings dashboard in your browser at dyno.gg/manage/guildid/warnings
- Use F12 or similiar to catch the request your browser sends to (POST /warnings), you might have to hit F5, find something like this:

![Image of Yaktocat](./example01.png)

- Send the request again and change the page size to something that is big enough so all modcases are one page (this way you have to follow this guide only one time)
- Now you got a response that looks like this:
```json
{
    "logs": [
        {
            "_id": "dummy",
            "guild": "dummy",
            "users": { },
            "mod": { },
            "reason": "dummy",
            "createdAt": "dummy",
            "__v": 0
        }
    ],
    "pageCount": 1
}
```
- In firefox do right click > copy all 
- Paste this content into a file e.g. `input.json`
- Execute the script `migrate_dyno_warnings.py` by using:
```py
python3 -m pip install -r requirements.txt
python3 migrate_dyno_warnings.py --input-file input.json
```
- Copy the output generated in output.sql
- Access the bash shell of your running db container: `docker exec -it masz_db bash`
- Connect to the mysql shell using `mysql`
- Execute `USE masz` and then paste the content of output.sql
- You should be ready to see your migrated data in the frontend
