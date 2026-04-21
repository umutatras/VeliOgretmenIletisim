import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import Swal from 'sweetalert2';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const user = authService.currentUser();
  
  const expectedRoles: string[] = route.data['roles'];

  if (user && expectedRoles.includes(user.role || '')) {
    return true;
  }

  Swal.fire({
    icon: 'error',
    title: 'Yetkisiz Erişim',
    text: 'Bu sayfayı görüntülemek için yetkiniz bulunmamaktadır.',
    timer: 3000,
    showConfirmButton: false
  });
  
  router.navigate(['/dashboard']);
  return false;
};
