import { Injectable } from '@angular/core';

import { Token } from './token.model';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  constructor() { }

  saveTokens(accessToken?: Token, refreshToken?: Token) {
    if (accessToken && refreshToken) {
      sessionStorage.setItem('accessTokenExpires', accessToken.expires);
      sessionStorage.setItem('accessToken', accessToken.value);
      sessionStorage.setItem('refreshTokenExpires', refreshToken.expires);
      sessionStorage.setItem('refreshToken', refreshToken.value);
    }
  }

  getAccessToken(): Token {
    let accessToken: Token = {
      value: sessionStorage.getItem('accessToken') ?? '',
      expires: sessionStorage.getItem('accessTokenExpires') ?? ''
    }
    return accessToken;
  }

  getRefreshToken(): Token {
    let refreshToken: Token = {
      value: sessionStorage.getItem('refreshToken') ?? '',
      expires: sessionStorage.getItem('refreshTokenExpires') ?? ''
    }
    return refreshToken;
  }

  isRefreshTokenValid(): boolean {
    return this.isValid(this.getRefreshToken());
  }

  isAccessTokenValid(): boolean {
    return this.isValid(this.getAccessToken());
  }

  private isValid(token: Token): boolean {
    if (token) {
      if (new Date(token.expires) > new Date())
        return true;
    }
    return false;
  }
}
