export interface IBaseEnum {
    getById(id: number): string;
    getAll(): IMASZEnum[];
}
export interface IMASZEnum {
    id: number;
    translationKey: string;
}

