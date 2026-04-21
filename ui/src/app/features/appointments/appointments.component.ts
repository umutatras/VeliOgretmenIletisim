import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentService, Appointment } from '../../core/services/appointment.service';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './appointments.html'
})
export class AppointmentsComponent implements OnInit {
  appointments = signal<Appointment[]>([]);
  isLoading = signal(true);

  constructor(private appointmentService: AppointmentService) {}

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading.set(true);
    this.appointmentService.getAppointments(1, 20).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.appointments.set(res.data.items);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  getStatusClass(status: string) {
    switch (status.toLowerCase()) {
      case 'approved': return 'bg-green-100 text-green-700 border-green-200';
      case 'pending': return 'bg-amber-100 text-amber-700 border-amber-200';
      case 'cancelled': return 'bg-red-100 text-red-700 border-red-200';
      default: return 'bg-slate-100 text-slate-700 border-slate-200';
    }
  }
}
