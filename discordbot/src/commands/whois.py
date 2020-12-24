import os
import re
from datetime import datetime

from discord.ext import commands
from discord import Embed, Member
from discord.errors import NotFound

from data import get_modcases_by_user_and_guild, get_guildconfig


regex = r"^[0-9]{18}$"

@commands.command(help="Whois information about an user.")
async def whois(ctx, userid):
    if not userid:
        return await ctx.send("Please use `whois <userid>`.")
    if not re.match(regex, userid):
        return await ctx.send("Please use `whois <userid>`.")
    if ctx.guild is None:
        return await ctx.send("Only useable in a guild.")
    
    try:
        cfg = await get_guildconfig(str(ctx.guild.id))
    except Exception as e:
        print(e)
        return await ctx.send("Failed to fetch data from database.")

    if cfg is None:
        if os.getenv('META_SERVICE_BASE_URL', ''):
            await ctx.send(f"MASZ is not registered on this guild.\nA siteadmin can register this guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/newguild?guildid={ctx.guild.id}")
        else:
            await ctx.send(f"MASZ is not registered on this guild.")
        return

    site_admins = os.getenv("DISCORD_SITE_ADMINS").strip(",")
    is_site_admin = str(ctx.author.id) in site_admins
    is_admin_or_mod = any([role for role in ctx.author.roles if str(role.id) in [cfg["ModRoleId"], cfg["AdminRoleId"]]])
    if not is_admin_or_mod and not is_site_admin:
        return await ctx.send("You do not have the permission to use this command.")
    
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
        for case in cases[:5]:
            info += f"[#{case['CaseId']} - {case['Title']}]"
            info += f"({os.getenv('META_SERVICE_BASE_URL', '')}/modcases/{ctx.guild.id}/{case['CaseId']})"
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
            for case in active_punishments[:5]:
                info += f"{case['Punishment']}"
                if case["PunishedUntil"] is not None:
                    info += f" (until {case['PunishedUntil'].strftime('%d.%m.%Y')}): "
                else:
                    info += ": "
                info += f"[#{case['CaseId']} - {case['Title']}]({os.getenv('META_SERVICE_BASE_URL', '')}/modcases/{ctx.guild.id}/{case['CaseId']})"
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
