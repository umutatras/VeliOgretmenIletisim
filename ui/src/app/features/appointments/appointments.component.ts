import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppointmentService, Appointment, Availability, MyStudent } from '../../core/services/appointment.service';
import { AuthService } from '../../core/services/auth.service';
import { Result, PagedResult } from '../../core/services/announcement.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './appointments.html'
})
export class AppointmentsComponent implements OnInit {
  private appointmentService = inject(AppointmentService);
  private authService = inject(AuthService);

  appointments = signal<Appointment[]>([]);
  isLoading = signal(true);
  currentUser = this.authService.currentUser;

  // Parent Booking Form
  myStudents = signal<MyStudent[]>([]);
  selectedStudentId = '';
  availabilities = signal<Availability[]>([]);
  selectedAvailabilityId = '';
  note = '';

  selectedStudent = computed(() => this.myStudents().find(s => s.id === this.selectedStudentId));

  ngOnInit() {
    this.loadAppointments();
    if (this.currentUser()?.role === 'Parent') {
      this.loadMyStudents();
    }
  }

  loadAppointments() {
    this.isLoading.set(true);
    this.appointmentService.getAppointments().subscribe({
      next: (res: Result<PagedResult<Appointment>>) => {
        if (res.isSuccess) this.appointments.set(res.data.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  loadMyStudents() {
    this.appointmentService.getMyStudents().subscribe((res: Result<MyStudent[]>) => {
      if (res.isSuccess) this.myStudents.set(res.data);
    });
  }

  onStudentChange() {
    const student = this.selectedStudent();
    if (student) {
      this.appointmentService.getTeacherAvailabilities(student.teacherId).subscribe((res: Result<Availability[]>) => {
        if (res.isSuccess) this.availabilities.set(res.data);
      });
    } else {
      this.availabilities.set([]);
    }
  }

  bookAppointment() {
    if (!this.selectedStudentId || !this.selectedAvailabilityId) {
      Swal.fire('Uyarı', 'Lütfen öğrenci ve müsaitlik zamanı seçin.', 'warning');
      return;
    }

    const command = {
      availabilityId: this.selectedAvailabilityId,
      studentId: this.selectedStudentId,
      note: this.note
    };

    this.appointmentService.createAppointment(command).subscribe((res: Result<any>) => {
      if (res.isSuccess) {
        Swal.fire('Başarılı', 'Randevunuz oluşturuldu.', 'success');
        this.loadAppointments();
        this.resetBookingForm();
      } else {
        Swal.fire('Hata', res.message, 'error');
      }
    });
  }

  resetBookingForm() {
    this.selectedStudentId = '';
    this.selectedAvailabilityId = '';
    this.note = '';
    this.availabilities.set([]);
  }

  getStatusBadgeClass(status: string): string {
    if (!status) return 'bg-primary';
    switch (status.toLowerCase()) {
      case 'onaylandı': case 'approved': return 'bg-success';
      case 'beklemede': case 'pending': return 'bg-warning';
      case 'iptal': case 'cancelled': return 'bg-danger';
      default: return 'bg-primary';
    }
  }
}
