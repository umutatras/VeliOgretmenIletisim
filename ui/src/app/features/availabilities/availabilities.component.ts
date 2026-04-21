import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppointmentService, Availability } from '../../core/services/appointment.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-availabilities',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './availabilities.html'
})
export class AvailabilitiesComponent implements OnInit {
  private appointmentService = inject(AppointmentService);

  availabilities = signal<Availability[]>([]);
  isLoading = signal(true);

  // Form Fields
  newSlot = {
    startTime: '',
    endTime: '',
    maxCapacity: 1,
    isGroup: false
  };

  ngOnInit() {
    this.loadMyAvailabilities();
  }

  loadMyAvailabilities() {
    this.isLoading.set(true);
    this.appointmentService.getMyAvailabilities().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.availabilities.set(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  save() {
    if (!this.newSlot.startTime || !this.newSlot.endTime) {
      Swal.fire('Uyarı', 'Başlangıç ve bitiş zamanı zorunludur.', 'warning');
      return;
    }

    this.appointmentService.createAvailability(this.newSlot).subscribe(res => {
      if (res.isSuccess) {
        Swal.fire('Başarılı', 'Müsaitlik zamanı eklendi.', 'success');
        this.newSlot = { startTime: '', endTime: '', maxCapacity: 1, isGroup: false };
        this.loadMyAvailabilities();
      }
    });
  }
}
