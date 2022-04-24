import type moment from "moment";
import type { AutomodAction } from "./AutomodActionEnum";
import type { AutomodType } from "./AutomodType";

export interface IAutomodEntry {
    id: number;
    guildId: string;
    autoModerationType: AutomodType;
    autoModerationAction: AutomodAction;
    userId: string;
    username: string;
    nickname?: any;
    discriminator: string;
    messageId: string;
    messageContent: string;
    createdAt: moment.Moment;
    associatedCaseId?: number;
}
