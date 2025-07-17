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

  constructor(private fb: FormBuilder) {

  }

  @Input() profile!: Profile;
  @Input() postProfileFn!: (profileGroup: FormData) => Observable<ProfileResponse>;
  @Input() edit!: boolean;
  @Output() switchToArtist = new EventEmitter<{ isArtist: boolean, isEdit: boolean }>();
  @Output() errorChange = new EventEmitter<string>();

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
      setTimeout(() => this.errorChange.emit(''), 0);
    } else {
      setTimeout(() => this.errorChange.emit('Kein gültiges Profil geladen'), 0);
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

    this.errorChange.emit('');

    const formData = new FormData();
    const formValue = this.profileForm.value;

    formData.append('name', formValue.name);
    formData.append('email', formValue.email);
    formData.append('password', formValue.password);
    formData.append('isComposer', formValue.isComposer);

    this.postProfileFn(formData).subscribe({
      next: (response) => {
        this.errorChange.emit('');
        console.log('Profile saved successfully'); // TODO: response verarbeiten
      },
      error: (error) => {
        this.errorChange.emit(typeof error.error === 'string' ? error.error : error?.message || 'Profil bearbeiten fehlgeschlagen');
      }
    });

    this.toggleEdit();
  }

  get password() { return this.profileForm.get('password'); }
  get name() { return this.profileForm.get('name'); }
  get email() {
    return this.profileForm.get('email');
  }
}
