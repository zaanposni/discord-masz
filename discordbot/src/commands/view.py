from discord.ext.commands.errors import BadArgument
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

from helpers import create_modcase_embed
from data import get_modcase_by_guild_and_case_id
from .infrastructure import record_usage, CommandDefinition, _is_admin_or_mod, registered_guild_only


async def _view(ctx, caseid):
    await registered_guild_only(ctx)
    record_usage(ctx)
    if not str(caseid).isnumeric():
        raise BadArgument("Invalid input")
    modcase = await get_modcase_by_guild_and_case_id(int(ctx.guild.id), int(caseid))
    if not modcase:
        return await ctx.send("Not found.")

    if modcase["UserId"] != str(ctx.author.id) and not await _is_admin_or_mod(ctx):
        return await ctx.send("You are not allowed to view this case.")

    embed = await create_modcase_embed(ctx.bot, modcase)
    if embed:
        await ctx.send(embed=embed)


view = CommandDefinition(
    func=_view,
    short_help="View details of a modcase.",
    long_help="View details of a modcase.",
    usage="view <case_id>",
    options=[
        create_option("caseid", "ID of the case to search.", SlashCommandOptionType.STRING, True)
    ]
)
