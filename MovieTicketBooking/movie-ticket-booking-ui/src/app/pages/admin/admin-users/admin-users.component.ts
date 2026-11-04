import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { BookingService } from '../../../services/booking.service';
import { User } from '../../../models/user';
import { Booking } from '../../../models/booking';

// Aggregated customer profile derived from actual backend booking transactions
export interface CustomerProfile {
  name: string;
  email: string;
  phone: string;
  totalBookings: number;
  totalSpent: number;
  lastBookingDate: string;
}

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss'
})
export class AdminUsersComponent implements OnInit {
  // Current logged in administrator
  currentUser = signal<User | null>(null);

  // Aggregated customers from real backend booking data
  customers: CustomerProfile[] = [];
  filteredCustomers: CustomerProfile[] = [];

  // Search keyword
  searchQuery: string = '';

  // Loading & Alert states
  isLoading: boolean = false;
  errorMessage: string = '';

  // Modal: View User Information Details
  selectedCustomer: CustomerProfile | null = null;
  showDetailsModal: boolean = false;

  constructor(
    public authService: AuthService,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {
    this.currentUser.set(this.authService.currentUser());
    this.loadCustomerProfilesFromBackend();
  }

  // Load customer profiles from actual backend transactions
  loadCustomerProfilesFromBackend(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.bookingService.getAllBookings().subscribe({
      next: (bookings: Booking[]) => {
        const customerMap = new Map<string, CustomerProfile>();

        bookings.forEach((b) => {
          const email = b.customerEmail.toLowerCase().trim();
          if (!customerMap.has(email)) {
            customerMap.set(email, {
              name: b.customerName,
              email: b.customerEmail,
              phone: b.customerPhone || 'N/A',
              totalBookings: 1,
              totalSpent: b.bookingStatus.toLowerCase() === 'confirmed' ? (b.totalAmount || 0) : 0,
              lastBookingDate: b.bookingTime || b.startTime
            });
          } else {
            const profile = customerMap.get(email)!;
            profile.totalBookings += 1;
            if (b.bookingStatus.toLowerCase() === 'confirmed') {
              profile.totalSpent += (b.totalAmount || 0);
            }
            if (new Date(b.bookingTime || b.startTime) > new Date(profile.lastBookingDate)) {
              profile.lastBookingDate = b.bookingTime || b.startTime;
            }
          }
        });

        this.customers = Array.from(customerMap.values());
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching booking data for customer profiles:', err);
        this.errorMessage = 'Unable to aggregate customer profiles from database bookings.';
        this.isLoading = false;
      }
    });
  }

  // Filter customers by search keyword
  applyFilter(): void {
    if (!this.searchQuery.trim()) {
      this.filteredCustomers = [...this.customers];
      return;
    }

    const q = this.searchQuery.toLowerCase().trim();
    this.filteredCustomers = this.customers.filter(
      (c) =>
        c.name.toLowerCase().includes(q) ||
        c.email.toLowerCase().includes(q) ||
        c.phone.includes(q)
    );
  }

  // View user details modal
  viewCustomerDetails(customer: CustomerProfile): void {
    this.selectedCustomer = customer;
    this.showDetailsModal = true;
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedCustomer = null;
  }
}
