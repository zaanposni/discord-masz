import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.viewpermission.global"
    },
    {
        "id": 1,
        "translationKey": "enums.viewpermission.guild"
    },
    {
        "id": 2,
        "translationKey": "enums.viewpermission.self"
    }
]

class viewpermission implements IBaseEnum {
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

export default new viewpermission();
