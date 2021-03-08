import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DonateComponent } from './components/basic/donate/donate.component';
import { GuidelinesComponent } from './components/basic/guidelines/guidelines.component';
import { IndexComponent } from './components/basic/index/index.component';
import { PatchnotesComponent } from './components/basic/patchnotes/patchnotes.component';
import { NotFoundComponent } from './components/errors/not-found/not-found.component';
import { GuildAddComponent } from './components/guilds/guild-add/guild-add.component';
import { GuildDashboardComponent } from './components/guilds/guild-dashboard/guild-dashboard.component';
import { GuildListComponent } from './components/guilds/guild-list/guild-list.component';
import { GuildOverviewComponent } from './components/guilds/guild-overview/guild-overview.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: 'guilds', component: GuildListComponent, canActivate: [AuthGuard] },
  { path: 'guilds/new', component: GuildAddComponent, canActivate: [AuthGuard] },
  { path: 'guilds/:guildid', component: GuildOverviewComponent, canActivate: [AuthGuard] },
  { path: 'guilds/:guildid/dash', component: GuildDashboardComponent, canActivate: [AuthGuard] },
  { path: 'patchnotes', component: PatchnotesComponent },
  { path: 'donate', component: DonateComponent },
  { path: 'guidelines', component: GuidelinesComponent },
  { path: 'login',  component: IndexComponent, pathMatch: 'full' },
  { path: '',  redirectTo: 'login', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
