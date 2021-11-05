import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { EmployeeService } from '../core/employee.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  isActivated: boolean = false;

  constructor(
    private router: Router,
    private employeeService: EmployeeService
  ) { }

  ngOnInit() {
    this.isActivated = this.employeeService.employeeDetails.isAccountActivated;

    if (!this.isActivated || this.router.url !== '/home')
      return;

    if (this.employeeService.employeeDetails.isParkingOwner)
      this.router.navigate(['/home/parking-registration'])
  }

  logout() {
    sessionStorage.clear();

    this.router.navigate(['/authentication/login']);
  }

  navigateToParkingDashboard() {
    this.router.navigate(['/home/parking-dashboard']);
  }
}
