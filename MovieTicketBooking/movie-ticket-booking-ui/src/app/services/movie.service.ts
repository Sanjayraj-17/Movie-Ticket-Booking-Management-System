import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Movie, CreateMovieDto, UpdateMovieDto } from '../models/movie';

@Injectable({
  providedIn: 'root'
})
export class MovieService {
  // Base URL pointing to the ASP.NET Core Movies Web API
  private readonly apiUrl = 'http://localhost:5020/api/movies';

  constructor(private http: HttpClient) {}

  // Get all movies from backend
  getAllMovies(): Observable<Movie[]> {
    return this.http.get<Movie[]>(this.apiUrl);
  }

  // Get single movie by ID
  getMovieById(id: number): Observable<Movie> {
    return this.http.get<Movie>(`${this.apiUrl}/${id}`);
  }

  // Search movies using optional filters
  searchMovies(title?: string, genre?: string, language?: string): Observable<Movie[]> {
    let params = new HttpParams();
    if (title) params = params.set('title', title);
    if (genre) params = params.set('genre', genre);
    if (language) params = params.set('language', language);

    return this.http.get<Movie[]>(`${this.apiUrl}/search`, { params });
  }

  // Create a new movie (Admin only)
  createMovie(dto: CreateMovieDto): Observable<Movie> {
    return this.http.post<Movie>(this.apiUrl, dto);
  }

  // Update an existing movie (Admin only)
  updateMovie(id: number, dto: UpdateMovieDto): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, dto);
  }

  // Delete a movie by ID (Admin only)
  deleteMovie(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}
