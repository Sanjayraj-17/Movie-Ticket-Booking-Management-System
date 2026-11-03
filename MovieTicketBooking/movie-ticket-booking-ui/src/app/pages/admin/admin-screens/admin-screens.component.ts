import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ScreenService } from '../../../services/screen.service';
import { TheatreService } from '../../../services/theatre.service';
import { SeatService } from '../../../services/seat.service';
import { AuthService } from '../../../services/auth.service';
import { Screen, Theatre } from '../../../models/theatre';
import { PhysicalSeat, CreateSeatDto, BulkCreateSeatsDto } from '../../../models/seat';

@Component({
  selector: 'app-admin-screens',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin-screens.component.html',
  styleUrl: './admin-screens.component.scss'
})
export class AdminScreensComponent implements OnInit {
  // Screens & Theatres
  screens: Screen[] = [];
  filteredScreens: Screen[] = [];
  theatres: Theatre[] = [];

  // Filter states
  selectedTheatreId: string = 'All';
  searchQuery: string = '';

  // Loading & Alert states
  isLoading: boolean = false;
  isSaving: boolean = false;
  isDeleting: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  // Screen Add / Edit Modal state
  showScreenModal: boolean = false;
  isEditMode: boolean = false;
  currentScreenId: number | null = null;
  screenForm = {
    theatreId: 0,
    screenName: '',
    totalSeats: 60
  };

  // Screen to delete
  screenToDelete: Screen | null = null;

  // Seat Management for a selected screen
  activeScreenForSeats: Screen | null = null;
  screenSeats: PhysicalSeat[] = [];
  groupedSeats: { row: string; seats: PhysicalSeat[] }[] = [];
  isLoadingSeats: boolean = false;

  // Bulk Seat Generator Form
  bulkForm: BulkCreateSeatsDto = {
    screenId: 0,
    rowNumber: 'A',
    totalSeatsInRow: 10,
    seatType: 'Standard'
  };

  // Single Seat Add Form
  singleSeatForm: CreateSeatDto = {
    screenId: 0,
    rowNumber: 'A',
    seatNumber: '1',
    seatType: 'Standard'
  };

  constructor(
    public authService: AuthService,
    private screenService: ScreenService,
    private theatreService: TheatreService,
    private seatService: SeatService
  ) {}

  ngOnInit(): void {
    this.loadTheatres();
    this.loadScreens();
  }

  // Load theatres for dropdowns
  loadTheatres(): void {
    this.theatreService.getAllTheatres().subscribe({
      next: (data) => (this.theatres = data),
      error: (err) => console.error('Error loading theatres:', err)
    });
  }

  // Load all screens
  loadScreens(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.screenService.getAllScreens().subscribe({
      next: (data) => {
        this.screens = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading screens:', err);
        this.errorMessage = 'Failed to load screens from backend.';
        this.isLoading = false;
      }
    });
  }

  // Filter screens by theatre and query
  applyFilter(): void {
    let list = [...this.screens];

    if (this.selectedTheatreId !== 'All') {
      const tId = Number(this.selectedTheatreId);
      list = list.filter((s) => s.theatreId === tId);
    }

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase().trim();
      list = list.filter((s) =>
        s.screenName.toLowerCase().includes(q) ||
        (s.theatreName && s.theatreName.toLowerCase().includes(q))
      );
    }

    this.filteredScreens = list;
  }

  // Open modal for Adding Screen
  openAddScreenModal(): void {
    this.isEditMode = false;
    this.currentScreenId = null;
    this.screenForm = {
      theatreId: this.theatres.length > 0 ? this.theatres[0].id : 0,
      screenName: '',
      totalSeats: 60
    };
    this.showScreenModal = true;
  }

  // Open modal for Editing Screen
  openEditScreenModal(screen: Screen): void {
    this.isEditMode = true;
    this.currentScreenId = screen.id;
    this.screenForm = {
      theatreId: screen.theatreId,
      screenName: screen.screenName,
      totalSeats: screen.totalSeats
    };
    this.showScreenModal = true;
  }

  closeScreenModal(): void {
    this.showScreenModal = false;
  }

  // Save Screen (Create or Update)
  saveScreen(): void {
    if (!this.screenForm.theatreId) {
      this.errorMessage = 'Please select a cinema theatre.';
      return;
    }

    if (!this.screenForm.screenName.trim()) {
      this.errorMessage = 'Screen name is required.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    if (this.isEditMode && this.currentScreenId) {
      this.screenService.updateScreen(this.currentScreenId, this.screenForm).subscribe({
        next: () => {
          this.isSaving = false;
          this.showScreenModal = false;
          this.successMessage = `Screen "${this.screenForm.screenName}" updated successfully.`;
          this.loadScreens();
          setTimeout(() => (this.successMessage = ''), 4000);
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Failed to update screen.';
        }
      });
    } else {
      this.screenService.createScreen(this.screenForm).subscribe({
        next: (created) => {
          this.isSaving = false;
          this.showScreenModal = false;
          this.successMessage = `Screen "${created.screenName}" added successfully.`;
          this.loadScreens();
          setTimeout(() => (this.successMessage = ''), 4000);
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Failed to create screen.';
        }
      });
    }
  }

  // Confirm delete screen
  confirmDeleteScreen(screen: Screen): void {
    this.screenToDelete = screen;
  }

  cancelDeleteScreen(): void {
    this.screenToDelete = null;
  }

  executeDeleteScreen(): void {
    if (!this.screenToDelete) return;

    this.isDeleting = true;
    this.errorMessage = '';

    this.screenService.deleteScreen(this.screenToDelete.id).subscribe({
      next: () => {
        this.isDeleting = false;
        this.successMessage = `Screen "${this.screenToDelete?.screenName}" deleted successfully.`;
        this.screenToDelete = null;
        this.loadScreens();
        setTimeout(() => (this.successMessage = ''), 4000);
      },
      error: (err) => {
        this.isDeleting = false;
        this.errorMessage = err.error?.message || 'Failed to delete screen. Ensure no active shows exist on this screen.';
      }
    });
  }

  // ==========================================
  // SEAT MANAGEMENT METHODS
  // ==========================================
  openSeatManager(screen: Screen): void {
    this.activeScreenForSeats = screen;
    this.bulkForm.screenId = screen.id;
    this.singleSeatForm.screenId = screen.id;
    this.loadSeatsForScreen(screen.id);
  }

  closeSeatManager(): void {
    this.activeScreenForSeats = null;
    this.screenSeats = [];
    this.groupedSeats = [];
  }

  loadSeatsForScreen(screenId: number): void {
    this.isLoadingSeats = true;
    this.seatService.getSeatsByScreenId(screenId).subscribe({
      next: (seats) => {
        this.screenSeats = seats;
        this.groupSeatsByRow(seats);
        this.isLoadingSeats = false;
      },
      error: (err) => {
        console.error('Error fetching seats:', err);
        this.isLoadingSeats = false;
      }
    });
  }

  groupSeatsByRow(seats: PhysicalSeat[]): void {
    const rowMap = new Map<string, PhysicalSeat[]>();
    seats.forEach((seat) => {
      const row = seat.rowNumber.toUpperCase();
      if (!rowMap.has(row)) {
        rowMap.set(row, []);
      }
      rowMap.get(row)!.push(seat);
    });

    const rows: { row: string; seats: PhysicalSeat[] }[] = [];
    Array.from(rowMap.keys())
      .sort()
      .forEach((rowKey) => {
        const sortedSeats = rowMap.get(rowKey)!.sort((a, b) => {
          const numA = parseInt(a.seatNumber) || 0;
          const numB = parseInt(b.seatNumber) || 0;
          return numA - numB;
        });
        rows.push({ row: rowKey, seats: sortedSeats });
      });

    this.groupedSeats = rows;
  }

  // Bulk Generate Row of Seats
  generateBulkSeats(): void {
    if (!this.bulkForm.rowNumber.trim() || this.bulkForm.totalSeatsInRow <= 0) {
      this.errorMessage = 'Please provide a valid row letter and seat count.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    this.seatService.bulkCreateSeats(this.bulkForm).subscribe({
      next: (created) => {
        this.isSaving = false;
        this.successMessage = `Generated ${created.length} seats for Row ${this.bulkForm.rowNumber.toUpperCase()}.`;
        if (this.activeScreenForSeats) {
          this.loadSeatsForScreen(this.activeScreenForSeats.id);
        }
        setTimeout(() => (this.successMessage = ''), 4000);
      },
      error: (err) => {
        this.isSaving = false;
        this.errorMessage = err.error?.message || 'Failed to bulk generate seats.';
      }
    });
  }

  // Delete Individual Seat
  deleteSingleSeat(seatId: number): void {
    this.seatService.deleteSeat(seatId).subscribe({
      next: () => {
        if (this.activeScreenForSeats) {
          this.loadSeatsForScreen(this.activeScreenForSeats.id);
        }
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to remove seat.';
      }
    });
  }
}
