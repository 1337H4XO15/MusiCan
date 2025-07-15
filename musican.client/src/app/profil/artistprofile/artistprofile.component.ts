import { Component, Input, OnInit, OnChanges, SimpleChanges, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Profile, ProfileResponse } from '../profil.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-artistprofile',
  standalone: false,
  templateUrl: './artistprofile.component.html',
  styleUrl: './artistprofile.component.css'
})
export class ArtistprofileComponent implements OnInit, OnChanges {
  artistForm!: FormGroup; // Non-null assertion
  isEditing: boolean = false;
  showPassword: boolean = false;
  error: boolean = false;

  constructor(private fb: FormBuilder) {

  }

  @Input() profile!: Profile;
  @Input() postProfileFn!: (profileGroup: FormGroup) => Observable<ProfileResponse>;
  @Input() edit!: boolean;
  @Output() switchToArtist = new EventEmitter<{ isArtist: boolean, isEdit: boolean }>();

  ngOnInit(): void {
    this.isEditing = this.edit;
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['edit']) {
      this.isEditing = changes['edit'].currentValue;
    }
    if (changes['profile'] && changes['profile'].currentValue) {
      this.initializeForm();
    }
  }

  private initializeForm(): void {
    if (this.profile && JSON.stringify(this.profile) !== '{}') {
      this.artistForm = this.fb.group({
        name: [this.profile.name, Validators.required],
        email: [this.profile.mail, [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        isComposer: [true],
        profileImage: ['https://upload.wikimedia.org/wikipedia/commons/6/6f/Beethoven.jpg'],
        birthYear: [this.profile.name, Validators.required],
        genre: [this.profile.genre, Validators.required],
        country: [this.profile.country, Validators.required],
        description: [this.profile.description]
      });
      this.error = false;
    } else {
      this.error = true;
    }
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  toggleRole(event: any) {
    this.switchToArtist.emit({ isArtist: false, isEdit: this.isEditing });
  }

  saveArtist() {
    if (this.artistForm.invalid) {
      this.artistForm.markAllAsTouched();
      return;
    }

    this.postProfileFn(this.artistForm).subscribe({
      next: (response) => {
        console.log('Profile saved successfully');
      },
      error: (error) => {
        console.error('Failed to load profile:', error);
      }
    });

    console.log('Gespeicherte Künstlerdaten:', this.artistForm.value);
    this.toggleEdit();
  }
}
