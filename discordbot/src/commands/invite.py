import os

from discord.ext import commands

from .record_usage import record_usage

@commands.command(help="How to invite this bot.")
@commands.before_invoke(record_usage)
async def invite(ctx):
    await ctx.send(f"You will have to host your own instance of this bot on your server or pc.\nCheckout https://github.com/zaanposni/discord-masz#hosting")
