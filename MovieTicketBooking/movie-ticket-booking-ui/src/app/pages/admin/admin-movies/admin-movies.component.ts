import { Component, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MovieService } from '../../../services/movie.service';
import { AuthService } from '../../../services/auth.service';
import { Movie, CreateMovieDto, UpdateMovieDto } from '../../../models/movie';

@Component({
  selector: 'app-admin-movies',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe],
  templateUrl: './admin-movies.component.html',
  styleUrl: './admin-movies.component.scss'
})
export class AdminMoviesComponent implements OnInit {
  // All movies from API
  movies: Movie[] = [];
  filteredMovies: Movie[] = [];

  // Filter and search state
  searchQuery: string = '';
  selectedGenre: string = 'All';
  genres: string[] = ['All', 'Action', 'Sci-Fi', 'Drama', 'Comedy', 'Thriller', 'Animation', 'Horror', 'Adventure'];

  // Loading & notification states
  isLoading: boolean = false;
  isSaving: boolean = false;
  isDeleting: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  // Modal / Form state for Add or Edit
  showModal: boolean = false;
  isEditMode: boolean = false;
  currentMovieId: number | null = null;

  // Form Fields
  movieForm = {
    title: '',
    description: '',
    language: 'English',
    genre: 'Action',
    durationInMinutes: 120,
    releaseDate: new Date().toISOString().split('T')[0],
    posterUrl: ''
  };

  // Movie to delete confirmation
  movieToDelete: Movie | null = null;

  constructor(
    public authService: AuthService,
    private movieService: MovieService
  ) {}

  ngOnInit(): void {
    this.loadMovies();
  }

  // Load all movies from the backend API
  loadMovies(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.movieService.getAllMovies().subscribe({
      next: (data) => {
        this.movies = data;
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching movies:', err);
        this.errorMessage = 'Failed to load movie catalog. Please check backend connection.';
        this.isLoading = false;
      }
    });
  }

  // Filter movies by search keyword and genre
  applyFilter(): void {
    let list = [...this.movies];

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase().trim();
      list = list.filter((m) =>
        m.title.toLowerCase().includes(q) ||
        m.language.toLowerCase().includes(q) ||
        m.genre.toLowerCase().includes(q)
      );
    }

    if (this.selectedGenre !== 'All') {
      list = list.filter((m) => m.genre.toLowerCase() === this.selectedGenre.toLowerCase());
    }

    this.filteredMovies = list;
  }

  // Open modal for Adding a new movie
  openAddModal(): void {
    this.isEditMode = false;
    this.currentMovieId = null;
    this.movieForm = {
      title: '',
      description: '',
      language: 'English',
      genre: 'Action',
      durationInMinutes: 120,
      releaseDate: new Date().toISOString().split('T')[0],
      posterUrl: ''
    };
    this.showModal = true;
  }

  // Open modal for Editing an existing movie
  openEditModal(movie: Movie): void {
    this.isEditMode = true;
    this.currentMovieId = movie.id;
    this.movieForm = {
      title: movie.title,
      description: movie.description,
      language: movie.language,
      genre: movie.genre,
      durationInMinutes: movie.durationInMinutes,
      releaseDate: movie.releaseDate ? movie.releaseDate.split('T')[0] : '',
      posterUrl: movie.posterUrl || ''
    };
    this.showModal = true;
  }

  // Close form modal
  closeModal(): void {
    this.showModal = false;
  }

  // Submit Add or Edit Form
  saveMovie(): void {
    if (!this.movieForm.title.trim()) {
      this.errorMessage = 'Movie title is required.';
      return;
    }

    if (this.movieForm.durationInMinutes <= 0) {
      this.errorMessage = 'Duration must be greater than 0 minutes.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    if (this.isEditMode && this.currentMovieId) {
      // Update existing movie via PUT /api/movies/{id}
      const updateDto: UpdateMovieDto = {
        title: this.movieForm.title.trim(),
        description: this.movieForm.description.trim(),
        language: this.movieForm.language.trim(),
        genre: this.movieForm.genre.trim(),
        durationInMinutes: this.movieForm.durationInMinutes,
        releaseDate: new Date(this.movieForm.releaseDate).toISOString(),
        posterUrl: this.movieForm.posterUrl.trim()
      };

      this.movieService.updateMovie(this.currentMovieId, updateDto).subscribe({
        next: () => {
          this.isSaving = false;
          this.showModal = false;
          this.successMessage = `Movie "${this.movieForm.title}" updated successfully.`;
          this.loadMovies();
          setTimeout(() => (this.successMessage = ''), 4000);
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Failed to update movie.';
        }
      });
    } else {
      // Create new movie via POST /api/movies
      const createDto: CreateMovieDto = {
        title: this.movieForm.title.trim(),
        description: this.movieForm.description.trim(),
        language: this.movieForm.language.trim(),
        genre: this.movieForm.genre.trim(),
        durationInMinutes: this.movieForm.durationInMinutes,
        releaseDate: new Date(this.movieForm.releaseDate).toISOString(),
        posterUrl: this.movieForm.posterUrl.trim()
      };

      this.movieService.createMovie(createDto).subscribe({
        next: (created) => {
          this.isSaving = false;
          this.showModal = false;
          this.successMessage = `Movie "${created.title}" added to catalog successfully.`;
          this.loadMovies();
          setTimeout(() => (this.successMessage = ''), 4000);
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Failed to create movie.';
        }
      });
    }
  }

  // Open delete confirmation modal
  confirmDelete(movie: Movie): void {
    this.movieToDelete = movie;
  }

  // Cancel deletion
  cancelDelete(): void {
    this.movieToDelete = null;
  }

  // Execute deletion via DELETE /api/movies/{id}
  executeDelete(): void {
    if (!this.movieToDelete) return;

    this.isDeleting = true;
    this.errorMessage = '';

    this.movieService.deleteMovie(this.movieToDelete.id).subscribe({
      next: () => {
        this.isDeleting = false;
        this.successMessage = `Movie "${this.movieToDelete?.title}" deleted successfully.`;
        this.movieToDelete = null;
        this.loadMovies();
        setTimeout(() => (this.successMessage = ''), 4000);
      },
      error: (err) => {
        this.isDeleting = false;
        this.errorMessage = err.error?.message || 'Failed to delete movie.';
      }
    });
  }

  // Helper for poster image fallback
  onImageError(event: Event): void {
    (event.target as HTMLImageElement).src = 'https://images.unsplash.com/photo-1489599849927-2ee91cede3ba?q=80&w=600&auto=format&fit=crop';
  }
}
