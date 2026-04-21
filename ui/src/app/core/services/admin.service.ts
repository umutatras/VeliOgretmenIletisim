import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, PagedResult } from './announcement.service';

export interface Department {
  id: string;
  name: string;
}

export interface Student {
  id: string;
  firstName: string;
  lastName: string;
  studentNumber: string;
  parentName: string;
}

export interface AuditLog {
  id: string;
  userId: string;
  action: string;
  httpMethod: string;
  requestPath: string;
  ipAddress: string;
  statusCode: number;
  executionTimeMs: number;
  createdDate: string;
  curlCommand?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = 'http://localhost:5066/api/Admin';

  constructor(private http: HttpClient) {}

  // Departments
  getDepartments(): Observable<Result<Department[]>> {
    return this.http.get<Result<Department[]>>(`${this.apiUrl}/departments`);
  }

  createDepartment(name: string): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/departments`, { name });
  }

  deleteDepartment(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/departments/${id}`);
  }

  // Students
  getStudents(page: number = 1, size: number = 10): Observable<Result<PagedResult<Student>>> {
    return this.http.get<Result<PagedResult<Student>>>(`${this.apiUrl}/students?pageNumber=${page}&pageSize=${size}`);
  }

  // Audit Logs
  getAuditLogs(page: number = 1, size: number = 20): Observable<Result<PagedResult<AuditLog>>> {
    return this.http.get<Result<PagedResult<AuditLog>>>(`${this.apiUrl}/audit-logs?pageNumber=${page}&pageSize=${size}`);
  }
}
