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
}

