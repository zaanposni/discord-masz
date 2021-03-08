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
import { MatNativeDateModule } from '@angular/material/core';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { IndexComponent } from './components/basic/index/index.component';
import { SettingsComponent } from './components/basic/settings/settings.component';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { AuthService } from './services/auth.service';
import { ApiInterceptor } from './services/ApiInterceptor';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ApiService } from './services/api.service';
import { CookieTrackerService } from './services/cookie-tracker.service';
import { AuthGuard } from './guards/auth.guard';
import { GuildCardComponent } from './components/guilds/guild-card/guild-card.component';
import { GuildDashboardComponent } from './components/guilds/guild-dashboard/guild-dashboard.component';
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

@NgModule({
  declarations: [
    AppComponent,
    IndexComponent,
    SettingsComponent,
    GuildCardComponent,
    GuildDashboardComponent,
    NotFoundComponent,
    PatchnotesComponent,
    GuidelinesComponent,
    DonateComponent,
    EpiclistComponent,
    GuildOverviewComponent,
    GuildAddComponent,
    GuildListComponent,
    GuildDeleteDialogComponent,
    ConfirmationDialogComponent
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
    AppRoutingModule
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
