import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.html'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  email = '';
  password = '';
  isLoading = signal(false);

  async onSubmit() {
    if (!this.email || !this.password) {
      Swal.fire('Uyarı', 'Lütfen tüm alanları doldurun.', 'warning');
      return;
    }

    this.isLoading.set(true);
    try {
      const success = await this.authService.login(this.email, this.password);
      if (success) {
        Swal.fire({
          icon: 'success',
          title: 'Hoş geldiniz!',
          text: 'Giriş başarılı.',
          timer: 1500,
          showConfirmButton: false
        });
        this.router.navigate(['/dashboard']);
      } else {
        Swal.fire('Hata', 'Geçersiz e-posta veya şifre.', 'error');
      }
    } catch (err: any) {
      Swal.fire('Hata', 'Sunucuyla iletişim kurulurken bir hata oluştu.', 'error');
    } finally {
      this.isLoading.set(false);
    }
  }
}
