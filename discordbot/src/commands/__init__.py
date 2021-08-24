from .invite import invite
from .version import version
from .features import features
from .whois import whois
from .register import register
from .url import url
from .mute import mute
from .kick import kick
from .ban import ban
from .warn import warn
from .report import report
from .tempmute import tempmute
from .tempban import tempban
from .cases import cases
from .viewg import viewg
from .view import view
from .track import track
from .cleanup import cleanup
from .github import github
from .help import help
from .say import say
from .infrastructure import register_commands, on_command_error


ALL_COMMANDS = [ invite, version, features, whois, register, url, mute, kick, ban, warn, report, tempmute, tempban, cases, viewg, view, track, cleanup, github, say, help ]

register_commands(ALL_COMMANDS)
