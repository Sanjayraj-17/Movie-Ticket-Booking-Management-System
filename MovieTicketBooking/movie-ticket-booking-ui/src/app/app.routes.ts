import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { CustomerDashboardComponent } from './pages/customer-dashboard/customer-dashboard.component';
import { MovieDetailsComponent } from './pages/movie-details/movie-details.component';
import { TheatreSelectionComponent } from './pages/theatre-selection/theatre-selection.component';
import { ShowSelectionComponent } from './pages/show-selection/show-selection.component';
import { SeatSelectionComponent } from './pages/seat-selection/seat-selection.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    title: 'MovieBook - Home'
  },
  {
    path: 'movies/:id',
    component: MovieDetailsComponent,
    title: 'MovieBook - Movie Details'
  },
  {
    path: 'theatres',
    component: TheatreSelectionComponent,
    title: 'MovieBook - Cinema Theatres'
  },
  {
    path: 'shows',
    component: ShowSelectionComponent,
    title: 'MovieBook - Show Selection'
  },
  {
    path: 'booking/seats/:showId',
    component: SeatSelectionComponent,
    title: 'MovieBook - Seat Selection'
  },
  {
    path: 'seats/:showId',
    redirectTo: 'booking/seats/:showId'
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
