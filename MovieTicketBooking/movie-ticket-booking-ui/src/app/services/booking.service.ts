import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking, CreateBookingRequest } from '../models/booking';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  // Base URL pointing to the ASP.NET Core Bookings API
  private readonly apiUrl = 'http://localhost:5020/api/bookings';

  constructor(private http: HttpClient) {}

  // Fetch all bookings from backend
  getAllBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(this.apiUrl);
  }

  // Fetch a single booking by ID
  getBookingById(id: number): Observable<Booking> {
    return this.http.get<Booking>(`${this.apiUrl}/${id}`);
  }

  // Place a new booking reservation
  createBooking(request: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(this.apiUrl, request);
  }

  // Cancel an existing booking and release seats back to Available
  cancelBooking(id: number): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}/cancel`, {});
  }
}
