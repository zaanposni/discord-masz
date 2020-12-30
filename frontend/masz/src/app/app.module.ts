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
import { ApiCacheService } from './services/api-cache.service';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,

    GuildOverviewComponent,
    GuildAddComponent,
    GuildPatchComponent,
    GuildListComponent,

    IndexComponent,

    PageNotFoundComponent,

    NavbarComponent,

    FooterComponent,

    PatchnotesComponent,

    CaseViewComponent,
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
    FormsModule
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
    ApiCacheService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
