export interface IDiscordUser {
    id: string;
    username: string;
    discriminator: string;
    imageUrl: string;
    locale: string;
    avatar: string;
    bot: boolean;
}