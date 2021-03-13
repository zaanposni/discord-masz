import { DbCount } from "./DbCount";

export interface DashboardCharts {
  modCases: DbCount[];
  punishments: DbCount[];
  autoModerations: DbCount[];
}