import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, PagedResult } from './announcement.service';

export interface Complaint {
  id: string;
  subject: string;
  content: string;
  adminResponse?: string;
  status: string;
  parentName: string;
  createdDate: string;
}

@Injectable({
  providedIn: 'root'
})
export class ComplaintService {
  private apiUrl = 'http://localhost:5066/api/Complaints';

  constructor(private http: HttpClient) {}

  getComplaints(page: number = 1, size: number = 10): Observable<Result<PagedResult<Complaint>>> {
    return this.http.get<Result<PagedResult<Complaint>>>(`${this.apiUrl}?pageNumber=${page}&pageSize=${size}`);
  }

  create(subject: string, content: string): Observable<Result<any>> {
    return this.http.post<Result<any>>(this.apiUrl, { subject, content });
  }

  answer(complaintId: string, response: string, status: number): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/answer`, { complaintId, response, status });
  }
}
