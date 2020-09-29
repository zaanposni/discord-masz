# Discord-MASZ

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

## Preview

placeholder

## Setup - Installation

### Operation System

Since I use Docker you can use an operating system of your choice, but I recommend ubuntu and will list the next steps based on a linux host.

### Requirements 

- [docker](https://docs.docker.com/engine/install/ubuntu/) & [docker-compose](https://docs.docker.com/compose/)
- [jq](https://stedolan.github.io/jq/download/) - a bash tool for json

### Setup

- Clone this repository
- Create a `config.json` file in the root of the project based on the template in `default-config.json`
- Start everything out of the box by running the `bootstrap.sh` script.
- Your application is now hosted at `127.0.0.1:5565`, you might want to redirect your reverse proxy or similiar to this location :)

## Contribute

Contributions are welcome. <br/>
If you are new to open source, checkout [this tutorial](https://github.com/firstcontributions/first-contributions).

