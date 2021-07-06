import os
import re
from datetime import datetime, timedelta
import requests

from discord.ext import commands
from discord import Member

from .checks import registered_guild_and_admin_or_mod_only
from helpers import parse_delta

regex = re.compile(r"^[0-9]{18}$")

headers = {
    'Authorization': os.getenv("DISCORD_BOT_TOKEN")
}

@commands.command(help="Ban a user.")
@registered_guild_and_admin_or_mod_only()
async def tempban(ctx, member: Member, time, *reason):
    if not len(reason):
        await ctx.send("Please provide a reason.")
        return
    
    time_range = parse_delta(time)
    if not time_range:
        time_range = timedelta(hours=1)
    punished_until = datetime.utcnow() + time_range
    
    reason = ' '.join(reason)
    modCase = {
        "title": reason[:99],
        "description": reason,
        "modid": ctx.author.id,
        "userid": member.id,
        "punishment": "TempBan",
        "labels": [],
        "PunishmentType": 3,
        "PunishmentActive": True,
        "PunishedUntil": punished_until.isoformat()
    }

    r = requests.post(f"http://masz_backend/internalapi/v1/guilds/{ctx.guild.id}/modcases", json=modCase, headers=headers)

    if r.status_code == 201:
        await ctx.send(f"Case #{r.json()['caseid']} created and user banned.\nFollow this link for more information: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")
    elif r.status_code == 401:
        await ctx.send("You are not allowed to do this.")
    else:
        await ctx.send(f"Something went wrong.\nCode: {r.status_code}\nText: {r.text}")


@tempban.error
async def tempban_error(ctx, error):
    if isinstance(error, commands.BadArgument):
        await ctx.send('I could not find that member...')
    if isinstance(error, commands.MissingRequiredArgument):
        await ctx.send(r"Please use \>tempban @user <time range> <reason>\nAlso see $help tempban")
