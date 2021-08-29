from time import time

from discord import Message


msg_board = {}  # the msg_board saves the timestamps of all newer messages sorted by guild and user.

def check_message(msg: Message, config):
    # for saving the "msg_board" globally. I'm aware that this is bad style, but i found no other solution
    global msg_board

    # read config
    allowed = config["Limit"]
    allowed_since = config["TimeLimitMinutes"]  # for this automod it is handled in seconds
    if allowed is None or allowed_since is None:
        return False

    # set guild config in msg_board if it doesn't exist
    if msg.guild.id not in msg_board:
        msg_board[msg.guild.id] = {}

    ts = time()  # current timestamp

    # filter out messages older thaan TimeLimitMinutes
    delts = ts - allowed_since  # delts is the time minus the TimeLimitMinutes => the time messages older than should be deleted
    for user in list(msg_board[msg.guild.id].keys()):
        new = [j for j in msg_board[msg.guild.id][user] if j > delts]
        if new:
            msg_board[msg.guild.id][user] = new
        else:
            del msg_board[msg.guild.id][user]

    # add the message to the "msg_board"
    if msg.author.id not in msg_board[msg.guild.id]:
        msg_board[msg.guild.id][msg.author.id] = [ts]
    else:
        msg_board[msg.guild.id][msg.author.id].append(ts)

    # count the number of messages and check them for being too high
    return len(msg_board[msg.guild.id][msg.author.id]) > allowed
