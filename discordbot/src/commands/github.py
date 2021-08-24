from .infrastructure import record_usage, CommandDefinition


async def _github(ctx):
    record_usage(ctx)
    await ctx.send(f"https://github.com/zaanposni/discord-masz")


github = CommandDefinition(
    func=_github,
    short_help="Displays the GitHub repository URL.",
    long_help="Displays the GitHub repository URL. Feature requests and contributions are welcome!"
)
