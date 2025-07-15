import { Component, Input, OnInit, OnChanges, SimpleChanges, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Profile, ProfileResponse, UserRole } from '../profil.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-userprofile',
  standalone: false,
  templateUrl: './userprofile.component.html',
  styleUrl: './userprofile.component.css',
})
export class UserprofileComponent implements OnInit, OnChanges {
  profileForm!: FormGroup; // Non-null assertion
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
      this.profileForm = this.fb.group({
        name: [this.profile.name, Validators.required],
        email: [this.profile.mail, [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        isComposer: [false]
      });
      this.error = false;
    } else {
      this.error = true;
    }
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
    if (!this.isEditing) {
      this.profileForm.patchValue({ password: '' }); // Passwort beim Verlassen zurücksetzen
    }
  }

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  toggleRole(event: any) {
    this.switchToArtist.emit({ isArtist: true, isEdit: this.isEditing });
  }

  saveProfile() {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched(); // zeigt Fehler im Template an
      return;
    }

    this.postProfileFn(this.profileForm).subscribe({
      next: (response) => {
        console.log('Profile saved successfully');
      },
      error: (error) => {
        console.error('Failed to load profile:', error);
      }
    });

    this.toggleEdit();
  }
}
