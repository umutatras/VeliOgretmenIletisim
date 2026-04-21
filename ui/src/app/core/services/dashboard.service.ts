import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from './announcement.service';

export interface DashboardStats {
  activeAnnouncementsCount: number;
  pendingAppointmentsCount: number;
  completedComplaintsCount: number;
  totalStudentsCount: number;
  recentAnnouncements: any[];
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7273/api/Dashboard';

  getStats(): Observable<Result<DashboardStats>> {
    return this.http.get<Result<DashboardStats>>(`${this.apiUrl}/stats`);
  }
}
