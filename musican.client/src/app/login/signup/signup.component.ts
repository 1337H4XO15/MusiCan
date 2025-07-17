import { Component, EventEmitter, Output } from '@angular/core';
import { AbstractControlOptions, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signup',
  standalone: false,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css',
})
export class SignupComponent {
  @Output() loadingChange = new EventEmitter<boolean>();
  @Output() errorChange = new EventEmitter<string>();

  registerForm: FormGroup;
  selectedProfileImage: File | null = null;
  showPassword: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    name: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required],
    isComposer: [false],
    profilePicture: [''],
    genre: [''],
    birthYear: [''],
    country: [''],
    description: ['']
    }, { validators: this.passwordMatchValidator } as AbstractControlOptions);

    this.registerForm.get('isComposer')?.valueChanges.subscribe(isComposer => {
      const genreControl = this.registerForm.get('genre');
      const countryControl = this.registerForm.get('country');
      const birthYearControl = this.registerForm.get('birthYear');

      if (isComposer) {
        genreControl?.setValidators([Validators.required]);
        countryControl?.setValidators([Validators.required]);
        birthYearControl?.setValidators([Validators.required]);
      } else {
        genreControl?.clearValidators();
        countryControl?.clearValidators();
        birthYearControl?.clearValidators();
      }

      genreControl?.updateValueAndValidity();
      countryControl?.updateValueAndValidity();
      birthYearControl?.updateValueAndValidity();
    });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onProfileImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input?.files?.length) {
      this.selectedProfileImage = input.files[0];

      this.registerForm.patchValue({ profileImage: this.selectedProfileImage });
      this.registerForm.get('profileImage')?.updateValueAndValidity();
    } else {
      this.selectedProfileImage = null;
    }
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loadingChange.emit(true);
    this.errorChange.emit('');

    const formData = new FormData();
    const formValue = this.registerForm.value;

    formData.append('name', formValue.name);
    formData.append('email', formValue.email);
    formData.append('password', formValue.password);
    formData.append('isComposer', formValue.isComposer);

    if (this.selectedProfileImage) {
      formData.append('mimetype', this.selectedProfileImage.type);
      formData.append('profileImage', this.selectedProfileImage);
    }

    if (formValue.birthYear) formData.append('birthYear', formValue.birthYear);
    if (formValue.genre) formData.append('genre', formValue.genre);
    if (formValue.country) formData.append('country', formValue.country);
    if (formValue.description) formData.append('description', formValue.description);

    this.authService.register(formData).subscribe({
      next: (response) => {
        this.loadingChange.emit(false);
        this.router.navigate(['/']);
      },
      error: (error) => {
        this.loadingChange.emit(false);
        this.errorChange.emit(typeof error.error === 'string' ? error.error : error?.message || 'Registration fehlgeschlagen');
      }
    });
  }

  // Helper methods for template
  get email() { return this.registerForm.get('email'); }
  get name() { return this.registerForm.get('name'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }
  get profilePicture() { return this.registerForm.get('profilePicture'); }
  get genre() { return this.registerForm.get('genre'); }
  get birthYear() { return this.registerForm.get('birthYear'); }
  get country() { return this.registerForm.get('country'); }
  get description() { return this.registerForm.get('description'); }
}
