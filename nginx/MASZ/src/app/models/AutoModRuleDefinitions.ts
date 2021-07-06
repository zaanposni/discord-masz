export interface AutoModRuleDefinition {
    type: number;
    title: string;
    description: string;
    showLimitField: boolean;
    showTimeLimitField: boolean;
    showCustomField?: boolean;
    tooltip?: string;
    link?: string;
    linkText?: string;
}