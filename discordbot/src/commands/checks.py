import os

from discord.ext import commands
from discord.ext.commands.errors import CheckFailure

from data import get_cached_guild_config
from helpers import console

site_admins = os.getenv("DISCORD_SITE_ADMINS").strip(",")


async def _is_admin_or_mod(ctx) -> bool:
    try:
        cfg = await get_cached_guild_config(str(ctx.guild.id))
    except Exception as e:
        console.critical("Failed to load guildconfig: {e}")
        return False
    
    is_site_admin = str(ctx.author.id) in site_admins
    is_admin_or_mod = any([role for role in ctx.author.roles if str(role.id) in cfg["ModRoles"].split(",") or str(role.id) in cfg["AdminRoles"].split(",")])
    return is_site_admin or is_admin_or_mod


async def _is_registered_guild(ctx) -> bool:
    if ctx.guild is None:
        return False
    
    try:
        cfg = await get_cached_guild_config(str(ctx.guild.id))
    except Exception as e:
        console.critical("Failed to load guildconfig: {e}")
        return False

    return cfg is not None


async def guild_only(ctx):
    if ctx.guild is None:
        await ctx.send("Only useable in a guild.")
        raise CheckFailure("Only useable in a guild.")


async def registered_guild_only(ctx):
    if ctx.guild is None:
        await ctx.send("Only useable in a guild.")
        raise CheckFailure("Only useable in a guild.")

    if not await _is_registered_guild(ctx):
        if os.getenv('META_SERVICE_BASE_URL', ''):
            await ctx.send(f"MASZ is not registered on this guild.\nA siteadmin can register this guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/guilds/new?guildid={ctx.guild.id}")
        else:
            await ctx.send(f"MASZ is not registered on this guild.")
        raise CheckFailure(f"MASZ is not registered on this guild.")


async def registered_guild_and_admin_or_mod_only(ctx):
    if ctx.guild is None:
        await ctx.send("Only useable in a guild.")
        raise CheckFailure(f"Only useable in a guild.")

    if not await _is_registered_guild(ctx):
        if os.getenv('META_SERVICE_BASE_URL', ''):
            await ctx.send(f"MASZ is not registered on this guild.\nA siteadmin can register this guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/guilds/new?guildid={ctx.guild.id}")
        else:
            await ctx.send(f"MASZ is not registered on this guild.")
        raise CheckFailure(f"MASZ is not registered on this guild.")

    if not await _is_admin_or_mod(ctx):
        await ctx.send("You do not have the permission to use this command.")
        raise CheckFailure(f"You do not have the permission to use this command.")


async def registered_guild_with_muted_role_and_admin_or_mod_only(ctx):
    if ctx.guild is None:
        await ctx.send("Only useable in a guild.")
        raise CheckFailure("Only useable in a guild.")

    if not await _is_registered_guild(ctx):
        if os.getenv('META_SERVICE_BASE_URL', ''):
            await ctx.send(f"MASZ is not registered on this guild.\nA siteadmin can register this guild at: {os.getenv('META_SERVICE_BASE_URL', '')}/guilds/new?guildid={ctx.guild.id}")
        else:
            await ctx.send(f"MASZ is not registered on this guild.")
        raise CheckFailure("MASZ is not registered on this guild.")

    if not await _is_admin_or_mod(ctx):
        await ctx.send("You do not have the permission to use this command.")
        raise CheckFailure("You do not have the permission to use this command.")
    
    cfg = await get_cached_guild_config(ctx.guild.id)
    if not cfg["MutedRoles"]:
        await ctx.send("Muted Roles are not defined.")
        raise CheckFailure("Muted Roles are not defined.")

    for role in cfg["MutedRoles"].split(","):
        muted_role = ctx.guild.get_role(int(role))
        if not (muted_role is not None and ctx.me.top_role > muted_role):
            await ctx.send("Muted Role is invalid or too high in role hierarchy.")
            raise CheckFailure("Muted Role is invalid or too high in role hierarchy.")
