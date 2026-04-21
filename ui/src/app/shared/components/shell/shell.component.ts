import { Component, signal, computed, inject, Renderer2 } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../../../core/services/auth.service';
import { FileService } from '../../../core/services/file.service';
import { UserService } from '../../../core/services/user.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Result } from '../../../core/services/announcement.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.html'
})
export class ShellComponent {
  private authService = inject(AuthService);
  private fileService = inject(FileService);
  private userService = inject(UserService);
  private router = inject(Router);
  private notificationService = inject(NotificationService);
  private renderer = inject(Renderer2);

  currentUser = this.authService.currentUser;
  userName = computed(() => this.currentUser()?.userName || 'User');
  profilePicture = signal<string | null>(null);

  constructor() {
    this.profilePicture.set(this.currentUser()?.profilePicturePath || null);
    this.notificationService.startConnection();
  }

  toggleSidebar() {
    const body = document.body;
    const sidebar = document.querySelector('.app-sidebar');
    
    if (body.classList.contains('sidenav-toggled')) {
      body.classList.remove('sidenav-toggled');
      sidebar?.classList.remove('open');
    } else {
      body.classList.add('sidenav-toggled');
      sidebar?.classList.add('open');
    }
  }

  logout() {
    Swal.fire({
      title: 'Çıkış Yapılıyor',
      text: 'Oturumunuz güvenli bir şekilde sonlandırılıyor.',
      icon: 'info',
      timer: 1500,
      showConfirmButton: false,
      timerProgressBar: true,
      allowOutsideClick: false,
      didOpen: () => {
        Swal.showLoading();
        this.authService.logout();
      }
    }).then(() => {
      this.router.navigate(['/auth/login']);
    });
  }

  onProfileImageClick(fileInput: HTMLInputElement) {
    fileInput.click();
  }

  uploadProfilePicture(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    
    if (file) {
      Swal.fire({
        title: 'Resim yükleniyor...',
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        }
      });

      this.fileService.upload(file, 'profiles').subscribe({
        next: (res: Result<string>) => {
          if (res.isSuccess) {
            const path = res.data;
            this.userService.updateProfilePicture(path).subscribe({
              next: (updateRes: Result<string>) => {
                if (updateRes.isSuccess) {
                  this.profilePicture.set(path);
                  
                  const user = this.authService.currentUser();
                  if (user) {
                    const updatedUser = { ...user, profilePicturePath: path };
                    localStorage.setItem('user', JSON.stringify(updatedUser));
                    this.authService.currentUser.set(updatedUser);
                  }

                  Swal.fire({
                    icon: 'success',
                    title: 'Başarılı!',
                    text: 'Profil resminiz başarıyla güncellendi.',
                    timer: 2000,
                    showConfirmButton: false
                  });
                } else {
                  Swal.fire('Hata', 'Veritabanı güncellenemedi.', 'error');
                }
              },
              error: () => Swal.fire('Hata', 'Profil güncelleme isteği başarısız.', 'error')
            });
          } else {
            Swal.fire('Hata', 'Dosya yüklenirken bir hata oluştu.', 'error');
          }
        },
        error: () => Swal.fire('Hata', 'Sunucuya dosya gönderilemedi.', 'error')
      });
    }
  }

  getProfileImageUrl(): string {
    const path = this.profilePicture();
    let finalPath = path;
    if (finalPath && finalPath.startsWith('wwwroot')) {
      finalPath = finalPath.replace('wwwroot', '');
    }
    
    return finalPath ? `https://localhost:7273${finalPath}` : 'assets/images/users/8.jpg';
  }
}
