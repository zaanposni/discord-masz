import os
import requests
from datetime import datetime

from discord import Member, Embed

from data import get_cached_guild_config, get_modcases_by_user_and_guild_with_active_mute


async def handle_member_join(member: Member):
    guildconfig = await get_cached_guild_config(str(member.guild.id))
    if not guildconfig:  # guild not registered or no config
        return
    
    if not guildconfig["MutedRoleId"]:
        return
    
    muted_role = member.guild.get_role(int(guildconfig["MutedRoleId"]))
    if muted_role:
        cases = await get_modcases_by_user_and_guild_with_active_mute(str(member.guild.id), str(member.id))
        if cases: 
            print(f"Reapplying muted role to {member} on guild {member.guild}.")
            await member.add_roles(muted_role)
