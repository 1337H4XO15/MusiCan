import { Component } from '@angular/core';

@Component({
  selector: 'app-add-notes',
  standalone: false,
  templateUrl: './add-notes.component.html',
  styleUrl: './add-notes.component.css',
})
export class AddNotesComponent {
  musicSheet = {
    title: '',
    author: '',
    year: null,
    genre: '',
  };

  selectedFile: File | null = null;

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input?.files?.length) {
      this.selectedFile = input.files[0];
    }
  }

  onSubmit() {
    if (!this.selectedFile) return;

    const formData = new FormData();
    formData.append('title', this.musicSheet.title);
    formData.append('author', this.musicSheet.author);
    if (this.musicSheet.year)
      formData.append('year', String(this.musicSheet.year));
    if (this.musicSheet.genre) formData.append('genre', this.musicSheet.genre);
    formData.append('pdf', this.selectedFile);

    // TODO: HTTP Upload via Service
    console.log('Hochgeladenes Formular:', formData);
    alert('Noten erfolgreich vorbereitet (Upload-Logik fehlt noch)');
  }
}
