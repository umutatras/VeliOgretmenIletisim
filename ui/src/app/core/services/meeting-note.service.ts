import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, PagedResult } from './announcement.service';

export interface MeetingNote {
  id: string;
  note: string;
  teacherName: string;
  parentName: string;
  createdDate: string;
  studentName: string;
}

@Injectable({
  providedIn: 'root'
})
export class MeetingNoteService {
  private apiUrl = 'https://localhost:7273/api/MeetingNotes';

  constructor(private http: HttpClient) {}

  getAll(page: number = 1, size: number = 20): Observable<Result<PagedResult<MeetingNote>>> {
    return this.http.get<Result<PagedResult<MeetingNote>>>(`${this.apiUrl}?pageNumber=${page}&pageSize=${size}`);
  }

  create(parentId: string, note: string): Observable<Result<any>> {
    return this.http.post<Result<any>>(this.apiUrl, { parentId, note });
  }

  getMyStudentsForTeacher(): Observable<Result<PagedResult<any>>> {
    return this.http.get<Result<PagedResult<any>>>('https://localhost:7273/api/Teachers/my-students?pageSize=100');
  }

  delete(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/${id}`);
  }
}
