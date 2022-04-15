import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.punishmenttype.warn"
    },
    {
        "id": 1,
        "translationKey": "enums.punishmenttype.mute"
    },
    {
        "id": 2,
        "translationKey": "enums.punishmenttype.kick"
    },
    {
        "id": 3,
        "translationKey": "enums.punishmenttype.ban"
    }
]

class punishmenttype implements IBaseEnum {
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

export default new punishmenttype();
