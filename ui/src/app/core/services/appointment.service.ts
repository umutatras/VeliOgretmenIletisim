import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, PagedResult } from './announcement.service';

export interface Appointment {
  id: string;
  studentName: string;
  teacherName: string;
  appointmentDate: string;
  status: string;
  note?: string;
}

export interface Availability {
  id: string;
  startTime: string;
  endTime: string;
  maxCapacity: number;
  isGroup: boolean;
  teacherId?: string;
  teacherName?: string;
}

export interface MyStudent {
  id: string;
  fullName: string;
  studentNumber: string;
  teacherId: string;
  teacherName: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7273/api';

  getAppointments(page: number = 1, size: number = 10): Observable<Result<PagedResult<Appointment>>> {
    return this.http.get<Result<PagedResult<Appointment>>>(`${this.apiUrl}/Appointments?pageNumber=${page}&pageSize=${size}`);
  }

  getTeacherAvailabilities(teacherId: string): Observable<Result<Availability[]>> {
    return this.http.get<Result<Availability[]>>(`${this.apiUrl}/Parents/teacher-availabilities/${teacherId}`);
  }

  getMyAvailabilities(): Observable<Result<Availability[]>> {
    return this.http.get<Result<Availability[]>>(`${this.apiUrl}/Teachers/my-availabilities`);
  }

  getMyStudents(): Observable<Result<MyStudent[]>> {
    return this.http.get<Result<MyStudent[]>>(`${this.apiUrl}/Parents/my-students`);
  }

  createAvailability(command: any): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/Teachers/availabilities`, command);
  }

  updateAvailability(command: any): Observable<Result<any>> {
    return this.http.put<Result<any>>(`${this.apiUrl}/Teachers/availabilities`, command);
  }

  deleteAvailability(id: string): Observable<Result<any>> {
    return this.http.delete<Result<any>>(`${this.apiUrl}/Teachers/availabilities/${id}`);
  }

  createAppointment(command: any): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/Appointments`, command);
  }
}
