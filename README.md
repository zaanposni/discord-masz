<h1>👮‍♂️ MASZ</h1>

<p>
  <a href="https://demomasz.zaanposni.com">
      <img src="https://img.shields.io/badge/demo-online-%234c1"
          alt="Online demo">
  </a>
  <img src="https://img.shields.io/badge/using-docker-blue">
  <a href="https://discord.gg/5zjpzw6h3S">
      <img src="https://img.shields.io/discord/779262870016884756?logo=discord"
          alt="Chat on Discord">
  </a>
</p>

⭐ **Infractions and managed (temporary) punishments** - to moderate your server<br/>
⭐ **Userscan** - quickly spot relations between users with a included visualization<br/>
⭐ **Quicksearch** - to reliably search for any infractions or notes a user has<br/>
⭐ **Automoderation** - to give trolls no chance<br/>
⭐ **Ban appeals and webhook notifications** - to moderate your server transparently<br/>
⭐ **A website and a discord bot** - to use MASZ<br/>
⭐ **Full API support** - for custom scripts<br/>

# 🚀 Demo <img src="https://img.shields.io/badge/demo-online-%234c1" alt="Online demo">

Visit [https://demomasz.zaanposni.com](https://demomasz.zaanposni.com) for a demo.<br/>
Furthermore, join the demo guild [https://discord.gg/7ubU6aWX9c](https://discord.gg/7ubU6aWX9c) to get the required permissions.

# 👀 Preview

![](/docs/dashboard.png)
![](/docs/userscan.png)

More previews and examples can be found at [https://github.com/zaanposni/discord-masz/tree/master/docs](https://github.com/zaanposni/discord-masz/tree/master/docs)

# 🤝 Support Server

Join this server to receive update information or get support: https://discord.gg/5zjpzw6h3S

# Hosting

You can **host your own instance of MASZ**, see below for instructions. <br/>
If you have any questions, feel free to contact me. <br/>
- [Discord server](https://discord.gg/5zjpzw6h3S)
- Discord `zaanposni#9295`
- [Mail](mailto:me@zaanposni.com)

# 🛠️ Setup - Selfhosting <img src="https://img.shields.io/badge/using-docker-blue">

## 🚀 TL;DR;

- Create a discord application at https://discord.com/developers/applications
- Set redirect urls on your discord application [as defined](https://github.com/zaanposni/discord-masz#discord-oauth).
- Enable **Server Members Intent** in your bot settings. 
- Execute the `setup.py` script to configure your app and `start.sh` (or `start.ps1` on windows) to start it.
- App will be hosted on `127.0.0.1:5565`, if you are hosting the app on a domain, redirect your reverse proxy to this local port!

## Requirements 

- [docker](https://docs.docker.com/engine/install/ubuntu/) & [docker-compose](https://docs.docker.com/compose/) (`docker-compose -v` > 1.25)
- [python3](https://www.python.org/) for setup

If you want to deploy on a domain:

- a (sub)domain to host the application on
- a reverse proxy on your host

## Discord OAuth

Create your own OAuth application [here](https://discord.com/developers/applications). <br/>
Also set the redirect paths in the tab `OAuth2`. Be sure to set the following (choose localhost or domain depending on your deployment):

<img src="/docs/redirects.png"/>

### Bot Intents

Enable **Server Members Intent** in your bot settings.

<img src="/docs/intents.png"/>

## Setup

- Download this repository `git clone https://github.com/zaanposni/discord-masz` ([zip link](https://codeload.github.com/zaanposni/discord-masz/zip/master))
- Use `python3 setup.py` (`python setup.py` on windows) to setup the configuration.
- Start the application with `./start.sh` (`start.ps1` on windows).
- App will be hosted on `127.0.0.1:5565`, if you are hosting the app on a domain, redirect your reverse proxy to this local port!

## First steps

- You can visit your application at `yourdomain.com` (or `127.0.0.1:5565`). You will see a login screen that will ask you to authenticate yourself using Discord OAuth2.
- After authorizing your service to use your Discord account you will see your profile picture in the toolbar.
- If you are logged in as a siteadmin, you can use the "register guild" (+) button to register your guilds and to get started.
- Based on wanted features and functionalities you might have to grant your bot advanced permissions, read below for more info.
- Checkout the bot commands using the `help` command. Default prefix is $.

## ⭐ Unban request feature

If you want banned users to see their cases, grant your bot the `ban people` permission. <br/>
This way they can see the reason for their ban and comment or send an unban request. <br/>
Furthermore, make sure the bot is high enough in the role hierarchy to ban people below him.

## ⭐ Punishment feature

If you want the application to execute punishments like mutes and bans and manage them automatically (like unban after defined time on tempban), grant your bot the following permissions based on your needs:

```
Manage roles - for muted role
Kick people
Ban people
```

Furthermore, make sure the bot is high enough in the role hierarchy to punish people below him.

## ⭐ Automoderation feature

To avoid any issue for message deletion or read permissions it is recommended to grant your bot a very high and strong or even the `administrator` role.

## ⭐ Invite tracking

Allows MASZ to track the invites new members are using. Grant your bot the `manage guild` permission to use this feature.

## ⭐ Strict permission check

You can enable strict permissions in your guildconfig. This mode will check your moderators role permissions before creating a modcase.
A moderator can only create a kick or ban modcase if he has the respective permission in discord.
If you do not enable this mode, moderators can create any modcase.

## 🛰️ Update

To install a new update of MASZ just use:
```
git pull
./start.sh
```

## 🗃️ Backup

There are backup example scripts in the `scripts` directory to backup uploaded files and the database.

## 🤖 API scripting

As a siteadmin you can create token to authenticate yourself while making API requests.
You can also use my [python library](https://github.com/zaanposni/masz-api-wrapper) to integrate the MASZ API into your project.

# Development

## Config

- Using the `setup.py` script. Choose "local" deployment for best development experience.

## Discord

If you are using a local deployed backend you have to define `https://127.0.0.1:port/` and `https://127.0.0.1:port/signin-discord` as valid redirect in your [Discord application settings](https://discord.com/developers/applications).

## Angular

If you want to develop on the angular frontend, it would be best if you deploy the backend via `./start.sh` and enable cors in the `Startup.cs` then use the `config.ts` to redirect all requests to `127.0.0.1:5565`.

# 🤝 Contribute

Contributions are welcome. <br/>
If you are new to open source, checkout [this tutorial](https://github.com/firstcontributions/first-contributions). <br/>
Feel free to get in touch with me via our support server https://discord.gg/5zjpzw6h3S or via friend request on discord: **zaanposni#9295**.
