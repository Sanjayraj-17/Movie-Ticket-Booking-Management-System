import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Theatre } from '../models/theatre';

@Injectable({
  providedIn: 'root'
})
export class TheatreService {
  // Base URL pointing to the ASP.NET Core Theatres Web API
  private readonly apiUrl = 'http://localhost:5020/api/theatres';

  constructor(private http: HttpClient) {}

  // Fetch all cinema theatres
  getAllTheatres(): Observable<Theatre[]> {
    return this.http.get<Theatre[]>(this.apiUrl);
  }

  // Fetch single theatre by ID
  getTheatreById(id: number): Observable<Theatre> {
    return this.http.get<Theatre>(`${this.apiUrl}/${id}`);
  }

  // Create a new theatre (Admin)
  createTheatre(theatre: Partial<Theatre>): Observable<Theatre> {
    return this.http.post<Theatre>(this.apiUrl, theatre);
  }

  // Update an existing theatre (Admin)
  updateTheatre(id: number, theatre: Partial<Theatre>): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, theatre);
  }

  // Delete a theatre by ID (Admin)
  deleteTheatre(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}

