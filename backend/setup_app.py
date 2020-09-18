import json
import os
import random
import string

def random_pass(pass_length=200):
    chars = string.ascii_letters + string.digits + '!@#$%^&*()'
    random.seed = (os.urandom(1024))
    return ''.join(random.choice(chars) for i in range(pass_length))

with open("./config.json", "r") as fh:
    config = json.load(fh)

db = config["database"]
CNC_STRING = f"Server={db['host']};Port={db['port']};Database={db['database']};Uid={db['user']};Pwd={db['pass']};"

PATH = os.path.join("masz", "appsettings.json")

with open(PATH, "r") as fh:
    data = json.load(fh)

data["ConnectionStrings"]["DefaultConnection"] = CNC_STRING
data["AppSettings"]["Token"] = random_pass(200)
data["InternalConfig"]["DiscordBotToken"] = config["discord"]["bot_token"]
data["InternalConfig"]["DiscordClientId"] = config["discord"]["oauth_client_id"]
data["InternalConfig"]["DiscordClientSecret"] = config["discord"]["oauth_client_secret"]
data["InternalConfig"]["ServiceHostName"] = config["meta"]["service_name"]
try:
    data["InternalConfig"]["SiteAdminDiscordUserIds"] = list(config["discord"]["site_admins"])
except:
    data["InternalConfig"]["SiteAdminDiscordUserIds"] = []

with open(PATH, "w") as fh:
    json.dump(data, fh)

print("Done.")
