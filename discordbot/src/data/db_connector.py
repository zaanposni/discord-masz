import os

from databases import Database

from helpers import console


c_str = f'mysql://{os.getenv("MYSQL_USER")}:{os.getenv("MYSQL_PASSWORD")}@{os.getenv("MYSQL_HOST")}:{os.getenv("MYSQL_PORT")}/{os.getenv("MYSQL_DATABASE")}'
database = Database(c_str)

async def connect():
    if not database.is_connected:
        console.info("Connecting database")
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

async def get_modcase_by_guild_and_case_id(guildid: str, caseid: str):
    query = "SELECT * FROM ModCases WHERE GuildId = :guildid AND CaseId = :caseid"
    values = { "guildid": guildid, "caseid": caseid }
    await connect()
    return await database.fetch_one(query=query, values=values)

async def get_modcases_by_user_and_guild_with_active_mute(guildid: str, userid: str):
    query = "SELECT * FROM ModCases WHERE GuildId = :guildid AND UserId = :userid AND PunishmentActive = 1 AND PunishmentType = 1 AND MarkedToDeleteAt IS NULL"
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

async def get_latest_joins_by_user_and_guild(userid: str, guildid: str):
    query = "SELECT * FROM UserInvites WHERE JoinedUserId = :userid AND GuildId = :guildid ORDER BY JoinedAt DESC"
    values = { "guildid": guildid, "userid": userid }
    await connect()
    return await database.fetch_all(query=query, values=values)

async def get_usernote_by_user_and_guild(userid: str, guildid: str):
    query = "SELECT * FROM UserNotes WHERE GuildId = :guildid AND UserId = :userid"
    values = { "guildid": guildid, "userid": userid }
    await connect()
    return await database.fetch_one(query=query, values=values)

async def get_invites_by_guild_and_code(guildid: str, code: str):
    query = "SELECT * FROM UserInvites WHERE GuildId = :guildid AND UsedInvite = :code"
    values = { "guildid": guildid, "code": code }
    await connect()
    return await database.fetch_all(query=query, values=values)
