import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeResolver } from './home.resolver';
import { HomeGuard } from './home.guard';
import { HomeComponent } from './home.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [HomeGuard],
    resolve: [HomeResolver],
    children: [
      {
        path: 'parking-dashboard',
        loadChildren: './parking-dashboard/parking-dashboard.module#ParkingDashboardModule'
      },
      {
        path: 'parking-registration',
        loadChildren: './parking-registration/parking-registration.module#ParkingRegistrationModule'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
