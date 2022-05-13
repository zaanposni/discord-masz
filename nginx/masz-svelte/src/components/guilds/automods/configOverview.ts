import type { IAutoModRuleDefinition } from "../../../models/IAutoModRuleDefinition";

export const types: IAutoModRuleDefinition[] = [
    {
        type: 0,
        key: "invites",
        showLimitField: false,
        showTimeLimitField: false,
        showCustomField: true,
        requireCustomField: false,
    },
    {
        type: 1,
        key: "emotes",
        showLimitField: true,
        showTimeLimitField: false,
    },
    {
        type: 2,
        key: "mentions",
        showLimitField: true,
        showTimeLimitField: false,
    },
    {
        type: 3,
        key: "attachments",
        showLimitField: true,
        showTimeLimitField: false,
    },
    {
        type: 4,
        key: "embeds",
        showLimitField: true,
        showTimeLimitField: false,
    },
    {
        type: 5,
        key: "automoderations",
        showLimitField: true,
        showTimeLimitField: true,
    },
    {
        type: 6,
        key: "customfilter",
        showLimitField: true,
        showTimeLimitField: false,
        showCustomField: true,
        tooltip: true,
        link: "https://gist.github.com/zaanposni/4f3aa7b29d54005d34eb78f6acfe93eb",
        requireCustomField: true,
    },
    {
        type: 7,
        key: "spam",
        showLimitField: true,
        showTimeLimitField: true,
        timeLimitFieldMessage: true,
    },
    {
        type: 8,
        key: "duplicatedchars",
        showLimitField: true,
        showTimeLimitField: false,
    },
    {
        type: 9,
        key: "link",
        showLimitField: true,
        showTimeLimitField: false,
        showCustomField: true,
        requireCustomField: false,
        link: "https://gist.github.com/zaanposni/5808c07c26ba04f81a9ef31c6dfa3a7e",
    },
];
