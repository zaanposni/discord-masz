from discord.ext.commands import Command
from discord_slash.client import SlashCommand

from client import slash, client
from helpers import console


def register_commands(commands):
    with console.status(f"[bold_green]Registering commands...[/bold_green]") as status:
        for command in commands:
            try:
                cmd = Command(
                        command["func"],
                        name=command.get("name", command["func"].__name__),
                        help=command.get("description"),
                        brief=command.get("description"),
                        usage=command.get("usage"),
                    )
            except KeyError as e:
                console.critical(f"Failed to register {command}, invalid config. {e}")
                continue            

            console.info(f"Registering command '{cmd.name}'.")
            client.add_command(cmd)
            slash.add_slash_command(
                cmd=command["func"],
                name=command.get("name", command["func"].__name__),
                description=command.get("description"),
                options=command.get("options"),
            )

    console.info(f"[bold_green]Registered {len(commands)} commands.[/bold_green]")