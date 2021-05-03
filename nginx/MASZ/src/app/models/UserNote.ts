export interface UserNote {
    id: number;
    guildId: string;
    userId: string;
    description: string;
    creatorId: string;
    updatedAt: Date;
}