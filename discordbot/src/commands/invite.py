import os

from discord.ext import commands


@commands.command(help="How to invite this bot.")
async def invite(ctx):
    await ctx.send(f"You will have to host your own instance of this bot on your server or pc.\nCheckout https://github.com/zaanposni/discord-masz#setup---tldr")
