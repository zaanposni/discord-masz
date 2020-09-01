import sys
import json
import os
import random
import string

start_args = sys.argv[1:]
print(start_args)

def random_pass(pass_length=200):
    chars = string.ascii_letters + string.digits + '!@#$%^&*()'
    random.seed = (os.urandom(1024))
    return ''.join(random.choice(chars) for i in range(pass_length))


# TODO: also get discord bot token and secrets
CNC_STRING = "Server={};Port={};Database={};Uid={};Pwd={};".format(start_args[0], start_args[1], start_args[2], start_args[3], start_args[4])

PATH = os.path.join("masz", "appsettings.json")

with open(PATH, "r") as fh:
    data = json.load(fh)

data["ConnectionStrings"]["DefaultConnection"] = CNC_STRING
data["AppSettings"]["Token"] = random_pass(200)
data["InternalConfig"]["DiscordBotToken"] = start_args[5]
data["InternalConfig"]["DiscordClientId"] = start_args[6]
data["InternalConfig"]["DiscordClientSecret"] = start_args[7]
try:
    data["InternalConfig"]["SiteAdminDiscordUserIds"] = start_args[8].split(",")
except:
    data["InternalConfig"]["SiteAdminDiscordUserIds"] = []

with open(PATH, "w") as fh:
    json.dump(data, fh)

print("Done.")
