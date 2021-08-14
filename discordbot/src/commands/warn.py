import os
import re
from datetime import datetime
import requests

from discord.ext import commands
from discord import Embed, Member
from discord.errors import NotFound

from .checks import registered_guild_and_admin_or_mod_only
from helpers import get_prefix
from .record_usage import record_usage


regex = re.compile(r"^[0-9]{18}$")

headers = {
    'Authorization': os.getenv("DISCORD_BOT_TOKEN")
}

@commands.command(help="Warn a user.")
@commands.before_invoke(record_usage)
@registered_guild_and_admin_or_mod_only()
async def warn(ctx, member: Member, *reason):
    if not len(reason):
        await ctx.send("Please provide a reason.")
        return
    
    reason = ' '.join(reason)
    modCase = {
        "title": reason[:99],
        "description": reason,
        "modid": ctx.author.id,
        "userid": member.id,
        "punishment": "Warn",
        "labels": [],
        "PunishmentType": 0,
        "PunishmentActive": False
    }

    r = requests.post(f"http://masz_backend/internalapi/v1/guilds/{ctx.guild.id}/modcases", json=modCase, headers=headers)

    if r.status_code == 201:
        await ctx.send(f"Case #{r.json()['caseid']} created.\nFollow this link for more information: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")
    elif r.status_code == 401:
        await ctx.send("You are not allowed to do this.")
    else:
        await ctx.send(f"Something went wrong.\nCode: {r.status_code}\nText: {r.text}")


@warn.error
async def warn_error(ctx, error):
    if isinstance(error, commands.BadArgument):
        await ctx.send('I could not find that member...')
    if isinstance(error, commands.MissingRequiredArgument):
        await ctx.send(f"Please use `{get_prefix()}warn <username|userid|usermention> <reason>`\nAlso see `{get_prefix()}help warn`")
