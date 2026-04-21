import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComplaintService, Complaint } from '../../core/services/complaint.service';
import { AuthService } from '../../core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-complaints',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './complaints.html'
})
export class ComplaintsComponent implements OnInit {
  private complaintService = inject(ComplaintService);
  private authService = inject(AuthService);

  complaints = signal<Complaint[]>([]);
  isLoading = signal(true);
  currentUser = this.authService.currentUser;

  // Form Fields
  subject = '';
  description = '';
  
  // Admin Response Fields
  selectedComplaint: Complaint | null = null;
  adminResponseText = '';
  adminResponseStatus = 2; // Default to 'İnceleniyor' (assuming 2 is İnceleniyor, 3 is Tamamlandı)

  ngOnInit() {
    this.loadComplaints();
  }

  loadComplaints() {
    this.isLoading.set(true);
    this.complaintService.getComplaints().subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.complaints.set(res.data.items);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  sendComplaint() {
    if (!this.subject || !this.description) {
      Swal.fire('Uyarı', 'Lütfen konu ve açıklama girin.', 'warning');
      return;
    }

    this.complaintService.create(this.subject, this.description).subscribe(res => {
      if (res.isSuccess) {
        Swal.fire('Gönderildi', 'Talebiniz başarıyla iletildi.', 'success');
        this.subject = '';
        this.description = '';
        this.loadComplaints();
      }
    });
  }

  openAnswerModal(complaint: Complaint) {
    this.selectedComplaint = complaint;
    this.adminResponseText = complaint.adminResponse || '';
  }

  submitAnswer() {
    if (!this.selectedComplaint || !this.adminResponseText) {
      Swal.fire('Uyarı', 'Lütfen bir yanıt yazın.', 'warning');
      return;
    }

    this.complaintService.answer(
      this.selectedComplaint.id, 
      this.adminResponseText, 
      Number(this.adminResponseStatus)
    ).subscribe(res => {
      if (res.isSuccess) {
        Swal.fire('Başarılı', 'Talep yanıtlandı ve durum güncellendi.', 'success');
        this.selectedComplaint = null;
        this.loadComplaints();
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    switch (status) {
      case 'Pending': return 'bg-warning-transparent text-warning';
      case 'InProgress': return 'bg-info-transparent text-info';
      case 'Resolved': return 'bg-success-transparent text-success';
      case 'Rejected': return 'bg-danger-transparent text-danger';
      default: return 'bg-primary-transparent text-primary';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'Pending': return 'Beklemede';
      case 'InProgress': return 'İnceleniyor';
      case 'Resolved': return 'Tamamlandı';
      case 'Rejected': return 'Reddedildi';
      default: return status;
    }
  }
}
