import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { IndexComponent } from './index/index.component';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthService } from './services/auth.service';
import { ApiInterceptor } from './services/ApiInterceptor';
import { CookieModule, CookieService } from 'ngx-cookie';
import { AuthGuard } from './guards/auth.guard';
import { GuildOverviewComponent } from './components/guild/guild-overview/guild-overview.component';
import { GuildAddComponent } from './components/guild/guild-add/guild-add.component';
import { GuildPatchComponent } from './components/guild/guild-patch/guild-patch.component';
import { GuildListComponent } from './components/guild/guild-list/guild-list.component';
import { PageNotFoundComponent } from './components/defaults/page-not-found/page-not-found.component';
import { ApiService } from './services/api.service';
import { NavbarComponent } from './layout/navbar/navbar.component';
import { FooterComponent } from './layout/footer/footer.component';
import { CommonModule } from '@angular/common';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NonAuthGuard } from './guards/non-auth.guard';
import { PatchnotesComponent } from './components/information/patchnotes/patchnotes.component';
import { CaseViewComponent } from './components/modcase/case-view/case-view.component';
import { FormsModule } from '@angular/forms';
import { SettingsComponent } from './components/information/settings/settings.component';
import { CaseNewComponent } from './components/modcase/case-new/case-new.component';
import { AutomodConfigComponent } from './components/moderation/automod-config/automod-config.component';
import { CaseEditComponent } from './components/modcase/case-edit/case-edit.component';
import { OwlDateTimeModule, OwlMomentDateTimeModule, OwlNativeDateTimeModule } from '@danielmoncada/angular-datetime-picker';
import { CookieTrackerService } from './services/cookie-tracker.service';
import { AutomodRuleComponent } from './components/moderation/automod-rule/automod-rule.component';
import { AutocompleteLibModule } from 'angular-ng-autocomplete';
import { CaseCardComponent } from './components/guild/guild-overview/case-card/case-card.component';
import { CaseTableComponent } from './components/guild/guild-overview/case-table/case-table.component';
import { ModeventsTableComponent } from './components/guild/guild-overview/modevents-table/modevents-table.component';
import { ModeventComponent } from './components/guild/guild-overview/modevent/modevent.component';
import { DonateComponent } from './components/information/donate/donate.component';
import { ProfileViewComponent } from './components/profile/profile-view/profile-view.component';
import { TemplateCardComponent } from './components/misc/template-card/template-card.component';
import { GuildDashboardComponent } from './components/guild/guild-dashboard/guild-dashboard.component';
import { ChartsModule } from 'ng2-charts';
import { DashboardChartsComponent } from './components/guild/guild-dashboard/dashboard-charts/dashboard-charts.component';
import { CountChartComponent } from './components/guild/guild-dashboard/dashboard-charts/count-chart/count-chart.component';
import { LoadingSpinnerComponent } from './components/misc/loading-spinner/loading-spinner.component';
import { DashboardQuicksearchComponent } from './components/guild/guild-dashboard/dashboard-quicksearch/dashboard-quicksearch.component';
import { DashboardCaseListComponent } from './components/guild/guild-dashboard/dashboard-case-list/dashboard-case-list.component';
import { DashboardMotdComponent } from './components/guild/guild-dashboard/dashboard-motd/dashboard-motd.component';
import { DashboardGuildStatsComponent } from './components/guild/guild-dashboard/dashboard-guild-stats/dashboard-guild-stats.component';
import { DashboardGuildinfoComponent } from './components/guild/guild-dashboard/dashboard-guildinfo/dashboard-guildinfo.component';
import { QuicksearchCaseResultComponent } from './components/guild/guild-dashboard/dashboard-quicksearch/quicksearch-case-result/quicksearch-case-result.component';
import { QuicksearchModerationResultComponent } from './components/guild/guild-dashboard/dashboard-quicksearch/quicksearch-moderation-result/quicksearch-moderation-result.component';
import { CaseCardCompactComponent } from './components/guild/guild-dashboard/dashboard-case-list/case-card-compact/case-card-compact.component';

@NgModule({
  declarations: [
    AppComponent,

    GuildOverviewComponent,
    GuildAddComponent,
    GuildPatchComponent,
    GuildListComponent,

    IndexComponent,
    NavbarComponent,
    FooterComponent,
    PatchnotesComponent,
    SettingsComponent,

    PageNotFoundComponent,

    CaseViewComponent,
    CaseNewComponent,
    CaseEditComponent,

    AutomodConfigComponent,
    AutomodRuleComponent,
    CaseCardComponent,
    CaseTableComponent,
    ModeventsTableComponent,
    ModeventComponent,
    DonateComponent,
    ProfileViewComponent,
    TemplateCardComponent,
    GuildDashboardComponent,
    DashboardChartsComponent,
    CountChartComponent,
    LoadingSpinnerComponent,
    DashboardQuicksearchComponent,
    DashboardCaseListComponent,
    DashboardMotdComponent,
    DashboardGuildStatsComponent,
    DashboardGuildinfoComponent,
    QuicksearchCaseResultComponent,
    QuicksearchModerationResultComponent,
    CaseCardCompactComponent
  ],
  imports: [
    CommonModule,
    ToastrModule.forRoot({
      progressBar: true,
      timeOut: 5000
    }),
    BrowserAnimationsModule,
    BrowserModule,
    HttpClientModule,
    CookieModule.forRoot(),
    AppRoutingModule,
    RouterModule,
    FormsModule,
    OwlDateTimeModule,
    OwlMomentDateTimeModule,
    AutocompleteLibModule,
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
    NonAuthGuard,
    ApiService,
    CookieTrackerService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
