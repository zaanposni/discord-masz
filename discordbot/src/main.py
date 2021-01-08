import requests
import os

import discord
from discord.errors import LoginFailure
from discord.ext import commands
from pretty_help import PrettyHelp, Navigation

from commands import ALL_COMMANDS
from data import connect as db_connect
from automod import check_message
from punishment import handle_member_join


nav = Navigation("⬅️", "➡️", "🚫")
color = discord.Color.dark_gold()

intents = discord.Intents.default()
intents.members = True

client = commands.Bot(os.getenv("BOT_PREFIX", "$") if str(os.getenv("BOT_PREFIX", "$")).strip() != "" else "$", intents=intents)  # prefix defaults to $
client.help_command = PrettyHelp(navigation=nav, color=color, active_time=5, no_category="Commands", sort_commands=True, show_index=False)

for command in ALL_COMMANDS:
    print(f"Register '{command}' command.")
    client.add_command(command)


@client.event
async def on_member_join(member):
    await handle_member_join(member)


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


@client.event
async def on_message(msg):
    moderated = await check_message(msg)
    if not moderated:
        await client.process_commands(msg)


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
