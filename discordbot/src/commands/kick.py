import os
import requests

from discord import Member
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

from .infrastructure import record_usage, registered_guild_and_admin_or_mod_only, CommandDefinition, defer_cmd


headers = {
    'Authorization': os.getenv("DISCORD_BOT_TOKEN")
}

async def _kick(ctx, member: Member, *, reason):
    await registered_guild_and_admin_or_mod_only(ctx)
    record_usage(ctx)
    await defer_cmd(ctx)
    if not reason:
        return await ctx.send("Please provide a reason.")

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


kick = CommandDefinition(
    func=_kick,
    short_help="Kick a member.",
    long_help="Kick a member. This also creates a modcase.",
    usage="kick <username|userid|usermention> <reason>",
    options=[
        create_option("member", "Member to kick.", SlashCommandOptionType.USER, True),
        create_option("reason", "Reason to kick.", SlashCommandOptionType.STRING, True),
    ]
)

