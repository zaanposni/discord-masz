import os
import requests
from datetime import datetime

from discord import Message, Embed

from .invite import check_message as check_invite
from .emotes import check_message as check_emotes
from .mentions import check_message as check_mentions
from .attachments import check_message as check_attachments
from .links import check_message as check_links
from .multiple_punishment import check_message as check_multiple
from .custom_words import check_message as check_custom
from data import get_cached_automod_config, get_cached_guild_config


SITE_ADMINS = os.getenv("DISCORD_SITE_ADMINS").strip(",")
type_map = {
    "0": "Invites are not allowed on this guild.",
    "1": "Too many emotes per message are not allowed on this guild.",
    "2": "Too many mentions per message are not allowed on this guild.",
    "3": "Too many attachments per message are not allowed on this guild.",
    "4": "Too many embeds per message are not allowed on this guild.",
    "5": "You triggered too many automoderations.",
    "6": "You used too many unallowed words."
}
punishments = {
    "0": "Warn",
    "1": "Mute",
    "2": "Kick",
    "3": "Ban"
}
headers = {
    'Authorization': os.getenv("DISCORD_BOT_TOKEN")
}

def check_filter(msg: Message, guildconfig, automodconfig) -> bool:
    if str(msg.author.id) in SITE_ADMINS:
        return False
    
    unallowed_roles = automodconfig["IgnoreRoles"].split(",") + guildconfig["ModRoles"].split(",") + guildconfig["AdminRoles"].split(",")
    for role in msg.author.roles:
        if str(role.id) in unallowed_roles:
            return False
    
    unallowed_channels = automodconfig["IgnoreChannels"].split(",")
    if str(msg.channel.id) in unallowed_channels:
        return False
    
    return True


def create_dm_embed(msg: Message, mod_type: int, config) -> Embed:
    embed = Embed(title="Automoderation")
    embed.color = 0xf71b02
    embed.timestamp = datetime.now()
    embed.description = type_map.get(str(mod_type), "You triggered the automoderation.")
    embed.add_field(name="Guild", value=f"{msg.guild.name} | {msg.guild.id}", inline=False)
    embed.add_field(name="Message", value=f"{msg.id}", inline=False)
    embed.add_field(name="Channel", value=f"<#{msg.channel.id}> | {msg.channel.id}", inline=False)
    if config["AutoModerationAction"] in [2, 3]:
        punishment = punishments[str(config['PunishmentType'])]
        if config["PunishmentDurationMinutes"]:
            punishment = f"Temp{punishment} for {config['PunishmentDurationMinutes']} Minutes."
        embed.add_field(name="Punishment", value=punishment, inline=False)
    embed.add_field(name="Content", value=f"> {msg.content[:1020]}", inline=False)  # limit is 1024
    embed.add_field(name="MASZ", value=f"You can view details to this automoderation on: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}", inline=False)

    return embed


def create_public_embed(msg: Message, mod_type: int, config) -> Embed:
    embed = Embed(title="Automoderation")
    embed.color = 0xf71b02
    embed.timestamp = datetime.now()
    embed.description = f'{msg.author.mention} {type_map.get(str(mod_type), "You triggered the automoderation.")}'
    if config["AutoModerationAction"] in [2, 3]:
        punishment = punishments[str(config['PunishmentType'])]
        if config["PunishmentDurationMinutes"]:
            punishment = f"Temp{punishment} for {config['PunishmentDurationMinutes']} Minutes."
        embed.add_field(name="Punishment", value=punishment, inline=False)
    embed.add_field(name="MASZ", value=f"You can view details to this automoderation on: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}", inline=False)

    return embed


def create_internal_embed(msg: Message, mod_type: int, config) -> Embed:
    embed = create_public_embed(msg, mod_type, config)
    embed.description += f"\nChannel: <#{msg.channel.id}>\nMessageId: {msg.id}"
    return embed

async def apply_punishment(msg: Message, mod_type: int, config, guildconfig):
    if guildconfig["ModInternalNotificationWebhook"]:
        try:
            requests.post(guildconfig["ModInternalNotificationWebhook"], json={ "embeds": [create_internal_embed(msg, mod_type, config).to_dict()] })
        except Exception as e:
            print("Failed to send staff notification.")
            print(e)
        
    if config["SendDmNotification"]:
        try:
            await msg.author.send(embed=create_dm_embed(msg, mod_type, config))
        except Exception as e:
            print("Failed to send dm notification.")
            print(e)
    
    url = f"http://masz_backend/internalapi/v1/guilds/{msg.guild.id}/modevent"

    payload = {
        "UserId": msg.author.id,
        "AutoModerationType": mod_type,
        "Username": msg.author.name,
        "Nickname": msg.author.nick,
        "Discriminator": msg.author.discriminator,
        "ChannelId": msg.channel.id,
        "MessageId": msg.id,
        "MessageContent": msg.content
    }

    requests.post(url, headers=headers, json=payload)

    if config["AutoModerationAction"] in [1, 3]:
        try:
            await msg.channel.send(embed=create_public_embed(msg, mod_type, config))
        except Exception as e:
            print("Failed to send notification.")
            print(e)
        try:
            await msg.delete()
        except Exception as e:
            print("Failed to delete message.")
            print(e)


def get_config_by_type(automodconfig, event_type):
    for x in automodconfig:
        if x["AutoModerationType"] == event_type:
            return x
    else:
        return None

async def check_multiple_punishment(msg: Message):
    guildconfig = await get_cached_guild_config(msg.guild.id)
    automodconfig = await get_cached_automod_config(msg.guild.id)
    if not (guildconfig and automodconfig):  # guild not registered or no config
        return
    event_type = 5
    config = get_config_by_type(automodconfig, event_type)
    if not config:
        return
    
    if await check_multiple(msg, config):
        if check_filter(msg, guildconfig, config):
            print(f"Found multiple automoderations by {msg.author} | {msg.author.id} in message {msg.id} in guild {msg.guild.name} | {msg.guild.id}.")
            await apply_punishment(msg, event_type, config, guildconfig)
            return True


async def check_message(msg: Message) -> bool:
    if msg.guild is None:
        return False

    if msg.author.bot:
        return False

    guildconfig = await get_cached_guild_config(msg.guild.id)
    automodconfig = await get_cached_automod_config(msg.guild.id)
    if not (guildconfig and automodconfig):  # guild not registered or no config
        return

    event_type = 0
    config = get_config_by_type(automodconfig, event_type)
    if config:
        if check_invite(msg):
            if check_filter(msg, guildconfig, config):
                print(f"Found invite by {msg.author} | {msg.author.id} in message {msg.id} in guild {msg.guild.name} | {msg.guild.id}.")
                await apply_punishment(msg, event_type, config, guildconfig)
                await check_multiple_punishment(msg)
                return True

    event_type = 1
    config = get_config_by_type(automodconfig, event_type)
    if config:
        if check_emotes(msg, config):
            if check_filter(msg, guildconfig, config):
                print(f"Found emotes by {msg.author} | {msg.author.id} in message {msg.id} in guild {msg.guild.name} | {msg.guild.id}.")
                await apply_punishment(msg, event_type, config, guildconfig)
                await check_multiple_punishment(msg)
                return True

    event_type = 2
    config = get_config_by_type(automodconfig, event_type)
    if config:
        if check_mentions(msg, config):
            if check_filter(msg, guildconfig, config):
                print(f"Found mentions by {msg.author} | {msg.author.id} in message {msg.id} in guild {msg.guild.name} | {msg.guild.id}.")
                await apply_punishment(msg, event_type, config, guildconfig)
                await check_multiple_punishment(msg)
                return True

    event_type = 3
    config = get_config_by_type(automodconfig, event_type)
    if config:
        if check_attachments(msg, config):
            if check_filter(msg, guildconfig, config):
                print(f"Found attachments by {msg.author} | {msg.author.id} in message {msg.id} in guild {msg.guild.name} | {msg.guild.id}.")
                await apply_punishment(msg, event_type, config, guildconfig)
                await check_multiple_punishment(msg)
                return True

    event_type = 4
    config = get_config_by_type(automodconfig, event_type)
    if config:
        if check_links(msg, config):
            if check_filter(msg, guildconfig, config):
                print(f"Found embeds by {msg.author} | {msg.author.id} in message {msg.id} in guild {msg.guild.name} | {msg.guild.id}.")
                await apply_punishment(msg, event_type, config, guildconfig)
                await check_multiple_punishment(msg)
                return True

    # 5 is TooManyAutomoderations

    event_type = 6
    config = get_config_by_type(automodconfig, event_type)
    if config:
        if check_custom(msg, config):
            if check_filter(msg, guildconfig, config):
                print(f"Found customs by {msg.author} | {msg.author.id} in message {msg.id} in guild {msg.guild.name} | {msg.guild.id}.")
                await apply_punishment(msg, event_type, config, guildconfig)
                await check_multiple_punishment(msg)
                return True

    return False

