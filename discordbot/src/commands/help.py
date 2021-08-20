from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

from .infrastructure import record_usage, CommandDefinition, help_service


complexe_help = {
    "duration": f"Use the following as duration: `1d` `1h` or `1m`.\nCombine them for a more detailed time range:\n- `1d12h30m` means 1 day, 12 hours, 30 minutes",
}


async def _help(ctx, cmd=None):
    record_usage(ctx)
    if cmd:
        cmd_details = help_service.get_command(cmd)
        if cmd_details:
            await ctx.send(cmd_details.long_help)
        else:
            await ctx.send(complexe_help.get(cmd, "Command not found."))  # default to complexe help dictionary for bonus help pages that are not registered commands
    else:
        await ctx.send(help_service.get_help_page())


help = CommandDefinition(
    func=_help,
    short_help="Displays manual pages for different commands.",
    long_help="Displays manual pages for different commands.",
    usage="help",
    options=[
        create_option("cmd", "Command.", SlashCommandOptionType.STRING, False)
    ]
)
