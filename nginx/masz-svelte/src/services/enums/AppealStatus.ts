import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.appealstatus.pending"
    },
    {
        "id": 1,
        "translationKey": "enums.appealstatus.approved"
    },
    {
        "id": 2,
        "translationKey": "enums.appealstatus.declined"
    }
]

class appealstatus implements IBaseEnum {
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

export default new appealstatus();
