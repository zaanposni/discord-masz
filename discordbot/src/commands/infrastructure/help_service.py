from typing import List, Optional

from .command_definition import CommandDefinition


class HelpService():
    _registered_commands: List[CommandDefinition] = []

    def register_command(self, command: CommandDefinition):
        self._registered_commands.append(command)
    
    def get_command(self, command_name: str) -> Optional[CommandDefinition]:
        for command in self._registered_commands:
            if command.name == command_name:
                return command
        return None

    def get_help_page(self) -> str:
        help_page = "```\n"
        for command in self._registered_commands:
            help_page += f"{command.name} - {command.short_help}\n"
        help_page += "\n```"
        return help_page

help_service = HelpService()
