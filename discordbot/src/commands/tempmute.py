import os
import re
from datetime import datetime, timedelta
import requests

from discord.ext import commands
from discord import Member

from .checks import registered_guild_with_muted_role_and_admin_or_mod_only
from helpers import parse_delta, get_prefix
from .record_usage import record_usage


regex = re.compile(r"^[0-9]{18}$")

headers = {
    'Authorization': os.getenv("DISCORD_BOT_TOKEN")
}

@commands.command(help="Mute a user.")
@commands.before_invoke(record_usage)
@registered_guild_with_muted_role_and_admin_or_mod_only()
async def tempmute(ctx, member: Member, time, *reason):
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
        "punishment": "TempMute",
        "labels": [],
        "PunishmentType": 1,
        "PunishmentActive": True,
        "PunishedUntil": punished_until.isoformat()
    }

    r = requests.post(f"http://masz_backend/internalapi/v1/guilds/{ctx.guild.id}/modcases", json=modCase, headers=headers)

    if r.status_code == 201:
        await ctx.send(f"Case #{r.json()['caseid']} created and user muted.\nFollow this link for more information: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")
    elif r.status_code == 401:
        await ctx.send("You are not allowed to do this.")
    else:
        await ctx.send(f"Something went wrong.\nCode: {r.status_code}\nText: {r.text}")


@tempmute.error
async def tempmute_error(ctx, error):
    if isinstance(error, commands.BadArgument):
        await ctx.send('I could not find that member...')
    if isinstance(error, commands.MissingRequiredArgument):
        await ctx.send(f"Please use `{get_prefix()}tempmute <username|userid|usermention> <duration> <reason>`\nAlso see `{get_prefix()}help tempmute`")
