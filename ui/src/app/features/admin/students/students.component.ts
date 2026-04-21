import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService, Student, UserBrief } from '../../../core/services/admin.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-students',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './students.html'
})
export class StudentsComponent implements OnInit {
  private adminService = inject(AdminService);
  
  students = signal<Student[]>([]);
  isLoading = signal(true);
  currentPage = signal(1);
  totalPages = signal(1);

  // Form Data
  newStudent = {
    firstName: '',
    lastName: '',
    studentNumber: '',
    parentId: '',
    teacherId: ''
  };

  parents = signal<UserBrief[]>([]);
  teachers = signal<UserBrief[]>([]);

  ngOnInit() {
    this.loadStudents();
    this.loadSelectionLists();
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

  loadSelectionLists() {
    // Role enum on backend: Teacher = 1, Parent = 2 (Assuming based on common patterns, let's check or use string if possible)
    // Actually, our API takes the Enum string or int. Let's use strings if the enum is string-backed or try 'Teacher'
    this.adminService.getUsersByRole('Teacher').subscribe(res => {
      if (res.isSuccess) this.teachers.set(res.data);
    });

    this.adminService.getUsersByRole('Parent').subscribe(res => {
      if (res.isSuccess) this.parents.set(res.data);
    });
  }

  saveStudent() {
    if (!this.newStudent.firstName || !this.newStudent.parentId || !this.newStudent.teacherId) {
      Swal.fire('Uyarı', 'Lütfen öğrenci bilgilerini, veliyi ve öğretmeni seçin.', 'warning');
      return;
    }

    this.adminService.createStudent(this.newStudent).subscribe(res => {
      if (res.isSuccess) {
        Swal.fire('Başarılı', 'Öğrenci başarıyla eklendi ve atamaları yapıldı.', 'success');
        this.resetForm();
        this.loadStudents();
      } else {
        Swal.fire('Hata', res.message || 'Öğrenci eklenirken bir hata oluştu.', 'error');
      }
    });
  }

  resetForm() {
    this.newStudent = {
      firstName: '',
      lastName: '',
      studentNumber: '',
      parentId: '',
      teacherId: ''
    };
  }
}
