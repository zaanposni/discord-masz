import { AutoModerationType } from "./AutoModerationType";

export interface AutoModerationEvent {
    id: number;
    guildId: string;
    autoModerationType: AutoModerationType;
    autoModerationAction: number;
    userId: string;
    username: string;
    nickname?: string;
    discriminator: string;
    messageId: string;
    messageContent: string;
    createdAt: Date;
    associatedCaseId?: number;
}