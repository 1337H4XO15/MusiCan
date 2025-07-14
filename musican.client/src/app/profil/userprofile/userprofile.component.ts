import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Profile, UserRole } from '../profil.component';

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

  ngOnInit(): void {
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['profile'] && changes['profile'].currentValue) {
      this.initializeForm();
    }
  }

  private initializeForm(): void {
    if (this.profile && JSON.stringify(this.profile) !== '{}') {
      this.profileForm = this.fb.group({
        username: [this.profile.name, Validators.required],
        email: [this.profile.mail, [Validators.required, Validators.email]],
        role: [this.profile.role, Validators.required],
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
    const newRole = event.target.checked ? UserRole.Artist : UserRole.User;
    this.profileForm.patchValue({ role: newRole });
  }

  saveProfile() {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched(); // zeigt Fehler im Template an
      return;
    }

    console.log('Profil wird gespeichert:', this.profileForm.value);
    this.toggleEdit();
  }
}
