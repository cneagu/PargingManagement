import { Component, OnDestroy, OnInit } from '@angular/core';
import { ValidationErrors } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { ProgressBarService } from '../../core/components/progress-bar/progress-bar.service';
import { TextService } from '../../core/text.service';
import { RegisterModel, RegisterStatus } from '../authentication.model';
import { AuthenticationService } from '../authentication.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnDestroy {
  firstName: string = '';
  lastName: string = '';
  email: string = '';
  password: string = '';
  passwordConfirmation: string = '';

  constructor(
    private authService: AuthenticationService,
    private router: Router,
    private textService: TextService,
    private snackbar: MatSnackBar,
    private progressBarService: ProgressBarService
  ) { }

  ngOnDestroy() {
    this.snackbar.dismiss();
  }

  register() {
    this.progressBarService.push(true);

    const registerModel: RegisterModel = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password
    };

    this.authService.register(registerModel)
      .subscribe((status: RegisterStatus) => {
        if (status !== RegisterStatus.Success) {
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

  getValidationError(errors: ValidationErrors | null): string {
    if (errors == null)
      return 'something is wrong';
    if (errors.minlength)
      return 'passwordTooShort';
    if (errors.maxlength)
      return 'passwordTooLong';
    if (errors.match)
      return 'passwordNotMatching';

    return 'requiredField';
  }

  private getErrorStatus(status: RegisterStatus): string {
    switch (status) {
      case RegisterStatus.InvalidEmail:
        return 'invalidEmailRegister';
    }
    return '';
  }
}
