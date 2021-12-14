import json

from deep_translator import GoogleTranslator
from rich.console import Console, NewLine


console = Console()

HANDLE_FILE = "frontend"
NEW_LANGUAGE = "fr"
NEW_SPLITTED_DATA = dict()

with open(HANDLE_FILE + ".json", "r", encoding="utf-8") as fh:
    data = json.load(fh)

def nested_set(dic, keys, value):
    for key in keys[:-1]:
        dic = dic.setdefault(key, {})
    dic[keys[-1]] = value

def generate_frontend_node(node, prevKeys = list()):
    global NEW_SPLITTED_DATA
    if "description" not in node:
        for key, value in node.items():
            generate_frontend_node(value, prevKeys + [key])
    else:
        console.log(f"Generating {'.'.join(prevKeys)}...")
        for lang, translation in node.items():
            if lang != NEW_LANGUAGE:
                nested_set(NEW_SPLITTED_DATA, prevKeys + [lang], translation)
        if NEW_LANGUAGE not in node:  # skip existing translations
            translation =  GoogleTranslator(source='en', target=NEW_LANGUAGE).translate(node["en"])
            t = translation.replace('\n', '\\n')
            nested_set(NEW_SPLITTED_DATA, prevKeys + [NEW_LANGUAGE], t)

for lang in ["fr", "es", "ru", "it"]:
    console.log(f"Translating {lang}...")
    NEW_LANGUAGE = lang
    for key, value in data.items():
        generate_frontend_node(value, [key])
    console.log(f"Generated {lang} translations.")

console.log(f"Saving {HANDLE_FILE}_{NEW_LANGUAGE}.json")
with open(f"{NEW_LANGUAGE}.json", "w", encoding="utf-8") as f:
    json.dump(NEW_SPLITTED_DATA, f, indent=4, ensure_ascii=False)

console.log("Done")
