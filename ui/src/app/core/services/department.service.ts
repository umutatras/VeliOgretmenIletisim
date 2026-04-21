import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from './announcement.service';

export interface Department {
  id: string;
  name: string;
  description?: string;
  teacherCount?: number;
}

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7273/api/Admin/departments';

  getDepartments(): Observable<Result<Department[]>> {
    return this.http.get<Result<Department[]>>(this.apiUrl);
  }

  createDepartment(dept: { name: string, description: string }): Observable<Result<any>> {
    return this.http.post<Result<any>>(this.apiUrl, dept);
  }

  deleteDepartment(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/${id}`);
  }
}
