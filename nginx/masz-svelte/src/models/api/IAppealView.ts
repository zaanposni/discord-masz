import { AppealStatus } from "./AppealStatus";
import type { IAppealStructure } from "./IAppealStructure";
import type { IAppealAnswer } from "./IAppealAnswer";
import type { IDiscordUser } from "../discord/IDiscordUser";
import type { ICompactCaseView } from "./ICompactCaseView";
import type moment from "moment";

export interface IAppealView {
    id: number;
    user?: IDiscordUser;
    userId: string;
    username: string;
    discriminator: string;
    mail?: string;
    guildId: number;
    status: AppealStatus;
    moderatorComment: string;
    lastModerator?: IDiscordUser;
    createdAt: moment.Moment;
    updatedAt: moment.Moment;
    userCanCreateNewAppeals?: moment.Moment;
    invalidDueToLaterRejoinAt?: moment.Moment;
    answers: IAppealAnswer[];
    structures: IAppealStructure[];
    latestCases: ICompactCaseView[];
}

export function getI18NStatus(appeal: IAppealView) {
    switch(+(appeal?.status ?? 0)) {
        case AppealStatus.Approved:
            return "enums.appealstatus.approved";
        case AppealStatus.Denied:
            return "enums.appealstatus.declined";
        default:
            return "enums.appealstatus.pending";
    };
}
