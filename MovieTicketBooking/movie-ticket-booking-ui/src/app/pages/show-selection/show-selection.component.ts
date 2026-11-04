import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Movie } from '../../models/movie';
import { Theatre } from '../../models/theatre';
import { Show } from '../../models/show';
import { MovieService } from '../../services/movie.service';
import { TheatreService } from '../../services/theatre.service';
import { ShowService } from '../../services/show.service';

export interface DateOption {
  date: Date;
  isoString: string;
  dayName: string;
  displayDate: string;
}

@Component({
  selector: 'app-show-selection',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, CurrencyPipe],
  templateUrl: './show-selection.component.html',
  styleUrl: './show-selection.component.scss'
})
export class ShowSelectionComponent implements OnInit {
  // Available filter options
  movies: Movie[] = [];
  theatres: Theatre[] = [];
  dateOptions: DateOption[] = [];

  // Selected filter states
  selectedMovieId: number | null = null;
  selectedTheatreId: number | null = null;
  selectedDateIso: string = 'All';

  // Active loaded models
  selectedMovie: Movie | null = null;
  selectedTheatre: Theatre | null = null;
  shows: Show[] = [];
  selectedShow: Show | null = null;

  // Loading and error states
  isLoading: boolean = true;
  isLoadingShows: boolean = false;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private movieService: MovieService,
    private theatreService: TheatreService,
    private showService: ShowService
  ) {}

  ngOnInit(): void {
    this.generateDateOptions();
    this.loadInitialData();
  }

  // Generate 7-day calendar date options starting from today
  private generateDateOptions(): void {
    const options: DateOption[] = [];
    const today = new Date();

    for (let i = 0; i < 7; i++) {
      const d = new Date(today);
      d.setDate(today.getDate() + i);

      const year = d.getFullYear();
      const month = String(d.getMonth() + 1).padStart(2, '0');
      const day = String(d.getDate()).padStart(2, '0');
      const iso = `${year}-${month}-${day}`;

      let dayName = d.toLocaleDateString('en-US', { weekday: 'short' });
      if (i === 0) dayName = 'Today';
      if (i === 1) dayName = 'Tomorrow';

      const displayDate = d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });

      options.push({
        date: d,
        isoString: iso,
        dayName,
        displayDate
      });
    }

    this.dateOptions = options;
  }

  // Load movies and theatres lists, then inspect query parameters
  private loadInitialData(): void {
    this.isLoading = true;

    // Load movies
    this.movieService.getAllMovies().subscribe({
      next: (moviesData) => {
        this.movies = moviesData;

        // Load theatres
        this.theatreService.getAllTheatres().subscribe({
          next: (theatresData) => {
            this.theatres = theatresData;
            this.isLoading = false;

            // Process query parameters from route
            this.route.queryParamMap.subscribe((params) => {
              const movieIdParam = params.get('movieId');
              const theatreIdParam = params.get('theatreId');
              const dateParam = params.get('date');

              if (movieIdParam) {
                this.selectedMovieId = Number(movieIdParam);
                this.selectedMovie = this.movies.find((m) => m.id === this.selectedMovieId) || null;
              } else if (this.movies.length > 0) {
                this.selectedMovieId = this.movies[0].id;
                this.selectedMovie = this.movies[0];
              }

              if (theatreIdParam) {
                this.selectedTheatreId = Number(theatreIdParam);
                this.selectedTheatre = this.theatres.find((t) => t.id === this.selectedTheatreId) || null;
              }

              if (dateParam) {
                this.selectedDateIso = dateParam;
              }

              this.loadShows();
            });
          },
          error: (err) => {
            console.error('Error fetching theatres:', err);
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        console.error('Error fetching movies:', err);
        this.isLoading = false;
      }
    });
  }

  // Load scheduled shows filtered by selected Movie, Theatre, and Date
  loadShows(): void {
    this.isLoadingShows = true;
    this.selectedShow = null;
    this.errorMessage = '';

    const movieParam = this.selectedMovieId ? this.selectedMovieId : undefined;
    const theatreParam = this.selectedTheatreId ? this.selectedTheatreId : undefined;
    const dateParam = this.selectedDateIso !== 'All' ? this.selectedDateIso : undefined;

    this.showService.getShowsFiltered(movieParam, theatreParam, dateParam).subscribe({
      next: (showsData) => {
        this.shows = showsData;
        this.isLoadingShows = false;

        // Auto-select first show if available
        if (this.shows.length > 0) {
          this.selectedShow = this.shows[0];
        }
      },
      error: (err) => {
        console.error('Error loading shows:', err);
        this.errorMessage = 'Failed to load showtimes. Please try again.';
        this.isLoadingShows = false;
      }
    });
  }

  // When customer changes movie from dropdown
  onMovieChange(): void {
    this.selectedMovie = this.movies.find((m) => m.id === Number(this.selectedMovieId)) || null;
    this.loadShows();
  }

  // When customer changes theatre from dropdown
  onTheatreChange(): void {
    if (this.selectedTheatreId) {
      this.selectedTheatre = this.theatres.find((t) => t.id === Number(this.selectedTheatreId)) || null;
    } else {
      this.selectedTheatre = null;
    }
    this.loadShows();
  }

  // When customer clicks a calendar date pill
  onDateSelect(isoString: string): void {
    this.selectedDateIso = isoString;
    this.loadShows();
  }

  // When customer selects a specific showtime pill
  selectShow(show: Show): void {
    this.selectedShow = show;
  }

  // Proceed to seat selection (Feature 5)
  proceedToSeatSelection(): void {
    if (this.selectedShow) {
      this.router.navigate(['/booking/seats', this.selectedShow.id]);
    }
  }
}
