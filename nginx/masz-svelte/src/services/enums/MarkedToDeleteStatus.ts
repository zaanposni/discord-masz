import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.markedtodeletestatus.none"
    },
    {
        "id": 1,
        "translationKey": "enums.markedtodeletestatus.unmarked"
    },
    {
        "id": 2,
        "translationKey": "enums.markedtodeletestatus.marked"
    }
]

class markedtodeletestatus implements IBaseEnum {
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

export default new markedtodeletestatus();
