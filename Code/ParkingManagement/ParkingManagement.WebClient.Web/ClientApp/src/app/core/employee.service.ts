import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { HttpService } from './http.service';
import { EmployeeDetails } from './employee.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  employeeDetails: EmployeeDetails | any;

  constructor(private httpService: HttpService) { }

  getEmployeeDetails(): Observable<EmployeeDetails> {
    return this.httpService.get<EmployeeDetails>('home/getEmployeeDetails')
      .pipe(
        map(data => {
          this.employeeDetails = data;
          return this.employeeDetails;
        }
        ));
  }
}
