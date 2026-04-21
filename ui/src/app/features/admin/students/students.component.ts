import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, Student } from '../../../core/services/admin.service';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './students.html'
})
export class StudentsComponent implements OnInit {
  students = signal<Student[]>([]);
  isLoading = signal(true);
  currentPage = signal(1);
  totalPages = signal(1);

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.loadStudents();
  }

  loadStudents(page: number = 1) {
    this.isLoading.set(true);
    this.adminService.getStudents(page, 10).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.students.set(res.data.items);
          this.totalPages.set(res.data.totalPages);
          this.currentPage.set(res.data.pageNumber);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
}
