import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { ShellComponent } from './shared/components/shell/shell.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth/login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'appointments',
        loadComponent: () => import('./features/appointments/appointments.component').then(m => m.AppointmentsComponent)
      },
      {
        path: 'announcements',
        loadComponent: () => import('./features/announcements/announcements.component').then(m => m.AnnouncementsComponent)
      },
      {
        path: 'complaints',
        loadComponent: () => import('./features/complaints/complaints.component').then(m => m.ComplaintsComponent)
      },
      {
        path: 'meeting-notes',
        loadComponent: () => import('./features/meeting-notes/meeting-notes.component').then(m => m.MeetingNotesComponent)
      },
      {
        path: 'admin/departments',
        loadComponent: () => import('./features/admin/departments/departments.component').then(m => m.DepartmentsComponent)
      },
      {
        path: 'admin/students',
        loadComponent: () => import('./features/admin/students/students.component').then(m => m.StudentsComponent)
      },
      {
        path: 'admin/audit-logs',
        loadComponent: () => import('./features/admin/audit-logs/audit-logs.component').then(m => m.AuditLogsComponent)
      }
    ]
  }
];
