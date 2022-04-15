import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.casecreationtype.default"
    },
    {
        "id": 1,
        "translationKey": "enums.casecreationtype.automoderation"
    },
    {
        "id": 2,
        "translationKey": "enums.casecreationtype.imported"
    },
    {
        "id": 3,
        "translationKey": "enums.casecreationtype.bycommand"
    }
]

class casecreationtype implements IBaseEnum {
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

export default new casecreationtype();
