# Contributing

All forms of contributions are **welcome**.\
Feel free to comment existing issues/discussions or create a new one if you have an idea!

- [https://discord.gg/5zjpzw6h3S](https://discord.gg/5zjpzw6h3S)
- [mail me](mailto:me@zaanposni.com)

## Translations

Contributions to add new translations/languages or correcting existing ones are welcome!\
Tranlating in MASZ is fairly simple and done via i18n (on the website).\
The backend uses custom generated code but this does not concerns you as a translator.

There are json files that include translation nodes and look like this:\
![example](/docs/translation_example.png)

As you can see there is a `description` that tells you what this translation node is about.\
To add your translation - lets say for french - simply add `'fr': 'Sourdine'`.

There are three translation files, these are located in the `./translations` directory.\
To quickly find your translation node either search for a known english translation or the existing translation you want to adjust.\
You can use tools like `Visual Studio Code` to search through these files.

If you want to adjust notifications, commands or DM messages -> `backend.json` or `backend_enum.json`.\
If you want to adjust texts or notifications on the website -> `frontend.json`.

To migrate your changes from these "base" files into the "compiled" ones, you have to execute the script `generate.py`.\
If you are not sure how to do this, no worries, just skip this step!

## How to publish my changes

You have to follow the process of `branch -> commit -> pull request`.\
If you are not sure what that means, check out [this tutorial](https://github.com/firstcontributions/first-contributions).

## Issues and Discussions

Feel free to open issues and discussions of any type. No matter if support, questions or feature requests!

**Note**: There are maintainer issues that we create to keep track of things that we have to do.\
If any of our issues catches your eye, feel free to comment on it or get in touch with us.

## Developing

Setting up dev environments can be frustrating. Do not hesitate to contact us (see above).
