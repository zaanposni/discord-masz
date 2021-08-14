import os
from datetime import datetime

from discord import Embed, Member, User, Guild
from discord.ext import commands

from data import get_modcases_by_user_and_guild
from .checks import registered_guild_only
from helpers import get_prefix
from .record_usage import record_usage


async def create_embed_for_guild(guild_id, user_id):
    embed = Embed(description=f"<@{user_id}>")    
    embed.timestamp = datetime.now()
    embed.set_footer(text=f"UserId: {user_id}")

    modcases = await get_modcases_by_user_and_guild(guild_id, user_id)
    active_punishments = [case for case in modcases if case["PunishmentActive"]]

    if modcases:
        info = ""
        for case in modcases[:-6:-1]:
            info += f"[#{case['CaseId']} - {case['Title']}]"
            info += f"({os.getenv('META_SERVICE_BASE_URL', '')}/guilds/{guild_id}/cases/{case['CaseId']})"
            info += "\n"
        
        if len(modcases) > 5:
            info += "[...]"

        embed.add_field(
            name=f"Cases [{len(modcases)}]",
            value=info,
            inline=False
            )
        
        if active_punishments:
            info = ""
            for case in active_punishments[:-6:-1]:
                info += f"{case['Punishment']}"
                if case["PunishedUntil"] is not None:
                    info += f" (until {case['PunishedUntil'].strftime('%d %b %Y')}): "
                else:
                    info += ": "
                info += f"[#{case['CaseId']} - {case['Title']}]({os.getenv('META_SERVICE_BASE_URL', '')}/guilds/{guild_id}/cases/{case['CaseId']})"
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


@commands.command(help="List cases of current user.")
@commands.before_invoke(record_usage)
async def cases(ctx, guild_id = None):
    if guild_id is None:
        if ctx.guild is None:
            return await ctx.send(f"Please use `{get_prefix()}cases [guild_id]`\nAlso see `{get_prefix()}help cases`")
        guild_id = ctx.guild.id
    embed = await create_embed_for_guild(guild_id, ctx.author.id)
    if embed:
        await ctx.send(embed=embed)
