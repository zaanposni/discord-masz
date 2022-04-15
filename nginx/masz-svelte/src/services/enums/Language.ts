import type { IBaseEnum, IMASZEnum } from "./IBaseEnum";

const enums: IMASZEnum[] = [
    {
        "id": 0,
        "translationKey": "enums.language.en"
    },
    {
        "id": 1,
        "translationKey": "enums.language.de"
    },
    {
        "id": 2,
        "translationKey": "enums.language.fr"
    },
    {
        "id": 3,
        "translationKey": "enums.language.es"
    },
    {
        "id": 4,
        "translationKey": "enums.language.it"
    },
    {
        "id": 5,
        "translationKey": "enums.language.at"
    },
    {
        "id": 6,
        "translationKey": "enums.language.ru"
    }
]

class language implements IBaseEnum {
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

export default new language();
