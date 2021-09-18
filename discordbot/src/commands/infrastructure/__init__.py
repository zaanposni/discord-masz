from typing import List

from helpers import console
from .checks import guild_only, registered_guild_only, registered_guild_and_admin_or_mod_only, registered_guild_with_muted_role_and_admin_or_mod_only, _is_admin_or_mod
from .command_registration import register_command as _register_command
from .command_registration import on_command_error
from .command_definition import CommandDefinition
from .record_usage import record_usage
from .defer_cmd import defer_cmd
from .help_service import help_service


def register_commands(commands: List[CommandDefinition]):
    with console.status(f"[bold_green]Registering commands...[/bold_green]") as status:
        for command in commands:
            try:
                _register_command(command)
            except KeyError as e:
                console.critical(f"Failed to register {command}, invalid config. {e}")
                continue
    console.info(f"[bold_green]Registered {len(commands)} commands.[/bold_green]")
