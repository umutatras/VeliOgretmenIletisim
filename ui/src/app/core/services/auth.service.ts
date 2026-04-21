import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

export interface LoginResponse {
  isSuccess: boolean;
  message: string;
  data: {
    token: string;
    userName: string;
    role: string;
    profilePicturePath?: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7273/api/Auth';
  
  currentUser = signal<{ userName: string, role?: string, profilePicturePath?: string } | null>(null);
  token = signal<string | null>(localStorage.getItem('token'));

  constructor(private http: HttpClient, private router: Router) {
    const savedUser = localStorage.getItem('user');
    if (savedUser) {
      this.currentUser.set(JSON.parse(savedUser));
    }
  }

  async login(email: string, password: string): Promise<boolean> {
    try {
      const response = await firstValueFrom(
        this.http.post<LoginResponse>(`${this.apiUrl}/login`, { email, password })
      );

      if (response.isSuccess && response.data) {
        const userData = { 
          userName: response.data.userName, 
          role: response.data.role,
          profilePicturePath: response.data.profilePicturePath
        };
        
        this.token.set(response.data.token);
        this.currentUser.set(userData);
        
        localStorage.setItem('token', response.data.token);
        localStorage.setItem('user', JSON.stringify(userData));
        
        return true;
      }
      return false;
    } catch (error) {
      console.error('Login failed', error);
      return false;
    }
  }

  register(command: any) {
    return this.http.post<{ isSuccess: boolean, message: string }>(`${this.apiUrl}/register`, command);
  }

  logout() {
    this.token.set(null);
    this.currentUser.set(null);
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  isLoggedIn(): boolean {
    return !!this.token();
  }

  getToken(): string | null {
    return this.token() || localStorage.getItem('token');
  }
}
