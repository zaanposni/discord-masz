import requests
import os

import discord
from discord.errors import LoginFailure
from discord.ext import commands

from utils.config import cfg

prefix = os.getenv("BOT_PREFIX", "$")
if prefix.strip() == "":
    prefix = "$"
client = commands.Bot(prefix)

@client.command()
async def version(ctx):
    r = requests.get(f"http://masz_nginx:80/static/version.json")
    if r.status_code != 200:
        ctx.send(f"Failed to fetch version info. Statuscode: {r.status_code}.")
        return

    deployed_version = r.json()["version"]

    r = requests.get("https://api.github.com/repos/zaanposni/discord-masz/releases")
    if r.status_code != 200:
        await ctx.send(f"Failed to fetch release info from github. Statuscode: {r.status_code}.")
        return

    newest_release = r.json()[0]
    if str(newest_release["tag_name"]).lower() != str(deployed_version).lower():
        await ctx.send(
            f"There seems to be a newer version **{newest_release['tag_name']}**. You are on **{deployed_version}**.\nContact your site admin to install the update." +
            "\n\nPatch Notes for the newest version:\n```\n" +
            newest_release["body"] + "\n```"
        )
        return
    else:
        await ctx.send(f"Your deployed version **{deployed_version}** is up to date with releases on GitHub.")



@client.event
async def on_ready():
    # console related
    # ================================================
    print(f"========================")
    print(f"Logged in as \"{client.user.name}\"")
    print(f"Online in {len(client.guilds)} Guilds.")
    print(f"========================")

    # discord related
    # ================================================
    activity = cfg.get("meta", dict()).get("service_base_url", "github.com/zaanposni/discord-masz")
    if activity:
        game = discord.Game(name=activity)
        await client.change_presence(activity=game)
        print(f"Set game: \"{game.name}\".")


def start_bot():
    try:
        token = cfg.get("discord", dict())["bot_token"]
    except KeyError:
        print(f"========================")
        print(f"'discord'.'bot_token' not found in config files!")
        return
    try:
        print(f"Logging in.")
        client.run(token, reconnect=True)
    except LoginFailure:
        print(f"========================")
        print(f"Login failure!")
        print(f"Please check your token.")
        return


start_bot()
