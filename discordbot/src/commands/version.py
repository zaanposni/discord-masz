import requests

from .infrastructure import record_usage, CommandDefinition
from helpers import get_prefix


async def _version(ctx):
    record_usage(ctx)
    try:
        r = requests.get(f"http://masz_nginx:80/static/version.json")
    except:
        await ctx.send(f"Failed to fetch deployed version info.")
        return
    if r.status_code != 200:
        ctx.send(f"Failed to fetch deployed version info. Statuscode: {r.status_code}.")
        return

    deployed_version = r.json()["version"]
    pre_release = r.json()["pre_release"]

    r = requests.get("https://api.github.com/repos/zaanposni/discord-masz/releases")
    if r.status_code != 200:
        await ctx.send(f"Failed to fetch release info from github. Statuscode: {r.status_code}.")
        return

    newest_release = r.json()[0]
    if str(newest_release["tag_name"]).lower() != str(deployed_version).lower():
        if pre_release:
            await ctx.send(f"Your deployed version **{deployed_version}** seems to be a pre release.\nThe newest stable release is **{newest_release['tag_name']}**.")
        else:
            await ctx.send(
                f"There seems to be a newer version **{newest_release['tag_name']}**. You are on **{deployed_version}**.\nContact your site admin to install the update." +
                "\n\nPatch Notes for the newest version:\n```\n" +
                newest_release["body"] + "\n```"
            )
    else:
        await ctx.send(f"Your deployed version **{deployed_version}** is up to date with releases on GitHub.")


version = CommandDefinition(
    func=_version,
    short_help="Checks for new releases on GitHub.",
    long_help=f"Checks for new releases on GitHub.",
)
