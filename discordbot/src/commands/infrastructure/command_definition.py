from typing import Callable, List

from helpers import get_prefix


class CommandDefinition:
    """
    A class that defines a command.
    """
    func: Callable = None
    name: str = None
    short_help: str = None
    description: str = None
    long_help: str = None
    usage: str = None
    options: List[dict] = []

    # constructor with default values
    def __init__(self, func: Callable, short_help: str, long_help: str, usage: str = None, options: List[dict] = None, name: str = None, skip_dots: bool = False, register_slash: bool = True):
        """
        Initialize the command definition.
        :param func: The function that implements the command.
        :param short_help: The short help of the command.
        :param long_help: The long help of the command.
        :param usage: The usage of the command.
        :param options: The options of the command.
        :param name: The invoke of the command.
        """
        self.func = func

        self.name = name if name is not None else func.__name__.strip("_")

        if not short_help.endswith(".") and not skip_dots:
            short_help += "."
        self.short_help = short_help
        self.description = short_help
        self.register_slash = register_slash

        if usage is None:
            usage = self.name
        if not usage.startswith(get_prefix()):
            self.usage = get_prefix() + usage
        else:
            self.usage = usage

        if not long_help.endswith(".") and not skip_dots:
            long_help += "."
        self.long_help = long_help + f"\n```\n{self.usage}\n```"
        
        self.options = options or []
