from discord.ext import commands

from helpers import get_prefix


output = """```
Commands:
  ban      Ban a user.
  tempban  Ban a user for a defined duration.
  features Checks if further configuration is needed to use MASZ features.
  help     Shows this message
  invite   How to invite this bot.
  kick     Kick a user.
  mute     Mute a user.
  tempmute Mute a user for a defined duration.
  register Displays the URL to register the current guild.
  url      Displays the URL MASZ is deployed on.
  version  Checks for new releases on GitHub.
  warn     Warn a user.
  whois    Whois information about a user.
  report   Reply to a message to report it to the moderators.
  cases    See a list of your modcases.
  view     View details of a modcase in the current guild.
  viewg    View details of a modcase given another guild id.
```
"""

complexe_help = {
    "ban": f"Ban a user.\n```\n{get_prefix()}ban <username|userid|usermention> <reason>\n```",
    "tempban": f"Ban a user for a defined duration.\n```\n{get_prefix()}tempban <username|userid|usermention> <duration> <reason>\n```Also see: `{get_prefix()}help duration`",
    "features": f"Checks if further configuration is needed to use MASZ features.\n```\n{get_prefix()}features\n```",
    "help": f"Well...",
    "invite": f"How to invite this bot.\n```\n{get_prefix()}invite\n```",
    "kick": f"Kick a user.\n```\n{get_prefix()}kick <username|userid|usermention> <reason>\n```",
    "mute": f"Mute a user.\n```\n{get_prefix()}mute <username|userid|usermention> <reason>\n```",
    "tempmute": f"Mute a user for a defined duration.\n```\n{get_prefix()}tempmute <username|userid|usermention> <duration> <reason>\n```Also see: `{get_prefix()}help duration`",
    "register": f"Displays the URL to register the current guild.\n```\n{get_prefix()}register\n```",
    "url": f"Displays the URL MASZ is deployed on.\n```\n{get_prefix()}url\n```",
    "version": f"Checks for new releases on GitHub.\n```\n{get_prefix()}version\n```",
    "warn": f"Warn a user.\n```\n{get_prefix()}warn <username|userid|usermention> <reason>\n```",
    "whois": f"Whois information about a user.\n```\n{get_prefix()}whois <username|userid|usermention>\n```",
    "report": f"Reply to a message to report it to the moderators.\n```\n{get_prefix()}report\n```",
    "cases": f"See a list of your modcases.\nOptionally filter by guild id.\n```\n{get_prefix()}cases [guild_id]\n```",
    "duration": f"Use the following as duration: `1d` `1h` or `1m`.\nCombine them for a more detailed time range:\n- `1d12h30m` means 1 day, 12 hours, 30 minutes",
    "view": f"View details of a modcase in the current guild.\n```\n{get_prefix()}view <caseid>\n```",
    "viewg": f"View details of a modcase given another guild id.\n```\n{get_prefix()}viewg <guildid> <caseid>\n```"
}

@commands.command()
async def help(context, arg=None):
    if arg:
        await context.send(complexe_help.get(arg, "Command not found."))
    else:
        await context.send(output)
