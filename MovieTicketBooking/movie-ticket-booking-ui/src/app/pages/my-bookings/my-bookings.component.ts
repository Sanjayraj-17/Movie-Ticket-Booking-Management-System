import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../../models/booking';
import { User } from '../../models/user';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './my-bookings.component.html',
  styleUrl: './my-bookings.component.scss'
})
export class MyBookingsComponent implements OnInit {
  // Current authenticated user signal
  currentUser = signal<User | null>(null);

  // Raw customer bookings and filtered list
  allBookings: Booking[] = [];
  displayedBookings: Booking[] = [];

  // Active filter tab: 'all' | 'upcoming' | 'completed' | 'cancelled'
  activeTab: 'all' | 'upcoming' | 'completed' | 'cancelled' = 'all';

  // Loading and alert states
  isLoading: boolean = true;
  isCancellingId: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    public authService: AuthService,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {
    this.currentUser.set(this.authService.currentUser());
    this.loadMyBookings();
  }

  // Load all bookings and enforce customer privacy isolation
  loadMyBookings(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const user = this.currentUser();
    const userEmail = user?.email?.trim().toLowerCase() || 'arun@gmail.com';

    this.bookingService.getAllBookings().subscribe({
      next: (bookings) => {
        // Enforce customer privacy isolation: display only bookings belonging to this customer
        this.allBookings = bookings.filter(
          (b) => b.customerEmail?.trim().toLowerCase() === userEmail
        );

        this.applyTabFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching bookings:', err);
        this.errorMessage = 'Unable to retrieve your bookings. Please verify backend connection.';
        this.isLoading = false;
      }
    });
  }

  // Set active filter tab and filter displayed bookings
  setTab(tab: 'all' | 'upcoming' | 'completed' | 'cancelled'): void {
    this.activeTab = tab;
    this.applyTabFilter();
  }

  // Apply tab criteria
  private applyTabFilter(): void {
    const now = new Date();

    switch (this.activeTab) {
      case 'upcoming':
        this.displayedBookings = this.allBookings.filter((b) => {
          const showTime = new Date(b.startTime);
          return showTime >= now && b.bookingStatus.toLowerCase() === 'confirmed';
        });
        break;

      case 'completed':
        this.displayedBookings = this.allBookings.filter((b) => {
          const showTime = new Date(b.startTime);
          return showTime < now && b.bookingStatus.toLowerCase() === 'confirmed';
        });
        break;

      case 'cancelled':
        this.displayedBookings = this.allBookings.filter(
          (b) => b.bookingStatus.toLowerCase() === 'cancelled'
        );
        break;

      case 'all':
      default:
        this.displayedBookings = [...this.allBookings];
        break;
    }
  }

  // Count helper for tabs
  getTabCount(tab: 'all' | 'upcoming' | 'completed' | 'cancelled'): number {
    const now = new Date();
    switch (tab) {
      case 'upcoming':
        return this.allBookings.filter(
          (b) => new Date(b.startTime) >= now && b.bookingStatus.toLowerCase() === 'confirmed'
        ).length;
      case 'completed':
        return this.allBookings.filter(
          (b) => new Date(b.startTime) < now && b.bookingStatus.toLowerCase() === 'confirmed'
        ).length;
      case 'cancelled':
        return this.allBookings.filter(
          (b) => b.bookingStatus.toLowerCase() === 'cancelled'
        ).length;
      case 'all':
      default:
        return this.allBookings.length;
    }
  }

  // Check if a booking can be cancelled (only upcoming confirmed bookings)
  canCancel(booking: Booking): boolean {
    const now = new Date();
    const showTime = new Date(booking.startTime);
    return booking.bookingStatus.toLowerCase() === 'confirmed' && showTime >= now;
  }

  // Cancel booking and release seats back to Available
  cancelBooking(bookingId: number): void {
    if (!confirm('Are you sure you want to cancel this booking? Reserved seats will be released.')) {
      return;
    }

    this.isCancellingId = bookingId;

    this.bookingService.cancelBooking(bookingId).subscribe({
      next: (res) => {
        this.isCancellingId = null;
        this.successMessage = res.message || 'Booking cancelled successfully. Seats have been released.';
        this.loadMyBookings();
        setTimeout(() => (this.successMessage = ''), 5000);
      },
      error: (err) => {
        this.isCancellingId = null;
        console.error('Error cancelling booking:', err);
        const msg = err.error?.message || 'Failed to cancel reservation.';
        this.errorMessage = msg;
        setTimeout(() => (this.errorMessage = ''), 5000);
      }
    });
  }
}
