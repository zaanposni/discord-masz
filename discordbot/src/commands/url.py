import os

from discord.ext import commands

from .record_usage import record_usage


@commands.command(help="Displays the URL MASZ is deployed on.")
@commands.before_invoke(record_usage)
async def url(ctx):
    await ctx.send(f"MASZ is deployed on: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")
