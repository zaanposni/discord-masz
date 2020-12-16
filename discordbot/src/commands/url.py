import os

from discord.ext import commands


@commands.command(help="Displays the URL MASZ is deployed on.")
async def url(ctx):
    await ctx.send(f"MASZ is deployed on: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")
