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
  public readonly Math = Math;
  
  students = signal<Student[]>([]);
  isLoading = signal(true);
  totalCount = signal(0);
  currentPage = signal(1);
  totalPages = signal(1);
  searchTerm = signal('');
  isSubmitting = signal(false);

  // Form Data
  newStudent: any = {
    firstName: '',
    lastName: '',
    studentNumber: '',
    parentId: null,
    teacherIds: []
  };

  parents = signal<UserBrief[]>([]);
  teachers = signal<UserBrief[]>([]);

  isEditing = signal(false);

  ngOnInit() {
    this.loadStudents();
    this.loadSelectionLists();
  }

  loadStudents(page: number = 1) {
    this.isLoading.set(true);
    this.adminService.getStudents(page, 50, this.searchTerm()).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.students.set(res.data.items);
          this.totalPages.set(res.data.totalPages);
          this.currentPage.set(res.data.pageNumber);
          this.totalCount.set(res.data.totalCount);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSearch() {
    this.currentPage.set(1);
    this.loadStudents(1);
  }

  loadSelectionLists() {
    this.adminService.getUsersByRole('Teacher').subscribe(res => {
      if (res.isSuccess) this.teachers.set(res.data);
    });

    this.adminService.getUsersByRole('Parent').subscribe(res => {
      if (res.isSuccess) this.parents.set(res.data);
    });
  }

  editStudent(s: any) {
    this.isEditing.set(true);
    this.newStudent = {
      ...s,
      parentId: s.parentId || null,
      teacherIds: s.teacherIds || []
    };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  saveStudent() {
    if (!this.newStudent.firstName || !this.newStudent.parentId) {
      Swal.fire('Uyarı', 'Lütfen ad ve veli bilgilerini doldurun.', 'warning');
      return;
    }

    const request = this.isEditing() 
      ? this.adminService.updateStudent(this.newStudent)
      : this.adminService.createStudent(this.newStudent);

    this.isSubmitting.set(true);
    request.subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        if (res.isSuccess) {
          Swal.fire('Başarılı', this.isEditing() ? 'Öğrenci güncellendi.' : 'Öğrenci eklendi.', 'success');
          this.resetForm();
          this.currentPage.set(1);
          this.searchTerm.set(''); // Arama kutusunu temizle
          this.loadStudents(1);
        } else {
          Swal.fire('Hata', res.message, 'error');
        }
      },
      error: (err) => {
        this.isSubmitting.set(false);
        Swal.fire('Hata', err.error?.message || 'Bir hata oluştu.', 'error');
      }
    });
  }

  resetForm() {
    this.isEditing.set(false);
    this.newStudent = {
      firstName: '',
      lastName: '',
      studentNumber: '',
      parentId: null,
      teacherIds: []
    };
  }

  deleteStudent(id: string) {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Bu öğrenci kaydı kalıcı olarak silinecektir!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Evet, Sil',
      cancelButtonText: 'Vazgeç'
    }).then((result) => {
      if (result.isConfirmed) {
        this.adminService.deleteStudent(id).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('Başarılı', 'Öğrenci silindi.', 'success');
            this.loadStudents(this.currentPage());
          } else {
            Swal.fire('Hata', res.message, 'error');
          }
        });
      }
    });
  }
}
