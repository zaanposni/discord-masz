import os

from databases import Database


c_str = f'mysql://{os.getenv("MYSQL_USER")}:{os.getenv("MYSQL_PASSWORD")}@{os.getenv("MYSQL_HOST")}:{os.getenv("MYSQL_PORT")}/{os.getenv("MYSQL_DATABASE")}'
database = Database(c_str)

async def connect():
    print("Connecting database")
    await database.connect()

async def get_guildconfig(guildid: str):
    query = "SELECT GuildId, ModRoleId, AdminRoleId, MutedRoleId FROM GuildConfigs WHERE GuildId = :guildid"
    values = { "guildid": guildid }
    return await database.fetch_one(query=query, values=values)


