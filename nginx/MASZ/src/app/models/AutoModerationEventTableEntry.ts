import { AutoModerationEvent } from "./AutoModerationEvent";
import { DiscordUser } from "./DiscordUser";

export interface AutoModerationEventTableEntry {
    autoModerationEvent: AutoModerationEvent;
    suspect?: DiscordUser;
}