import { AutoModerationEvent } from "./AutoModerationEvent";

export interface AutoModerationEventInfo {
    events: AutoModerationEvent[];
    count: number;
}