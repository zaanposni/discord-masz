from discord import Message

from data import get_automodevents_by_user_since_minutes

async def check_message(msg: Message, config) -> bool:
    allowed = config["Limit"]
    allowed_since = config["TimeLimitMinutes"]
    if allowed is None or allowed_since is None:
        return False

    events = await get_automodevents_by_user_since_minutes(str(msg.author.id), int(allowed_since))
    if events:
        return len(events) > allowed
    return False
