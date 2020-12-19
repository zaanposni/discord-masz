import requests
import os

import discord
from discord.errors import LoginFailure
from discord.ext import commands
from pretty_help import PrettyHelp, Navigation

from commands import ALL_COMMANDS
from data import connect as db_connect


nav = Navigation("⬅️", "➡️", "🚫")
color = discord.Color.dark_gold()

client = commands.Bot(os.getenv("BOT_PREFIX", "$") if str(os.getenv("BOT_PREFIX", "$")).strip() != "" else "$")  # prefix defaults to $
client.help_command = PrettyHelp(navigation=nav, color=color, active_time=5, no_category="Commands", sort_commands=True, show_index=False)

for command in ALL_COMMANDS:
    print(f"Register '{command}' command.")
    client.add_command(command)

@client.event
async def on_ready():
    await db_connect()

    print(f"Logged in as \"{client.user.name}\"")
    print(f"Online in {len(client.guilds)} Guilds.")

    activity = os.getenv("META_SERVICE_BASE_URL", "github.com/zaanposni/discord-masz")
    if activity:
        game = discord.Game(name=activity)
        await client.change_presence(activity=game)
        print(f"Set game: \"{game.name}\".")


def start_bot():
    try:
        token = os.getenv("DISCORD_BOT_TOKEN")
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
