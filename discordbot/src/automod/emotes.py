import re

from discord import Message
import emoji

CUSTOM_EMOTE_REGEX = re.compile(r'<a?:\w*:\d*>')
EMOTE_REGEX = emoji.get_emoji_regexp()

def check_message(msg: Message, config) -> bool:
    allowed = config["Limit"]
    if allowed is None:
        return False
        
    custom_emotes = CUSTOM_EMOTE_REGEX.findall(msg.content)
    if len(custom_emotes) > allowed:  # skip normal emote check if possible
        return True
    
    emotes = EMOTE_REGEX.findall(msg.content)
    return len(custom_emotes) + len(emotes) > allowed
