import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.scheduledmessagefailurereason.unknown"
    },
    {
        "id": 1,
        "translationKey": "enums.scheduledmessagefailurereason.channelnotfound"
    },
    {
        "id": 2,
        "translationKey": "enums.scheduledmessagefailurereason.insufficientpermission"
    }
]

class scheduledmessagefailurereason implements IBaseEnum {
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

export default new scheduledmessagefailurereason();
