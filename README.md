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

MASZ is a moderation tool from **discord moderators** for **discord moderators**.<br/>
Use **automoderation**, **temporary punishments**, **moderation events**, **webhooks**, **fileuploads** or **the discord bot** to keep track of anything that happens on your server.<br/>
On **the website** all members can login and view moderation events registered on their name and also comment on it.<br/>

# Used by

- [Community Discord "Best of Bundestag"](https://discord.gg/ezMtSwR) 1800 members
- ["Liberale Community"](https://discord.gg/uf9bHhNMmD) 250 members
- ["Advertise Your Server"](https://discord.gg/promote) 52000 members

# Preview

| Guild overview  | Single moderation case  |
| --------------- | ----------------------- |
![](/docs/modcases.png)  |  ![](/docs/modcase.png)


More previews and examples can be found at [https://github.com/zaanposni/discord-masz/tree/master/docs](https://github.com/zaanposni/discord-masz/tree/master/docs)

# Hosting

You can **host your own instance of MASZ for free**, see below for instructions. <br/>
If you are not experienced enough to do so or just want to use MASZ right now, feel free to contact me. <br/>
I will host an instance for you for a small fee. <br/>
- Discord `zaanposni#9295`
- Mail `me@zaanposni.com`

# Setup - Selfhosting <img src="https://img.shields.io/badge/using-docker-blue">

## TL;DR;

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
- After authorizing your service to use your Discord account you will see your profile picture in the top right corner of the index page.
- If you are logged in as a site admin you can use the "register guild" button to register your guilds and to get started.
- Based on wanted features and functionalities you might have to grant your bot advanced permissions, read below for more info.
- Checkout the bot commands using the `help` command. Default prefix is $.

## Unban request feature

If you want banned users to see their cases, grant your bot the `ban people` permission. <br/>
This way they can see the reason for their ban and comment or send an unban request. <br/>
Furthermore, make sure the bot is high enough in the role hierarchy to ban people below him.

## Punishment feature

If you want the application to execute punishments like mutes and bans and manage them automatically (like unban after defined time on tempban), grant your bot the following permissions based on your needs:

```
Manage roles - for muted role
Kick people
Ban people
```

Furthermore, make sure the bot is high enough in the role hierarchy to punish people below him.

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

## Angular

If you want to develop on the angular frontend, it would be best if you deploy the backend via `./start.sh` and enable cors in the `Startup.cs` then use the `config.ts` to redirect all requests to `127.0.0.1:5565`.

# Contribute

Contributions are welcome. <br/>
If you are new to open source, checkout [this tutorial](https://github.com/firstcontributions/first-contributions). <br/>
Feel free to get in touch with me via our support server https://discord.gg/5zjpzw6h3S or via friend request on discord: **zaanposni#9295**.
