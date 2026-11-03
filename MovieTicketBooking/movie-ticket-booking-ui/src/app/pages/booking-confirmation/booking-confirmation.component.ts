import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { BookingService } from '../../services/booking.service';
import { Booking, CreateBookingRequest } from '../../models/booking';
import { ShowSeat } from '../../models/seat';

export interface PendingBookingState {
  showId: number;
  movieTitle: string;
  theatreName: string;
  screenName: string;
  startTime: string;
  seats: ShowSeat[];
  totalAmount: number;
}

@Component({
  selector: 'app-booking-confirmation',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './booking-confirmation.component.html',
  styleUrl: './booking-confirmation.component.scss'
})
export class BookingConfirmationComponent implements OnInit {
  // Pending booking data from seat selection session
  pendingBooking: PendingBookingState | null = null;

  // Customer contact inputs
  customerName: string = '';
  customerEmail: string = '';
  customerPhone: string = '+1-555-0199';

  // Confirmed booking response from backend
  confirmedBooking: Booking | null = null;

  // Process states
  isSubmitting: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    public authService: AuthService,
    private bookingService: BookingService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPendingBooking();
    this.prefillCustomerDetails();
  }

  // Retrieve pending booking details from sessionStorage
  private loadPendingBooking(): void {
    const raw = sessionStorage.getItem('pending_booking');
    if (raw) {
      try {
        this.pendingBooking = JSON.parse(raw) as PendingBookingState;
      } catch (err) {
        console.error('Failed to parse pending booking:', err);
      }
    }
  }

  // Pre-fill customer details from logged-in user profile if available
  private prefillCustomerDetails(): void {
    const user = this.authService.currentUser();
    if (user) {
      this.customerName = user.fullName || '';
      this.customerEmail = user.email || '';
    } else {
      this.customerName = 'Arun Kumar';
      this.customerEmail = 'arun@gmail.com';
    }
  }

  // Submit booking to backend API
  confirmBooking(): void {
    if (!this.pendingBooking || this.pendingBooking.seats.length === 0) {
      this.errorMessage = 'No seats selected. Please restart the booking process.';
      return;
    }

    if (!this.customerName.trim() || !this.customerEmail.trim() || !this.customerPhone.trim()) {
      this.errorMessage = 'Please provide customer name, email address, and phone number.';
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    // Prepare CreateBookingRequest with seat IDs only (backend calculates total amount)
    const request: CreateBookingRequest = {
      showId: this.pendingBooking.showId,
      customerName: this.customerName.trim(),
      customerEmail: this.customerEmail.trim(),
      customerPhone: this.customerPhone.trim(),
      seatIds: this.pendingBooking.seats.map((s) => s.seatId)
    };

    this.bookingService.createBooking(request).subscribe({
      next: (response) => {
        this.confirmedBooking = response;
        this.isSubmitting = false;
        this.successMessage = 'Booking confirmed successfully!';

        // Clear temporary pending state
        sessionStorage.removeItem('pending_booking');
      },
      error: (err) => {
        console.error('Error confirming booking:', err);
        this.isSubmitting = false;
        const msg = err.error?.message || 'Failed to complete booking. Seats may no longer be available.';
        this.errorMessage = msg;
      }
    });
  }

  // Print digital ticket receipt
  printTicket(): void {
    window.print();
  }
}
