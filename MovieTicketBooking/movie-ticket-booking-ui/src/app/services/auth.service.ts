import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Base URL pointing to the ASP.NET Core Web API
  private readonly apiUrl = 'http://localhost:5020/api/auth';

  // Reactive signals for the logged-in user state
  currentUser = signal<User | null>(this.getStoredUser());
  isLoggedIn = signal<boolean>(!!this.getStoredToken());

  constructor(private http: HttpClient) {}

  // Login a user and store JWT token & profile in browser localStorage
  login(request: LoginRequest): Observable<{ message: string; data: AuthResponse }> {
    return this.http.post<{ message: string; data: AuthResponse }>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        if (response && response.data) {
          this.setSession(response.data);
        }
      })
    );
  }

  // Register a new customer
  register(request: RegisterRequest): Observable<{ message: string; data: AuthResponse }> {
    return this.http.post<{ message: string; data: AuthResponse }>(`${this.apiUrl}/register`, request).pipe(
      tap((response) => {
        if (response && response.data) {
          this.setSession(response.data);
        }
      })
    );
  }

  // Save session to localStorage and update reactive signals
  private setSession(authData: AuthResponse): void {
    localStorage.setItem('auth_token', authData.token);
    const user: User = {
      userId: authData.userId,
      fullName: authData.fullName,
      email: authData.email,
      role: authData.role
    };
    localStorage.setItem('auth_user', JSON.stringify(user));
    this.currentUser.set(user);
    this.isLoggedIn.set(true);
  }

  // Log out current user and clear stored credentials
  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('auth_user');
    this.currentUser.set(null);
    this.isLoggedIn.set(false);
  }

  // Read stored JWT token
  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  // Read stored user from localStorage safely
  getStoredUser(): User | null {
    const raw = localStorage.getItem('auth_user');
    if (!raw) return null;
    try {
      return JSON.parse(raw) as User;
    } catch {
      return null;
    }
  }

  private getStoredToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  // Check if current user has Admin role
  isAdmin(): boolean {
    const user = this.currentUser();
    return user?.role?.toLowerCase() === 'admin';
  }
}
