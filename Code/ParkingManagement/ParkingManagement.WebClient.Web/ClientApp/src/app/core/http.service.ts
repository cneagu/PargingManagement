import { Observable, of } from 'rxjs';
import { flatMap, map, share } from 'rxjs/operators'
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

import { TokenService } from './token.service';
import { RefreshReturn } from './token.model';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private baseApiUrl: string;
  private refreshObs$: Observable<boolean> | undefined;

  constructor(private http: HttpClient, private router: Router, private tokenService: TokenService) {
    this.baseApiUrl = 'api/';
  }

  unsecureGet<T>(url: string): Observable<T> {
    return this.http.get<T>(`${this.baseApiUrl}${url}`, {
      headers: this.getHeaders(false)
    });
  }

  unsecurePost<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseApiUrl}${url}`, body, {
      headers: this.getHeaders(false)
    });
  }

  get<T>(url: string): Observable<T> {
    return this.checkToken().pipe(
      flatMap(() => {
        return this.http.get<T>(`${this.baseApiUrl}${url}`, { headers: this.getHeaders(true) });
      }));
  };

  post<T>(url: string, body: any): Observable<T> {
    return this.checkToken().pipe(
      flatMap(() => {
        return this.http.post<T>(`${this.baseApiUrl}${url}`, body, { headers: this.getHeaders(true) });
      }));
  };

  delete<T>(url: string, id: string): Observable<T> {
    return this.checkToken().pipe(
      flatMap(() => {
        return this.http.delete<T>(`${this.baseApiUrl}${url}`, {
          headers: this.getHeaders(true),
          params: {
            'id': id
          }
        });
      }));
  };


  private getHeaders(isSecure: boolean) {
    if (isSecure)
      return new HttpHeaders({
        'X-Requested-With': 'XMLHttpRequest',
        'Authorization': `Bearer ${this.tokenService.getAccessToken().value}`
      });
    return new HttpHeaders({
      'X-Requested-With': 'XMLHttpRequest'
    });
  }

  private checkToken(): Observable<boolean> {
    if (!this.tokenService.isRefreshTokenValid()) {
      sessionStorage.clear();
      this.router.navigate(['/authentication/login']);
    }

    if (!this.tokenService.isAccessTokenValid()) {
      if (this.refreshObs$) { return this.refreshObs$; }
      this.refreshObs$ = this.unsecurePost<RefreshReturn>('authentication/refresh',
        this.tokenService.getRefreshToken()).pipe(
          map(data => {
            this.tokenService.saveTokens(data.accessToken, data.refreshToken);
            this.refreshObs$ = null as any;
            return true;
          })
        ).pipe(share());
      return this.refreshObs$;
    };
    return of(true);
  }
}
