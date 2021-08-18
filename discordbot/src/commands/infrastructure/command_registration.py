from discord.ext.commands import Command, MissingRequiredArgument, BadArgument

from client import slash, client

from helpers import console, get_prefix
from .help_service import help_service
from .command_definition import CommandDefinition


async def on_command_error(ctx, error):
    if isinstance(error, MissingRequiredArgument):
        error_msg = "Missing argument(s)!"
    elif isinstance(error, BadArgument):
        error_msg = "Invalid argument(s)!"
    else:
        error_msg = None
    if error_msg:
        cmd = help_service.get_command(str(ctx.command))
        if cmd is not None:
            await ctx.send(f"{error_msg}\nPlease use: `{cmd.usage}`")
        else:
            await ctx.send(f"{error_msg}\nPlease refer to: `{get_prefix()}help {str(ctx.command)}`.")

def register_command(command: CommandDefinition):
    cmd = Command(
            command.func,
            name=command.name,
            help=command.long_help,
            brief=command.short_help
        )
    cmd.on_error = on_command_error
    console.info(f"Registering command '{cmd.name}'.")
    help_service.register_command(command)
    client.add_command(cmd)
    slash.add_slash_command(
        cmd=command.func,
        name=command.name,
        description=command.description,
        options=command.options
    )
