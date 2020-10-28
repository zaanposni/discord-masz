<h1 align="center">Discord-MASZ</h1>
test
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

## Operation System

Since I use Docker you can use an operating system of your choice, but I recommend ubuntu and will list the next steps based on a linux host.

## Requirements 

- [docker](https://docs.docker.com/engine/install/ubuntu/) & [docker-compose](https://docs.docker.com/compose/)
- [jq](https://stedolan.github.io/jq/download/) - a bash tool for json

## Discord OAuth

Requests are authenticated using Discord OAuth2. Create your own OAuth application [here](https://discord.com/developers/applications). <br/>
You will have to use `Client ID` and `Client Secret` in the tab `General Information` and the bot token at `Bot` later in the local config file. <br/>
Also set the redirect paths in the tab `OAuth2`. Be sure to set `https://yourdomain.com/` and `https://yourdomain.com/signin-discord`. <br/>
If you only want to try out the project on your pc, you can also use `http://127.0.0.1:5565/` and `http://127.0.0.1:5565/signin-discord`.

**Important:** Invite and join the bot of your registered application to all Discord servers that you want to use. <br/>
If you want to use the `banned` feature (banned users can still see the guild and their cases so they know what lead to a ban), give the bot the `ban people` permission, otherwise the bot does not need any further permissions.

## Setup

- Download this repository `git clone https://github.com/zaanposni/discord-masz` ([zip link](https://codeload.github.com/zaanposni/discord-masz/zip/master))
- Create a `config.json` file in the root of the project based on the template in `default-config.json`
  - `site_admins` is a list of Discord user id strings. This list is used to authorize users to add new guilds to the application or similiar admin tasks.
  - `service_name` should be the name/domain the service is hosted on.
  - `service_base_url` is the URL the service is hosted on.
  - `nginx_mode` can be either `local` or `prod` and describes if you are hosting a development or production environment.
- Start everything out of the box by running the `bootstrap.sh` script.
- Your application is now hosted at `127.0.0.1:5565`, you might want to redirect your reverse proxy or similiar to this location :)

## First steps:

- You can visit your application at `127.0.0.1:5565`. You will see a login screen that will ask you to authenticate yourself using Discord OAuth2.
- After authorizing your service to use your Discord account you will see your profile picture in the top right corner of the index page.
- Check your browser cookies for the access token and execute the following request to register your discord guild in the backend: <br/>
  `modPublicNotificationWebhook` and `modInternalNotificationWebhook` are used to send notification embeds to your moderation team and guild members - you can leave them empty. <br/>
  Be sure to be logged in as a site_admin user defined in your `config.json` as only those are authorized to register new guilds
```bash
curl --location --request POST '127.0.0.1:5565/api/v1/configs/<guildid>' \
--header 'Content-Type: application/json' \
--header 'Cookie: masz_access_token=<your_cookie>' \
--data-raw '{
    "modroleid": "id",
    "adminroleid": "id",
    "modPublicNotificationWebhook": "url_to_discord_webhook",
    "modInternalNotificationWebhook": "url_to_discord_webhook"
}'
```

- After creating the guild config object you can refresh the browser page and use MASZ :)

## Migration

To migrate your existing data from the Dynobot checkout [this documentation](scripts#migrate-from-dynobot-to-masz)

# Development

If you want to develop the frontend using your own symfony server, you can change the default path to the API in `src/Config/Config.php`. <br/>
If you are using a local deployed backend you have to define `https://127.0.0.1:port/` and `https://127.0.0.1:port/signin-discord` as valid redirect in your [Discord application settings](https://discord.com/developers/applications)

# Contribute

Contributions are welcome. <br/>
If you are new to open source, checkout [this tutorial](https://github.com/firstcontributions/first-contributions).

