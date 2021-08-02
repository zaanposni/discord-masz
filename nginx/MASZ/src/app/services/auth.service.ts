import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AppUser } from '../models/AppUser';
import { API_URL } from '../config/config';
import { ReplaySubject, Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { Params, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new ReplaySubject<any>();
  currentUser$ = this.currentUserSubject.asObservable();
  private init = false;
  private loginBlacklist = ['/patchnotes', '/oauthfailed', '/guidelines', '/donate'];

  constructor(private http: HttpClient, private router: Router, private cookieService: CookieService, private toastr: ToastrService) { }

  loadUserConfig() {
    this.http.get(API_URL + '/discord/users/@me').subscribe((data) => {
      this.currentUserSubject.next(data);
    }, (error) => {
      this.currentUserSubject.error(null);
      this.handleError(error);
    });
  }

  getUserProfile(reinit: boolean = false): Observable<AppUser> {
    if (reinit || ( !this.init && this.isLoggedIn())) {
      this.init = true;
      this.loadUserConfig();
    }
    return this.currentUser$;
  }

  isModInGuild(guildId: string): Observable<boolean> {
    return this.currentUser$.pipe(map((data: AppUser) => {
      return data.modGuilds.find(x => x.id === guildId) !== undefined || data.adminGuilds.find(x => x.id === guildId) !== undefined || data.isAdmin;
    }));
  }

  isAdminInGuild(guildId: string): Observable<boolean> {
    return this.currentUser$.pipe(map((data: AppUser) => {
      return data.adminGuilds.find(x => x.id === guildId) !== undefined || data.isAdmin;
    }));
  }

  resetCache() {
    this.init = false;
  }

  getToken(): string {
    return this.cookieService.get('masz_access_token') || '';
  }

  isLoggedIn(): boolean {
    return this.cookieService.hasKey('masz_access_token');
  }

  public doLogout(): void {
    const removeToken = this.cookieService.remove('masz_access_token');
    if (removeToken == null) {
      this.router.navigate(['login']);
    }
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let msg = '';
    if (error.error instanceof ErrorEvent) {
      // client-side error
      msg = error.error.message;
    } else {
      // server-side error
      if (error.status == 401 && this.isLoggedIn()) {
        this.toastr.error("You have been logged out.");
        if (location.pathname.includes('login') || location.pathname === '' || location.pathname === '/') {
          this.router.navigate(['login']);
        } else {
          if (!this.loginBlacklist.includes(location.pathname)) {
            let params: Params = {
              'ReturnUrl': location.pathname
            }
            this.router.navigate(['login'], { queryParams: params });
          }
        }
      }
      msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(msg);
  }
}
