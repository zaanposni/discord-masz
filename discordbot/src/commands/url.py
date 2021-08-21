import os

from .infrastructure import record_usage, CommandDefinition


async def _url(ctx):
    record_usage(ctx)
    await ctx.send(f"MASZ is deployed on {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")


url = CommandDefinition(
    func=_url,
    short_help="Displays the URL MASZ is deployed on.",
    long_help="Displays the URL MASZ is deployed on.",
)
