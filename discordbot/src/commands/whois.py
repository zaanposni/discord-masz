import os
import re
from datetime import datetime

from discord.ext import commands
from discord import Embed, Member
from discord.errors import NotFound

from data import get_modcases_by_user_and_guild, get_cached_guild_config
from .checks import registered_guild_and_admin_or_mod_only

regex = re.compile(r"^[0-9]{18}$")

@commands.command(help="Whois information about an user.")
@registered_guild_and_admin_or_mod_only()
async def whois(ctx, userid):
    if not userid:
        return await ctx.send("Please use `whois <userid>`.")
    if not regex.match(userid):
        return await ctx.send("Please use `whois <userid>`.")
    
    try:
        member = await ctx.guild.fetch_member(userid)
    except NotFound:
        member = await ctx.bot.fetch_user(userid)
    
    cases = await get_modcases_by_user_and_guild(ctx.guild.id, userid)
    active_punishments = [case for case in cases if case["PunishmentActive"]]

    embed = Embed(description=f"<@{userid}>")    
    embed.timestamp = datetime.now()
    embed.set_footer(text=f"UserId: {userid}")
    
    if member:
        if isinstance(member, Member):
            if member.joined_at:
                embed.add_field(name="Joined", value=member.joined_at.strftime("%d.%m.%Y %H:%M:%S"), inline=True)

        embed.add_field(name="Registered", value=member.created_at.strftime("%d.%m.%Y %H:%M:%S"), inline=True)
        if isinstance(member, Member):
            if member.roles[1:]:
                embed.add_field(name=f"Roles [{len(member.roles) - 1}]", value=" ".join([f"<@&{role.id}>" for role in member.roles[1:]]), inline=False)

        embed.set_author(name=str(member), icon_url=member.avatar_url)
        embed.set_thumbnail(url=member.avatar_url)

    if cases:
        info = ""
        for case in cases[:-6:-1]:
            info += f"[#{case['CaseId']} - {case['Title']}]"
            info += f"({os.getenv('META_SERVICE_BASE_URL', '')}/guilds/{ctx.guild.id}/cases/{case['CaseId']})"
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
                info += f"[#{case['CaseId']} - {case['Title']}]({os.getenv('META_SERVICE_BASE_URL', '')}/guilds/{ctx.guild.id}/cases/{case['CaseId']})"
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

    await ctx.send(embed=embed)
