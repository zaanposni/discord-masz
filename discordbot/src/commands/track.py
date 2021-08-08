from datetime import datetime

from discord.errors import NotFound
from discord.ext import commands
from discord import Embed

from .checks import registered_guild_and_admin_or_mod_only
from data import get_invites_by_guild_and_code


@commands.command(help="Track an invite, its creator and its users.")
@registered_guild_and_admin_or_mod_only()
async def track(ctx, code):
    if "discord" not in code:
        full_code = f"https://discord.gg/{code}"
    else:
        full_code = code

    invites = await get_invites_by_guild_and_code(ctx.guild.id, full_code)
    if not invites:
        return await ctx.send("Invite not found in database.")
    try:
        creator = await ctx.bot.fetch_user(invites[0]["InviteIssuerId"])
    except NotFound:
        creator = None
    
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
        embed.description = f"`{full_code}` was created by {creator.mention} at `{invites[0]['InviteCreatedAt'].strftime('%d %b %Y %H:%M:%S')}`."
    else:        
        embed.description = f"`{full_code}` was created by `{creator.id}` at `{invites[0]['InviteCreatedAt'].strftime('%d %b %Y %H:%M:%S')}`."

    used_by = ""
    for invite in invites:
        if len(used_by) > 900:
            used_by += "[...]"
            break
        if invitees.get(invite['JoinedUserId']):
            used_by += f"- `{invitees[invite['JoinedUserId']].name}#{invitees[invite['JoinedUserId']].discriminator}` `{invite['JoinedUserId']}` - `{invite['JoinedAt'].strftime('%d %b %Y %H:%M:%S')}`\n"
        else:
            used_by += f"- `{invite['JoinedUserId']}` - `{invite['JoinedAt'].strftime('%d %b %Y %H:%M:%S')}`\n"

    embed.add_field(name=f"Used by [{len(invites)}]", value=used_by, inline=False)
    embed.set_footer(text=f"Invite: {full_code}")
    embed.timestamp = datetime.now()

    return await ctx.send(embed=embed)
