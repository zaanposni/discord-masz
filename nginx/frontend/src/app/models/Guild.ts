import { GuildRole } from "./GuildRole";

export interface Guild {
  id: string;
  name: string;
  icon: string;
  roles: GuildRole[];
}
