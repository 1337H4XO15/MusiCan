import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-userprofile',
  standalone: false,
  templateUrl: './userprofile.component.html',
  styleUrl: './userprofile.component.css',
})
export class UserprofileComponent {
  profileForm: FormGroup;
  isEditing = false;
  showPassword = false;

  constructor(private fb: FormBuilder) {
    this.profileForm = this.fb.group({
      email: ['user@example.com', [Validators.required, Validators.email]],
      username: ['MaxMustermann', Validators.required],
      password: ['123456', Validators.minLength(6)],
      role: ['user', Validators.required],
    });
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
    const newRole = event.target.checked ? 'artist' : 'user';
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
