import os

from databases import Database


c_str = f'mysql://{os.getenv("MYSQL_USER")}:{os.getenv("MYSQL_PASSWORD")}@{os.getenv("MYSQL_HOST")}:{os.getenv("MYSQL_PORT")}/{os.getenv("MYSQL_DATABASE")}'
database = Database(c_str)

async def connect():
    print("Connecting database")
    await database.connect()

async def get_guildconfig(guildid: str):
    query = "SELECT * FROM GuildConfigs WHERE GuildId = :guildid"
    values = { "guildid": guildid }
    return await database.fetch_one(query=query, values=values)

async def get_modcases_by_user_and_guild(guildid: str, userid: str):
    query = "SELECT * FROM ModCases WHERE GuildId = :guildid AND UserId = :userid"
    values = { "guildid": guildid, "userid": userid }
    return await database.fetch_all(query=query, values=values)

async def get_automod_config_by_guild(guildid: str):
    query = "SELECT * FROM AutoModerationConfigs WHERE GuildId = :guildid"
    values = { "guildid": guildid }
    return await database.fetch_all(query=query, values=values)
