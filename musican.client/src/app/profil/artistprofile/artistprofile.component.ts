import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-artistprofile',
  standalone: false,
  templateUrl: './artistprofile.component.html',
  styleUrl: './artistprofile.component.css'
})
export class ArtistprofileComponent {
  artistForm: FormGroup;
  isEditing = false;
  showPassword = false;

  constructor(private fb: FormBuilder) {
    this.artistForm = this.fb.group({
      profileImage: ['https://upload.wikimedia.org/wikipedia/commons/6/6f/Beethoven.jpg'],
      username: ['Ludwig van Beethoven', Validators.required],
      email: ['beethoven@classicmail.com', [Validators.required, Validators.email]],
      password: ['symphonie9', Validators.minLength(6)],
      birthYear: ['1770', Validators.required],
      genre: ['Klassik', Validators.required],
      country: ['Deutschland', Validators.required],
      description: ['Komponist von Symphonien, Sonaten und Meisterwerken.']
    });
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  saveArtist() {
    if (this.artistForm.invalid) {
      this.artistForm.markAllAsTouched();
      return;
    }

    console.log('Gespeicherte Künstlerdaten:', this.artistForm.value);
    this.toggleEdit();
  }
}
