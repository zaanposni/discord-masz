import os
import re
from datetime import datetime

from discord import Embed, Member, User, Guild
from discord.errors import NotFound

from data import get_modcases_by_user_and_guild, get_cached_guild_config, get_latest_joins_by_user_and_guild, get_usernote_by_user_and_guild

async def create_whois_embed(guild: Guild, user: User):
    try:
        member = await guild.fetch_member(user.id)
    except NotFound:
        member = user
    
    cases = await get_modcases_by_user_and_guild(guild.id, user.id)
    active_punishments = [case for case in cases if case["PunishmentActive"]]

    embed = Embed(description=f"<@{user.id}>")    
    embed.timestamp = datetime.now()
    embed.set_footer(text=f"UserId: {user.id}")
    
    invites = await get_latest_joins_by_user_and_guild(user.id, guild.id)
    if member:
        if isinstance(member, Member):
            if member.joined_at:
                text = member.joined_at.strftime("%d.%m.%Y %H:%M:%S")
                if invites:
                    text += f"\nUsed invite: {invites[0]['UsedInvite']}\nBy: <@{invites[0]['InviteIssuerId']}>"
                embed.add_field(name="Joined", value=text, inline=True)

        embed.add_field(name="Registered", value=member.created_at.strftime("%d.%m.%Y %H:%M:%S"), inline=True)
        if isinstance(member, Member):
            if member.roles[1:]:
                embed.add_field(name=f"Roles [{len(member.roles) - 1}]", value=" ".join([f"<@&{role.id}>" for role in member.roles[1:]]), inline=False)

        embed.set_author(name=str(member), icon_url=member.avatar_url)
        embed.set_thumbnail(url=member.avatar_url)

    note = await get_usernote_by_user_and_guild(user.id, guild.id)
    if note:
        embed.add_field(
            name=f"Usernote",
            value=note['Description'],
            inline=False
            )

    if cases:
        info = ""
        for case in cases[:-6:-1]:
            info += f"[#{case['CaseId']} - {case['Title']}]"
            info += f"({os.getenv('META_SERVICE_BASE_URL', '')}/guilds/{guild.id}/cases/{case['CaseId']})"
            info += "\n"
        
        if len(cases) > 5:
            info += "[...]"

        embed.add_field(
            name=f"Cases [{len(cases)}]",
            value=info,
            inline=False
            )
        
        if active_punishments:
            info = ""
            for case in active_punishments[:-6:-1]:
                info += f"{case['Punishment']}"
                if case["PunishedUntil"] is not None:
                    info += f" (until {case['PunishedUntil'].strftime('%d.%m.%Y')}): "
                else:
                    info += ": "
                info += f"[#{case['CaseId']} - {case['Title']}]({os.getenv('META_SERVICE_BASE_URL', '')}/guilds/{guild.id}/cases/{case['CaseId']})"
                info += "\n"
            
            if len(active_punishments) > 5:
                info += "[...]"

            embed.add_field(
                name=f"Active Punishments [{len(active_punishments)}]",
                value=info,
                inline=False
            )
            
    else:
        embed.add_field(name=f"Cases [0]", value="There are no cases for this user.", inline=False)
    
    return embed