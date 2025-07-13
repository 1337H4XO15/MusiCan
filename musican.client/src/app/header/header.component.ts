import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service'; 

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  isDarkMode: boolean = false;
  isLoggedIn: boolean = false;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    const stored = localStorage.getItem('darkmode');
    this.isDarkMode = stored === 'true';
    this.applyDarkMode();
    this.checkLoginStatus();
  }

  checkLoginStatus(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
  }

  toggleDarkMode(): void {
    this.isDarkMode = !this.isDarkMode;
    localStorage.setItem('darkmode', String(this.isDarkMode));
    this.applyDarkMode();
  }

  applyDarkMode(): void {
    document.body.classList.toggle('dark-mode', this.isDarkMode);
  }
}
