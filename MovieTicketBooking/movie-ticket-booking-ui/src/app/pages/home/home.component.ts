import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Movie } from '../../models/movie';
import { MovieService } from '../../services/movie.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  movies: Movie[] = [];
  errorMessage: string = '';
  isLoading: boolean = true;

  constructor(private movieService: MovieService) {}

  ngOnInit(): void {
    this.loadMovies();
  }

  loadMovies(): void {
    this.isLoading = true;
    this.movieService.getAllMovies().subscribe({
      next: (data) => {
        this.movies = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading movies:', error);
        this.errorMessage = 'Unable to load movies. Please make sure the backend is running.';
        this.isLoading = false;
      }
    });
  }

  scrollToMovies(): void {
    document.getElementById('movies')?.scrollIntoView({
      behavior: 'smooth'
    });
  }
}
