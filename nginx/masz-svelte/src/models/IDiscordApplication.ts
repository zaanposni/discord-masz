export interface IDiscordApplication {
    id: string;
    name: string;
    description: string;
    iconUrl: string;
    iconHash?: string;
    privacyPolicyUrl?: string;
    termsOfServiceUrl?: string;
}