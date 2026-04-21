import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html'
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  registerForm = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    role: 2 // Default: Teacher (Domain/Enums/UserRole.Teacher = 2)
  };

  isLoading = signal(false);

  register() {
    if (!this.registerForm.firstName || !this.registerForm.lastName || 
        !this.registerForm.email || !this.registerForm.password) {
      Swal.fire('Hata', 'Lütfen tüm alanları doldurunuz.', 'error');
      return;
    }

    this.isLoading.set(true);
    this.authService.register(this.registerForm).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          Swal.fire({
            title: 'Kayıt Başarılı!',
            text: res.message,
            icon: 'success',
            confirmButtonText: 'Tamam'
          }).then(() => {
            this.router.navigate(['/auth/login']);
          });
        } else {
          Swal.fire('Hata', res.message, 'error');
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'Kayıt sırasında bir hata oluştu.';
        Swal.fire('Hata', errorMsg, 'error');
        this.isLoading.set(false);
      }
    });
  }
}
