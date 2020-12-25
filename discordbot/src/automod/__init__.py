import os
import requests
from datetime import datetime

from discord import Message, Embed

from .invite import check_message as check_invite
from data import get_cached_automod_config, get_cached_guild_config


SITE_ADMINS = os.getenv("DISCORD_SITE_ADMINS").strip(",")

def check_filter(msg: Message, guildconfig, automodconfig) -> bool:
    if str(msg.author.id) in SITE_ADMINS:
        return False
    
    unallowed_roles = automodconfig["IgnoreRoles"].split(",") + [guildconfig["ModRoleId"]] + [guildconfig["AdminRoleId"]]
    for role in msg.author.roles:
        if str(role.id) in unallowed_roles:
            return False
    
    unallowed_channels = automodconfig["IgnoreChannels"].split(",")
    if str(msg.channel.id) in unallowed_channels:
        return False
    
    return True


type_map = {
    "0": "Invites are not allowed on this guild."
}
punishments = {
    "0": "Warn",
    "1": "Mute",
    "2": "Kick",
    "3": "Ban"
}

def create_dm_embed(msg: Message, type: int, config) -> Embed:
    embed = Embed(title="Automoderation")
    embed.color = 0xf71b02
    embed.timestamp = datetime.now()
    embed.description = type_map.get(str(type), "You triggered the automoderation.")
    embed.add_field(name="Guild", value=f"{msg.guild.name} | {msg.guild.id}", inline=False)
    embed.add_field(name="Message", value=f"{msg.id}", inline=False)
    embed.add_field(name="Channel", value=f"<#{msg.channel.id}> | {msg.channel.id}", inline=False)
    if config["AutoModerationAction"] in [2, 3]:
        punishment = punishments[str(config['PunishmentType'])]
        if config["PunishmentDurationMinutes"]:
            punishment = f"Temp{punishment} for {config['PunishmentDurationMinutes']} Minutes."
        embed.add_field(name="Punishment", value=punishment, inline=False)
    embed.add_field(name="Content", value=f"> {msg.content}", inline=False)
    embed.add_field(name="MASZ", value=f"You can view details to this automoderation on: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}", inline=False)

    return embed

async def apply_punishment(msg: Message, type: int, config):
    if config["SendDmNotification"]:
        await msg.author.send(embed=create_dm_embed(msg, type, config))
    
    url = f"http://masz_backend/internalapi/v1/guilds/{msg.guild.id}/modcases"

    payload = {
        "UserId": msg.author.id,
        "AutoModerationType": type,
        "Username": msg.author.name,
        "Nickname": msg.author.nick,
        "Discriminator": msg.author.discriminator,
        "MessageId": msg.id,
        "MessageContent": msg.content
    }

    headers = {
        'Authorization': os.getenv("DISCORD_BOT_TOKEN")
    }

    requests.post(url, headers=headers, json=payload)

    if config["AutoModerationAction"] in [1, 3]:
        await msg.delete()


async def check_message(msg: Message):
    if msg.guild is None:
        return False

    if msg.author.bot:
        return False

    guildconfig = await get_cached_guild_config(msg.guild.id)
    automodconfig = await get_cached_automod_config(msg.guild.id)

    if check_invite(msg):
        event_type = 0
        config = next((x for x in automodconfig if x["AutoModerationType"] == event_type), None)
        if config:
            if check_filter(msg, guildconfig, config):
                await apply_punishment(msg, event_type, config)
                return True

    return False

