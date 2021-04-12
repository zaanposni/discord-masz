from discord import Message

from data import get_automodevents_by_user_since_minutes

def check_message(msg: Message, config) -> bool:
    allowed = config["Limit"]
    allowedSince = config["TimeLimitMinutes"]
    if allowed is None or allowedSince is None:
        return False

    events = get_automodevents_by_user_since_minutes(str(msg.author.id), int(allowedSince))
    if events:
        return len(events) > allowed
    return False
