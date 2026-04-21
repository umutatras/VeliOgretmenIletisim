import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
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
    path: 'auth/register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
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
        path: 'profile',
        loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent)
      },
      {
        path: 'announcements',
        loadComponent: () => import('./features/announcements/announcements.component').then(m => m.AnnouncementsComponent)
      },
      {
        path: 'complaints',
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Parent'] },
        loadComponent: () => import('./features/complaints/complaints.component').then(m => m.ComplaintsComponent)
      },
      {
        path: 'meeting-notes',
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Teacher'] },
        loadComponent: () => import('./features/meeting-notes/meeting-notes.component').then(m => m.MeetingNotesComponent)
      },
      {
        path: 'availabilities',
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Teacher'] },
        loadComponent: () => import('./features/availabilities/availabilities.component').then(m => m.AvailabilitiesComponent)
      },
      {
        path: 'teacher/students',
        canActivate: [roleGuard],
        data: { roles: ['Teacher'] },
        loadComponent: () => import('./features/teacher-students/teacher-students.component').then(m => m.TeacherStudentsComponent)
      },
      {
        path: 'admin/departments',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./features/admin/departments/departments.component').then(m => m.DepartmentsComponent)
      },
      {
        path: 'admin/students',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./features/admin/students/students.component').then(m => m.StudentsComponent)
      },
      {
        path: 'admin/audit-logs',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./features/admin/audit-logs/audit-logs.component').then(m => m.AuditLogsComponent)
      },
      {
        path: 'admin/user-approval',
        canActivate: [roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./features/admin/user-approval/user-approval.component').then(m => m.UserApprovalComponent)
      }
    ]
  }
];
