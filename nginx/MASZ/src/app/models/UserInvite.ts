export interface UserInvite {
    id: number;
    guildId: string;
    targetChannelId?: string;
    joinedUserId: string;
    usedInvite: string;
    inviteIssuerId: string;
    joinedAt: Date;
    inviteCreatedAt: Date;
}