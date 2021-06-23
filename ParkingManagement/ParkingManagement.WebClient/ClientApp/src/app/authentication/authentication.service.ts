import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { HttpService } from '../core/http.service';
import { TokenService } from '../core/token.service';

import {
  LoginModel,
  LoginReturn,
  LoginStatus,
  RegisterModel,
  RegisterReturn,
  RegisterStatus,
  ForgotPassword,
  ForgotPasswordReturn,
} from './authentication.model';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private httpService: HttpService, private tokenService: TokenService) { }

  login(loginModel: LoginModel): Observable<LoginStatus> {
    sessionStorage.clear();

    return this.httpService.unsecurePost<LoginReturn>('authentication/login', loginModel)
      .pipe(
        map(result => {
          this.tokenService.saveTokens(result.accessToken, result.refreshToken);

          return result.status;
        }
        ));
  }

  register(registerModel: RegisterModel): Observable<RegisterStatus> {
    return this.httpService.unsecurePost<RegisterReturn>('authentication/register', registerModel)
      .pipe(
        map(result => {
          this.tokenService.saveTokens(result.accessToken, result.refreshToken);

          return result.status;
        }
        ));
  }

  forgotPassword(email: string): Observable<ForgotPassword> {
    return this.httpService.unsecurePost<ForgotPasswordReturn>('authentication/forgotPassword', email)
      .pipe(
        map(result => {
          return result.status;
        }
        ));
  }
}
