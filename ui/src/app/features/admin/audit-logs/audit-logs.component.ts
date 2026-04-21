import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, AuditLog } from '../../../core/services/admin.service';

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './audit-logs.html'
})
export class AuditLogsComponent implements OnInit {
  logs = signal<AuditLog[]>([]);
  isLoading = signal(true);
  currentPage = signal(1);
  totalPages = signal(1);

  constructor(private adminService: AdminService) {}

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

  getStatusCodeClass(code: number) {
    if (code >= 200 && code < 300) return 'text-green-600 bg-green-50';
    if (code >= 400) return 'text-red-600 bg-red-50';
    return 'text-amber-600 bg-amber-50';
  }
}
