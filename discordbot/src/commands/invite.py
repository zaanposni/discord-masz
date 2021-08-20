from .infrastructure import record_usage, CommandDefinition
from helpers import get_prefix


async def _invite(ctx):
    record_usage(ctx)
    await ctx.send(f"You will have to host your own instance of MASZ on your server or pc.\nCheckout https://github.com/zaanposni/discord-masz#hosting")


invite = CommandDefinition(
    func=_invite,
    short_help="How to invite this bot.",
    long_help=f"How to invite this bot."
)
