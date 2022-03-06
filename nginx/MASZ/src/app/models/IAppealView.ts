import { AppealStatus } from "./AppealStatus";
import { DiscordUser } from "./DiscordUser";
import { IAppealStructure } from "./IAppealStructure";
import { IAppealAnswer } from "./IAppealAnswer";

export interface IAppealView {
    id: number;
    user?: DiscordUser;
    userId: string;
    username: string;
    discriminator: string;
    mail?: string;
    guildId: number;
    status: AppealStatus;
    moderatorComment: string;
    lastModerator?: DiscordUser;
    createdAt: Date;
    updatedAt: Date;
    userCanCreateNewAppeals?: Date;
    invalidDueToLaterRejoinAt?: Date;
    answers: IAppealAnswer[];
    structures: IAppealStructure[];
}
