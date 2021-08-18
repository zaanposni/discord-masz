from discord import User
from discord_slash.utils.manage_commands import create_option, SlashCommandOptionType

from helpers import create_whois_embed, get_prefix
from .infrastructure import record_usage, registered_guild_and_admin_or_mod_only, CommandDefinition


async def _whois(ctx, user: User):
    await registered_guild_and_admin_or_mod_only(ctx)
    record_usage(ctx)
    embed = await create_whois_embed(ctx.guild, user)
    if embed:
        await ctx.send(embed=embed)


whois = CommandDefinition(
    func=_whois,
    short_help="Whois information about a user.",
    long_help=f"Whois information about a user.",
    usage=f"{get_prefix()}whois <username|userid|usermention>",
    options=[
        create_option("user", "User to scan.", SlashCommandOptionType.USER, True)
    ]
)
