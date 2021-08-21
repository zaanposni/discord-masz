from discord import TextChannel
from discord.ext.commands import Context
from .infrastructure import record_usage, CommandDefinition, registered_guild_and_admin_or_mod_only
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

async def _say(ctx, channel: TextChannel = None, *,message):
    await registered_guild_and_admin_or_mod_only(ctx)
    record_usage(ctx)
    if message is None:
        await ctx.send("Please write a message.")
        return
    if Channel is None:
        channel = ctx.channel
    slash = isinstance(ctx, SlashContext)
    if slash:
        try:
            await ctx.message.add_reaction("👀")
        except Exception as e:
            console.error("Failed to add reaction to say command: {e}")
    try:
        await channel.send(message)
        success = True
    except Exception as f:
        success = False
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
