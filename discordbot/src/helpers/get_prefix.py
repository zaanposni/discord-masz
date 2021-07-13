import os

CACHED = None

def get_prefix(default_value: str = "$") -> str:
    global CACHED
    if CACHED:
        return CACHED
    CACHED = os.getenv("BOT_PREFIX", default_value)
    return CACHED
