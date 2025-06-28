import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  mode: 'signin' | 'signup' = 'signin';

  switchMode(mode: 'signin' | 'signup') {
    this.mode = mode;
  }
}
