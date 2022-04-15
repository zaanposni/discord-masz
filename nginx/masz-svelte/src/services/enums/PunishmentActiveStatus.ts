import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.punishmentactivestatus.none"
    },
    {
        "id": 1,
        "translationKey": "enums.punishmentactivestatus.inactive"
    },
    {
        "id": 2,
        "translationKey": "enums.punishmentactivestatus.active"
    }
]

class punishmentactivestatus implements IBaseEnum {
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

export default new punishmentactivestatus();
