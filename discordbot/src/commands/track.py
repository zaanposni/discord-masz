from datetime import datetime

from discord.errors import HTTPException, NotFound
from discord import Embed
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType
from discord_slash import SlashContext

from data import get_invites_by_guild_and_code
from .infrastructure import record_usage, CommandDefinition, registered_guild_and_admin_or_mod_only
from helpers import console


async def _track(ctx, code):
    await registered_guild_and_admin_or_mod_only(ctx)
    record_usage(ctx)
    if "discord" not in code:
        full_code = f"https://discord.gg/{code}"
    else:
        full_code = code

    if isinstance(ctx, SlashContext):
        try:
            await ctx.defer()
        except Exception as e:  # will only work in slash context
            console.error("Failed to defer slash track command: {e}")

    invites = await get_invites_by_guild_and_code(ctx.guild.id, full_code)
    if invites:
        created_at = invites[0]['InviteCreatedAt'].strftime('%d %b %Y %H:%M:%S') if invites[0]['InviteCreatedAt'] else None
        usages = len(invites)
        try:
            creator = await ctx.bot.fetch_user(invites[0]["InviteIssuerId"])
        except NotFound:
            creator = None
    else:
        try:
            invite = await ctx.bot.fetch_invite(full_code)
        except NotFound:
            return await ctx.send(f"Could not find invite in database or in this guild.")
        except HTTPException:
            return await ctx.send("Failed to fetch invite.")
        if invite.guild.id != ctx.guild.id:
            return await ctx.send(f"Invite is not in this guild.")
        creator = invite.inviter
        created_at = invite.created_at.strftime('%d %b %Y %H:%M:%S') if invite.created_at else None
        usages = invite.uses if invite.uses else 0

    invitees = {}
    count = 0  # only do this for the first 20 users
    for invite in invites:
        if count > 20:
            break
        if invite["JoinedUserId"] not in invitees:
            count += 1
            invitees[invite["JoinedUserId"]] = await ctx.bot.fetch_user(invite["JoinedUserId"])

    embed = Embed()
    if creator:
        embed.set_author(name=f"{creator.name}#{creator.discriminator}", icon_url=creator.avatar_url, url=creator.avatar_url)

    if created_at:
        embed.description = f"`{full_code}` was created by <@{creator.id}> at `{created_at} (UTC)`."
    else:
        embed.description = f"`{full_code}` was created by <@{creator.id}>."

    used_by = ""
    for invite in invites:
        if len(used_by) > 900:
            used_by += "[...]"
            break
        if invitees.get(invite['JoinedUserId']):
            used_by += f"- `{invitees[invite['JoinedUserId']].name}#{invitees[invite['JoinedUserId']].discriminator}` `{invite['JoinedUserId']}` - `{invite['JoinedAt'].strftime('%d %b %Y %H:%M:%S')} (UTC)`\n"
        else:
            used_by += f"- `{invite['JoinedUserId']}` - `{invite['JoinedAt'].strftime('%d %b %Y %H:%M:%S')} (UTC)`\n"
    if not invites:
        used_by = "This invite has not been tracked by MASZ yet."

    embed.add_field(name=f"Used by [{usages}]", value=used_by, inline=False)
    embed.set_footer(text=f"Invite: {full_code}")
    embed.timestamp = datetime.now()

    return await ctx.send(embed=embed)


track = CommandDefinition(
    func=_track,
    short_help="Track an invite, its creator and its users.",
    long_help="Track an invite in your guild, its creator and its users.\nEither enter the invite code or the url in the format `https://discord.gg/<code>`.",
    usage="track <code|url>",
    options=[
        create_option("code", "the invite code or link.", SlashCommandOptionType.STRING, True)
    ]
)
