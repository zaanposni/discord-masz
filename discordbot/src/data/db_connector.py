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

async def get_modcases_by_user_and_guild(guildid: str, userid: str):
    query = "SELECT CaseId, CreatedAt, Description, GuildId, Labels, LastEditedAt, Valid, " \
            "LastEditedByModId, ModId, Username, Nickname, OccuredAt, Others, PunishedUntil, " \
            "Punishment, PunishmentActive, PunishmentType, Severity, Title, UserId " \
            "FROM ModCases WHERE GuildId = :guildid AND UserId = :userid"
    values = { "guildid": guildid, "userid": userid }
    return await database.fetch_all(query=query, values=values)
