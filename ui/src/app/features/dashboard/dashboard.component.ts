import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { AnnouncementService, Announcement } from '../../core/services/announcement.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html'
})
export class DashboardComponent implements OnInit {
  userName = signal('');
  announcements = signal<Announcement[]>([]);
  isLoading = signal(true);

  constructor(
    private authService: AuthService,
    private announcementService: AnnouncementService
  ) {
    this.userName.set(this.authService.currentUser()?.userName || 'User');
  }

  ngOnInit() {
    this.loadAnnouncements();
  }

  loadAnnouncements() {
    this.announcementService.getAll(1, 5).subscribe({
      next: (res) => {
        if (res.isSuccess) {
          this.announcements.set(res.data.items);
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  logout() {
    this.authService.logout();
  }
}
