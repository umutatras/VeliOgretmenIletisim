import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, Department } from '../../../core/services/admin.service';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './departments.html'
})
export class DepartmentsComponent implements OnInit {
  departments = signal<Department[]>([]);
  
  // Regular string for ngModel
  newDeptName = '';
  
  isLoading = signal(true);

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.loadDepartments();
  }

  loadDepartments() {
    this.isLoading.set(true);
    this.adminService.getDepartments().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.departments.set(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  createDepartment() {
    if (!this.newDeptName) return;
    
    this.adminService.createDepartment(this.newDeptName).subscribe(res => {
      if (res.isSuccess) {
        this.newDeptName = '';
        this.loadDepartments();
      }
    });
  }

  deleteDepartment(id: string) {
    if (confirm('Bu departmanı silmek istediğinize emin misiniz?')) {
      this.adminService.deleteDepartment(id).subscribe(res => {
        if (res.isSuccess) {
          this.loadDepartments();
        }
      });
    }
  }
}
