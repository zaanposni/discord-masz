import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.lockedcommentstatus.none"
    },
    {
        "id": 1,
        "translationKey": "enums.lockedcommentstatus.locked"
    },
    {
        "id": 2,
        "translationKey": "enums.lockedcommentstatus.unlocked"
    }
]

class lockedcommentstatus implements IBaseEnum {
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

export default new lockedcommentstatus();
