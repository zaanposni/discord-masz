import os

from discord import Embed

from data import get_cached_guild_config
from .modcase_punishment import get_modcase_punishment
from helpers import console


async def create_modcase_embed(bot, modcase) -> Embed:    
    guild_config = await get_cached_guild_config(modcase["GuildId"])
    if not guild_config:
        return None

    if not modcase:
        return None

    # https://discord.com/developers/docs/resources/channel#embed-limits
    embed = Embed()
    embed.url = f"{os.getenv('META_SERVICE_BASE_URL')}/guilds/{modcase['GuildId']}/cases/{modcase['CaseId']}"
    embed.timestamp = modcase["CreatedAt"]

    try:
        user = await bot.fetch_user(int(modcase["UserId"]))
        embed.set_thumbnail(url=user.avatar_url)
    except Exception as e:
        console.critical("Failed to fetch user for modcase: {e}")

    title = f'#{modcase["CaseId"]} {modcase["Title"][:200]}'   
    if len(modcase["Title"]) > 200:
        title + " [...]"
    embed.title = title

    desc = modcase["Description"][:2000]
    if len(modcase["Description"]) > 2000:
        desc + " [...]"
    embed.description = desc 
    
    embed.add_field(
        name="⚖️ - Punishment",
        value=get_modcase_punishment(modcase),
        inline=True
    )
    if modcase["PunishedUntil"]:
        embed.add_field(
            name="⏰ - Punished Until (UTC)",
            value=modcase['PunishedUntil'].strftime('%d %b %Y %H:%M:%S'),
            inline=True
        )
    
    if modcase["Labels"]:
        labels = ""
        for label in modcase['Labels'].split(","):
            labels += f"`{label}` "
        embed.add_field(
            name="🏷️ - Labels",
            value=labels,
            inline=False
        )

    if guild_config["PublishModeratorInfo"]:
        try:
            mod = await bot.fetch_user(int(modcase["ModId"]))
            embed.set_author(
                name=mod.name,
                url=mod.avatar_url,
                icon_url=mod.avatar_url
            )
        except Exception as e:
            console.critical("Failed to fetch user for modcase: {e}")
    else:
        embed.set_footer(text="Moderator hidden")

    return embed
