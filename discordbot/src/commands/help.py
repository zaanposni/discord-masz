from discord.ext import commands

from helpers import get_prefix


output = """```
Commands:
  ban      Ban a user.
  tempban  Ban a user for a defined time.
  features Checks if further configuration is needed to use MASZ features.
  help     Shows this message
  invite   How to invite this bot.
  kick     Kick a user.
  mute     Mute a user.
  tempmute Mute a user for a defined time.
  register Displays the URL to register the current guild.
  url      Displays the URL MASZ is deployed on.
  version  Checks for new releases on GitHub.
  warn     Warn a user.
  whois    Whois information about a user.
  report   Reply to a message to report it to the moderators.
```
"""

complexe_help = {
    "ban": f"Ban a user.\nYou can use the users id, name or tag.\n```\n{get_prefix()}ban @user <reason>\n```Also see: {get_prefix()}help tempban",
    "tempban": f"Ban a user for a defined time.\nYou can use the users id, name or tag.\nUse the following as time range: `1d` `1h` or `1m`\nDefaults to 1 hour.\n```\n{get_prefix()}tempban @user <time range> <reason>\n```Also see: {get_prefix()}help ban",
    "features": f"Checks if further configuration is needed to use MASZ features.\n```\n{get_prefix()}features\n```",
    "help": f"Well...",
    "invite": f"How to invite this bot.\n```\n{get_prefix()}invite\n```",
    "kick": f"Kick a user.\nYou can use the users id, name or tag.\n```\n{get_prefix()}kick @user <reason>\n```",
    "mute": f"Mute a user.\nYou can use the users id, name or tag.\n```\n{get_prefix()}mute @user <reason>\n```Also see: {get_prefix()}help tempmute",
    "tempmute": f"Mute a user for a defined time.\nYou can use the users id, name or tag.\nUse the following as time range: `1d` `1h` or `1m`\nDefaults to 1 hour.\n```\n{get_prefix()}tempmute @user <time range> <reason>\n```Also see: {get_prefix()}help mute",
    "register": f"Displays the URL to register the current guild.\n```\n{get_prefix()}register\n```",
    "url": f"Displays the URL MASZ is deployed on.\n```\n{get_prefix()}url\n```",
    "version": f"Checks for new releases on GitHub.\n```\n{get_prefix()}version\n```",
    "warn": f"Warn a user.\nYou can use the users id, name or tag.\n```\n{get_prefix()}warn @user <reason>\n```",
    "whois": f"Whois information about a user.\nYou can use the users id, name or tag.\n```\n{get_prefix()}whois @user\n```",
    "report": f"Reply to a message to report it to the moderators.\n```\n{get_prefix()}report\n```"
}

@commands.command()
async def help(context, arg=None):
    if arg:
        await context.send(complexe_help.get(arg, "Command not found."))
    else:
        await context.send(output)
