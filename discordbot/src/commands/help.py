from discord.ext import commands

output = """```
Commands:
  ban      Ban a user.
  features Checks if further configuration is needed to use MASZ features.
  help     Shows this message
  invite   How to invite this bot.
  kick     Kick a user.
  mute     Mute a user.
  register Displays the URL to register the current guild.
  url      Displays the URL MASZ is deployed on.
  version  Checks for new releases on GitHub.
  warn     Warn a user.
  whois    Whois information about a user.
  report   Reply to a message to report it to the moderators.
```
"""

@commands.command()
async def help(context):
    await context.send(output)
