import json
import os
from shutil import copyfile, rmtree

ENV_FILE = """MYSQL_PORT=3306
MYSQL_DATABASE=db
MYSQL_USER=mysqldummy
MYSQL_PASSWORD=mysqldummy
MYSQL_ROOT_PASSWORD=root"""

DEFAULT_CONFIG = {
    "mysql_database": {
        "host": "db",
        "port": "3306",
        "database": "masz",
        "user": "mysqldummy",
        "pass": "mysqldummy",
        "root_pass": "root"
    },
    "discord": {
        "bot_token": "",
        "oauth_client_id": "",
        "oauth_client_secret": "",
        "site_admins": []
    },
    "meta": {
        "service_name": "masz",
        "service_domain": "domain.com",
        "service_base_url": "http://domain.com",
        "nginx_mode": "local"
    }
}

print("=============================================")
print("=                                           =")
print("=                   MASZ                    =")
print("=                                           =")
print("=============================================")
print("Please be sure that you are in the root directory of this project when executing this script.\n\n")

try:
    os.mkdir("./.deployment")
except FileExistsError:
    rmtree("./.deployment")
    os.mkdir("./.deployment")
except Exception as e:
    raise(e)

while True:
    nginx_mode = input("Do you want to deploy on a domain or on localhost as a test version? (local/domain): ")
    if str(nginx_mode).lower() == "local":
        DEFAULT_CONFIG["meta"]["nginx_mode"] = "local"
        copyfile("./nginx/nginx-local.conf", "./.deployment/nginx.conf")
        copyfile("./webinterface/app/.env.dev", "./.deployment/.env")
        break
    elif str(nginx_mode).lower() == "domain":
        DEFAULT_CONFIG["meta"]["nginx_mode"] = "prod"
        copyfile("./nginx/nginx-prod.conf", "./.deployment/nginx.conf")
        copyfile("./webinterface/app/.env.prod", "./.deployment/.env")
        break
    else:
        print("Invalid input please enter 'local' or 'domain'.")

if DEFAULT_CONFIG["meta"]["nginx_mode"] == "local":
    DEFAULT_CONFIG["meta"]["service_domain"] = "127.0.0.1:5565"
    DEFAULT_CONFIG["meta"]["service_base_url"] = "http://127.0.0.1:5565"
else:
    domain = input("What is the (sub)domain you are hosting the application on? (my.example.com): ")
    DEFAULT_CONFIG["meta"]["service_name"] = domain
    DEFAULT_CONFIG["meta"]["service_domain"] = domain
    DEFAULT_CONFIG["meta"]["service_base_url"] = f"https://{domain}"
    print("\nPlease be sure to configure your reverse proxy correctly, the docker container will be listening on local port 5565.\n")

DEFAULT_CONFIG["discord"]["bot_token"] = input("Please enter the discord bot token: ")
DEFAULT_CONFIG["discord"]["oauth_client_id"] = input("Please enter the discord bot client id: ")
DEFAULT_CONFIG["discord"]["oauth_client_secret"] = input("Please enter the discord bot client secret: ")

while True:
    site_admin = input("Please enter the discord id of users that should be site admins. It is recommended to be just one. You can enter as many as you want. Enter 'x' if you are finished: ")
    if (str(site_admin)).lower() == "x":
        break
    DEFAULT_CONFIG["discord"]["site_admins"].append(site_admin)

print("\nSaving files...")

with open("./config.json", "w") as fh:
    json.dump(DEFAULT_CONFIG, fh)

with open("./.deployment/.docker.env", "w") as fh:
    fh.write(ENV_FILE)

if DEFAULT_CONFIG["meta"]["nginx_mode"] != "local":
    with open("./.deployment/.domain", "w") as fh:
        fh.write(DEFAULT_CONFIG["meta"]["service_domain"])

print("\nYou are finished.\nYou can execute this script again if you want to change anything.\n")
print(f"You can use the start.sh for linux or start.ps1 for windows to start the application.\nAfter starting you can access the panel at: {DEFAULT_CONFIG['meta']['service_base_url']}")
