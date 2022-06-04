import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.automoderationtype.inviteposted"
    },
    {
        "id": 1,
        "translationKey": "enums.automoderationtype.toomanyemotes"
    },
    {
        "id": 2,
        "translationKey": "enums.automoderationtype.toomanymentions"
    },
    {
        "id": 3,
        "translationKey": "enums.automoderationtype.toomanyattachments"
    },
    {
        "id": 4,
        "translationKey": "enums.automoderationtype.toomanyembeds"
    },
    {
        "id": 5,
        "translationKey": "enums.automoderationtype.toomanyautomoderations"
    },
    {
        "id": 6,
        "translationKey": "enums.automoderationtype.customwordfilter"
    },
    {
        "id": 7,
        "translationKey": "enums.automoderationtype.toomanymessages"
    },
    {
        "id": 8,
        "translationKey": "enums.automoderationtype.toomanyduplicatedcharacters"
    },
    {
        "id": 9,
        "translationKey": "enums.automoderationtype.toomanylinks"
    }
]

class automoderationtype implements IBaseEnum {
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

export default new automoderationtype();
