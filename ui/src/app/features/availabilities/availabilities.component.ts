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
  newSlot: any = {
    id: undefined,
    startTime: '',
    endTime: '',
    maxCapacity: 1,
    isGroup: false
  };

  isEditing = signal(false);

  ngOnInit() {
    this.loadMyAvailabilities();
  }

  loadMyAvailabilities() {
    this.isLoading.set(true);
    this.appointmentService.getMyAvailabilities().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.availabilities.set(res.data);
        } else {
          Swal.fire('Hata', res.message || 'Müsaitlikler yüklenemedi.', 'error');
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Availability Load Error:', err);
        Swal.fire('Hata', 'Sunucuya bağlanırken bir sorun oluştu.', 'error');
        this.isLoading.set(false);
      }
    });
  }

  edit(item: Availability) {
    this.isEditing.set(true);
    // Safe substring for datetime-local input
    this.newSlot = {
      ...item,
      startTime: item.startTime ? item.startTime.substring(0, 16) : '',
      endTime: item.endTime ? item.endTime.substring(0, 16) : ''
    };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit() {
    this.isEditing.set(false);
    this.newSlot = { id: undefined, startTime: '', endTime: '', maxCapacity: 1, isGroup: false };
  }

  save() {
    if (!this.newSlot.startTime || !this.newSlot.endTime) {
      Swal.fire('Uyarı', 'Başlangıç ve bitiş zamanı zorunludur.', 'warning');
      return;
    }

    // Convert to ISO for backend consistency
    const payload = {
      ...this.newSlot,
      startTime: new Date(this.newSlot.startTime).toISOString(),
      endTime: new Date(this.newSlot.endTime).toISOString()
    };

    if (this.isEditing() && !payload.id) {
       Swal.fire('Hata', 'Güncellenecek kayıt ID\'si bulunamadı.', 'error');
       return;
    }

    const request = this.isEditing() 
      ? this.appointmentService.updateAvailability(payload)
      : this.appointmentService.createAvailability(payload);

    request.subscribe({
      next: (res) => {
        if (res.isSuccess) {
          Swal.fire({
            icon: 'success',
            title: 'Başarılı',
            text: this.isEditing() ? 'Müsaitlik güncellendi.' : 'Müsaitlik zamanı eklendi.',
            timer: 1500,
            showConfirmButton: false
          });
          this.cancelEdit();
          this.loadMyAvailabilities();
        } else {
          Swal.fire('Hata', res.message, 'error');
        }
      },
      error: (err) => {
        console.error('Save Error:', err);
        Swal.fire('Hata', 'İşlem başarısız oldu. Lütfen verileri kontrol edin.', 'error');
      }
    });
  }

  delete(id: string) {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Bu müsaitlik zamanı silinecektir!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Evet, sil!',
      cancelButtonText: 'Vazgeç'
    }).then((result) => {
      if (result.isConfirmed) {
        this.appointmentService.deleteAvailability(id).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('Silindi!', 'Müsaitlik zamanı başarıyla kaldırıldı.', 'success');
            this.loadMyAvailabilities();
          } else {
            Swal.fire('Hata', res.message, 'error');
          }
        });
      }
    });
  }
}
