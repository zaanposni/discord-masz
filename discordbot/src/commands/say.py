#third-party imports
from discord import TextChannel
from discord.ext.commands import Context
from discord_slash import SlashContext
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

# integrated imports
from helpers import console
from .infrastructure import record_usage, CommandDefinition, registered_guild_and_admin_or_mod_only

# procedure when the say command is triggered
async def _say(ctx, channel: TextChannel = None, *,message):
    #check permissions
    await registered_guild_and_admin_or_mod_only(ctx)
    # record the usage
    record_usage(ctx)
    # check if no message is specified. If, cancel command procedure and output error
    if message is None:
        await ctx.send("Please write a message.",hidden=True)
        return
    # If no channel is specified, use the current channel. Technically not required as the channel is required
    if channel is None:
        channel = ctx.channel
    #check whether command is triggered by slash command or message. required for feedback 
    slash = isinstance(ctx, SlashContext)
    # try to send the message the user wants to. Save whether this was successful
    try:
        await channel.send(message)
        success = True
    except Exception as f:
        success = False
    # If the message has been sent, tell that to the user with a reaction(command message) or a message(slash command). This includes an error handler if the confirmation couldn't be sent too
    if success:
        if slash:
            try:
                await ctx.send("Message sent", hidden=True)
            except Exception as e:
                console.error("confirmation message couldn't be sent. The command may have worked anyway.: {e}")
        else:
            try:
                await ctx.message.add_reaction("✅")
            except Exception as e:
                console.error("Failed to add reaction to say command: {e}")
    # and do the same if the message couldn't been sent
    else:
        if slash:
            try:
                await ctx.send("Message couldn't be sent. Maybe 'Send message'-Permission is not given to this bot for the demanded channel?", hidden=True)
            except Exception as e:
                console.error("confirmation message couldn't be sent. Sending the required message also failed: {e}")
        else:
            try:
                await ctx.message.add_reaction("❌")
            except Exception as e:
                console.error("Failed to add reaction to say command: {e}")


# register the command for slash commands
say = CommandDefinition(
    func= _say,
    short_help = "Let the bot send a message",
    long_help="This command lets the mod write a message. The first Argument is the channel. \n The second Argument is the message you want the bot to send. This can be any message you can write in discord.\n This command requires Admin or Moderator privileges.",
    usage="say <channel> <message>",
    options = [
        create_option("channel","channel to write the message in", SlashCommandOptionType.CHANNEL, True),
        create_option("message","message content the bott shall write", SlashCommandOptionType.STRING, True)
    ]
)
