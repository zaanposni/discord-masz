from discord.ext import commands

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
    "ban": "Ban a user.\nYou can use the users id, name or tag.\n```\n$ban @user <reason>\n```\nAlso see: $help tempban",
    "tempban": "Ban a user for a defined time.\nYou can use the users id, name or tag.\nUse the following as time range: `1d` `1h` or `1m`\n```\n$ban @user <time range> <reason>\n```\nAlso see: $help ban",
    "features": "Checks if further configuration is needed to use MASZ features.\n```\n$features\n```",
    "help": "Well...",
    "invite": "How to invite this bot.\n```\n$invite\n```",
    "kick": "Kick a user.\nYou can use the users id, name or tag.\n```\n$kick @user <reason>\n```",
    "mute": "Mute a user.\nYou can use the users id, name or tag.\n```\n$mute @user <reason>\n```\nAlso see: $help tempmute",
    "register": "Displays the URL to register the current guild.\n```\n$register\n```",
    "url": "Displays the URL MASZ is deployed on.\n```\n$url\n```",
    "version": "Checks for new releases on GitHub.\n```\n$version\n```",
    "warn": "Warn a user.\nYou can use the users id, name or tag.\n```\n$warn @user <reason>\n```",
    "whois": "Whois information about a user.\nYou can use the users id, name or tag.\n```\n$whois @user\n```",
    "report": "Reply to a message to report it to the moderators.\n```\n$report\n```"
}

@commands.command()
async def help(context, arg=None):
    print(arg)
    if arg:
        await context.send(complexe_help.get(arg, "Command not found."))
    else:
        await context.send(output)
