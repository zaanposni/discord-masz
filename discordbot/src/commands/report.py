import requests

from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

from data import get_cached_guild_config
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
                    data = {
                        "content": f"<@{ctx.author.id}> reported a message from <@{msg.author.id}> in <#{ctx.channel.id}>.\n<{msg.jump_url}>\n```\n{msg.content[:1800]}\n```"
                    }
                    r = requests.post(guild["ModInternalNotificationWebhook"], json=data)
                    if r.status_code == 204:
                        await ctx.message.add_reaction("✅")


report = CommandDefinition(
    func=_report,
    short_help="Report a message to the moderators.",
    long_help="Use this while replying to a message to report it to the moderators."
)

