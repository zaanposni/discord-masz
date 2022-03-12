export interface IGuildAuditLogRuleDefinition {
    type: number;
    key: string;
    comingSoon?: boolean;
    channelFilter?: boolean;
    roleFilter?: boolean;
}