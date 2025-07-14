import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Profile } from '../profil.component';

@Component({
  selector: 'app-artistprofile',
  standalone: false,
  templateUrl: './artistprofile.component.html',
  styleUrl: './artistprofile.component.css'
})
export class ArtistprofileComponent implements OnInit {
  artistForm!: FormGroup; // Non-null assertion
  isEditing: boolean = false;
  showPassword: boolean = false;

  constructor(private fb: FormBuilder) {

  }

  @Input() profile!: Profile;

  ngOnInit(): void {
    this.artistForm = this.fb.group({
      profileImage: ['https://upload.wikimedia.org/wikipedia/commons/6/6f/Beethoven.jpg'],
      username: [this.profile.name, Validators.required],
      email: [this.profile.mail, [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      birthYear: [this.profile.name, Validators.required],
      genre: [this.profile.genre, Validators.required],
      country: [this.profile.country, Validators.required],
      description: [this.profile.discription]
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
