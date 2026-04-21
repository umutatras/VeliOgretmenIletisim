import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result, PagedResult } from './announcement.service';

export interface Availability {
  id: string;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  teacherId: string;
}

export interface Appointment {
  id: string;
  parentName: string;
  studentName: string;
  teacherName: string;
  appointmentDate: string;
  status: string;
  note?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl = 'https://localhost:7273/api';

  constructor(private http: HttpClient) {}

  // For Admin/Teachers to see appointments
  getAppointments(page: number = 1, size: number = 10): Observable<Result<PagedResult<Appointment>>> {
    return this.http.get<Result<PagedResult<Appointment>>>(`${this.apiUrl}/Appointments?pageNumber=${page}&pageSize=${size}`);
  }

  // To get available slots of a specific teacher
  getTeacherAvailabilities(teacherId: string): Observable<Result<Availability[]>> {
    return this.http.get<Result<Availability[]>>(`${this.apiUrl}/Teachers/${teacherId}/availabilities`);
  }

  // For parents to book
  createAppointment(command: any): Observable<Result<any>> {
    return this.http.post<Result<any>>(`${this.apiUrl}/Appointments`, command);
  }
}
