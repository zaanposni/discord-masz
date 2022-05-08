import type { IDiscordChannel } from "../discord/IDiscordChannel";
import type { IDiscordUser } from "../discord/IDiscordUser";
import type { ScheduledMessageFailureReason } from "./ScheduledMessageFailureReason";
import { ScheduledMessageStatus } from "./ScheduledMessageStatus";
import type * as moment from "moment";

export interface IScheduledMessage {
    id: number;
    name: string;
    content: string;
    scheduledFor: moment.Moment;
    status: ScheduledMessageStatus;
    guildId: string;
    channelId: string;
    creatorId: string;
    lastEditedById: string;
    createdAt: moment.Moment;
    lastEditedAt: moment.Moment;
    failureReason?: ScheduledMessageFailureReason;
    creator: IDiscordUser;
    lastEdited: IDiscordUser;
    channel: IDiscordChannel;
}


export function getI18NMessageStatus(message: IScheduledMessage) {
    switch(message.status) {
        case ScheduledMessageStatus.Sent:
            return "enums.scheduledmessagestatus.sent";
        case ScheduledMessageStatus.Failed:
            return "enums.scheduledmessagestatus.failed";
        default:
            return "enums.scheduledmessagestatus.pending";
    };
}
