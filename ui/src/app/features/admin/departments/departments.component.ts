import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DepartmentService, Department } from '../../../core/services/department.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './departments.html'
})
export class DepartmentsComponent implements OnInit {
  private deptService = inject(DepartmentService);

  departments = signal<Department[]>([]);
  isLoading = signal(true);

  // New Dept Form
  newDept = {
    name: '',
    description: ''
  };

  ngOnInit() {
    this.loadDepartments();
  }

  loadDepartments() {
    this.isLoading.set(true);
    this.deptService.getDepartments().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.departments.set(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  save() {
    if (!this.newDept.name) {
      Swal.fire('Uyarı', 'Departman adı zorunludur.', 'warning');
      return;
    }

    this.deptService.createDepartment(this.newDept).subscribe(res => {
      if (res.isSuccess) {
        Swal.fire('Başarılı', 'Departman eklendi.', 'success');
        this.newDept = { name: '', description: '' };
        this.loadDepartments();
      }
    });
  }

  delete(id: string) {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Bu departman silinecektir!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Evet, sil!',
      cancelButtonText: 'Vazgeç'
    }).then((result) => {
      if (result.isConfirmed) {
        this.deptService.deleteDepartment(id).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('Silindi', 'Departman başarıyla silindi.', 'success');
            this.loadDepartments();
          }
        });
      }
    });
  }
}
