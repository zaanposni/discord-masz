import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.automoderationchannelnotificationbehavior.sendnotification"
    },
    {
        "id": 1,
        "translationKey": "enums.automoderationchannelnotificationbehavior.sendnotificationanddelete"
    },
    {
        "id": 2,
        "translationKey": "enums.automoderationchannelnotificationbehavior.nonotification"
    }
]

class automoderationchannelnotificationbehavior implements IBaseEnum {
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

export default new automoderationchannelnotificationbehavior();
