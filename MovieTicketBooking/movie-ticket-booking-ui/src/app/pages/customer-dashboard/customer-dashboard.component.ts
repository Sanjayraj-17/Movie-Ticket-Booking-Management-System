import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../../models/booking';
import { User } from '../../models/user';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.scss'
})
export class CustomerDashboardComponent implements OnInit {
  // Current authenticated user signal
  currentUser = signal<User | null>(null);

  // Bookings state
  allCustomerBookings: Booking[] = [];
  upcomingBookings: Booking[] = [];
  previousBookings: Booking[] = [];

  // Loading and error states
  isLoading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  // Quick sign-in modal/form values for unauthenticated testing
  loginEmail: string = 'arun@gmail.com';
  loginPassword: string = 'Password@123';
  isLoggingIn: boolean = false;

  constructor(
    public authService: AuthService,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {
    this.currentUser.set(this.authService.currentUser());
    if (this.currentUser()) {
      this.loadCustomerBookings();
    }
  }

  // Load bookings for the authenticated customer from the backend API
  loadCustomerBookings(): void {
    const user = this.currentUser();
    if (!user) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.bookingService.getAllBookings().subscribe({
      next: (bookings) => {
        // Enforce customer isolation: filter only bookings belonging to this customer
        const userEmail = user.email.trim().toLowerCase();
        this.allCustomerBookings = bookings.filter(
          (b) => b.customerEmail?.trim().toLowerCase() === userEmail
        );

        const now = new Date();
        this.upcomingBookings = [];
        this.previousBookings = [];

        for (const b of this.allCustomerBookings) {
          const showDate = new Date(b.startTime);
          if (showDate >= now && b.bookingStatus.toLowerCase() === 'confirmed') {
            this.upcomingBookings.push(b);
          } else {
            this.previousBookings.push(b);
          }
        }

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching bookings:', err);
        this.errorMessage = 'Unable to load your bookings. Please verify backend connection.';
        this.isLoading = false;
      }
    });
  }

  // Calculate total amount spent on confirmed bookings
  getTotalSpent(): number {
    return this.allCustomerBookings
      .filter((b) => b.bookingStatus.toLowerCase() === 'confirmed')
      .reduce((sum, b) => sum + (b.totalAmount || 0), 0);
  }

  // Cancel an upcoming booking
  cancelBooking(bookingId: number): void {
    if (!confirm('Are you sure you want to cancel this booking? Reserved seats will be released.')) {
      return;
    }

    this.bookingService.cancelBooking(bookingId).subscribe({
      next: (res) => {
        this.successMessage = res.message || 'Booking cancelled successfully.';
        this.loadCustomerBookings();
        setTimeout(() => (this.successMessage = ''), 5000);
      },
      error: (err) => {
        console.error('Error cancelling booking:', err);
        const msg = err.error?.message || 'Failed to cancel booking. Please try again.';
        this.errorMessage = msg;
        setTimeout(() => (this.errorMessage = ''), 5000);
      }
    });
  }

  // Handle in-dashboard quick login
  handleLogin(): void {
    if (!this.loginEmail || !this.loginPassword) {
      this.errorMessage = 'Please enter both email and password.';
      return;
    }

    this.isLoggingIn = true;
    this.errorMessage = '';

    this.authService.login({ email: this.loginEmail, password: this.loginPassword }).subscribe({
      next: (res) => {
        this.isLoggingIn = false;
        this.currentUser.set(this.authService.currentUser());
        this.loadCustomerBookings();
      },
      error: (err) => {
        this.isLoggingIn = false;
        this.errorMessage = err.error?.message || 'Invalid email or password.';
      }
    });
  }

  // Handle logout from dashboard
  handleLogout(): void {
    this.authService.logout();
    this.currentUser.set(null);
    this.allCustomerBookings = [];
    this.upcomingBookings = [];
    this.previousBookings = [];
  }
}
