import re

from discord import Message

INVITE_REGEX = re.compile(r"(https?:\/\/)?(www\.)?(discord(app)?\.(gg|io|me|li|com)(\/invite)?)\/(?!.+\/.+).+[a-z]")

def check_message(msg: Message) -> bool:
    return INVITE_REGEX.search(msg.content)
