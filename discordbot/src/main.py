import requests
import os
import sys
import traceback

import discord
from discord.errors import LoginFailure
from discord.ext import commands

from commands import ALL_COMMANDS
from automod import check_message
from punishment import handle_member_join


intents = discord.Intents.default()
intents.members = True

client = commands.Bot(os.getenv("BOT_PREFIX", "$") if str(os.getenv("BOT_PREFIX", "$")).strip() != "" else "$", intents=intents, help_command=None)  # prefix defaults to $

for command in ALL_COMMANDS:
    print(f"Register '{command}' command.")
    client.add_command(command)


@client.event
async def on_command_error(ctx, error):
    if isinstance(error, commands.errors.CheckFailure) or isinstance(error, commands.errors.BadArgument) or isinstance(error, commands.errors.MissingRequiredArgument):
        pass
    else:
        print('Ignoring exception in command {}:'.format(ctx.command), file=sys.stderr)
        traceback.print_exception(type(error), error, error.__traceback__, file=sys.stderr)


@client.event
async def on_member_join(member):
    await handle_member_join(member)


@client.event
async def on_ready():
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


@client.event
async def on_raw_message_edit(payload):
    channel = client.get_channel(payload.channel_id)
    if not channel:
        return
    try:
        msg = await channel.fetch_message(payload.message_id)
    except Exception as e:  # not found, forbidden, similar
        print(e)
        return
    
    await check_message(msg)

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
