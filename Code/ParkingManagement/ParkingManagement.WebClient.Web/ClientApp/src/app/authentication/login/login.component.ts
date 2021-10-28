import { MatSnackBar } from '@angular/material/snack-bar';
import { Component, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';

import { TextService } from '../../core/text.service';
import { ProgressBarService } from '../../core/components/progress-bar/progress-bar.service';

import { AuthenticationService } from '../authentication.service';
import { LoginStatus } from '../authentication.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  email: string = '';
  password: string = '';

  constructor(
    private authService: AuthenticationService,
    private router: Router,
    private textService: TextService,
    private snackbar: MatSnackBar,
    private progressBarService: ProgressBarService
  ) { }

  ngOnDestroy() {
  }

  login() {
    this.progressBarService.push(true);

    this.authService.login({
      email: this.email,
      password: this.password
    }).subscribe((status: LoginStatus) => {
      if (status !== LoginStatus.Success) {
        const message = this.textService.getText(this.getErrorStatus(status));

        this.snackbar.open(message, this.textService.getText('close'));
      } else {
        this.router.navigate(['/home']);
      }

      this.progressBarService.push(false);
    }, (error: any) => {
      const message = this.textService.getText('genericError');
      this.snackbar.open(message, this.textService.getText('close'));

      this.progressBarService.push(false);

    });
  }

  private getErrorStatus(status: LoginStatus): string {
    switch (status) {
      case LoginStatus.InvalidEmail:
        return 'invalidEmailLogin';
      case LoginStatus.AccountDisabled:
        return 'accountDisabled';
      case LoginStatus.InvalidPassword:
        return 'invalidPassword';
    }
    return 'error';
  }
}
