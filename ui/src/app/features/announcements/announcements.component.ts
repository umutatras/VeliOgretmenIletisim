import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AnnouncementService, Announcement } from '../../core/services/announcement.service';
import { FileService } from '../../core/services/file.service';

@Component({
  selector: 'app-announcements',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './announcements.html'
})
export class AnnouncementsComponent implements OnInit {
  announcements = signal<Announcement[]>([]);
  isLoading = signal(true);
  
  // Regular strings for ngModel
  title = '';
  content = '';
  attachmentPath: string | null = null;
  isUploading = signal(false);

  constructor(
    private announcementService: AnnouncementService,
    private fileService: FileService
  ) {}

  ngOnInit() {
    this.loadAnnouncements();
  }

  loadAnnouncements() {
    this.isLoading.set(true);
    this.announcementService.getAll(1, 20).subscribe({
      next: (res) => {
        if (res.isSuccess) this.announcements.set(res.data.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.isUploading.set(true);
      this.fileService.upload(file, 'announcements').subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.attachmentPath = res.data;
          }
          this.isUploading.set(false);
        },
        error: () => this.isUploading.set(false)
      });
    }
  }

  create() {
    if (!this.title || !this.content) return;

    this.announcementService.create({
      title: this.title,
      content: this.content,
      attachmentPath: this.attachmentPath
    }).subscribe(res => {
      if (res.isSuccess) {
        this.title = '';
        this.content = '';
        this.attachmentPath = null;
        this.loadAnnouncements();
      }
    });
  }

  delete(id: string) {
    if (confirm('Duyuruyu silmek istiyor musunuz?')) {
      this.announcementService.delete(id).subscribe(res => {
        if (res.isSuccess) this.loadAnnouncements();
      });
    }
  }
}
