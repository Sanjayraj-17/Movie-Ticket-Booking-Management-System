import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PhysicalSeat, CreateSeatDto, BulkCreateSeatsDto } from '../models/seat';

@Injectable({
  providedIn: 'root'
})
export class SeatService {
  // Base URL pointing to the ASP.NET Core Seats Web API
  private readonly apiUrl = 'http://localhost:5020/api/seats';

  constructor(private http: HttpClient) {}

  // Fetch all physical seats configured for a specific screen
  getSeatsByScreenId(screenId: number): Observable<PhysicalSeat[]> {
    return this.http.get<PhysicalSeat[]>(`${this.apiUrl}/screen/${screenId}`);
  }

  // Fetch single seat by ID
  getSeatById(id: number): Observable<PhysicalSeat> {
    return this.http.get<PhysicalSeat>(`${this.apiUrl}/${id}`);
  }

  // Create a single seat
  createSeat(dto: CreateSeatDto): Observable<PhysicalSeat> {
    return this.http.post<PhysicalSeat>(this.apiUrl, dto);
  }

  // Bulk generate seats for an entire row (e.g. Row 'A', 10 seats)
  bulkCreateSeats(dto: BulkCreateSeatsDto): Observable<PhysicalSeat[]> {
    return this.http.post<PhysicalSeat[]>(`${this.apiUrl}/bulk`, dto);
  }

  // Delete a seat by ID
  deleteSeat(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}
