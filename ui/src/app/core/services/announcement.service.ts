import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Announcement {
  id: string;
  title: string;
  content: string;
  teacherName: string;
  createdDate: string;
  attachmentPath?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface Result<T> {
  isSuccess: boolean;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  private apiUrl = 'https://localhost:7273/api/Announcements';

  constructor(private http: HttpClient) {}

  getAll(page: number = 1, size: number = 10): Observable<Result<PagedResult<Announcement>>> {
    return this.http.get<Result<PagedResult<Announcement>>>(`${this.apiUrl}?pageNumber=${page}&pageSize=${size}`);
  }

  create(command: any): Observable<Result<any>> {
    return this.http.post<Result<any>>(this.apiUrl, command);
  }

  delete(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/${id}`);
  }
}
