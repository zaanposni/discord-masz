def get_modcase_punishment(modcase) -> str:
    if modcase["PunishmentType"] == 0:
        return "Warn"
    elif modcase["PunishmentType"] == 1:
        if modcase["punishedUntil"]:
            return "TempMute"
        return "Mute"
    elif modcase["PunishmentType"] == 2:
        return "Kick"
    elif modcase["PunishmentType"] == 3:
        if modcase["punishedUntil"]:
            return "TempBan"
        return "Ban"
    else:
        return "Unknown"
