import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.apierror.unknown"
    },
    {
        "id": 1,
        "translationKey": "enums.apierror.invaliddiscorduser"
    },
    {
        "id": 2,
        "translationKey": "enums.apierror.protectedmodcasesuspect"
    },
    {
        "id": 3,
        "translationKey": "enums.apierror.protectedmodcasesuspectisbot"
    },
    {
        "id": 4,
        "translationKey": "enums.apierror.protectedmodcasesuspectissiteadmin"
    },
    {
        "id": 5,
        "translationKey": "enums.apierror.protectedmodcasesuspectisteam"
    },
    {
        "id": 6,
        "translationKey": "enums.apierror.resourcenotfound"
    },
    {
        "id": 7,
        "translationKey": "enums.apierror.invalididentity"
    },
    {
        "id": 8,
        "translationKey": "enums.apierror.guildunregistered"
    },
    {
        "id": 9,
        "translationKey": "enums.apierror.unauthorized"
    },
    {
        "id": 10,
        "translationKey": "enums.apierror.guildundefinedmutedroles"
    },
    {
        "id": 11,
        "translationKey": "enums.apierror.modcaseismarkedtobedeleted"
    },
    {
        "id": 12,
        "translationKey": "enums.apierror.modcaseisnotmarkedtobedeleted"
    },
    {
        "id": 13,
        "translationKey": "enums.apierror.guildalreadyregistered"
    },
    {
        "id": 14,
        "translationKey": "enums.apierror.notallowedindemomode"
    },
    {
        "id": 15,
        "translationKey": "enums.apierror.rolenotfound"
    },
    {
        "id": 16,
        "translationKey": "enums.apierror.tokencannotmanagethisresource"
    },
    {
        "id": 17,
        "translationKey": "enums.apierror.tokenalreadyregistered"
    },
    {
        "id": 18,
        "translationKey": "enums.apierror.cannotbesameuser"
    },
    {
        "id": 19,
        "translationKey": "enums.apierror.resourcealreadyexists"
    },
    {
        "id": 20,
        "translationKey": "enums.apierror.modcasedoesnotallowcomments"
    },
    {
        "id": 21,
        "translationKey": "enums.apierror.lastcommentalreadyfromsuspect"
    },
    {
        "id": 22,
        "translationKey": "enums.apierror.invalidautomoderationaction"
    },
    {
        "id": 23,
        "translationKey": "enums.apierror.invalidautomoderationtype"
    },
    {
        "id": 24,
        "translationKey": "enums.apierror.toomanytemplates"
    },
    {
        "id": 25,
        "translationKey": "enums.apierror.invalidfilepath"
    },
    {
        "id": 26,
        "translationKey": "enums.apierror.noguildsregistered"
    },
    {
        "id": 27,
        "translationKey": "enums.apierror.onlyusableinaguild"
    },
    {
        "id": 28,
        "translationKey": "enums.apierror.invalidauditlogevent"
    },
    {
        "id": 29,
        "translationKey": "enums.apierror.protectedscheduledmessage"
    },
    {
        "id": 30,
        "translationKey": "enums.apierror.invaliddateforscheduledmessage"
    }
]

class apierror implements IBaseEnum {
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

export default new apierror();
