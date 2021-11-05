import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

import { EmployeeService } from '../core/employee.service';
import { ProgressBarService } from '../core/components/progress-bar/progress-bar.service';
import { TextService } from '../core/text.service';

@Injectable({
  providedIn: 'root'
})
export class HomeResolver implements Resolve<boolean> {
  constructor(private employeeService: EmployeeService,
    private progressBarService: ProgressBarService,
    private textService: TextService,
    private snackbar: MatSnackBar
  ) { }

  resolve(): Observable<boolean> {
    this.progressBarService.push(true);

    return this.employeeService.getEmployeeDetails()
      .pipe(
        map(() => {
          this.progressBarService.push(false);

          return true;
        }),
        catchError((error: any) => {
          this.progressBarService.push(false);

          const message = this.textService.getText('genericError');
          this.snackbar.open(message, this.textService.getText('close'));
          return Observable.throw(error);
        })
      );
  }
}
