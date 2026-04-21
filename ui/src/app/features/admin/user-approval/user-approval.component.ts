import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../core/services/admin.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-user-approval',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-approval.html'
})
export class UserApprovalComponent implements OnInit {
  private adminService = inject(AdminService);
  
  pendingUsers = signal<any[]>([]);
  isLoading = signal(true);

  ngOnInit() {
    this.loadPendingUsers();
  }

  loadPendingUsers() {
    this.isLoading.set(true);
    this.adminService.getPendingApprovals().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.pendingUsers.set(res.data);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  approve(userId: string) {
    Swal.fire({
      title: 'Kullanıcıyı onaylıyor musunuz?',
      text: "Bu işlemden sonra kullanıcı sisteme giriş yapabilecektir.",
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Evet, Onayla',
      cancelButtonText: 'Vazgeç'
    }).then((result) => {
      if (result.isConfirmed) {
        this.adminService.approveUser(userId).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('Başarılı', 'Kullanıcı onaylandı.', 'success');
            this.loadPendingUsers();
          } else {
            Swal.fire('Hata', res.message, 'error');
          }
        });
      }
    });
  }
}
