import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Show } from '../../models/show';
import { ShowSeat, ShowSeatAvailabilitySummary, SeatRowGroup } from '../../models/seat';
import { ShowService } from '../../services/show.service';

@Component({
  selector: 'app-seat-selection',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './seat-selection.component.html',
  styleUrl: './seat-selection.component.scss'
})
export class SeatSelectionComponent implements OnInit {
  showId!: number;
  show: Show | null = null;
  availabilitySummary: ShowSeatAvailabilitySummary | null = null;
  rowGroups: SeatRowGroup[] = [];

  // Selected seats state
  selectedSeats: ShowSeat[] = [];

  // Loading and error states
  isLoading: boolean = true;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private showService: ShowService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('showId');
    if (idParam) {
      this.showId = Number(idParam);
      this.loadShowAndSeats();
    } else {
      this.errorMessage = 'No valid show identifier provided.';
      this.isLoading = false;
    }
  }

  // Load show details and real-time seat availability
  loadShowAndSeats(): void {
    this.isLoading = true;
    this.errorMessage = '';

    // 1. Fetch show metadata
    this.showService.getShowById(this.showId).subscribe({
      next: (showData) => {
        this.show = showData;

        // 2. Fetch live seat inventory and availability grid
        this.showService.getSeatAvailability(this.showId).subscribe({
          next: (summaryData) => {
            this.availabilitySummary = summaryData;
            this.groupSeatsByRow(summaryData.seats || []);
            this.isLoading = false;
          },
          error: (err) => {
            console.error('Error fetching seat availability:', err);
            this.errorMessage = 'Unable to load seating chart for this show.';
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        console.error('Error fetching show details:', err);
        this.errorMessage = 'Show not found or unavailable.';
        this.isLoading = false;
      }
    });
  }

  // Group physical seats into rows (e.g., Row A, Row B)
  private groupSeatsByRow(seats: ShowSeat[]): void {
    const rowMap = new Map<string, ShowSeat[]>();

    for (const seat of seats) {
      const rowKey = (seat.row || 'A').toUpperCase();
      if (!rowMap.has(rowKey)) {
        rowMap.set(rowKey, []);
      }
      rowMap.get(rowKey)!.push(seat);
    }

    this.rowGroups = Array.from(rowMap.entries())
      .map(([rowLabel, rowSeats]) => ({
        rowLabel,
        seats: rowSeats.sort((a, b) =>
          a.seatNumber.localeCompare(b.seatNumber, undefined, { numeric: true })
        )
      }))
      .sort((a, b) => a.rowLabel.localeCompare(b.rowLabel));
  }

  // Check if a seat is currently selected
  isSeatSelected(seat: ShowSeat): boolean {
    return this.selectedSeats.some((s) => s.seatId === seat.seatId);
  }

  // Toggle selection of an available seat
  toggleSeat(seat: ShowSeat): void {
    // Prevent selection of booked seats
    if (seat.status.toLowerCase() !== 'available') {
      return;
    }

    const index = this.selectedSeats.findIndex((s) => s.seatId === seat.seatId);
    if (index > -1) {
      // Deselect seat
      this.selectedSeats.splice(index, 1);
    } else {
      // Select seat
      this.selectedSeats.push(seat);
    }
  }

  // Clear all selected seats
  clearSelection(): void {
    this.selectedSeats = [];
  }

  // Calculate live total price for all selected seats
  calculateTotalAmount(): number {
    return this.selectedSeats.reduce((sum, seat) => sum + (seat.price || 0), 0);
  }

  // Proceed to Booking Confirmation (Feature 6)
  proceedToConfirmation(): void {
    if (this.selectedSeats.length === 0 || !this.show) {
      return;
    }

    const pendingBooking = {
      showId: this.show.id,
      movieTitle: this.show.movieTitle,
      theatreName: this.show.theatreName,
      screenName: this.show.screenName,
      startTime: this.show.startTime,
      seats: this.selectedSeats,
      totalAmount: this.calculateTotalAmount()
    };

    sessionStorage.setItem('pending_booking', JSON.stringify(pendingBooking));
    this.router.navigate(['/booking/confirmation']);
  }
}
