import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeacherService, TeacherStudent } from '../../core/services/teacher.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-teacher-students',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher-students.html'
})
export class TeacherStudentsComponent implements OnInit {
  private teacherService = inject(TeacherService);
  
  students = signal<TeacherStudent[]>([]);
  parents = signal<any[]>([]);
  isLoading = signal(true);
  isEditing = signal(false);

  newStudent: any = {
    firstName: '',
    lastName: '',
    studentNumber: '',
    parentId: null
  };

  ngOnInit() {
    this.loadStudents();
    this.loadParents();
  }

  loadStudents() {
    this.isLoading.set(true);
    this.teacherService.getMyStudents().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.students.set(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadParents() {
    this.teacherService.getParents().subscribe(res => {
      if (res.isSuccess) this.parents.set(res.data);
    });
  }

  editStudent(s: any) {
    this.isEditing.set(true);
    this.newStudent = { ...s };
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

    request.subscribe(res => {
      if (res.isSuccess) {
        Swal.fire('Başarılı', this.isEditing() ? 'Öğrenci güncellendi.' : 'Öğrenci eklendi.', 'success');
        this.resetForm();
        this.loadStudents();
      } else {
        Swal.fire('Hata', res.message, 'error');
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
      parentId: null
    };
  }
}
