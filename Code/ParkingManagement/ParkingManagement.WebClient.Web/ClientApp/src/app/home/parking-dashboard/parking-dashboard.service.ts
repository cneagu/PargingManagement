import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HttpService } from '../../core/http.service';

import { AllocationResult, DashboardData } from './parking-dashboard.model';
@Injectable({
  providedIn: 'root'
})
export class ParkingDashboardService {

  constructor(private httpService: HttpService) { }

  getDashboardData(departmentId: string): Observable<DashboardData> {
    return this.httpService.get<DashboardData>('home/getDashboardData?departmentId=' + departmentId);
  }

  getAllocationResult(date: string, departmentId: string): Observable<AllocationResult[]> {
    return this.httpService.get<AllocationResult[]>('home/getAllocationResult/' + date + '/' + departmentId);
  }
}
