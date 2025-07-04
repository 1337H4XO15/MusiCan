import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})

export class LoginComponent {
  isLoginMode = true;
  loading = false;
  error = '';

  onLoadingChange(loading: boolean): void {
    this.loading = loading;
  }

  onErrorChange(error: string): void {
    this.error = error;
  }

  onModeChange(mode: 'login' | 'register'): void {
    this.isLoginMode = mode === 'login';
    this.error = '';
  }
}
