import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComplaintService, Complaint } from '../../core/services/complaint.service';

@Component({
  selector: 'app-complaints',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './complaints.html'
})
export class ComplaintsComponent implements OnInit {
  complaints = signal<Complaint[]>([]);
  isLoading = signal(true);
  
  // Regular strings for ngModel
  subject = '';
  content = '';

  selectedComplaint = signal<Complaint | null>(null);
  adminResponse = '';

  constructor(private complaintService: ComplaintService) {}

  ngOnInit() {
    this.loadComplaints();
  }

  loadComplaints() {
    this.isLoading.set(true);
    this.complaintService.getComplaints().subscribe({
      next: (res) => {
        if (res.isSuccess) this.complaints.set(res.data.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  create() {
    if (!this.subject || !this.content) return;
    this.complaintService.create(this.subject, this.content).subscribe(res => {
      if (res.isSuccess) {
        this.subject = '';
        this.content = '';
        this.loadComplaints();
      }
    });
  }

  submitResponse() {
    const comp = this.selectedComplaint();
    if (!comp || !this.adminResponse) return;

    this.complaintService.answer(comp.id, this.adminResponse, 1).subscribe(res => {
      if (res.isSuccess) {
        this.adminResponse = '';
        this.selectedComplaint.set(null);
        this.loadComplaints();
      }
    });
  }
}
