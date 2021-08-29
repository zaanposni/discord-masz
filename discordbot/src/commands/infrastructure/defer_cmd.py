from typing import Union

from discord.ext.commands.context import Context
from discord_slash.context import SlashContext

from helpers import console


async def defer_cmd(ctx: Union[SlashContext, Context]):
    if isinstance(ctx, SlashContext):
        try:
            await ctx.defer()
        except Exception as e:
            console.error(f"Failed to defer slash cleanup command: {e}")
