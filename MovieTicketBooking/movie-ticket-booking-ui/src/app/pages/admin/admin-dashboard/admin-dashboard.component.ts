import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { MovieService } from '../../../services/movie.service';
import { TheatreService } from '../../../services/theatre.service';
import { ShowService } from '../../../services/show.service';
import { BookingService } from '../../../services/booking.service';
import { Booking } from '../../../models/booking';
import { User } from '../../../models/user';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  // Current user state
  currentUser = signal<User | null>(null);
  isAdmin = signal<boolean>(false);

  // Summary Metrics
  totalMovies: number = 0;
  totalTheatres: number = 0;
  totalShows: number = 0;
  totalBookings: number = 0;
  totalRevenue: number = 0;

  // Recent Activity
  recentBookings: Booking[] = [];

  // Loading and alert states
  isLoading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  // Admin login credentials
  adminEmail: string = 'admin@example.com';
  adminPassword: string = 'Admin@123';
  isLoggingIn: boolean = false;

  constructor(
    public authService: AuthService,
    private movieService: MovieService,
    private theatreService: TheatreService,
    private showService: ShowService,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {
    this.refreshAuthStatus();
    if (this.isAdmin()) {
      this.loadAdminMetrics();
    }
  }

  // Refresh user and admin status from AuthService
  refreshAuthStatus(): void {
    this.currentUser.set(this.authService.currentUser());
    this.isAdmin.set(this.authService.isAdmin());
  }

  // Load all statistics across backend APIs
  loadAdminMetrics(): void {
    this.isLoading = true;
    this.errorMessage = '';

    // 1. Fetch Movies
    this.movieService.getAllMovies().subscribe({
      next: (movies) => {
        this.totalMovies = movies.length;
      },
      error: (err) => console.error('Error fetching movies for admin:', err)
    });

    // 2. Fetch Theatres
    this.theatreService.getAllTheatres().subscribe({
      next: (theatres) => {
        this.totalTheatres = theatres.length;
      },
      error: (err) => console.error('Error fetching theatres for admin:', err)
    });

    // 3. Fetch Shows
    this.showService.getAllShows().subscribe({
      next: (shows) => {
        this.totalShows = shows.length;
      },
      error: (err) => console.error('Error fetching shows for admin:', err)
    });

    // 4. Fetch Bookings and compute revenue & recent list
    this.bookingService.getAllBookings().subscribe({
      next: (bookings) => {
        this.totalBookings = bookings.length;
        this.recentBookings = bookings.slice(0, 5);

        this.totalRevenue = bookings
          .filter((b) => b.bookingStatus.toLowerCase() === 'confirmed')
          .reduce((sum, b) => sum + (b.totalAmount || 0), 0);

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching bookings for admin:', err);
        this.errorMessage = 'Unable to fetch booking metrics. Ensure your user has Admin privileges.';
        this.isLoading = false;
      }
    });
  }

  // Handle Admin Sign In
  handleAdminLogin(): void {
    if (!this.adminEmail || !this.adminPassword) {
      this.errorMessage = 'Please provide both admin email and password.';
      return;
    }

    this.isLoggingIn = true;
    this.errorMessage = '';

    this.authService.login({ email: this.adminEmail, password: this.adminPassword }).subscribe({
      next: (res) => {
        this.isLoggingIn = false;
        this.refreshAuthStatus();

        if (this.isAdmin()) {
          this.successMessage = 'Welcome to the Admin Portal!';
          this.loadAdminMetrics();
          setTimeout(() => (this.successMessage = ''), 4000);
        } else {
          this.errorMessage = 'The signed-in account does not possess the Admin role.';
        }
      },
      error: (err) => {
        this.isLoggingIn = false;
        this.errorMessage = err.error?.message || 'Invalid administrator credentials.';
      }
    });
  }

  // Handle Admin Logout
  handleAdminLogout(): void {
    this.authService.logout();
    this.refreshAuthStatus();
    this.totalMovies = 0;
    this.totalTheatres = 0;
    this.totalShows = 0;
    this.totalBookings = 0;
    this.totalRevenue = 0;
    this.recentBookings = [];
  }
}
