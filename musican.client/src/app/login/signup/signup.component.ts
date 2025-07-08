import { Component, EventEmitter, OnInit, Output } from '@angular/core';
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
  @Output() modeChange = new EventEmitter<'login' | 'register'>();
  @Output() loadingChange = new EventEmitter<boolean>();
  @Output() errorChange = new EventEmitter<string>();

  registerForm: FormGroup;
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
      isComposer: [false]
    }, { validators: this.passwordMatchValidator } as AbstractControlOptions);
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.loadingChange.emit(true);
      this.errorChange.emit('');

      this.authService.register(this.registerForm.value).subscribe({
        next: (response) => {
          this.loadingChange.emit(false);
          this.router.navigate(['/index']);
        },
        error: (error) => {
          this.loadingChange.emit(false);
          this.errorChange.emit(error.error || 'Registration failed');
        }
      });
    }
  }

  switchToLogin(): void {
    this.modeChange.emit('login');
  }

  // Helper methods for template
  get name() { return this.registerForm.get('name'); }
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }
  get isComposer() { return this.registerForm.get('isComposer'); }
}
