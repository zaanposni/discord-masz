from discord.ext import commands

from .checks import _is_admin_or_mod, registered_guild_only
from helpers import create_modcase_embed
from data import get_modcase_by_guild_and_case_id


@commands.command(help="View details of a modcase.")
@registered_guild_only()
async def view(ctx, case_id):
    if not str(case_id).isnumeric():
        return await ctx.send("Invalid input")
    modcase = await get_modcase_by_guild_and_case_id(int(ctx.guild.id), int(case_id))
    if not modcase:
        return await ctx.send("Not found.")

    if modcase["UserId"] != str(ctx.author.id) and not await _is_admin_or_mod(ctx):
        return await ctx.send("You are not allowed to view this case.")

    embed = await create_modcase_embed(ctx.bot, modcase)
    if embed:
        await ctx.send(embed=embed)
