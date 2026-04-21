import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Result<T> {
  data: T;
  isSuccess: boolean;
  message: string;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface Announcement {
  id: string;
  title: string;
  content: string;
  createdDate: string;
  isGeneral: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7273/api/Announcements';

  getAnnouncements(page: number = 1, size: number = 10): Observable<Result<PagedResult<Announcement>>> {
    return this.http.get<Result<PagedResult<Announcement>>>(`${this.apiUrl}?pageNumber=${page}&pageSize=${size}`);
  }

  createAnnouncement(announcement: any): Observable<Result<any>> {
    return this.http.post<Result<any>>(this.apiUrl, announcement);
  }

  deleteAnnouncement(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/${id}`);
  }
}
