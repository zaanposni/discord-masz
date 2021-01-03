export interface DiscordUser {
  id: string;
  username: string;
  discriminator: string;
  avatar: string;
  bot: boolean;
  imageUrl: string;
}