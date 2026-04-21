import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MeetingNoteService, MeetingNote } from '../../core/services/meeting-note.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-meeting-notes',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './meeting-notes.html'
})
export class MeetingNotesComponent implements OnInit {
  notes = signal<MeetingNote[]>([]);
  isLoading = signal(true);
  students = signal<any[]>([]);
  selectedParentId = '';
  note = '';

  constructor(private meetingNoteService: MeetingNoteService) {}

  ngOnInit() {
    this.loadNotes();
    this.loadMyStudents();
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

  loadMyStudents() {
    this.meetingNoteService.getMyStudentsForTeacher().subscribe(res => {
      if (res.isSuccess) this.students.set(res.data);
    });
  }

  create() {
    if (!this.selectedParentId || !this.note) {
      Swal.fire('Uyarı', 'Lütfen bir veli seçin ve notunuzu yazın.', 'warning');
      return;
    }

    this.meetingNoteService.create(this.selectedParentId, this.note).subscribe(res => {
      if (res.isSuccess) {
        Swal.fire({
          icon: 'success',
          title: 'Başarılı!',
          text: 'Görüşme notu başarıyla paylaşıldı.',
          timer: 2000,
          showConfirmButton: false
        });
        this.note = '';
        this.loadNotes();
      } else {
        Swal.fire('Hata', res.message, 'error');
      }
    });
  }

  delete(id: string) {
    Swal.fire({
      title: 'Emin misiniz?',
      text: "Bu görüşme notu kalıcı olarak silinecektir!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Evet, sil!',
      cancelButtonText: 'Vazgeç'
    }).then((result) => {
      if (result.isConfirmed) {
        this.meetingNoteService.delete(id).subscribe(res => {
          if (res.isSuccess) {
            Swal.fire('Silindi!', 'Not başarıyla kaldırıldı.', 'success');
            this.loadNotes();
          } else {
            Swal.fire('Hata', res.message, 'error');
          }
        });
      }
    });
  }
}
