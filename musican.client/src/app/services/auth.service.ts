import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

export interface LoginRequest {
  nameormail: string;
  password: string;
}

export interface AuthResponse {
  authToken: string;
  name: string;
  expireTime: string;
}

export interface User {
  token: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7012/Auth';
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    // Check if user is logged in on service initialization
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      const user = JSON.parse(storedUser);
      if (this.isTokenValid(user.token)) {
        this.currentUserSubject.next(user);
      } else {
        this.logout();
      }
    }
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap(response => {
          const user: User = {
            name: response.name,
            token: response.authToken
          };

          localStorage.setItem('currentUser', JSON.stringify(user));
          localStorage.setItem('tokenExpiry', response.expireTime);
          this.currentUserSubject.next(user);
        })
      );
  }

  register(userData: FormData): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, userData)
      .pipe(
        tap(response => {
          const user: User = {
            name: response.name,
            token: response.authToken
          };

          localStorage.setItem('currentUser', JSON.stringify(user));
          localStorage.setItem('tokenExpiry', response.expireTime);
          this.currentUserSubject.next(user);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('currentUser');
    localStorage.removeItem('tokenExpiry');
    this.currentUserSubject.next(null);
    this.router.navigate(['/home']);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isLoggedIn(): boolean {
    const user = this.getCurrentUser();
    return user !== null && this.isTokenValid(user.token);
  }

  getToken(): string | null {
    const user = this.getCurrentUser();
    return user ? user.token : null;
  }

  private isTokenValid(token: string): boolean {
    if (!token) return false;

    const expiry = localStorage.getItem('tokenExpiry');
    if (!expiry) return false;

    const expiryDate = new Date(expiry);
    const now = new Date();

    return expiryDate > now;
  }
}
