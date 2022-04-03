export interface IUserMapping {
    id: number;
    guildId: string;
    userA: string;
    userB: string;
    creatorUserId: string;
    createdAt: Date;
    reason: string;
}