import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';

export interface LoginResponse {
  isSuccess: boolean;
  message: string;
  data: {
    token: string;
    fullName: string;
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
        const rawData = response.data as any;
        const userData = { 
          userName: rawData.fullName || rawData.FullName, 
          role: rawData.role || rawData.Role,
          profilePicturePath: rawData.profilePicturePath || rawData.ProfilePicturePath
        };
        
        console.log('Login successful, user data mapped:', userData);

        this.token.set(rawData.token || rawData.Token);
        this.currentUser.set(userData);
        
        localStorage.setItem('token', rawData.token || rawData.Token);
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
