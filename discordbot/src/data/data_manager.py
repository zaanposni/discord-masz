from datetime import datetime, timedelta

from .db_connector import get_automod_config_by_guild, get_guildconfig


CACHE_FOR_MINUTES = 1
CACHED = {
    "automodconfig": { },
    "guildconfig": { }
 }

async def get_cached_automod_config(guildid: str):
    global CACHED
    try:
        if guildid in CACHED["automodconfig"]:
            if CACHED["automodconfig"][guildid]["expires"] >= datetime.now():
                return CACHED["automodconfig"][guildid]["obj"]
    except KeyError:
        pass

    CACHED["automodconfig"][guildid] = dict()
    CACHED["automodconfig"][guildid]["obj"] = await get_automod_config_by_guild(guildid)
    CACHED["automodconfig"][guildid]["expires"] = datetime.now() + timedelta(minutes=CACHE_FOR_MINUTES)

    return CACHED["automodconfig"][guildid]["obj"]


async def get_cached_guild_config(guildid: str):
    global CACHED
    try:
        if guildid in CACHED["guildconfig"]:
            if CACHED["guildconfig"][guildid]["expires"] >= datetime.now():
                return CACHED["guildconfig"][guildid]["obj"]
    except KeyError:
        pass
    
    CACHED["guildconfig"][guildid] = dict()
    CACHED["guildconfig"][guildid]["obj"] = await get_guildconfig(guildid)
    CACHED["guildconfig"][guildid]["expires"] = datetime.now() + timedelta(minutes=CACHE_FOR_MINUTES)

    return CACHED["guildconfig"][guildid]["obj"]
