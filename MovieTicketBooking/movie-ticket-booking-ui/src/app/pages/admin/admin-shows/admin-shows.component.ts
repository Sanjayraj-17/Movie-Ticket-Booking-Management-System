import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ShowService } from '../../../services/show.service';
import { MovieService } from '../../../services/movie.service';
import { TheatreService } from '../../../services/theatre.service';
import { ScreenService } from '../../../services/screen.service';
import { AuthService } from '../../../services/auth.service';
import { Show, CreateShowDto, UpdateShowDto } from '../../../models/show';
import { Movie } from '../../../models/movie';
import { Theatre, Screen } from '../../../models/theatre';

@Component({
  selector: 'app-admin-shows',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './admin-shows.component.html',
  styleUrl: './admin-shows.component.scss'
})
export class AdminShowsComponent implements OnInit {
  // Shows list
  shows: Show[] = [];
  filteredShows: Show[] = [];

  // Dropdown reference data
  movies: Movie[] = [];
  theatres: Theatre[] = [];
  screens: Screen[] = [];
  theatreScreensForModal: Screen[] = [];

  // Filter states
  searchQuery: string = '';
  selectedMovieId: string = 'All';
  selectedTheatreId: string = 'All';
  selectedDate: string = '';

  // Loading & Alert states
  isLoading: boolean = false;
  isSaving: boolean = false;
  isDeleting: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  // Add / Edit Modal state
  showModal: boolean = false;
  isEditMode: boolean = false;
  currentShowId: number | null = null;

  // Add Show Form
  showForm = {
    movieId: 0,
    theatreId: 0,
    screenId: 0,
    startTime: '',
    endTime: '',
    price: 12.00
  };

  // Edit Show Form (backend UpdateShowDto takes StartTime, EndTime, Price)
  editForm: UpdateShowDto = {
    startTime: '',
    endTime: '',
    price: 12.00
  };
  editingShowInfo: { movieTitle: string; theatreName: string; screenName: string } | null = null;

  // Delete confirmation
  showToDelete: Show | null = null;

  constructor(
    public authService: AuthService,
    private showService: ShowService,
    private movieService: MovieService,
    private theatreService: TheatreService,
    private screenService: ScreenService
  ) {}

  ngOnInit(): void {
    this.loadReferenceData();
    this.loadShows();
  }

  // Load Movies, Theatres, Screens for forms & filters
  loadReferenceData(): void {
    this.movieService.getAllMovies().subscribe({
      next: (data) => (this.movies = data),
      error: (err) => console.error('Error loading movies:', err)
    });

    this.theatreService.getAllTheatres().subscribe({
      next: (data) => (this.theatres = data),
      error: (err) => console.error('Error loading theatres:', err)
    });

    this.screenService.getAllScreens().subscribe({
      next: (data) => (this.screens = data),
      error: (err) => console.error('Error loading screens:', err)
    });
  }

  // Load all shows from backend
  loadShows(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.showService.getAllShows().subscribe({
      next: (data) => {
        this.shows = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading shows:', err);
        this.errorMessage = 'Failed to load show schedules from backend.';
        this.isLoading = false;
      }
    });
  }

  // Filter shows by query, movie, theatre, date
  applyFilter(): void {
    let list = [...this.shows];

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase().trim();
      list = list.filter((s) =>
        s.movieTitle.toLowerCase().includes(q) ||
        s.theatreName.toLowerCase().includes(q) ||
        s.screenName.toLowerCase().includes(q)
      );
    }

    if (this.selectedMovieId !== 'All') {
      const mId = Number(this.selectedMovieId);
      list = list.filter((s) => s.movieId === mId);
    }

    if (this.selectedTheatreId !== 'All') {
      const tId = Number(this.selectedTheatreId);
      list = list.filter((s) => s.theatreId === tId);
    }

    if (this.selectedDate) {
      list = list.filter((s) => s.startTime && s.startTime.startsWith(this.selectedDate));
    }

    this.filteredShows = list;
  }

  // When Theatre changes in Add Modal, filter available screens
  onTheatreChangeInModal(): void {
    const tId = Number(this.showForm.theatreId);
    this.theatreScreensForModal = this.screens.filter((s) => s.theatreId === tId);
    if (this.theatreScreensForModal.length > 0) {
      this.showForm.screenId = this.theatreScreensForModal[0].id;
    } else {
      this.showForm.screenId = 0;
    }
  }

  // When Movie changes or Start Time changes, auto-suggest End Time
  onMovieOrStartTimeChange(): void {
    if (!this.showForm.startTime) return;

    const movie = this.movies.find((m) => m.id === Number(this.showForm.movieId));
    const duration = movie && movie.durationInMinutes > 0 ? movie.durationInMinutes : 120;

    const start = new Date(this.showForm.startTime);
    if (!isNaN(start.getTime())) {
      // Add duration + 20 minutes buffer for cleaning/trailers
      const end = new Date(start.getTime() + (duration + 20) * 60000);
      this.showForm.endTime = this.formatDateTimeLocal(end);
    }
  }

  // Helper to format Date into datetime-local string (YYYY-MM-DDTHH:mm)
  formatDateTimeLocal(date: Date): string {
    const pad = (n: number) => (n < 10 ? '0' + n : n);
    const year = date.getFullYear();
    const month = pad(date.getMonth() + 1);
    const day = pad(date.getDate());
    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  // Open Add Show Modal
  openAddModal(): void {
    this.isEditMode = false;
    this.currentShowId = null;

    const now = new Date();
    now.setHours(now.getHours() + 2, 0, 0, 0); // default 2 hours from now

    const defaultMovieId = this.movies.length > 0 ? this.movies[0].id : 0;
    const defaultTheatreId = this.theatres.length > 0 ? this.theatres[0].id : 0;

    this.showForm = {
      movieId: defaultMovieId,
      theatreId: defaultTheatreId,
      screenId: 0,
      startTime: this.formatDateTimeLocal(now),
      endTime: '',
      price: 12.00
    };

    this.onTheatreChangeInModal();
    this.onMovieOrStartTimeChange();
    this.showModal = true;
  }

  // Open Edit Show Modal
  openEditModal(show: Show): void {
    this.isEditMode = true;
    this.currentShowId = show.id;
    this.editingShowInfo = {
      movieTitle: show.movieTitle,
      theatreName: show.theatreName,
      screenName: show.screenName
    };

    const start = new Date(show.startTime);
    const end = new Date(show.endTime);

    this.editForm = {
      startTime: this.formatDateTimeLocal(start),
      endTime: this.formatDateTimeLocal(end),
      price: show.price
    };

    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }

  // Save Show (Create or Update)
  saveShow(): void {
    this.isSaving = true;
    this.errorMessage = '';

    if (this.isEditMode && this.currentShowId) {
      if (!this.editForm.startTime || !this.editForm.endTime) {
        this.errorMessage = 'Start time and end time are required.';
        this.isSaving = false;
        return;
      }

      if (new Date(this.editForm.endTime) <= new Date(this.editForm.startTime)) {
        this.errorMessage = 'End time must be later than start time.';
        this.isSaving = false;
        return;
      }

      const updateDto: UpdateShowDto = {
        startTime: new Date(this.editForm.startTime).toISOString(),
        endTime: new Date(this.editForm.endTime).toISOString(),
        price: Number(this.editForm.price)
      };

      this.showService.updateShow(this.currentShowId, updateDto).subscribe({
        next: () => {
          this.isSaving = false;
          this.showModal = false;
          this.successMessage = 'Show schedule updated successfully.';
          this.loadShows();
          setTimeout(() => (this.successMessage = ''), 4000);
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Failed to update show schedule.';
        }
      });
    } else {
      if (!this.showForm.movieId || !this.showForm.screenId) {
        this.errorMessage = 'Please select a movie and auditorium screen.';
        this.isSaving = false;
        return;
      }

      if (!this.showForm.startTime || !this.showForm.endTime) {
        this.errorMessage = 'Start time and end time are required.';
        this.isSaving = false;
        return;
      }

      if (new Date(this.showForm.endTime) <= new Date(this.showForm.startTime)) {
        this.errorMessage = 'End time must be later than start time.';
        this.isSaving = false;
        return;
      }

      const createDto: CreateShowDto = {
        movieId: Number(this.showForm.movieId),
        screenId: Number(this.showForm.screenId),
        startTime: new Date(this.showForm.startTime).toISOString(),
        endTime: new Date(this.showForm.endTime).toISOString(),
        price: Number(this.showForm.price)
      };

      this.showService.createShow(createDto).subscribe({
        next: (created) => {
          this.isSaving = false;
          this.showModal = false;
          this.successMessage = `Show for "${created.movieTitle}" scheduled successfully.`;
          this.loadShows();
          setTimeout(() => (this.successMessage = ''), 4000);
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Failed to schedule show. Check for overlaps or ensure screen has seats.';
        }
      });
    }
  }

  // Delete Confirmation
  confirmDelete(show: Show): void {
    this.showToDelete = show;
  }

  cancelDelete(): void {
    this.showToDelete = null;
  }

  executeDelete(): void {
    if (!this.showToDelete) return;

    this.isDeleting = true;
    this.errorMessage = '';

    this.showService.deleteShow(this.showToDelete.id).subscribe({
      next: () => {
        this.isDeleting = false;
        this.successMessage = `Show #${this.showToDelete?.id} (${this.showToDelete?.movieTitle}) deleted successfully.`;
        this.showToDelete = null;
        this.loadShows();
        setTimeout(() => (this.successMessage = ''), 4000);
      },
      error: (err) => {
        this.isDeleting = false;
        this.errorMessage = err.error?.message || 'Failed to delete show schedule.';
      }
    });
  }
}

