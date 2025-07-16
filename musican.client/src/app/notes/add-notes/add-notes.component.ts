import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MusicService } from '../../services/music.service';

@Component({
  selector: 'app-add-notes',
  standalone: false,
  templateUrl: './add-notes.component.html',
  styleUrl: './add-notes.component.css',
})
export class AddNotesComponent implements OnInit {
  musicForm: FormGroup;
  error: boolean = false;
  selectedFile: File | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private musicService: MusicService,
    private router: Router
  ) {
    this.musicForm = this.fb.group({
      title: ['', Validators.required],
      author: ['', Validators.required],
      releaseyear: [''],
      genre: [''],
      file: [null, Validators.required]
    });
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input?.files?.length) {
      this.selectedFile = input.files[0];
      this.musicForm.patchValue({ file: this.selectedFile });
      this.musicForm.get('file')?.updateValueAndValidity();
    }
  }

  onSubmit(): void {
    if (this.musicForm.valid && this.selectedFile) {
      const formData = new FormData();
      const formValue = this.musicForm.value;

      formData.append('title', formValue.title);
      formData.append('author', formValue.author);
      if (formValue.releaseyear) formData.append('releaseyear', String(formValue.releaseyear));
      if (formValue.genre) formData.append('genre', formValue.genre);
      formData.append('mimetype', 'application/pdf')
      formData.append('file', this.selectedFile);

      this.musicService.postMusic(formData).subscribe({
        next: (response) => {
          this.router.navigate(['/home']); // TODO: success
        },
        error: (error) => {
          this.error = true
        }
      });
    }
  }
}
