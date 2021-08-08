#imports
from discord import Message
#from sched import scheduler
from time import time#, sleep
#from os import system

# global definitions
# Scheduler reset is not yet implemented
#s = scheduler(time, sleep) # just in case data doesn't get deleted as it should, this timer resets the board after a week so data will be removed.
msgboard = [] # the msgboard saves the timestamps of all newer messages sorted by player. Messages older than "TimeLimitMinutes" should not be saved.


#s.enter(604800, 1, system, arguments={"reboot now"}) not yet iplemented

def check_message(msg: Message, config):
    #for saving the "msgboard" globally. I'm aware that this is bad style, but i found no other solution
    global msgboard
    # read config
    allowed = config["Limit"]
    allowedSince = config["TimeLimitMinutes"]
    if allowed is None or allowedSince is None:
        return False
    ts = time()
    #filter out messages older thaan TimeLimitMinutes
    delts = ts - allowedSince * 60 # delts is the time minus the TimeLimitMinutes aka the time messages older than should be deleted
    temprm = [] # the list of names that should be removed because of no messages saved
    for i in range(len(msgboard)):
        filtertemp = [j for j in msgboard[i][1] if j > delts] # filtertemp only contains messages newer than delts
        if filtertemp == []:
            temprm.insert(0,i) # add empty user message list to the deletion candidates. Added in first place to "turn the list around"
        msgboard[i][1] = filtertemp
    for i in temprm:
        #remove all empty items
        msgboard.pop(i)
    
    # add the message to the "msgboard", count the number of messages and check them for being too high
    tempmsgcount = 0
    for i in range(len(msgboard)):
        #check whether the board already contains the author of the message
        if msgboard[i][0] == msg.author:
            msgboard[i][1].append(ts)
            return len(msgboard[i][1]) > allowed
    else:
        #and else let it contain it
        msgboard.append([msg.author,[ts]])
        return 1 > allowed
    
