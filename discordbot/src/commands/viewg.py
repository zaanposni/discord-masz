from discord.ext.commands.errors import BadArgument
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

from helpers import create_modcase_embed
from data import get_modcase_by_guild_and_case_id
from .infrastructure import record_usage, CommandDefinition, _is_admin_or_mod


async def _viewg(ctx, guildid, caseid):
    record_usage(ctx)
    if not str(guildid).isnumeric() or not str(caseid).isnumeric():
        raise BadArgument("Invalid input")
    modcase = await get_modcase_by_guild_and_case_id(int(guildid), int(caseid))
    if not modcase:
        return await ctx.send("Not found.")

    if modcase["UserId"] != str(ctx.author.id) and not await _is_admin_or_mod(ctx):
        return await ctx.send("You are not allowed to view this case.")

    embed = await create_modcase_embed(ctx.bot, modcase)
    if embed:
        await ctx.send(embed=embed)


viewg = CommandDefinition(
    func=_viewg,
    short_help="View details of a modcase.",
    long_help="View details of a modcase.",
    usage="viewg <guild_id> <case_id>",
    options=[
        create_option("guildid", "Guild to search cases in.", SlashCommandOptionType.STRING, True),
        create_option("caseid", "ID of the case to search.", SlashCommandOptionType.STRING, True)
    ]
)

