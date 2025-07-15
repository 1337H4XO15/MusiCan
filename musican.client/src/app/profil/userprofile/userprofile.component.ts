import { Component, Input, OnInit, OnChanges, SimpleChanges, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Profile, ProfileResponse } from '../profil.component';
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
  @Input() postProfileFn!: (profileGroup: FormData) => Observable<ProfileResponse>;
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

  toggleEdit(): void {
    this.isEditing = !this.isEditing;
    if (!this.isEditing) {
      this.profileForm.patchValue({ password: '' }); // Passwort beim Verlassen zurücksetzen
    }
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  toggleRole(event: any): void {
    this.switchToArtist.emit({ isArtist: true, isEdit: this.isEditing });
  }

  saveProfile(): void {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const formData = new FormData();
    const formValue = this.profileForm.value;

    formData.append('name', formValue.name);
    formData.append('email', formValue.email);
    formData.append('password', formValue.password);
    formData.append('isComposer', formValue.isComposer);

    this.postProfileFn(formData).subscribe({
      next: (response) => {
        console.log('Profile saved successfully'); // TODO: response verarbeiten
      },
      error: (error) => {
        console.error('Failed to load profile:', error); // TODO: Error
      }
    });

    this.toggleEdit();
  }
}
