import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, PagedResult } from './announcement.service';

export interface TeacherStudent {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  studentNumber: string;
  parentId: string;
  parentName: string;
  teacherNames: string[];
  teacherIds: string[];
}

@Injectable({
  providedIn: 'root'
})
export class TeacherService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7273/api/Teachers';

  getMyStudents(page: number = 1, size: number = 10, searchTerm: string = ''): Observable<Result<PagedResult<TeacherStudent>>> {
    return this.http.get<Result<PagedResult<TeacherStudent>>>(`${this.apiUrl}/my-students?pageNumber=${page}&pageSize=${size}&searchTerm=${searchTerm}`);
  }

  addStudent(student: any): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/students`, student);
  }

  updateStudent(student: any): Observable<Result<any>> {
    return this.http.put<Result<any>>(`${this.apiUrl}/students`, student);
  }

  deleteStudent(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/students/${id}`);
  }

  getParents(): Observable<Result<any[]>> {
    return this.http.get<Result<any[]>>(`${this.apiUrl}/parents-lookup`);
  }

  getTeachers(): Observable<Result<any[]>> {
    return this.http.get<Result<any[]>>(`${this.apiUrl}/teachers-lookup`);
  }
}
