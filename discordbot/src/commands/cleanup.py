from discord import TextChannel
from discord.errors import Forbidden
from discord.ext.commands import Context
from discord_slash import SlashContext
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType


from helpers import console
from .infrastructure import record_usage, CommandDefinition, registered_guild_and_admin_or_mod_only


def is_bot(m):
    return m.author.bot


def has_attchment(m):
    return bool(m.attachments)


async def _cleanup(ctx, mode: str, channel: TextChannel = None, count: int = 100):
    await registered_guild_and_admin_or_mod_only(ctx)
    record_usage(ctx)
    mode = mode.lower()
    if ctx.guild is None:
        return
    if channel is None:
        channel = ctx.channel
    if count > 1000:
        count = 1000
        await ctx.send("I can't clean up more than 1000 messages at once. Continuing with 1000...")
    if count < 0:
        return await ctx.send("I can't clean up negative messages...")

    try:
        if mode in ["invite", "invites"]:
            await add_loading_reaction(ctx)
            invite_count = 0
            for invite in await ctx.guild.invites():
                invite_count += 1
                await invite.delete()
            return await ctx.send(f"Cleaned up {invite_count} invite(s).")
        elif mode in ["bot", "bots"]:
            await add_loading_reaction(ctx)
            deleted = await channel.purge(limit=count, check=is_bot)
            return await ctx.send(f"Deleted {len(deleted)} message(s) in {channel.mention}.")
        elif mode in ["attachment", "attachments"]:
            await add_loading_reaction(ctx)
            deleted = await channel.purge(limit=count, check=has_attchment)
            return await ctx.send(f"Deleted {len(deleted)} message(s) in {channel.mention}.")
        elif mode in ["message", "messages"]:
            await add_loading_reaction(ctx)
            deleted = await channel.purge(limit=count)
            return await ctx.send(f"Deleted {len(deleted)} message(s) in {channel.mention}.")
        elif mode in ["reaction", "reactions"]:
            await add_loading_reaction(ctx)
            message_count = 0
            reaction_count = 0
            async for message in channel.history(limit=count):
                if ctx.message and message.id == ctx.message.id:
                    continue  # do not clean current message with loading reaction
                if message.reactions:
                    message_count += 1
                    reaction_count += len(message.reactions)
                    await message.clear_reactions()
            return await ctx.send(f"Deleted {reaction_count} reaction(s) in {message_count} message(s) in {channel.mention}.")
        else:
            return await ctx.send("Invalid mode. Please use `invites`, `bots`, `attachments`, `messages`, or `reactions`.")
    except Forbidden:
        return await ctx.send("I don't have the `manage_messages` or `read_message_history` permission.")


async def add_loading_reaction(ctx):
    if isinstance(ctx, SlashContext):
        try:
            await ctx.defer()
        except Exception as e:  # will only work in slash context
            console.error("Failed to defer slash cleanup command: {e}")
    if isinstance(ctx, Context):
        try:
            await ctx.message.add_reaction("👀")
        except Exception as e:  # maybe the bot has no permissions to add reactions but that should not break this command
            console.error("Failed to add reaction to cleanup command: {e}")


cleanup = CommandDefinition(
    func=_cleanup,
    short_help="Cleanup specific data from the server and/or channel.",
    long_help="Cleanup specific data from the server and/or channel.\nValid modes:\n```\nattachments - delete all messages with files\nbot - delete all messages sent by a bot\ninvites - delete all invites of the current guild\nmessages - delete all messages\nreactions - delete all reactions to messages\n```",
    usage="cleanup <mode> [channel=current] [count=100]",
    options=[
        create_option("mode", "which data you want to delete.", SlashCommandOptionType.STRING, True),
        create_option("channel", "where to delete, defaults to current.", SlashCommandOptionType.CHANNEL, False),
        create_option("count", "how many messages to scan for your mode.", SlashCommandOptionType.INTEGER, False),
    ],
    skip_dots=True
)

