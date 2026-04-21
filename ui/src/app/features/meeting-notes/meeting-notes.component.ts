import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MeetingNoteService, MeetingNote } from '../../core/services/meeting-note.service';

@Component({
  selector: 'app-meeting-notes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './meeting-notes.html'
})
export class MeetingNotesComponent implements OnInit {
  notes = signal<MeetingNote[]>([]);
  isLoading = signal(true);

  // Regular strings for ngModel
  title = '';
  content = '';

  constructor(private meetingNoteService: MeetingNoteService) {}

  ngOnInit() {
    this.loadNotes();
  }

  loadNotes() {
    this.isLoading.set(true);
    this.meetingNoteService.getAll(1, 50).subscribe({
      next: (res) => {
        if (res.isSuccess) this.notes.set(res.data.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  create() {
    if (!this.title || !this.content) return;
    this.meetingNoteService.create(this.title, this.content).subscribe(res => {
      if (res.isSuccess) {
        this.title = '';
        this.content = '';
        this.loadNotes();
      }
    });
  }

  delete(id: string) {
    if (confirm('Bu notu silmek istiyor musunuz?')) {
      this.meetingNoteService.delete(id).subscribe(res => {
        if (res.isSuccess) this.loadNotes();
      });
    }
  }
}
