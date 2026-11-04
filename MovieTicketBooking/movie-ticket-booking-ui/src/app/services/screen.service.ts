import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Screen } from '../models/theatre';

@Injectable({
  providedIn: 'root'
})
export class ScreenService {
  // Base URL pointing to the ASP.NET Core Screens Web API
  private readonly apiUrl = 'http://localhost:5020/api/screens';

  constructor(private http: HttpClient) {}

  // Fetch all screens across all theatres
  getAllScreens(): Observable<Screen[]> {
    return this.http.get<Screen[]>(this.apiUrl);
  }

  // Fetch a specific screen by ID
  getScreenById(id: number): Observable<Screen> {
    return this.http.get<Screen>(`${this.apiUrl}/${id}`);
  }

  // Fetch all auditorium screens belonging to a specific theatre
  getScreensByTheatreId(theatreId: number): Observable<Screen[]> {
    return this.http.get<Screen[]>(`${this.apiUrl}/theatre/${theatreId}`);
  }

  // Create a new screen in a theatre (Admin)
  createScreen(dto: { theatreId: number; screenName: string; totalSeats: number }): Observable<Screen> {
    return this.http.post<Screen>(this.apiUrl, dto);
  }

  // Update an existing screen (Admin)
  updateScreen(id: number, dto: { theatreId: number; screenName: string; totalSeats: number }): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, dto);
  }

  // Delete a screen by ID (Admin)
  deleteScreen(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}
