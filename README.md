<h1 align="center">Discord-MASZ</h1>

<p align="center">
  <img src="https://img.shields.io/badge/contributions-welcome-lightgreen">
  <img src="https://img.shields.io/github/contributors/zaanposni/discord-masz">
  <a href="https://github.com/zaanposni/discord-masz/blob/master/LICENSE"><img src="https://img.shields.io/github/license/zaanposni/discord-masz.svg"/></a>
  <img src="https://img.shields.io/badge/using-docker-blue">
  <a href="https://discord.gg/5zjpzw6h3S">
      <img src="https://img.shields.io/discord/779262870016884756?logo=discord"
          alt="Chat on Discord"></a>
</p>

MASZ is a management and moderation overview tool for **Discord Moderators** and **Admins**. <br/>
Keep track of all **moderation events** and **current punishments** on your server, **search reliably** for entries and be one step ahead of trolls and rule breakers. <br/>
Set up various limits and rules, such as **spam or banned words**, that the app will **automatically moderate**, for example with mutes or warnings. All logs can be checked on the website.<br/>
The core of this tool are the **modcases**, a case represents a rule violation, an important note or similar. <br/>
The server members and your moderators can be **notified** individually about the creation. <br/>
The user for whom the case was created can also see it on the website, take a stand and your server is moderated **transparently**. <br/>
This application can also **manage temporary punishments** just as temp mutes for a variable time you can define.

# Used by

- [Community Discord "Best of Bundestag"](https://discord.gg/ezMtSwR) 1800 members
- ["Liberale Community"](https://discord.gg/uf9bHhNMmD) 250 members

# Warning

This application is only for self deployment. <br/>
You can deploy it on your private computer for testing but if you want others to access the website or use the bot 24/7, you will need a domain, a server and a reverse proxy as well as enough knowledge to set those up, and maintain them.

# Preview

<p>Detailed view for single modcase (click to reveal)</p>
<img src="/docs/modcase.png"/>
<details>
  <summary>Modcase overview (click to reveal)</summary>
  <img src="/docs/modcases.png"/>
</details>
<details>
  <summary>Comments on a modcase(click to reveal)</summary>
  <img src="/docs/modcase-comments.png"/>
</details>
<details>
  <summary>Uploaded files on a modcase(click to reveal)</summary>
  <img src="/docs/modcase-files.png"/>
</details>
<details>
  <summary>Notification embed for your guild members and moderation team (click to reveal)</summary>
  <img src="/docs/embed.png"/>
</details>
<details>
  <summary>Discord Bot commands</summary>
  <img src="/docs/bot-commands.png"/>
</details>
<details>
  <summary>AutoModeration event log</summary>
  <img src="/docs/automoderations.png"/>
</details>
<details>
  <summary>AutoModeration configuration</summary>
  <img src="/docs/automoderationconfig.png"/>
</details>


# Setup - TL;DR;

- Create a discord application at https://discord.com/developers/applications
- Set redirect urls on your discord application [as defined](https://github.com/zaanposni/discord-masz#discord-oauth).
- Execute the `setup.py` script to configure your app and `start.sh` (or `start.ps1` on windows) to start it.
- App will be hosted on `127.0.0.1:5565`, if you are hosting the app on a domain, redirect your reverse proxy to this local port!

# Setup - Installation

## Requirements 

- [docker](https://docs.docker.com/engine/install/ubuntu/) & [docker-compose](https://docs.docker.com/compose/)
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
- Use `python3 start.py` (`python start.py` on windows) to setup the configuration.
- Start the application with `./start.sh` (`start.ps1` on windows).

## First steps

- You can visit your application at `yourdomain.com` (or `127.0.0.1:5565`). You will see a login screen that will ask you to authenticate yourself using Discord OAuth2.
- After authorizing your service to use your Discord account you will see your profile picture in the top right corner of the index page.
- If you are logged in as a site admin you can use the "register guild" button to register your guilds and to get started.
- Based on wanted features and functionalities you might have to grant your bot advanced permissions, read below for more info.
- Checkout the bot commands using the `help` command. Default prefix is $.

## Unban request feature

If you want banned users to see their cases, grant your bot the `ban people` permission. <br/>
This way they can see the reason for their ban and comment or send an unban request.

## Punishment feature

If you want the application to execute punishments like mutes and bans and manage them automatically (like unban after defined time on tempban), grant your bot the following permissions based on your needs:

```
Manage roles - for muted role
Kick people
Ban people
```

## Automoderation feature

To avoid any issue for message deletion or read permissions it is recommended to grant your bot a very high and strong or even the `administrator` role.

## Update

To install a new update of MASZ just use:
```
git pull
./start.sh
```

## Migration

To migrate your existing data from the Dynobot checkout [this documentation](scripts#migrate-from-dynobot-to-masz).

## Backup

There are backup example scripts in the `scripts` directory to backup uploaded files and the database.

# Development

## Config

- Using the `setup.py` script. Choose "local" deployment for best development experience.

## Discord

If you are using a local deployed backend you have to define `https://127.0.0.1:port/` and `https://127.0.0.1:port/signin-discord` as valid redirect in your [Discord application settings](https://discord.com/developers/applications).

# Contribute

Contributions are welcome. <br/>
If you are new to open source, checkout [this tutorial](https://github.com/firstcontributions/first-contributions). <br/>
Feel free to get in touch with me via our support server https://discord.gg/5zjpzw6h3S or via friend request on discord: **zaanposni#9295**.
