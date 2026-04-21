import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService, UserProfile } from '../../core/services/user.service';
import { FileService } from '../../core/services/file.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.html'
})
export class ProfileComponent implements OnInit {
  private userService = inject(UserService);
  private fileService = inject(FileService);

  profile = signal<UserProfile | null>(null);
  isLoading = signal(true);

  // Change Password Form
  passwordData = {
    oldPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.isLoading.set(true);
    this.userService.getProfile().subscribe(res => {
      if (res.isSuccess) {
        this.profile.set(res.data);
      }
      this.isLoading.set(false);
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.fileService.upload(file).subscribe((res: any) => {
        if (res.isSuccess) {
          this.userService.updateProfilePicture(res.data).subscribe(uRes => {
            if (uRes.isSuccess) {
              Swal.fire('Başarılı', 'Profil resmi güncellendi. Değişiklikleri görmek için sayfayı yenileyin.', 'success');
              this.loadProfile();
            }
          });
        }
      });
    }
  }

  changePassword() {
    if (this.passwordData.newPassword !== this.passwordData.confirmPassword) {
      Swal.fire('Hata', 'Yeni şifreler eşleşmiyor.', 'error');
      return;
    }

    this.userService.changePassword(this.passwordData).subscribe(res => {
      if (res.isSuccess) {
        Swal.fire('Başarılı', 'Şifreniz değiştirildi.', 'success');
        this.passwordData = { oldPassword: '', newPassword: '', confirmPassword: '' };
      } else {
        Swal.fire('Hata', res.message, 'error');
      }
    });
  }
}
