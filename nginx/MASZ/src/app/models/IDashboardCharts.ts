import { DbCount } from "./DbCount";

export interface IDashboardCharts {
  modCases: DbCount[];
  punishments: DbCount[];
  appeals: DbCount[];
  autoModerations: DbCount[];
}