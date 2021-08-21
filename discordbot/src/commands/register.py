import os

from .infrastructure import record_usage, CommandDefinition


async def _register(ctx):
    record_usage(ctx)
    if ctx.guild is None:
        await ctx.send(f"A siteadmin can register a guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/guilds/new")
    else:
        await ctx.send(f"A siteadmin can register this guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/guilds/new?guildid={ctx.guild.id}")


register = CommandDefinition(
    func=_register,
    short_help="Displays the URL to register the current guild.",
    long_help=f"Displays the URL to register the current guild."
)
