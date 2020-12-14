import os.path
import json

BASE_PATH = "./.config.json"


class Config:
    def __init__(self):
        self.options = {}
        self.reload()

    def reload(self):
        print(f"Reloading configfile: {os.path.join(BASE_PATH)}")
        with open(BASE_PATH, "r") as fh:
            self.options = json.load(fh)

    def get(self, key: str, default=None):
        return self.options.get(key, default)

cfg = Config()
