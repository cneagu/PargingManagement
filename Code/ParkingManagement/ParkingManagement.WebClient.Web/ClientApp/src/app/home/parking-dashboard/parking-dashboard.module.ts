import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { SharedModule } from '../../shared/shared.module';

import { ParkingDashboardComponent } from './parking-dashboard.component';
import { ParkingDashboardService } from './parking-dashboard.service';

const routes: Routes = [
  {
    path: '',
    component: ParkingDashboardComponent
  }
];

@NgModule({
  declarations: [ParkingDashboardComponent],
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild(routes),
    MatToolbarModule,
    MatButtonModule,
    MatSnackBarModule,
    MatTableModule,
    MatProgressSpinnerModule,
    SharedModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatIconModule,
    MatNativeDateModule,
  ],
  providers: [ParkingDashboardService]
})
export class ParkingDashboardModule { }
