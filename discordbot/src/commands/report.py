import re
import requests

from discord_slash.context import MenuContext
from discord_slash.model import ContextMenuType

from data import get_cached_guild_config
from client import slash
from .infrastructure import registered_guild_only, record_usage, CommandDefinition


async def _report(ctx):
    await registered_guild_only(ctx)
    record_usage(ctx)
    if ctx.message.reference:
        if ctx.message.reference.message_id:
            guild = await get_cached_guild_config(str(ctx.guild.id))
            if guild:
                if guild["ModInternalNotificationWebhook"]:
                    msg = await ctx.fetch_message(ctx.message.reference.message_id)
                    if msg:
                        report_string = f"<@{ctx.author.id}> reported a message from <@{msg.author.id}> in <#{ctx.channel.id}>."
                        if msg.content:
                            report_string += f"\n<{msg.jump_url}>\n```\n{msg.content[:1800]}\n```"
                        if msg.attachments:
                            report_string += f"\nThis message contains {len(msg.attachments)} attachments."
                        data = {
                            "content": report_string
                        }
                        r = requests.post(guild["ModInternalNotificationWebhook"], json=data)
                        if r.status_code == 204:
                            await ctx.message.add_reaction("✅")


report = CommandDefinition(
    func=_report,
    short_help="Report a message to the moderators.",
    long_help="Use this while replying to a message to report it to the moderators.",
    register_slash=False
)


@slash.context_menu(target=ContextMenuType.MESSAGE,
                    name="report")
async def report_menu(ctx: MenuContext):
    await registered_guild_only(ctx.target_message)
    record_usage(ctx)
    if ctx.target_message:
        guild = await get_cached_guild_config(str(ctx.target_message.guild.id))
        if guild:
            if guild["ModInternalNotificationWebhook"]:
                report_string = f"<@{ctx.author_id}> reported a message from <@{ctx.target_message.author.id}> in <#{ctx.target_message.channel.id}>.\n<{ctx.target_message.jump_url}>"
                if ctx.target_message.content:
                    report_string += f"\n```\n{ctx.target_message.content[:1800]}\n```"
                if ctx.target_message.attachments:
                    report_string += f"\nThis message contains {len(ctx.target_message.attachments)} attachments."
                data = {
                    "content": report_string
                }
                r = requests.post(guild["ModInternalNotificationWebhook"], json=data)
                if r.status_code == 204:
                    await ctx.send("Report sent.", hidden=True)
