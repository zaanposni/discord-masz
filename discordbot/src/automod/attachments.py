from discord import Message


def check_message(msg: Message, config) -> bool:
    allowed = config["Limit"]
    if allowed is None:
        return False

    return len(msg.attachments) > allowed
