import { GuildRole } from "./GuildRole";

export interface Guild {
  id: string;
  name: string;
  iconUrl: string;
  roles: GuildRole[];
}
