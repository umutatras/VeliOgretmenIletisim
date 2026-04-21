import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from './announcement.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:7273/api/Users';

  constructor(private http: HttpClient) {}

  updateProfilePicture(path: string): Observable<Result<string>> {
    return this.http.post<Result<string>>(`${this.apiUrl}/profile-picture`, { profilePicturePath: path });
  }
}
