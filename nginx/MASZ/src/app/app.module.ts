import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { MatSliderModule } from '@angular/material/slider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatStepperModule } from '@angular/material/stepper';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDialogModule } from '@angular/material/dialog';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatNativeDateModule } from '@angular/material/core';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { IndexComponent } from './components/basic/index/index.component';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { AuthService } from './services/auth.service';
import { ApiInterceptor } from './services/ApiInterceptor';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ApiService } from './services/api.service';
import { CookieTrackerService } from './services/cookie-tracker.service';
import { AuthGuard } from './guards/auth.guard';
import { GuildCardComponent } from './components/guilds/guild-card/guild-card.component';
import { NotFoundComponent } from './components/errors/not-found/not-found.component';
import { CookieModule } from 'ngx-cookie';
import { PatchnotesComponent } from './components/basic/patchnotes/patchnotes.component';
import { GuidelinesComponent } from './components/basic/guidelines/guidelines.component';
import { DonateComponent } from './components/basic/donate/donate.component';
import { EpiclistComponent } from './components/basic/epiclist/epiclist.component';
import { CommonModule } from '@angular/common';
import { GuildOverviewComponent } from './components/guilds/guild-overview/guild-overview.component';
import { GuildAddComponent } from './components/guilds/guild-add/guild-add.component';
import { GuildListComponent } from './components/guilds/guild-list/guild-list.component';
import { GuildDeleteDialogComponent } from './components/guilds/guild-delete-dialog/guild-delete-dialog.component';
import { ConfirmationDialogComponent } from './components/dialogs/confirmation-dialog/confirmation-dialog.component';
import { GuildEditComponent } from './components/guilds/guild-edit/guild-edit.component';
import { ModcaseAddComponent } from './components/modcase/modcase-add/modcase-add.component';
import { ModcaseEditComponent } from './components/modcase/modcase-edit/modcase-edit.component';
import { ModcaseViewComponent } from './components/modcase/modcase-view/modcase-view.component';
import { GuildDashboardComponent } from './components/guilds/guild-dashboard/guild-dashboard.component';
import { ModcaseTableComponent } from './components/modcase/modcase-table/modcase-table.component';
import { AutomodTableComponent } from './components/automod/automod-table/automod-table.component';
import { DashboardMotdComponent } from './components/guilds/guild-dashboard/dashboard-motd/dashboard-motd.component';
import { DashboardQuicksearchComponent } from './components/guilds/guild-dashboard/dashboard-quicksearch/dashboard-quicksearch.component';
import { DashboardGuildinfoComponent } from './components/guilds/guild-dashboard/dashboard-guildinfo/dashboard-guildinfo.component';
import { DashboardGuildStatsComponent } from './components/guilds/guild-dashboard/dashboard-guild-stats/dashboard-guild-stats.component';
import { DashboardCaseListComponent } from './components/guilds/guild-dashboard/dashboard-case-list/dashboard-case-list.component';
import { DashboardCommentListComponent } from './components/guilds/guild-dashboard/dashboard-comment-list/dashboard-comment-list.component';
import { DashboardChartsComponent } from './components/guilds/guild-dashboard/dashboard-charts/dashboard-charts.component';
import { GuildConfigComponent } from './components/guilds/guild-config/guild-config.component';
import { QuicksearchCaseResultComponent } from './components/guilds/guild-dashboard/dashboard-quicksearch/quicksearch-case-result/quicksearch-case-result.component';
import { QuicksearchModerationResultComponent } from './components/guilds/guild-dashboard/dashboard-quicksearch/quicksearch-moderation-result/quicksearch-moderation-result.component';
import { CountChartComponent } from './components/guilds/guild-dashboard/dashboard-charts/count-chart/count-chart.component';
import { ChartsModule } from 'ng2-charts';
import { ModcaseCardComponent } from './components/modcase/modcase-table/modcase-card/modcase-card.component';
import { ModcaseCardCompactComponent } from './components/guilds/guild-dashboard/dashboard-case-list/modcase-card-compact/modcase-card-compact.component';
import { DashboardAutomodSplitComponent } from './components/guilds/guild-dashboard/dashboard-automod-split/dashboard-automod-split.component';
import { AutomodCardComponent } from './components/automod/automod-card/automod-card.component';
import { CommentsCardCompactComponent } from './components/guilds/guild-dashboard/dashboard-comment-list/comments-card-compact/comments-card-compact.component';
import { MotdConfigComponent } from './components/guilds/guild-config/motd-config/motd-config.component';
import { AutomodConfigComponent } from './components/guilds/guild-config/automod-config/automod-config.component';
import { AutomodRuleComponent } from './components/guilds/guild-config/automod-config/automod-rule/automod-rule.component';

@NgModule({
  declarations: [
    AppComponent,
    IndexComponent,
    GuildCardComponent,
    NotFoundComponent,
    PatchnotesComponent,
    GuidelinesComponent,
    DonateComponent,
    EpiclistComponent,
    GuildOverviewComponent,
    GuildAddComponent,
    GuildListComponent,
    GuildDeleteDialogComponent,
    ConfirmationDialogComponent,
    GuildEditComponent,
    ModcaseAddComponent,
    ModcaseEditComponent,
    ModcaseViewComponent,
    GuildDashboardComponent,
    ModcaseTableComponent,
    AutomodTableComponent,
    DashboardMotdComponent,
    DashboardQuicksearchComponent,
    DashboardGuildinfoComponent,
    DashboardGuildStatsComponent,
    DashboardCaseListComponent,
    DashboardCommentListComponent,
    DashboardChartsComponent,
    GuildConfigComponent,
    QuicksearchCaseResultComponent,
    QuicksearchModerationResultComponent,
    CountChartComponent,
    ModcaseCardComponent,
    ModcaseCardCompactComponent,
    DashboardAutomodSplitComponent,
    AutomodCardComponent,
    CommentsCardCompactComponent,
    MotdConfigComponent,
    AutomodConfigComponent,
    AutomodRuleComponent
  ],
  imports: [
    CommonModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    CookieModule.forRoot(),
    ToastrModule.forRoot({
      progressBar: true,
      timeOut: 5000
    }),
    MatChipsModule,
    MatSlideToggleModule,
    MatTabsModule,
    MatCheckboxModule,
    MatDialogModule,
    MatSelectModule,
    MatInputModule,
    MatStepperModule,
    MatExpansionModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatListModule,
    MatSidenavModule,
    MatIconModule,
    MatToolbarModule,
    MatButtonModule,
    MatSliderModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatNativeDateModule,

    BrowserModule,
    AppRoutingModule,
    ChartsModule
  ],
  providers: [
    ToastrService,
    AuthService, {
      provide: HTTP_INTERCEPTORS,
      useClass: ApiInterceptor,
      multi: true
    },
    AuthGuard,
    ApiService,
    CookieTrackerService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
