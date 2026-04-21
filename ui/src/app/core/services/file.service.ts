import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from './announcement.service';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  private apiUrl = 'https://localhost:7273/api/Files';

  constructor(private http: HttpClient) {}

  upload(file: File, folder: string = 'general'): Observable<Result<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<Result<string>>(`${this.apiUrl}/upload?folder=${folder}`, formData);
  }
}
