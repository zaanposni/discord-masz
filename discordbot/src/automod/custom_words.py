from discord import Message

def check_message(msg: Message, config) -> bool:
    allowed = config["Limit"]
    if allowed is None:
        return False

    custom_words = config["CustomWordFilter"]
    if custom_words is None:
        return False
    custom_words = custom_words.split("\n")

    matches = 0;
    content = msg.content.lower()
    for custom_word in custom_words:
        matches += content.count(custom_word.strip())
        if matches > allowed:
            break

    return matches > allowed
