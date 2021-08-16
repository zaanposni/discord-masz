import discord
from discord.ext import commands
from discord_slash import SlashCommand

from helpers import get_prefix, console

intents = discord.Intents.default()
intents.members = True

console.info(f"Using prefix '{get_prefix()}'.")
console.info("Registering intents.")
console.info("Deactivating default help command.")

client = commands.Bot(get_prefix() if str(get_prefix()).strip() != "" else "$", intents=intents, help_command=None)  # prefix defaults to $
slash = SlashCommand(client, sync_commands=True)
