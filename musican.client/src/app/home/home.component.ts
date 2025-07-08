import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent {
  isLoggedIn = true;

  constructor(private router: Router) {}

  handleButtonClick() {
    if (this.isLoggedIn) {
      this.router.navigate(['/hinzufügen']);
    } else {
      this.router.navigate(['/login']);
    }
  }
}
