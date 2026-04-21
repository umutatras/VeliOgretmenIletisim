import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, AuditLog } from '../../../core/services/admin.service';

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './audit-logs.html'
})
export class AuditLogsComponent implements OnInit {
  private adminService = inject(AdminService);

  logs = signal<AuditLog[]>([]);
  isLoading = signal(true);
  currentPage = signal(1);
  totalPages = signal(1);

  ngOnInit() {
    this.loadLogs();
  }

  loadLogs(page: number = 1) {
    this.isLoading.set(true);
    this.adminService.getAuditLogs(page, 20).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.logs.set(res.data.items);
          this.totalPages.set(res.data.totalPages);
          this.currentPage.set(res.data.pageNumber);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  getStatusCodeBadgeClass(code: number): string {
    if (code >= 200 && code < 300) return 'bg-success';
    if (code >= 400 && code < 500) return 'bg-warning';
    if (code >= 500) return 'bg-danger';
    return 'bg-info';
  }

  getMethodBadgeClass(method: string): string {
    switch (method.toUpperCase()) {
      case 'GET': return 'bg-primary-transparent text-primary';
      case 'POST': return 'bg-success-transparent text-success' ;
      case 'PUT': return 'bg-info-transparent text-info';
      case 'DELETE': return 'bg-danger-transparent text-danger';
      default: return 'bg-light text-dark';
    }
  }
}
