import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeacherService, TeacherStudent } from '../../core/services/teacher.service';
import { AuthService } from '../../core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-teacher-students',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher-students.html'
})
export class TeacherStudentsComponent implements OnInit {
  private teacherService = inject(TeacherService);
  private authService = inject(AuthService);
  public readonly Math = Math;
  
  currentUser = this.authService.currentUser;
  students = signal<TeacherStudent[]>([]);
  teachers = signal<any[]>([]);

  newStudent: any = {
    firstName: '',
    lastName: '',
    studentNumber: '',
    parentId: null,
    teacherIds: []
  };

  searchTerm = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  isLoading = signal(false);
  isEditing = signal(false);
  parents = signal<any[]>([]);
  isSubmitting = signal(false);

  ngOnInit() {
    this.loadStudents();
    this.loadParents();
    this.loadTeachers();
  }

  loadStudents() {
    this.isLoading.set(true);
    this.teacherService.getMyStudents(this.pageNumber(), this.pageSize(), this.searchTerm()).subscribe({
      next: (res) => {
        if (res.isSuccess && res.data) {
          this.students.set(res.data.items);
          this.totalCount.set(res.data.totalCount);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onPageChange(page: number) {
    this.pageNumber.set(page);
    this.loadStudents();
  }

  onSearch() {
    this.loadStudents();
  }

  loadParents() {
    this.teacherService.getParents().subscribe(res => {
      if (res.isSuccess) this.parents.set(res.data);
    });
  }

  loadTeachers() {
    this.teacherService.getTeachers().subscribe(res => {
      if (res.isSuccess) this.teachers.set(res.data);
    });
  }

  editStudent(s: any) {
    this.isEditing.set(true);
    this.newStudent = { 
      ...s,
      teacherIds: s.teacherIds || []
    };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  saveStudent() {
    if (!this.newStudent.firstName || !this.newStudent.parentId) {
      Swal.fire('Uyarı', 'Lütfen öğrenci adını ve veliyi seçin.', 'warning');
      return;
    }

    const request = this.isEditing()
      ? this.teacherService.updateStudent(this.newStudent)
      : this.teacherService.addStudent(this.newStudent);

    this.isSubmitting.set(true);
    request.subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        if (res.isSuccess) {
          Swal.fire('Başarılı', this.isEditing() ? 'Öğrenci güncellendi.' : 'Öğrenci eklendi.', 'success');
          this.resetForm();
          this.pageNumber.set(1);
          this.searchTerm.set(''); // Arama kutusunu temizle
          this.loadStudents();
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

  deleteStudent(id: string) {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Bu öğrenci kaydı silinecektir!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Evet, Sil',
      cancelButtonText: 'Vazgeç'
    }).then((result) => {
      if (result.isConfirmed) {
        this.teacherService.deleteStudent(id).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('Başarılı', 'Öğrenci silindi.', 'success');
            this.loadStudents();
          } else {
            Swal.fire('Hata', res.message, 'error');
          }
        });
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
}
