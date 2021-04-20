import os

from databases import Database


c_str = f'mysql://{os.getenv("MYSQL_USER")}:{os.getenv("MYSQL_PASSWORD")}@{os.getenv("MYSQL_HOST")}:{os.getenv("MYSQL_PORT")}/{os.getenv("MYSQL_DATABASE")}'
database = Database(c_str)

async def connect():
    if not database.is_connected:
        print("Connecting database")
        await database.connect()

async def get_guildconfig(guildid: str):
    query = "SELECT * FROM GuildConfigs WHERE GuildId = :guildid"
    values = { "guildid": guildid }
    await connect()
    return await database.fetch_one(query=query, values=values)

async def get_modcases_by_user_and_guild(guildid: str, userid: str):
    query = "SELECT * FROM ModCases WHERE GuildId = :guildid AND UserId = :userid"
    values = { "guildid": guildid, "userid": userid }
    await connect()
    return await database.fetch_all(query=query, values=values)

async def get_modcases_by_user_and_guild_with_active_mute(guildid: str, userid: str):
    query = "SELECT * FROM ModCases WHERE GuildId = :guildid AND UserId = :userid AND PunishmentActive = 1 AND PunishmentType = 1"
    values = { "guildid": guildid, "userid": userid }
    await connect()
    return await database.fetch_all(query=query, values=values)

async def get_automod_config_by_guild(guildid: str):
    query = "SELECT * FROM AutoModerationConfigs WHERE GuildId = :guildid"
    values = { "guildid": guildid }
    await connect()
    return await database.fetch_all(query=query, values=values)

async def get_automodevents_by_user_since_minutes(userid: str, minutes: int):
    query = "SELECT * FROM AutoModerationEvents WHERE UserId = :userid AND CreatedAt >= NOW() - INTERVAL :minutes MINUTE"
    values = { "userid": userid, "minutes": minutes }
    await connect()
    return await database.fetch_all(query=query, values=values)
