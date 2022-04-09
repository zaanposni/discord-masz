import hashlib
import shutil

from rich.console import Console


# generate hash based on file content above
def generate_hash(file_content: str):
    return hashlib.sha256(file_content.encode("utf-8")).hexdigest()

console = Console()

with open("public/index.html") as fh:
    index_html = fh.read()

with open("public/global.css") as fh:
    global_css = fh.read()

with open("public/build/bundle.js") as fh:
    main_js = fh.read()

with open("public/build/bundle.js.map") as fh:
    main_js_map = fh.read()

with open("public/build/bundle.css") as fh:
    bundle_css = fh.read()

console.log("Generating hashes...")

LANGUAGES = [
    "en",
    "de",
    "at",
    "es",
    "fr",
    "it",
    "ru"
]
languages_hashes = {}
for lang in LANGUAGES:
    with open(f"public/i18n/{lang}.json") as fh:
        lang_js = fh.read()
    languages_hashes[lang] = generate_hash(lang_js)

global_css_hash = generate_hash(global_css)
main_js_hash = generate_hash(main_js)
main_js_map_hash = generate_hash(main_js_map)
bundle_css_hash = generate_hash(bundle_css)

console.log("Generated hashes:")
console.log(f"global_css: {global_css_hash}")
console.log(f"main_js: {main_js_hash}")
console.log(f"main_js_map: {main_js_map_hash}")
console.log(f"bundle_css: {bundle_css_hash}")
for lang in languages_hashes:
    console.log(f"{lang}.json: {languages_hashes[lang]}")

console.log("Writing hashes to index.html...")

index_html = index_html.replace(
    "global.css", f"global-{global_css_hash}.css"
).replace(
    "bundle.js", f"bundle-{main_js_hash}.js"
).replace(
    "bundle.js.map", f"bundle-{main_js_map_hash}.js.map"
).replace(
    "bundle.css", f"bundle-{bundle_css_hash}.css"
)

with open("public/index.html", "w") as fh:
    fh.write(index_html)

console.log("Writing hashes to bundle.js and bundle.js.map ...")
for lang in languages_hashes:
    main_js = main_js.replace(
        f"/i18n/{lang}.json", f"/i18n/{lang}-{languages_hashes[lang]}.json"
    ).replace(
        "sourceMappingURL=bundle.js.map", f"sourceMappingURL=bundle-{main_js_map_hash}.js.map"
    )

    main_js_map = main_js_map.replace(
        f"/i18n/{lang}.json", f"/i18n/{lang}-{languages_hashes[lang]}.json"
    )

with open("public/build/bundle.js", "w") as fh:
    fh.write(main_js)

with open("public/build/bundle.js.map", "w") as fh:
    fh.write(main_js_map)

console.log("Moving files...")

shutil.move("public/global.css", f"public/global-{global_css_hash}.css")
shutil.move("public/build/bundle.js", f"public/build/bundle-{main_js_hash}.js")
shutil.move("public/build/bundle.js.map", f"public/build/bundle-{main_js_map_hash}.js.map")
shutil.move("public/build/bundle.css", f"public/build/bundle-{bundle_css_hash}.css")
for lang in languages_hashes:
    shutil.move(f"public/i18n/{lang}.json", f"public/i18n/{lang}-{languages_hashes[lang]}.json")

console.log("Moved files.")

console.log("Done!")
