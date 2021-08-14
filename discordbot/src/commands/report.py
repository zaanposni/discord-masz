import os
import requests

from discord.ext import commands

from .checks import registered_guild_only
from data import get_cached_guild_config
from .record_usage import record_usage


@commands.command(help="Use this while replying to a message to report it to the moderators.")
@registered_guild_only()
@commands.before_invoke(record_usage)
async def report(ctx):
    if ctx.message.reference:
        if ctx.message.reference.message_id:
            guild = await get_cached_guild_config(str(ctx.guild.id))
            if guild:
                if guild["ModInternalNotificationWebhook"]:
                    msg = await ctx.fetch_message(ctx.message.reference.message_id)
                    data = {
                        "content": f"<@{ctx.author.id}> reported a message from <@{msg.author.id}> in <#{ctx.channel.id}>.\n<{msg.jump_url}>\n```\n{msg.content[:1800]}\n```"
                    }
                    r = requests.post(guild["ModInternalNotificationWebhook"], json=data)
                    if r.status_code == 204:
                        await ctx.message.add_reaction("✅")
