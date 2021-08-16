from helpers import console

def record_usage(ctx):
    if ctx.guild:
        console.info(f"{ctx.author} used {ctx.command} in {ctx.guild}|#{ctx.channel}")
    else:
        console.info(f"{ctx.author} used {ctx.command} in DM")
