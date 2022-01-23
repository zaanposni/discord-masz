import json

try:
    from rich import print
    from rich.panel import Panel
    from rich.prompt import Prompt, Confirm
    from rich.table import Table
    from rich.console import Console
except ImportError:
    print("Please install the rich module.\npip install rich")
    exit(1)


print(Panel("\nWelcome to [red]MASZ[/red] :eyes: \nBe sure that you are in the root directory of this project when executing this script.\nSupport discord: https://discord.gg/5zjpzw6h3S\n", title="MASZ"))

with open("./translations/supported_languages.json", "r", encoding="utf-8") as f:
    SUPPORTED_LANGUAGES = json.load(f)

ENV_FILE = {
    "MYSQL_PORT": "3306",
    "MYSQL_USER": "mysqldummy",
    "MYSQL_PASSWORD": "mysqldummy",
    "MYSQL_DATABASE": "masz",
    "MYSQL_ROOT_PASSWORD": "root"
}

local = Prompt.ask(":question_mark: Do you want to deploy on a domain or on localhost as a test version", choices=["domain", "local"], default="domain").lower() == "local"
if local:
    ENV_FILE["DEPLOY_MODE"] = "local"
    ENV_FILE["META_SERVICE_DOMAIN"] = "127.0.0.1:5565"
    ENV_FILE["META_SERVICE_BASE_URL"] = "http://127.0.0.1:5565"
    ENV_FILE["META_SERVICE_NAME"] = "masz"
    service_base_url = "http://127.0.0.1:5565"
else:
    ENV_FILE["DEPLOY_MODE"] = "domain"
    domain = Prompt.ask(":question_mark: Enter your (sub)domain", default="masz.example.com")
    ENV_FILE["META_SERVICE_DOMAIN"] = domain
    ENV_FILE["META_SERVICE_BASE_URL"] = f"https://{domain}"
    ENV_FILE["META_SERVICE_NAME"] = domain
    ENV_FILE["DEPLOY_DOMAIN"] = domain
    service_base_url = f"https://{domain}"
    Confirm.ask(":exclamation_mark: [bright_black]Be sure to redirect your reverse proxy correctly[/bright_black].\n[bright_black]The docker container will be listening on local port [/bright_black][bright_green]5565[bright_green].", default=True)

ENV_FILE["DISCORD_BOT_TOKEN"] = Prompt.ask(":question_mark: Enter your Discord bot token")
ENV_FILE["DISCORD_OAUTH_CLIENT_ID"] = Prompt.ask(":question_mark: Enter your Discord OAuth client ID")
ENV_FILE["DISCORD_OAUTH_CLIENT_SECRET"] = Prompt.ask(":question_mark: Enter your Discord OAuth client secret")

print(":question_mark: Enter the discord id of users that should be site admins.\n[bright_black]It is recommended to be just one. You can enter as many as you want.[/bright_black]")
admins = []
while True:
    site_admin = Prompt.ask(f"{len(admins) + 1}. Admin | Enter '[bright_red]x[/bright_red]' if you are finished")
    if str(site_admin).lower() == "x":
        break
    admins.append(site_admin)

ENV_FILE["DISCORD_SITE_ADMINS"] = ','.join(admins)

ENV_FILE["ENABLE_DEMO_MODE"] = "false"
ENV_FILE["ENABLE_CUSTOM_PLUGINS"] = "false"
ENV_FILE["ENABLE_CORS"] = "false"
ENV_FILE["ENABLE_PUBLIC_FILES"] = "false"

env_string = ""
for key, value in ENV_FILE.items():
    env_string += f"{key}={value}\n"

with open("./.env", "w") as fh:
    fh.write(env_string)

print("\n:+1: [bright_green]You are finished[/bright_green].\n[bright_black]You can execute this script again if you want to change anything.[/bright_black]")
print(f"\n:rocket: [bright_green]You can use \"docker-compose up -d\" to start the application[/bright_green].\n[bright_black]After starting you can access the panel at:[/bright_black] {service_base_url}/\n")
