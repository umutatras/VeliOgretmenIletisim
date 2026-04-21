import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from './announcement.service';

export interface TeacherStudent {
  id: string;
  firstName: string;
  lastName: string;
  studentNumber: string;
  parentId: string;
  parentName: string;
}

@Injectable({
  providedIn: 'root'
})
export class TeacherService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7273/api/Teachers';

  getMyStudents(): Observable<Result<TeacherStudent[]>> {
    return this.http.get<Result<TeacherStudent[]>>(`${this.apiUrl}/my-students`);
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
    // We can use a public endpoint or admin endpoint for list (if authorized)
    return this.http.get<Result<any[]>>('https://localhost:7273/api/Admin/users-by-role/Parent');
  }
}
