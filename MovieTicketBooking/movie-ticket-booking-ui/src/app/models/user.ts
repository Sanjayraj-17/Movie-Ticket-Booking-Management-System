// User and Authentication Models
export interface User {
  userId: number;
  fullName: string;
  email: string;
  role: string;
}

export interface AuthResponse {
  token: string;
  expiration: string;
  userId: number;
  fullName: string;
  email: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}
