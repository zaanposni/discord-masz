import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.scheduledmessagestatus.pending"
    },
    {
        "id": 1,
        "translationKey": "enums.scheduledmessagestatus.sent"
    },
    {
        "id": 2,
        "translationKey": "enums.scheduledmessagestatus.failed"
    }
]

class scheduledmessagestatus implements IBaseEnum {
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

export default new scheduledmessagestatus();
