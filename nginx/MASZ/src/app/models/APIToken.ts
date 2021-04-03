export interface APIToken {
    id: number;
    name: string;
    tokenSalt?: any;
    tokenHash?: any;
    createdAt: Date;
    validUntil: Date;
}