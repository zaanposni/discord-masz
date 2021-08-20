import os
from datetime import datetime, timedelta
import requests

from discord import Member
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

from helpers import parse_delta
from .infrastructure import record_usage, registered_guild_with_muted_role_and_admin_or_mod_only, CommandDefinition


headers = {
    'Authorization': os.getenv("DISCORD_BOT_TOKEN")
}

async def _tempmute(ctx, member: Member, duration, *, reason):
    await registered_guild_with_muted_role_and_admin_or_mod_only(ctx)
    record_usage(ctx)
    if not reason:
        await ctx.send("Please provide a reason.")
        return
    
    time_range = parse_delta(duration)
    if not time_range:
        time_range = timedelta(hours=1)
    punished_until = datetime.utcnow() + time_range
    
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


tempmute = CommandDefinition(
    func=_tempmute,
    short_help="Tempmute a member.",
    long_help="Tempmute a member. This also creates a modcase.",
    usage="tempmute <username|userid|usermention> <duration> <reason>",
    options=[
        create_option("member", "Member to mute.", SlashCommandOptionType.USER, True),
        create_option("duration", "Duration to mute.", SlashCommandOptionType.STRING, True),
        create_option("reason", "Reason to mute.", SlashCommandOptionType.STRING, True),
    ]
)
