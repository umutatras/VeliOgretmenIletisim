import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from './announcement.service';

export interface UserProfile {
  id: string;
  userName: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  profilePicturePath?: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7273/api/Users';

  getProfile(): Observable<Result<UserProfile>> {
    return this.http.get<Result<UserProfile>>(`${this.apiUrl}/profile`);
  }

  updateProfilePicture(path: string): Observable<Result<string>> {
    return this.http.post<Result<string>>(`${this.apiUrl}/profile-picture`, { profilePicturePath: path });
  }

  changePassword(passwordData: any): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/change-password`, passwordData);
  }
}
