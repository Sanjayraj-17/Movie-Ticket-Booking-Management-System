import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Theatre, Screen } from '../../models/theatre';
import { Show } from '../../models/show';
import { Movie } from '../../models/movie';
import { TheatreService } from '../../services/theatre.service';
import { ScreenService } from '../../services/screen.service';
import { ShowService } from '../../services/show.service';
import { MovieService } from '../../services/movie.service';

@Component({
  selector: 'app-theatre-selection',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './theatre-selection.component.html',
  styleUrl: './theatre-selection.component.scss'
})
export class TheatreSelectionComponent implements OnInit {
  // Theatre list state
  theatres: Theatre[] = [];
  filteredTheatres: Theatre[] = [];
  cities: string[] = [];
  selectedCity: string = 'All';
  searchQuery: string = '';

  // Currently selected theatre
  selectedTheatre: Theatre | null = null;
  screens: Screen[] = [];
  shows: Show[] = [];

  // Optional contextual movie filter
  selectedMovieId: number | null = null;
  contextMovie: Movie | null = null;

  // Loading and error states
  isLoadingTheatres: boolean = true;
  isLoadingDetails: boolean = false;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private theatreService: TheatreService,
    private screenService: ScreenService,
    private showService: ShowService,
    private movieService: MovieService
  ) {}

  ngOnInit(): void {
    // Check if a movieId query parameter was provided
    this.route.queryParamMap.subscribe((params) => {
      const movieIdParam = params.get('movieId');
      if (movieIdParam) {
        this.selectedMovieId = Number(movieIdParam);
        this.loadContextMovie(this.selectedMovieId);
      }
      this.loadTheatres();
    });
  }

  // Load all theatres from the backend Theatre API
  loadTheatres(): void {
    this.isLoadingTheatres = true;
    this.errorMessage = '';

    this.theatreService.getAllTheatres().subscribe({
      next: (data) => {
        this.theatres = data;
        this.extractCities(data);
        this.applyFilters();
        this.isLoadingTheatres = false;

        // Auto-select the first theatre if available
        if (this.theatres.length > 0 && !this.selectedTheatre) {
          this.selectTheatre(this.theatres[0]);
        }
      },
      error: (err) => {
        console.error('Error fetching theatres:', err);
        this.errorMessage = 'Unable to load cinema theatres. Please verify backend connection.';
        this.isLoadingTheatres = false;
      }
    });
  }

  // Extract distinct cities for filtering
  private extractCities(theatres: Theatre[]): void {
    const citySet = new Set<string>();
    for (const t of theatres) {
      if (t.city && t.city.trim()) {
        citySet.add(t.city.trim());
      }
    }
    this.cities = ['All', ...Array.from(citySet).sort()];
  }

  // Filter theatres by city and search query
  applyFilters(): void {
    let result = [...this.theatres];

    if (this.selectedCity !== 'All') {
      result = result.filter(
        (t) => t.city.toLowerCase() === this.selectedCity.toLowerCase()
      );
    }

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase().trim();
      result = result.filter(
        (t) =>
          t.name.toLowerCase().includes(q) ||
          t.city.toLowerCase().includes(q) ||
          t.address.toLowerCase().includes(q)
      );
    }

    this.filteredTheatres = result;
  }

  // Select a theatre to view its active screens and scheduled shows
  selectTheatre(theatre: Theatre): void {
    this.selectedTheatre = theatre;
    this.isLoadingDetails = true;
    this.screens = [];
    this.shows = [];

    // 1. Fetch screens belonging to this theatre
    this.screenService.getScreensByTheatreId(theatre.id).subscribe({
      next: (screensData) => {
        this.screens = screensData;
      },
      error: (err) => {
        console.error('Error fetching screens for theatre:', err);
      }
    });

    // 2. Fetch shows scheduled at this theatre
    const movieIdParam = this.selectedMovieId ? this.selectedMovieId : undefined;
    this.showService.getShowsFiltered(movieIdParam, theatre.id).subscribe({
      next: (showsData) => {
        this.shows = showsData;
        this.isLoadingDetails = false;
      },
      error: (err) => {
        console.error('Error fetching shows for theatre:', err);
        this.isLoadingDetails = false;
      }
    });
  }

  // Load contextual movie title if filtered by movie
  private loadContextMovie(movieId: number): void {
    this.movieService.getMovieById(movieId).subscribe({
      next: (movie) => {
        this.contextMovie = movie;
      },
      error: (err) => console.error('Error fetching context movie:', err)
    });
  }

  // Clear contextual movie filter
  clearMovieFilter(): void {
    this.selectedMovieId = null;
    this.contextMovie = null;
    if (this.selectedTheatre) {
      this.selectTheatre(this.selectedTheatre);
    }
  }
}

