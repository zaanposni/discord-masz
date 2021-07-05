from discord import Message

def check_message(msg: Message, config) -> bool:
    allowed = config["Limit"]
    if allowed is None:
        return False

    custom_words = config["CustomWordFilter"]
    if custom_words is None:
        return False
    custom_words = custom_words.split("\n")
    print(custom_words)

    matches = 0;
    content = msg.content.lower()
    print(content)
    for custom_word in custom_words:
        if custom_word.strip().lower() in content:
            matches += 1
        if matches > allowed:
            break

    return matches > allowed
