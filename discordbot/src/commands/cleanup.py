from discord import TextChannel, Client
from discord.ext import commands
from discord.errors import Forbidden

from helpers import get_prefix
from .checks import registered_guild_and_admin_or_mod_only

def is_bot(m):
    return m.author.bot

def has_attchment(m):
    return bool(m.attachments)


@commands.command(help="Cleanup specific data from the server and/or channel.")
@registered_guild_and_admin_or_mod_only()
async def cleanup(ctx, mode: str, channel: TextChannel = None, count: int = 100):
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
    if count != 0:
        count += 1  # add one to make sure we don't count the message itself

    try:
        if mode in ["invite", "invites"]:
            await add_loading_reaction(ctx)
            invite_count = 0
            for invite in await ctx.guild.invites():
                invite_count += 1
                await invite.delete()
            return await ctx.send(f"Cleaned up {count} invite(s).")
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
                if message.id == ctx.message.id:
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
    try:
        await ctx.message.add_reaction("👀")
    except Exception:  # maybe the bot has no permissions to add reactions but that should not break this command
        pass    


@cleanup.error
async def cleanup_error(ctx, error):
    if isinstance(error, commands.BadArgument):
        await ctx.send(str(error).replace('"int"', '"number"'))  # log user friendly
    if isinstance(error, commands.MissingRequiredArgument):
        await ctx.send(f"{str(error).capitalize()}\nPlease use `{get_prefix()}cleanup <mode> [channel=current] [count=100]`\nAlso see `{get_prefix()}help cleanup`")
