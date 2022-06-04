import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.editstatus.none"
    },
    {
        "id": 1,
        "translationKey": "enums.editstatus.unedited"
    },
    {
        "id": 2,
        "translationKey": "enums.editstatus.edited"
    }
]

class editstatus implements IBaseEnum {
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

export default new editstatus();
