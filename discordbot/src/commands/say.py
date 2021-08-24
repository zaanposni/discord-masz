from discord import TextChannel
from discord.errors import Forbidden
from discord_slash import SlashContext
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

from helpers import console
from .infrastructure import record_usage, CommandDefinition, registered_guild_and_admin_or_mod_only

async def _say(ctx, channel: TextChannel = None, *, message):
    await registered_guild_and_admin_or_mod_only(ctx)
    record_usage(ctx)

    if message is None:
        return await ctx.send("Please write a message.", hidden=True)

    if channel is None:
        channel = ctx.channel

    try:
        await channel.send(message)
        success = True
    except Forbidden:
        if isinstance(ctx, SlashContext):
            return await ctx.send("I don't have the `send_message` or `view_channel` permission.", hidden=True)
        else:  # discord.py mimimi doesn't support extra kwargs
            return await ctx.send("I don't have the `send_message` or `view_channel` permission.")
    except Exception as e:
        console.critical(f"Failed to send say message: {e}")
        success = False

    if success:
        if isinstance(ctx, SlashContext):
            try:
                await ctx.send("Message sent", hidden=True)
            except Exception as e:
                console.critical(f"Confirmation message couldn't be sent. The command may have worked anyway: {e}")
        else:
            try:
                await ctx.message.add_reaction("✅")
            except Exception as e:
                console.critical(f"Failed to add reaction to say command: {e}")
    else:
        try:
            if isinstance(ctx, SlashContext):
                await ctx.send("Failed to send message.", hidden=True)
            else:  # discord.py mimimi doesn't support extra kwargs
                await ctx.send("Failed to send message.")
        except Exception as e:
            console.critical(f"Confirmation message couldn't be sent: {e}")


say = CommandDefinition(
    func= _say,
    short_help = "Let the bot send a message",
    long_help="This command lets the mod write a message in a defined channel. Caution: This command can mention everyone!",
    usage="say <channel> <message>",
    options = [
        create_option("message", "message content the bot shall write", SlashCommandOptionType.STRING, True),
        create_option("channel", "channel to write the message in", SlashCommandOptionType.CHANNEL, False)
    ]
)
