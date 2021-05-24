from discord.ext import commands
from discord import User

from .checks import registered_guild_and_admin_or_mod_only
from helpers import create_whois_embed

@commands.command(help="Whois information about a user.")
@registered_guild_and_admin_or_mod_only()
async def whois(ctx, user: User):
    embed = await create_whois_embed(ctx.guild, user)
    if embed:
        await ctx.send(embed=embed)

@whois.error
async def whois_error(ctx, error):
    if isinstance(error, commands.BadArgument):
        await ctx.send('I could not find that user...')
    if isinstance(error, commands.MissingRequiredArgument):
        await ctx.send(r"Please use \>whois @user")