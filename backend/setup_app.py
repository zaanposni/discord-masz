import json
import os
import random
import string

def random_pass(pass_length=200):
    chars = string.ascii_letters + string.digits + '!@#$%^&*()'
    random.seed = (os.urandom(1024))
    return ''.join(random.choice(chars) for i in range(pass_length))

CNC_STRING = f'Server={os.getenv("MYSQL_HOST")};Port={os.getenv("MYSQL_PORT")};Database={os.getenv("MYSQL_DATABASE")};Uid={os.getenv("MYSQL_USER")};Pwd={os.getenv("MYSQL_PASSWORD")};'

PATH = os.path.join("masz", "appsettings.json")

with open(PATH, "r") as fh:
    data = json.load(fh)

data["ConnectionStrings"]["DefaultConnection"] = CNC_STRING
data["AppSettings"]["Token"] = random_pass(200)
data["InternalConfig"]["DiscordBotToken"] = os.getenv("DISCORD_BOT_TOKEN")
data["InternalConfig"]["DiscordClientId"] = os.getenv("DISCORD_OAUTH_CLIENT_ID")
data["InternalConfig"]["DiscordClientSecret"] = os.getenv("DISCORD_OAUTH_CLIENT_SECRET")
data["InternalConfig"]["ServiceHostName"] = os.getenv("META_SERVICE_NAME")
data["InternalConfig"]["ServiceBaseUrl"] = os.getenv("META_SERVICE_BASE_URL")
try:
    data["InternalConfig"]["SiteAdminDiscordUserIds"] = list(os.getenv("DISCORD_SITE_ADMINS").split(","))
except:
    data["InternalConfig"]["SiteAdminDiscordUserIds"] = []

with open(PATH, "w") as fh:
    json.dump(data, fh)

print("Done.")
