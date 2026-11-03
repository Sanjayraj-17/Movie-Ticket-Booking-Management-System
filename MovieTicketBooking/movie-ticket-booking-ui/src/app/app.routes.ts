import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { CustomerDashboardComponent } from './pages/customer-dashboard/customer-dashboard.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    title: 'MovieBook - Home'
  },
  {
    path: 'dashboard',
    component: CustomerDashboardComponent,
    title: 'MovieBook - Customer Dashboard'
  },
  {
    path: '**',
    redirectTo: ''
  }
];
