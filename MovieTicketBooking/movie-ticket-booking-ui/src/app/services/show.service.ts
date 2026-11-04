import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Show, CreateShowDto, UpdateShowDto } from '../models/show';
import { ShowSeatAvailabilitySummary } from '../models/seat';

@Injectable({
  providedIn: 'root'
})
export class ShowService {
  // Base URL pointing to the ASP.NET Core Shows API
  private readonly apiUrl = 'http://localhost:5020/api/shows';

  constructor(private http: HttpClient) {}

  // Get all shows
  getAllShows(): Observable<Show[]> {
    return this.http.get<Show[]>(this.apiUrl);
  }

  // Get single show by ID
  getShowById(id: number): Observable<Show> {
    return this.http.get<Show>(`${this.apiUrl}/${id}`);
  }

  // Filter shows by movieId, theatreId, and/or date
  getShowsFiltered(movieId?: number, theatreId?: number, date?: string): Observable<Show[]> {
    let params = new HttpParams();
    if (movieId) params = params.set('movieId', movieId.toString());
    if (theatreId) params = params.set('theatreId', theatreId.toString());
    if (date) params = params.set('date', date);

    return this.http.get<Show[]>(`${this.apiUrl}/filter`, { params });
  }

  // Get interactive seat availability summary for a show
  getSeatAvailability(showId: number): Observable<ShowSeatAvailabilitySummary> {
    return this.http.get<ShowSeatAvailabilitySummary>(`${this.apiUrl}/${showId}/availability`);
  }

  // Create a new show schedule (Admin)
  createShow(dto: CreateShowDto): Observable<Show> {
    return this.http.post<Show>(this.apiUrl, dto);
  }

  // Update show schedule / price (Admin)
  updateShow(id: number, dto: UpdateShowDto): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, dto);
  }

  // Delete a show schedule (Admin)
  deleteShow(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}
