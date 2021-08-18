import os

from .infrastructure import record_usage


async def _url(ctx):
    record_usage(ctx)
    await ctx.send(f"MASZ is deployed on: {os.getenv('META_SERVICE_BASE_URL', 'URL not set.')}")


url = {
    "func": _url,
    "description": "Displays the URL MASZ is deployed on."
}
