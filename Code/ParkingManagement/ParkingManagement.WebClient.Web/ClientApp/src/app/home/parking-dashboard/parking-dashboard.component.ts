import { Component, OnInit } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { Location } from '@angular/common';

import { TextService } from '../../core/text.service';
import { EmployeeService } from '../../core/employee.service';

import { ParkingDashboardService } from './parking-dashboard.service';
import { AllocationResult, Department, DashboardData } from './parking-dashboard.model';

@Component({
  selector: 'app-parking-dashboard',
  templateUrl: './parking-dashboard.component.html',
  styleUrls: ['./parking-dashboard.component.scss']
})
export class ParkingDashboardComponent implements OnInit {
  allocationResult: MatTableDataSource<AllocationResult> | any;
  displayedColumns: string[] = ['parkingSpotNumber', 'employeeName'];
  departments: Department[] | undefined ;
  currentDepartmentId: string = '';
  date: Date = new Date();
  maxDate = new Date();
  isDateInvalid: boolean = true;
  headerText: string = '';

  constructor(
    private location: Location,
    private parkingDashboardService: ParkingDashboardService,
    private employeeService: EmployeeService,
    private textService: TextService,
    private snackbar: MatSnackBar,
  ) { }

  ngOnInit() {
    this.loadDashboardData();
    this.headerText = this.textService.getText('today')
  }

  back() {
    this.location.back();
  }

  loadDashboardData() {
    this.parkingDashboardService.getDashboardData(this.employeeService.employeeDetails.departmentId).subscribe(
      (data: DashboardData) => {
        data.allocationResults = this.convertAllocationResult(data.allocationResults);
        this.allocationResult = new MatTableDataSource(data.allocationResults);
        this.departments = data.departments;
        this.currentDepartmentId = this.employeeService.employeeDetails.departmentId;
      },
      error => {
        const message = this.textService.getText('genericError');
        this.snackbar.open(message, this.textService.getText('close'));
      })
  }

  onSubmit() {
    this.parkingDashboardService.getAllocationResult(this.date.toISOString(), this.currentDepartmentId).subscribe(
      (data: AllocationResult[]) => {
        data = this.convertAllocationResult(data);
        this.allocationResult = new MatTableDataSource(data);
        this.headerText = this.date.toDateString();
      },
      error => {
        const message = this.textService.getText('genericError');
        this.snackbar.open(message, this.textService.getText('close'));
      })
  }

  convertAllocationResult(allocationResult: AllocationResult[]): AllocationResult[] {
    allocationResult.forEach(item => {
      if (item.employeeName == '') {
        item.employeeName = 'unallocated';
      }
    });

    return allocationResult;
  }

  filterBy(filterValue: string) {
    this.allocationResult.filter = filterValue.trim().toLowerCase();
  }

  isWorkingDay(date: Date): boolean {
    const day = date.getDay();
    return day !== 0 && day !== 6;
  }

  onStartDateChange(event: MatDatepickerInputEvent<Date>) {
    this.date = event.value !== null ? event.value : new Date();
    this.isDateInvalid = !this.date || this.date > this.maxDate;
  }
}
