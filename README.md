<h1 align="center">Discord-MASZ</h1>

<p align="center">
<img src="https://img.shields.io/badge/contributions-welcome-lightgreen">
<img src="https://img.shields.io/github/contributors/zaanposni/discord-masz">
<a href="https://github.com/zaanposni/discord-masz/blob/master/LICENSE"><img src="https://img.shields.io/github/license/zaanposni/discord-masz.svg"/></a>
<img src="https://img.shields.io/badge/using-asp.net-blueviolet">
<img src="https://img.shields.io/badge/using-symfony-black">
<img src="https://img.shields.io/badge/using-docker-blue">
<img src="https://img.shields.io/badge/using-nginx-green">
<img src="https://img.shields.io/badge/using-mysql-orange">
</p>

MASZ is a management and moderation overview tool for **Discord Moderators** and **Admins**. <br/>
Keep track of all **moderation events** on your server, **search reliably** for entries and be one step ahead of trolls and rule breakers. <br/>
The core of this tool are the **modcases**, a case represents a rule violation, an important note or similar. <br/>
The server members and your moderators can be **notified** individually about the creation. <br/>
The user for whom the case was created can also see it on the website, take a stand and your server is moderated **transparently**.

# Support and Discussion Server

Join our discord server for support or similar https://discord.gg/5zjpzw6h3S.

# Used by

- [Community Discord "Best of Bundestag"](https://discord.gg/ezMtSwR) 1800 members
- ["Liberale Community"](https://discord.gg/uf9bHhNMmD) 250 members

# Preview

<details>
  <summary>Modcase overview (click to reveal)</summary>
  <img src="/docs/modcases.png"/>
</details>
<details>
  <summary>Detailed view for single modcase (click to reveal)</summary>
  <img src="/docs/modcase.png"/>
</details>
<details>
  <summary>Notification embed for your guild members and moderation team (click to reveal)</summary>
  <img src="/docs/embed.png"/>
</details>

# Setup - Installation

The following guide assumes you want to deploy a production environment.

## Operation System

Since I use Docker you can use an operating system of your choice, but I recommend ubuntu and will list the next steps based on a linux host.

## Requirements 

- [docker](https://docs.docker.com/engine/install/ubuntu/) & [docker-compose](https://docs.docker.com/compose/)
- [jq](https://stedolan.github.io/jq/download/) - a bash tool for json
- a subdomain to host the application on

## Discord OAuth

Requests are authenticated using Discord OAuth2. Create your own OAuth application [here](https://discord.com/developers/applications). <br/>
You will have to use `Client ID` and `Client Secret` in the tab `General Information` and the bot token at `Bot` later in the local config file. <br/>
Also set the redirect paths in the tab `OAuth2`. Be sure to set the following:
```
http://yourdomain.com/
http://yourdomain.com/signin-discord
https://yourdomain.com/
https://yourdomain.com/signin-discord
```

## Setup

- Download this repository `git clone https://github.com/zaanposni/discord-masz` ([zip link](https://codeload.github.com/zaanposni/discord-masz/zip/master))
- Create a `config.json` file in the root of the project based on the template in `default-config.json`
  - `site_admins` is a list of Discord user id strings. This list is used to authorize users to add new guilds to the application or similiar admin tasks.
  - `service_name` should be the name/domain the service is hosted on.
  - `service_domain` is the domain the service is hosted on.
  - `service_base_url` is the URL the service is hosted on.
- Start everything out of the box by running the `bootstrap.sh` script.
- Your application is now hosted at `yourdomain.com`, you might want to redirect your reverse proxy or similiar to this location :)

## First steps:

- You can visit your application at `yourdomain.com`. You will see a login screen that will ask you to authenticate yourself using Discord OAuth2.
- After authorizing your service to use your Discord account you will see your profile picture in the top right corner of the index page.
- If you are logged in as a site admin you can use the "register guild" button to register your guilds and to get started. If you do not see the button please verify that your discord user id is in the `site_admins` list of your `config.json`

## Ban feature:

If you want banned users to see their cases, grant your bot the `ban people` permission. <br/>
This way they can see the reason for their ban and comment or send an unban request.

## Migration

To migrate your existing data from the Dynobot checkout [this documentation](scripts#migrate-from-dynobot-to-masz).

# Development

## Config

Change `nginx_mode` in your config to `local` to deactivate rate limit and set correct headers for local deployment. <br/>
Links in discord are generated using `service_domain` and `service_base_url`. If you want to test those, you have to adjust your config to `127.0.0.1:5565`. <br/>
If you want to develop the frontend using your own symfony server, you can change the default path to the API in `src/Config/Config.php`. <br/>

## Discord

If you are using a local deployed backend you have to define `https://127.0.0.1:port/` and `https://127.0.0.1:port/signin-discord` as valid redirect in your [Discord application settings](https://discord.com/developers/applications).

# Contribute

Contributions are welcome. <br/>
If you are new to open source, checkout [this tutorial](https://github.com/firstcontributions/first-contributions).

