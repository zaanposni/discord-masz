import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.guildauditlogevent.messagesent"
    },
    {
        "id": 1,
        "translationKey": "enums.guildauditlogevent.messageupdated"
    },
    {
        "id": 2,
        "translationKey": "enums.guildauditlogevent.messagedeleted"
    },
    {
        "id": 3,
        "translationKey": "enums.guildauditlogevent.usernameupdated"
    },
    {
        "id": 4,
        "translationKey": "enums.guildauditlogevent.avatarupdated"
    },
    {
        "id": 5,
        "translationKey": "enums.guildauditlogevent.nicknameupdated"
    },
    {
        "id": 6,
        "translationKey": "enums.guildauditlogevent.memberrolesupdated"
    },
    {
        "id": 7,
        "translationKey": "enums.guildauditlogevent.memberjoined"
    },
    {
        "id": 8,
        "translationKey": "enums.guildauditlogevent.memberremoved"
    },
    {
        "id": 9,
        "translationKey": "enums.guildauditlogevent.banadded"
    },
    {
        "id": 10,
        "translationKey": "enums.guildauditlogevent.banremoved"
    },
    {
        "id": 11,
        "translationKey": "enums.guildauditlogevent.invitecreated"
    },
    {
        "id": 12,
        "translationKey": "enums.guildauditlogevent.invitedeleted"
    },
    {
        "id": 13,
        "translationKey": "enums.guildauditlogevent.threadcreated"
    },
    {
        "id": 14,
        "translationKey": "enums.guildauditlogevent.voicejoined"
    },
    {
        "id": 15,
        "translationKey": "enums.guildauditlogevent.voiceleft"
    },
    {
        "id": 16,
        "translationKey": "enums.guildauditlogevent.voicemoved"
    },
    {
        "id": 17,
        "translationKey": "enums.guildauditlogevent.reactionadded"
    },
    {
        "id": 18,
        "translationKey": "enums.guildauditlogevent.reactionremoved"
    }
]

class guildauditlogevent implements IBaseEnum {
    getById(id: number): string {
        const enumItem = enums.find(e => e.id === id);
        if (enumItem) {
            return enumItem.translationKey;
        }
        return "unknown";
    }

    getAll(): IMASZEnum[] {
        return enums;
    }
}

export default new guildauditlogevent();
