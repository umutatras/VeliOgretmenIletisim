import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, PagedResult } from './announcement.service';

export interface Department {
  id: string;
  name: string;
  description?: string;
}

export interface Student {
  id: string;
  firstName: string;
  lastName: string;
  studentNumber: string;
  phoneNumber: string;
  parentName: string;
  parentId: string;
  teacherNames: string[];
  teacherIds: string[];
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

export interface UserBrief {
  id: string;
  fullName: string;
  email: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7273/api/Admin';

  // Departments
  getDepartments(): Observable<Result<Department[]>> {
    return this.http.get<Result<Department[]>>(`${this.apiUrl}/departments`);
  }

  createDepartment(dept: { name: string, description: string }): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/departments`, dept);
  }

  deleteDepartment(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/departments/${id}`);
  }

  // Students
  getStudents(page: number, size: number, searchTerm: string = ''): Observable<Result<PagedResult<Student>>> {
    return this.http.get<Result<PagedResult<Student>>>(`${this.apiUrl}/students?pageNumber=${page}&pageSize=${size}&searchTerm=${searchTerm}`);
  }

  createStudent(student: any): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/students`, student);
  }

  deleteStudent(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/students/${id}`);
  }

  updateStudent(student: any): Observable<Result<any>> {
    return this.http.put<Result<any>>(`${this.apiUrl}/students`, student);
  }

  // Users
  getUsersByRole(role: string): Observable<Result<UserBrief[]>> {
    return this.http.get<Result<UserBrief[]>>(`${this.apiUrl}/users-by-role/${role}`);
  }

  // Audit Logs
  getAuditLogs(page: number = 1, size: number = 20): Observable<Result<PagedResult<AuditLog>>> {
    return this.http.get<Result<PagedResult<AuditLog>>>(`${this.apiUrl}/audit-logs?pageNumber=${page}&pageSize=${size}`);
  }

  // Approvals
  getPendingApprovals(): Observable<Result<any[]>> {
    return this.http.get<Result<any[]>>(`${this.apiUrl}/pending-approvals`);
  }

  approveUser(userId: string): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/approve-user`, { userId });
  }
}
