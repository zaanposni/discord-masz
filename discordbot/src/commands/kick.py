import os
import re
from datetime import datetime
import requests

from discord.ext import commands
from discord import Embed, Member
from discord.errors import NotFound

from .checks import registered_guild_and_admin_or_mod_only
from helpers import get_prefix


regex = re.compile(r"^[0-9]{18}$")

headers = {
    'Authorization': os.getenv("DISCORD_BOT_TOKEN")
}

@commands.command(help="Kick a user.")
@registered_guild_and_admin_or_mod_only()
async def kick(ctx, member: Member, *reason):
    if not len(reason):
        await ctx.send("Please provide a reason.")
        return
    
    reason = ' '.join(reason)
    modCase = {
        "title": reason[:99],
        "description": reason,
        "modid": ctx.author.id,
        "userid": member.id,
        "punishment": "Kick",
        "labels": [],
        "PunishmentType": 2,
        "PunishmentActive": True
    }

    r = requests.post(f"http://masz_backend/internalapi/v1/guilds/{ctx.guild.id}/modcases", json=modCase, headers=headers)

    if r.status_code == 201:
        await ctx.send(f"Case #{r.json()['caseid']} created and user kicked.\nFollow this link for more information: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")
    elif r.status_code == 401:
        await ctx.send("You are not allowed to do this.")
    else:
        await ctx.send(f"Something went wrong.\nCode: {r.status_code}\nText: {r.text}")


@kick.error
async def kick_error(ctx, error):
    if isinstance(error, commands.BadArgument):
        await ctx.send('I could not find that member...')
    if isinstance(error, commands.MissingRequiredArgument):
        await ctx.send(f"Please use `{get_prefix()}kick <username|userid|usermention> <reason>`\nAlso see `{get_prefix()}help kick`")
