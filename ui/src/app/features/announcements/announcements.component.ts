import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnnouncementService, Announcement } from '../../core/services/announcement.service';
import { AuthService } from '../../core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-announcements',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './announcements.html'
})
export class AnnouncementsComponent implements OnInit {
  private announcementService = inject(AnnouncementService);
  private authService = inject(AuthService);

  announcements = signal<Announcement[]>([]);
  isLoading = signal(true);
  currentUser = this.authService.currentUser;

  // Management Form
  newAnnouncement = {
    title: '',
    content: '',
    isGeneral: true
  };

  ngOnInit() {
    this.loadAnnouncements();
  }

  loadAnnouncements() {
    this.isLoading.set(true);
    this.announcementService.getAnnouncements().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.announcements.set(res.data.items);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  save() {
    if (!this.newAnnouncement.title || !this.newAnnouncement.content) {
      Swal.fire('Uyarı', 'Başlık ve içerik alanları zorunludur.', 'warning');
      return;
    }

    this.announcementService.createAnnouncement(this.newAnnouncement).subscribe(res => {
      if (res.isSuccess) {
        Swal.fire({
          icon: 'success',
          title: 'Başarılı!',
          text: 'Duyuru yayına alındı.',
          timer: 2000,
          showConfirmButton: false
        });
        this.resetForm();
        this.loadAnnouncements();
      } else {
        Swal.fire('Hata', res.message, 'error');
      }
    });
  }

  delete(id: string) {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Duyuru kalıcı olarak silinecektir!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Evet, sil!',
      cancelButtonText: 'Vazgeç'
    }).then((result) => {
      if (result.isConfirmed) {
        this.announcementService.deleteAnnouncement(id).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('Silindi!', 'Duyuru başarıyla kaldırıldı.', 'success');
            this.loadAnnouncements();
          }
        });
      }
    });
  }

  resetForm() {
    this.newAnnouncement = { title: '', content: '', isGeneral: true };
  }
}
