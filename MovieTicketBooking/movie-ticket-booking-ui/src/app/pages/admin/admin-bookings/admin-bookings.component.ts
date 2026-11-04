import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { BookingService } from '../../../services/booking.service';
import { AuthService } from '../../../services/auth.service';
import { Booking } from '../../../models/booking';

@Component({
  selector: 'app-admin-bookings',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './admin-bookings.component.html',
  styleUrl: './admin-bookings.component.scss'
})
export class AdminBookingsComponent implements OnInit {
  // All bookings from backend
  bookings: Booking[] = [];
  filteredBookings: Booking[] = [];

  // Filter states
  searchQuery: string = '';
  selectedStatus: 'ALL' | 'CONFIRMED' | 'CANCELLED' = 'ALL';
  selectedDate: string = '';

  // Summary Metrics
  totalConfirmedCount: number = 0;
  totalCancelledCount: number = 0;
  totalRevenue: number = 0;

  // Loading & Alert states
  isLoading: boolean = false;
  isCancelling: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  // Modal: View Booking Details
  selectedBooking: Booking | null = null;
  showDetailsModal: boolean = false;

  // Modal: Confirm Cancellation
  bookingToCancel: Booking | null = null;

  constructor(
    public authService: AuthService,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  // Load all customer bookings from API
  loadBookings(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.bookingService.getAllBookings().subscribe({
      next: (data) => {
        // Sort newest first
        this.bookings = data.sort((a, b) => b.id - a.id);
        this.computeMetrics();
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading bookings:', err);
        this.errorMessage = 'Failed to load bookings from backend. Ensure administrator privileges.';
        this.isLoading = false;
      }
    });
  }

  // Compute summary stats
  computeMetrics(): void {
    this.totalConfirmedCount = this.bookings.filter(
      (b) => b.bookingStatus.toLowerCase() === 'confirmed'
    ).length;

    this.totalCancelledCount = this.bookings.filter(
      (b) => b.bookingStatus.toLowerCase() === 'cancelled'
    ).length;

    this.totalRevenue = this.bookings
      .filter((b) => b.bookingStatus.toLowerCase() === 'confirmed')
      .reduce((sum, b) => sum + (b.totalAmount || 0), 0);
  }

  // Apply search query, status tab, and date filter
  applyFilter(): void {
    let list = [...this.bookings];

    // Status filter
    if (this.selectedStatus === 'CONFIRMED') {
      list = list.filter((b) => b.bookingStatus.toLowerCase() === 'confirmed');
    } else if (this.selectedStatus === 'CANCELLED') {
      list = list.filter((b) => b.bookingStatus.toLowerCase() === 'cancelled');
    }

    // Keyword search filter
    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase().trim();
      list = list.filter((b) =>
        b.id.toString().includes(q) ||
        (b.customerName && b.customerName.toLowerCase().includes(q)) ||
        (b.customerEmail && b.customerEmail.toLowerCase().includes(q)) ||
        (b.customerPhone && b.customerPhone.includes(q)) ||
        (b.movieTitle && b.movieTitle.toLowerCase().includes(q)) ||
        (b.theatreName && b.theatreName.toLowerCase().includes(q)) ||
        (b.screenName && b.screenName.toLowerCase().includes(q))
      );
    }

    // Date filter
    if (this.selectedDate) {
      list = list.filter(
        (b) =>
          (b.startTime && b.startTime.startsWith(this.selectedDate)) ||
          (b.bookingTime && b.bookingTime.startsWith(this.selectedDate))
      );
    }

    this.filteredBookings = list;
  }

  // Change status filter tab
  setStatusFilter(status: 'ALL' | 'CONFIRMED' | 'CANCELLED'): void {
    this.selectedStatus = status;
    this.applyFilter();
  }

  // Open booking details modal
  viewBookingDetails(booking: Booking): void {
    this.selectedBooking = booking;
    this.showDetailsModal = true;
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedBooking = null;
  }

  // Open cancel confirmation
  confirmCancel(booking: Booking): void {
    this.bookingToCancel = booking;
  }

  cancelAbort(): void {
    this.bookingToCancel = null;
  }

  // Execute booking cancellation via PUT /api/bookings/{id}/cancel
  executeCancel(): void {
    if (!this.bookingToCancel) return;

    this.isCancelling = true;
    this.errorMessage = '';

    this.bookingService.cancelBooking(this.bookingToCancel.id).subscribe({
      next: () => {
        this.isCancelling = false;
        this.successMessage = `Booking #${this.bookingToCancel?.id} has been cancelled and seats released.`;
        if (this.selectedBooking && this.selectedBooking.id === this.bookingToCancel?.id) {
          this.selectedBooking.bookingStatus = 'Cancelled';
        }
        this.bookingToCancel = null;
        this.loadBookings();
        setTimeout(() => (this.successMessage = ''), 4000);
      },
      error: (err) => {
        this.isCancelling = false;
        this.errorMessage = err.error?.message || 'Failed to cancel booking.';
      }
    });
  }
}

