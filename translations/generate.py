import json

from rich.console import Console


console = Console()

BACKEND_OUTPUT_PATH = "../backend/masz/Translations/Translation.cs"
FRONTEND_OUTPUT_PATH = "../nginx/MASZ/src/assets/i18n/"

DEFAULT_ENUM_NAMESPACE = "masz.Enums"

TRANSLATION_NODES = 0
TRANSLATION_STATS = dict()

BACKEND_STRING = """using System;\nusing masz.Extensions;\nusing masz.Enums;\n\nnamespace masz.Translations
{
    public class Translation
    {
        public Language preferredLanguage { get; set; }
        private Translation(Language preferredLanguage = Language.en)
        {
            this.preferredLanguage = preferredLanguage;
        }
        public static Translation Ctx(Language preferredLanguage = Language.en) {
            return new Translation(preferredLanguage);
        }
"""

BACKEND_TEMPLATE_END = """
    }
}
"""
with console.status("[bold green]Generating backend...") as status:
    with open("backend.json", "r", encoding="utf-8") as f:
        BACKEND_DATA = json.load(f)

    def generate_backend_node(node, prevKeys = ""):
        global TRANSLATION_NODES
        global TRANSLATION_STATS
        global BACKEND_STRING
        if "description" not in node:
            for key, value in node.items():
                generate_backend_node(value, prevKeys + key)
        else:
            console.log(f"Generating {prevKeys}...")
            TRANSLATION_NODES += 1
            vars = [f"{v} {k}" for k, v in node.get("var_types", dict()).items()]
            insert_interpolation = "$" if vars else ""
            generate = f"\t\tpublic string {prevKeys}({', '.join(vars)}) " + "{\n"
            generate += "\t\t\tswitch (preferredLanguage) {\n"
            for lang, translation in node.items():
                if lang.lower() in ["description", "var_types"]:
                    continue
                t = translation.replace('\n', '\\n')
                TRANSLATION_STATS[lang.lower()] = TRANSLATION_STATS.get(lang.lower(), 0) + 1
                generate += f"\t\t\t\tcase Language.{lang.lower()}:\n"
                generate += f"\t\t\t\t\treturn {insert_interpolation}\"{t}\";\n"
            generate += "\t\t\t}\n"
            t = node['en'].replace('\n', '\\n')
            generate += f"\t\t\treturn {insert_interpolation}\"{t}\";\n"
            generate += "\t\t}\n"
            BACKEND_STRING += generate

    for key, value in BACKEND_DATA.items():
        generate_backend_node(value, key)

    with open("backend_enum.json", "r", encoding="utf-8") as f:
        BACKEND_DATA = json.load(f)

    def generate_backend_enum(node, enum_name):
        global TRANSLATION_NODES
        global TRANSLATION_STATS
        global BACKEND_STRING
        console.log(f"Generating {enum_name}...")
        vars = [f"{v} {k}" for k, v in node.get("var_types", dict()).items()]
        generate = f"\t\tpublic string Enum({DEFAULT_ENUM_NAMESPACE}.{enum_name} enumValue) " + "{\n"
        generate += "\t\t\tswitch (enumValue) {\n"
        for enum_value, translations in node.items():
            TRANSLATION_NODES += 1
            generate += f"\t\t\t\tcase {DEFAULT_ENUM_NAMESPACE}.{enum_name}.{enum_value}:\n"
            generate += "\t\t\t\t\tswitch (preferredLanguage) {\n"
            for lang, translation in translations.items():
                if lang.lower() in ["description", "var_types"]:
                    continue
                t = translation.replace('\n', '\\n')
                TRANSLATION_STATS[lang.lower()] = TRANSLATION_STATS.get(lang.lower(), 0) + 1
                generate += f"\t\t\t\t\t\tcase Language.{lang.lower()}:\n"
                generate += f"\t\t\t\t\t\t\treturn \"{t}\";\n"
            t = translations["en"].replace('\n', '\\n')
            generate += f"\t\t\t\t\t\tdefault:\n"
            generate += f"\t\t\t\t\t\t\treturn \"{t}\";\n"
            generate += "\t\t\t\t\t}\n"
            t = translations['en'].replace('\n', '\\n')
        generate += "\t\t\t}\n"
        generate += f"\t\t\treturn \"Unknown\";\n"
        generate += "\t\t}\n"
        BACKEND_STRING += generate

    for key, value in BACKEND_DATA.items():
        generate_backend_enum(value, key)

    BACKEND_STRING += BACKEND_TEMPLATE_END

    with open(BACKEND_OUTPUT_PATH, "w", encoding="utf-8") as f:
        f.write(BACKEND_STRING.replace("\t", "    "))

console.log("[bright_green]Generated backend[/bright_green].")

def nested_set(dic, keys, value):
    for key in keys[:-1]:
        dic = dic.setdefault(key, {})
    dic[keys[-1]] = value

with console.status("[bold green]Generating frontend...") as status:
    with open("frontend.json", "r", encoding="utf-8") as f:
        FRONTEND_DATA = json.load(f)
    NEW_SPLITTED_DATA = dict()

    def generate_frontend_node(node, prevKeys = list()):
        global TRANSLATION_NODES
        global TRANSLATION_STATS
        global NEW_SPLITTED_DATA
        if "description" not in node:
            for key, value in node.items():
                generate_frontend_node(value, prevKeys + [key])
        else:
            console.log(f"Generating {','.join(prevKeys)}...")
            TRANSLATION_NODES += 1
            for lang, translation in node.items():
                if lang.lower() in ["description"]:
                    continue
                t = translation.replace('\n', '\\n')
                TRANSLATION_STATS[lang.lower()] = TRANSLATION_STATS.get(lang.lower(), 0) + 1
                nested_set(NEW_SPLITTED_DATA, [lang] + prevKeys, t)

    for key, value in FRONTEND_DATA.items():
        generate_frontend_node(value, [key])

    for lang, value in NEW_SPLITTED_DATA.items():
        console.log(f"Saving {lang}.json")
        with open(FRONTEND_OUTPUT_PATH + f"{lang}.json", "w", encoding="utf-8") as f:
            json.dump(value, f, indent=4, ensure_ascii=False)

console.log("[bright_green]Generated frontend[/bright_green].")


with console.status("[bold green]Generating language stats...") as status:
    with open("supported_languages.json", "r", encoding="utf-8") as f:
        SUPPORTED_LANGUAGES = json.load(f)
    for lang, value in SUPPORTED_LANGUAGES.items():
        lang = lang.lower()
        if lang in TRANSLATION_STATS:
            SUPPORTED_LANGUAGES[lang]["supported"] = int(TRANSLATION_STATS[lang] / TRANSLATION_NODES * 100)
        else:
            SUPPORTED_LANGUAGES[lang]["supported"] = 0
    with open("supported_languages.json", "w", encoding="utf-8") as f:
        json.dump(SUPPORTED_LANGUAGES, f, indent=4, ensure_ascii=False)

console.log("[bright_green]Generated language stats[/bright_green].")
