import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { CustomerDashboardComponent } from './pages/customer-dashboard/customer-dashboard.component';
import { MovieDetailsComponent } from './pages/movie-details/movie-details.component';
import { TheatreSelectionComponent } from './pages/theatre-selection/theatre-selection.component';
import { ShowSelectionComponent } from './pages/show-selection/show-selection.component';
import { SeatSelectionComponent } from './pages/seat-selection/seat-selection.component';
import { BookingConfirmationComponent } from './pages/booking-confirmation/booking-confirmation.component';
import { MyBookingsComponent } from './pages/my-bookings/my-bookings.component';
import { AdminDashboardComponent } from './pages/admin/admin-dashboard/admin-dashboard.component';
import { AdminMoviesComponent } from './pages/admin/admin-movies/admin-movies.component';
import { AdminTheatresComponent } from './pages/admin/admin-theatres/admin-theatres.component';
import { AdminScreensComponent } from './pages/admin/admin-screens/admin-screens.component';
import { AdminShowsComponent } from './pages/admin/admin-shows/admin-shows.component';
import { adminGuard } from './guards/admin.guard';

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
    path: 'booking/confirmation',
    component: BookingConfirmationComponent,
    title: 'MovieBook - Booking Confirmation'
  },
  {
    path: 'my-bookings',
    component: MyBookingsComponent,
    title: 'MovieBook - My Bookings'
  },
  {
    path: 'dashboard',
    component: CustomerDashboardComponent,
    title: 'MovieBook - Customer Dashboard'
  },
  {
    path: 'admin',
    redirectTo: 'admin/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'admin/dashboard',
    component: AdminDashboardComponent,
    canActivate: [adminGuard],
    title: 'MovieBook - Admin Dashboard'
  },
  {
    path: 'admin/movies',
    component: AdminMoviesComponent,
    canActivate: [adminGuard],
    title: 'MovieBook - Admin Movie Management'
  },
  {
    path: 'admin/theatres',
    component: AdminTheatresComponent,
    canActivate: [adminGuard],
    title: 'MovieBook - Admin Theatre Management'
  },
  {
    path: 'admin/screens',
    component: AdminScreensComponent,
    canActivate: [adminGuard],
    title: 'MovieBook - Admin Screen & Seat Management'
  },
  {
    path: 'admin/shows',
    component: AdminShowsComponent,
    canActivate: [adminGuard],
    title: 'MovieBook - Admin Show Management'
  },
  {
    path: '**',
    redirectTo: ''
  }
];
