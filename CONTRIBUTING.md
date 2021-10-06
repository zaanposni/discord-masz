# Contributing

All forms of contributions are **welcome**.<br/>
Feel free to comment existing issues/discussions or create a new one if you have an idea!

- [https://discord.gg/5zjpzw6h3S](https://discord.gg/5zjpzw6h3S)
- Discord: `zaanposni#9295`
- [mail me](mailto:me@zaanposni.com)

## Translations

Contributions to add new translations/languages or correcting existing ones are welcome!<br/>
Tranlating in MASZ is fairly simple and done via i18n (on the website).<br/>
The Backend uses custom generated code but this does not concerns you as a translator.<br/>

There are json files that include translation nodes and look like this:<br/>
![](/docs/translation_example.png)

As you can see there is a `description` that tells you what this translation node is about.<br/>
To add your translation - lets say for french - simply add `'fr': 'Sourdine'`.

Backend translation (used for notifications) are located in the `./translations` directory while translations for the website are located in every subfolder of `./nginx/MASZ/src/app`.<br/>
To quickly find your translation node either search for a known english translation or the existing translation you want to adjust. You can use tools like `Visual Studio Code` to search through the entire codebase.

## How to publish my changes

You have to follow the process of `branch -> commit -> pull request`.<br/>
If you are not sure what that means, check out [this tutorial](https://github.com/firstcontributions/first-contributions). <br/>

## Issues and Discussions

Feel free to open issues and discussions of any type. No matter if support, questions or feature requests!

**Note**: There are maintainer issues that we create to keep track of things that we have to do.<br/>
If any of our issues catches your eye, feel free to comment on it or get in touch with us.

## Developing

Setting up dev environments can be frustrating. Do not hesitate to contact us (see above).

