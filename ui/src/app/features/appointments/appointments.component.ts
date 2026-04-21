import { Component, OnInit, OnDestroy, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppointmentService, Appointment, Availability, MyStudent } from '../../core/services/appointment.service';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { Result, PagedResult } from '../../core/services/announcement.service';
import { interval, Subscription } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './appointments.html'
})
export class AppointmentsComponent implements OnInit, OnDestroy {
  private appointmentService = inject(AppointmentService);
  private authService = inject(AuthService);
  private userService = inject(UserService);

  private pollSubscription?: Subscription;

  appointments = signal<any[]>([]); // Using any to avoid type mismatch during rapid dev
  isLoading = signal(true);
  currentUser = this.authService.currentUser;

  // Parent Booking
  myStudents = signal<any[]>([]);
  allTeacherAvailabilities = signal<any[]>([]);
  selectedAvailabilityId = '';
  selectedStudentId = '';
  note = '';

  // Teacher Approval
  teacherRequests = signal<any[]>([]);

  isParent = computed(() => {
    const role = this.currentUser()?.role?.toString().toLowerCase();
    return role === 'parent' || role === '3';
  });

  isTeacher = computed(() => {
    const role = this.currentUser()?.role?.toString().toLowerCase();
    return role === 'teacher' || role === '2';
  });

  ngOnInit() {
    this.refreshAllData();

    // Polling: Every 15 seconds
    this.pollSubscription = interval(15000).subscribe(() => {
      this.refreshAllData();
    });
  }

  ngOnDestroy() {
    this.pollSubscription?.unsubscribe();
  }

  refreshAllData() {
    if (this.isParent()) {
      this.loadAvailabilitiesForParent();
      this.loadParentStudents();
    } else if (this.isTeacher()) {
      this.loadTeacherRequests();
    }
    this.viewMyExistingAppointments();
  }

  viewMyExistingAppointments() {
    this.isLoading.set(true);
    this.appointmentService.getAppointments().subscribe({
      next: (res: any) => {
        if (res.isSuccess) this.appointments.set(res.data.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  // PARENT LOGIC
  loadParentStudents() {
    this.userService.getMyStudentsForParent().subscribe(res => {
      if (res.isSuccess) this.myStudents.set(res.data);
    });
  }

  loadAvailabilitiesForParent() {
    this.appointmentService.getAvailabilitiesForParent().subscribe(res => {
      if (res.isSuccess) this.allTeacherAvailabilities.set(res.data);
    });
  }

  bookAppointment() {
    if (!this.selectedAvailabilityId) {
      Swal.fire('Uyarı', 'Lütfen bir zaman dilimi seçin.', 'warning');
      return;
    }

    const command = {
      availabilityId: this.selectedAvailabilityId,
      studentId: this.selectedStudentId || null,
      note: this.note
    };

    this.appointmentService.applyForAppointment(command).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          Swal.fire('Başarılı', res.message || 'Randevu başvurunuz alındı.', 'success');
          this.note = '';
          this.selectedAvailabilityId = '';
          this.viewMyExistingAppointments();
          this.loadAvailabilitiesForParent();
        } else {
          // Bu kısım 200 dönüp isSuccess false olan durumlar için
          Swal.fire('Uyarı', res.message, 'info');
        }
      },
      error: (err) => {
        // 400 Bad Request gibi hataları burada yakalıyoruz
        const errorMessage = err.error?.message || 'Randevu başvurusu sırasında bir hata oluştu veya bu saate zaten başvurunuz var.';
        Swal.fire('Bilgi', errorMessage, 'info');
        this.isLoading.set(false);
      }
    });
  }

  // TEACHER LOGIC
  loadTeacherRequests() {
    this.isLoading.set(true);
    this.appointmentService.getTeacherAppointments().subscribe(res => {
      if (res.isSuccess) this.teacherRequests.set(res.data);
      this.isLoading.set(false);
    });
  }

  updateStatus(appointmentId: string, status: number) {
    Swal.fire({
      title: status === 2 ? 'Onaylıyor musunuz?' : 'Reddediyor musunuz?',
      input: 'text',
      inputLabel: 'Notunuz (İsteğe bağlı)',
      showCancelButton: true,
      confirmButtonText: 'Evet',
      cancelButtonText: 'Vazgeç'
    }).then((result) => {
      if (result.isConfirmed) {
        this.appointmentService.updateAppointmentStatus({
          appointmentId: appointmentId,
          status: status,
          teacherNote: result.value
        }).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('İşlem Tamam', res.message, 'success');
            this.loadTeacherRequests();
          } else {
            Swal.fire('Hata', res.message, 'error');
          }
        });
      }
    });
  }

  getStatusBadgeClass(status: any): string {
    // status can be number or string
    const s = status?.toString().toLowerCase();
    if (s === '1' || s === 'pending' || s === 'beklemede') return 'bg-warning';
    if (s === '2' || s === 'approved' || s === 'onaylandı') return 'bg-success';
    if (s === '3' || s === 'cancelled' || s === 'iptal' || s === 'reddedildi') return 'bg-danger';
    return 'bg-secondary';
  }

  getStatusText(status: any): string {
    const s = status?.toString();
    if (s === '1' || s === 'Pending') return 'Beklemede';
    if (s === '2' || s === 'Approved') return 'Onaylandı';
    if (s === '3' || s === 'Cancelled') return 'Reddedildi/İptal';
    return 'Bilinmiyor';
  }
}
