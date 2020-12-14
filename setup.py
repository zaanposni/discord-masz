import json
import os
from shutil import copyfile, rmtree

ENV_FILE = """MYSQL_PORT=3306
MYSQL_DATABASE=masz
MYSQL_USER=mysqldummy
MYSQL_PASSWORD=mysqldummy
MYSQL_ROOT_PASSWORD=root"""

print("=============================================")
print("=                                           =")
print("=                   MASZ                    =")
print("=                                           =")
print("=============================================")
print("Please be sure that you are in the root directory of this project when executing this script.\n\n")

local = True
while True:
    nginx_mode = input("Do you want to deploy on a domain or on localhost as a test version? (local/domain): ")
    if str(nginx_mode).lower() == "local":
        ENV_FILE += f"\nDEPLOY_MODE=local"
        break
    elif str(nginx_mode).lower() == "domain":
        local = False
        ENV_FILE += f"\nDEPLOY_MODE=prod"
        break
    else:
        print("Invalid input please enter 'local' or 'domain'.")

if local:
    ENV_FILE += f"\nMETA_SERVICE_DOMAIN=127.0.0.1:5565"
    ENV_FILE += f"\nMETA_SERVICE_BASE_URL=http://127.0.0.1:5565"
    ENV_FILE += f"\nMETA_SERVICE_NAME=masz"
    service_base_url = "http://127.0.0.1:5565"
else:
    domain = input("What is the (sub)domain you are hosting the application on? (my.example.com): ")
    ENV_FILE += f"\nMETA_SERVICE_NAME={domain}"
    ENV_FILE += f"\nMETA_SERVICE_BASE_URL=https://{domain}"
    ENV_FILE += f"\nMETA_SERVICE_DOMAIN={domain}"
    ENV_FILE += f"\nDEPLOY_DOMAIN={domain}"
    service_base_url = f"https://{domain}"
    print("\nPlease be sure to configure your reverse proxy correctly, the docker container will be listening on local port 5565.\n")

ENV_FILE += f'\nDISCORD_BOT_TOKEN={input("Please enter the discord bot token: ")}'
ENV_FILE += f'\nDISCORD_OAUTH_CLIENT_ID={input("Please enter the discord bot client id: ")}'
ENV_FILE += f'\nDISCORD_OAUTH_CLIENT_SECRET={input("Please enter the discord bot client secret: ")}'

BOT_PREFIX = str(input("Please enter the prefix the discord bot should listen to (default is $): ")).strip()
if BOT_PREFIX == "":
    BOT_PREFIX = "$"
ENV_FILE += f"\nBOT_PREFIX={BOT_PREFIX}"

print("Please enter the discord id of users that should be site admins. It is recommended to be just one. You can enter as many as you want.")
admins = []
while True:
    site_admin = input(f"{len(admins) + 1}. Admin | Enter 'x' if you are finished: ")
    if (str(site_admin)).lower() == "x":
        break
    admins.append(site_admin)

ENV_FILE += f"\nDISCORD_SITE_ADMINS={','.join(admins)}"
print("\nSaving files...")

try:
    os.mkdir("./.deployment")
except FileExistsError:
    rmtree("./.deployment")
    os.mkdir("./.deployment")
except Exception as e:
    raise(e)

with open("./.deployment/.docker.env", "w") as fh:
    fh.write(ENV_FILE)

print("\nYou are finished.\nYou can execute this script again if you want to change anything.\n")
print(f"You can use the start.sh for linux or start.ps1 for windows to start the application.\nAfter starting you can access the panel at: {service_base_url}")
