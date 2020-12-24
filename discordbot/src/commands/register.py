import os

from discord.ext import commands


@commands.command(help="Displays the URL to register the current guild.")
async def register(ctx):
    if ctx.guild is None:
        await ctx.send(f"A siteadmin can register a guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/newguild")
    else:
        await ctx.send(f"A siteadmin can register this guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/newguild?guildid={ctx.guild.id}")
