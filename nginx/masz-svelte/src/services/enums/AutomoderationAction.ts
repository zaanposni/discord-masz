import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.automoderationaction.none"
    },
    {
        "id": 1,
        "translationKey": "enums.automoderationaction.contentdeleted"
    },
    {
        "id": 2,
        "translationKey": "enums.automoderationaction.casecreated"
    },
    {
        "id": 3,
        "translationKey": "enums.automoderationaction.contentdeletedandcasecreated"
    },
    {
        "id": 4,
        "translationKey": "enums.automoderationaction.timeout"
    }
]

class automoderationaction implements IBaseEnum {
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

export default new automoderationaction();
