import os
import re
from datetime import datetime
import requests

from discord.ext import commands
from discord import Embed, Member
from discord.errors import NotFound

from .checks import registered_guild_with_muted_role_and_admin_or_mod_only

regex = re.compile(r"^[0-9]{18}$")

headers = {
    'Authorization': os.getenv("DISCORD_BOT_TOKEN")
}

@commands.command(help="Mute a user.")
@registered_guild_with_muted_role_and_admin_or_mod_only()
async def mute(ctx, member: Member, *reason):
    if not len(reason):
        await ctx.send("Please provide a reason.")
        return
    
    reason = ' '.join(reason)
    modCase = {
        "title": reason[:99],
        "description": reason,
        "modid": ctx.author.id,
        "userid": member.id,
        "punishment": "Mute",
        "labels": [],
        "PunishmentType": 1,
        "PunishmentActive": True
    }

    r = requests.post(f"http://masz_backend/internalapi/v1/guilds/{ctx.guild.id}/modcases", json=modCase, headers=headers)

    if r.status_code == 201:
        await ctx.send(f"Case #{r.json()['caseid']} created and user muted.\nFollow this link for more information: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")
    elif r.status_code == 401:
        await ctx.send("You are not allowed to do this.")
    else:
        await ctx.send("Something went wrong.")


@mute.error
async def mute_error(ctx, error):
    if isinstance(error, commands.BadArgument):
        await ctx.send('I could not find that member...')
    if isinstance(error, commands.MissingRequiredArgument):
        await ctx.send(r"Please use \>mute @user the reason")
