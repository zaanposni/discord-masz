# 👮 MASZ

[![https://demomasz.zaanposni.com](https://img.shields.io/badge/demo-online-%234c1)](https://demomasz.zaanposni.com)
[![https://github.com/users/zaanposni/packages/container/package/masz_backend](https://img.shields.io/badge/using-docker-blue)](https://github.com/users/zaanposni/packages/container/package/masz_backend)
![LatestVersion](https://maszindex.zaanposni.com/api/v1/views/version/current/readme)
[![https://discord.gg/5zjpzw6h3S](https://img.shields.io/discord/779262870016884756?logo=discord)](https://discord.gg/5zjpzw6h3S)
[![SupportedLanguages](https://img.shields.io/badge/translated-7%20languages-brightgreen)](https://github.com/zaanposni/discord-masz/blob/master/translations/supported_languages.json)

⭐ **Infractions and managed (temporary) punishments** - to moderate your server\
⭐ **Userscan** - quickly spot relations between users with a included visualization\
⭐ **Quicksearch** - to reliably search for any infractions or notes a user has\
⭐ **Localization** - timezones and languages are fully customizable\
⭐ **Automoderation** - to give trolls no chance\
⭐ **Ban appeals and webhook notifications** - to moderate your server transparently\
⭐ **A website and a discord bot** - to use MASZ\
⭐ **Full API and plugin support** - for custom scripts and automations

### 🚀 Demo 

Visit [https://demomasz.zaanposni.com](https://demomasz.zaanposni.com) for a demo.\
Furthermore, join the demo guild [https://discord.gg/7ubU6aWX9c](https://discord.gg/7ubU6aWX9c) to get the required permissions.

[![https://demomasz.zaanposni.com](https://img.shields.io/badge/demo-online-%234c1?style=for-the-badge)](https://demomasz.zaanposni.com)

### 👀 Preview

![dashboard preview](https://raw.githubusercontent.com/zaanposni/discord-masz/master/docs/dashboard.png)

**Previews and examples can be found at:** [https://github.com/zaanposni/discord-masz/tree/master/docs](https://github.com/zaanposni/discord-masz/tree/master/docs)

### 🤝 Support Server

Join this server to receive update information or get support: [https://discord.gg/5zjpzw6h3S](https://discord.gg/5zjpzw6h3S)

## 🛠 Self Hosting

You can **host your own instance of MASZ**, see below for instructions.\
If you have any questions, feel free to contact Zaanposni, or a support member:

- [Discord server](https://discord.gg/5zjpzw6h3S)
- [Email](mailto:me@zaanposni.com)

Note that MASZ is not hosted publicly. You will have to host your own instance.\
Furthermore, only deployments on a linux and windows server are supported. Read below for software requirements.\
Free hosting providers like replit or heroku **do not work**. You will have to use a VPS instead.

#### TL;DR

- Create a discord application at [https://discord.com/developers/applications](https://discord.com/developers/applications)
- Set redirect urls on your discord application [as defined](https://github.com/zaanposni/discord-masz#discord-oauth).
- Enable **Server Members** and **Message Content Intent** in your bot settings.
- App will be hosted on `127.0.0.1:5565`. Deploy with Docker or on bare metal.

#### If you want to deploy on a domain:

- a (sub)domain to host the application on
- a reverse proxy on your host

### Request logging and ratelimit

MASZ uses the `X-Forwarded-For` http header for logging and ratelimit.\
Ensure that this header is set in your reverse proxy for best experience.

### Discord OAuth

Create your own OAuth application [here](https://discord.com/developers/applications).
Also set the redirect paths in the tab `OAuth2`.\
Be sure to set the following (choose localhost or domain depending on your deployment):

![redirect example](https://raw.githubusercontent.com/zaanposni/discord-masz/master/docs/redirects.png)

### Bot Intents

Enable **Server Members** and **Message Content Intent** in your bot settings.

![intents example](https://raw.githubusercontent.com/zaanposni/discord-masz/master/docs/intents.png)

### Enabling Restricted Features

#### ⭐ Unban request feature

If you want banned users to see their cases, grant your bot the `ban people` permission.\
This way they can see the reason for their ban and comment or send an unban request.\
Furthermore, make sure the bot is high enough in the role hierarchy to ban people below him.

#### ⭐ Punishment feature

If you want the application to execute punishments like mutes and bans and manage them automatically (like unban after defined time on tempban), grant your bot the following permissions based on your needs:

```md
Manage roles - for muted role
Kick people
Ban people
```

Furthermore, make sure the bot is high enough in the role hierarchy to punish people below him.

#### ⭐ Automoderation feature

To avoid any issue for message deletion or read permissions it is recommended to grant your bot a very high and strong or even the `administrator` role.

#### ⭐ Invite tracking

Allows MASZ to track the invites new members are using. Grant your bot the `manage guild` permission to use this feature.

#### ⭐ Strict permission check

You can enable strict permissions in your guildconfig. This mode will check your moderators role permissions before creating a modcase.\
A moderator can only create a kick or ban modcase if he has the respective permission in discord.\
If you do not enable this mode, moderators can create any modcase.

## ✔️ Docker Setup (Recommended)

[![https://github.com/users/zaanposni/packages/container/package/masz_backend](https://img.shields.io/badge/using-docker-blue?style=for-the-badge)](https://github.com/users/zaanposni/packages/container/package/masz_backend)

#### Requirements

- [docker](https://docs.docker.com/engine/install/ubuntu/) & [docker-compose](https://docs.docker.com/compose/) (`docker-compose -v` > 1.25)
- [python3](https://www.python.org/) for setup

#### Instructions

- Download this repository `git clone https://github.com/zaanposni/discord-masz` ([zip link](https://codeload.github.com/zaanposni/discord-masz/zip/master))
- Use `python3 setup.py` (`python setup.py` on windows) to setup the configuration.
- Start the application with `docker-compose up -d`.
- App will be hosted on `127.0.0.1:5565`, if you are hosting the app on a domain, redirect your reverse proxy to this local port!

####  Update

To install a new update of MASZ just use:

```bash
docker-compose pull
docker-compose up -d
```

## 💻 Self Hosting (Development)

### .NET

- If you're using an IDE like Visual Studio, you can copy and paste your ` launchSettingsExample.json` file inside `Properties` if your C# solution to `launchSettings.json`, replacing the values to your testing variables. On running the program, it will treat these as environmental variables.

### Angular

If you want to develop on the angular frontend, set the env var `ENABLE_CORS=true` for the backend container.
To use the angular instance, please change the APP_BASE_URL and ENABLE_CORS values to their development alternatives in the ``config.ts`` file inside the ``src/app/config`` directory
Then use `ng serve` to get a hotload angular instance.

## ↪ After Deployment

### 🐾 First Steps
- You can visit your application at `yourdomain.com` (or `127.0.0.1:5565`). You will see a login screen that will ask you to authenticate yourself using Discord OAuth2.
- After authorizing your service to use your Discord account you will see your profile picture in the toolbar (this is hosted on `127.0.0.1:4200` when developing).
- If you are logged in as a siteadmin, you can use the "register guild" (+) button to register your guilds and to get started.
- Based on wanted features and functionalities you might have to grant your bot advanced permissions, read below for more info.

### 🗃️ Backup

There are backup example scripts in the `scripts` directory to backup uploaded files and the database.

### 🤖 API scripting

As a siteadmin you can create a token to authenticate yourself while making API requests.\
You can also use my [python library](https://github.com/zaanposni/masz-api-wrapper) to integrate the MASZ API into your project.

### 🤝 Contribute

Contributions are welcome.\
You can find our contributions guidelines [here](CONTRIBUTING.md).\
If you are new to open source, checkout [this tutorial](https://github.com/firstcontributions/first-contributions).\
Feel free to get in touch with me via our support server [https://discord.gg/5zjpzw6h3S](https://discord.gg/5zjpzw6h3S).

### 💁🏻 Further Help:

Feel free to join our discord at [https://discord.gg/5zjpzw6h3S](https://discord.gg/5zjpzw6h3S) if you have further questions about your dev environment.
