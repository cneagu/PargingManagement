import { Injectable } from '@angular/core';
import {  CanActivate, Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class HomeGuard implements CanActivate {
  constructor(private router: Router) { }

  canActivate(): boolean {
    const expires = sessionStorage.getItem('refreshTokenExpires');
    if (!expires || (new Date(expires)) < (new Date())) {
      this.router.navigate(['/authentication/login']);
      return false;
    }
    return true;
  }
}
