import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Movie } from '../../models/movie';
import { Show } from '../../models/show';
import { MovieService } from '../../services/movie.service';
import { ShowService } from '../../services/show.service';

export interface TheatreShowsGroup {
  theatreId: number;
  theatreName: string;
  shows: Show[];
}

@Component({
  selector: 'app-movie-details',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './movie-details.component.html',
  styleUrl: './movie-details.component.scss'
})
export class MovieDetailsComponent implements OnInit {
  movieId!: number;
  movie: Movie | null = null;
  shows: Show[] = [];
  theatreGroups: TheatreShowsGroup[] = [];

  isLoading: boolean = true;
  isLoadingShows: boolean = true;
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private movieService: MovieService,
    private showService: ShowService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.movieId = Number(idParam);
      this.loadMovieDetails();
      this.loadMovieShows();
    } else {
      this.errorMessage = 'Invalid movie identifier provided.';
      this.isLoading = false;
    }
  }

  // Fetch movie metadata by ID
  loadMovieDetails(): void {
    this.isLoading = true;
    this.movieService.getMovieById(this.movieId).subscribe({
      next: (data) => {
        this.movie = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching movie details:', err);
        this.errorMessage = 'Movie not found or failed to load.';
        this.isLoading = false;
      }
    });
  }

  // Fetch scheduled shows for this movie and group by theatre
  loadMovieShows(): void {
    this.isLoadingShows = true;
    this.showService.getShowsFiltered(this.movieId).subscribe({
      next: (data) => {
        this.shows = data;
        this.groupShowsByTheatre(data);
        this.isLoadingShows = false;
      },
      error: (err) => {
        console.error('Error fetching shows for movie:', err);
        this.isLoadingShows = false;
      }
    });
  }

  // Group shows list into distinct theatres for clear organization
  private groupShowsByTheatre(shows: Show[]): void {
    const groupMap = new Map<number, TheatreShowsGroup>();

    for (const show of shows) {
      if (!groupMap.has(show.theatreId)) {
        groupMap.set(show.theatreId, {
          theatreId: show.theatreId,
          theatreName: show.theatreName || `Theatre #${show.theatreId}`,
          shows: []
        });
      }
      groupMap.get(show.theatreId)!.shows.push(show);
    }

    this.theatreGroups = Array.from(groupMap.values());
  }

  // Smooth scroll to the showtimes schedule section
  scrollToShowtimes(): void {
    document.getElementById('showtimes-section')?.scrollIntoView({
      behavior: 'smooth'
    });
  }
}
