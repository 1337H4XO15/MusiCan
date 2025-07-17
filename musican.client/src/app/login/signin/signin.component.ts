import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signin',
  standalone: false,
  templateUrl: './signin.component.html',
  styleUrl: './signin.component.css'
})

export class SigninComponent implements OnInit {
  @Output() loadingChange = new EventEmitter<boolean>();
  @Output() errorChange = new EventEmitter<string>();

  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      nameormail: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/home']);
    }
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.loadingChange.emit(true);
      this.errorChange.emit('');

      this.authService.login(this.loginForm.value).subscribe({
        next: (response) => {
          this.loadingChange.emit(false);
          this.router.navigate(['/home']);
        },
        error: (error) => {
          this.loadingChange.emit(false);
          this.errorChange.emit(typeof error.error === 'string' ? error.error : error?.message || 'Anmeldung fehlgeschlagen');
        }
      });
    }
  }

  // Helper methods for template
  get nameormail() { return this.loginForm.get('nameormail'); }
  get password() { return this.loginForm.get('password'); }
  get remember() { return this.loginForm.get('remember'); }
}
