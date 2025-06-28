import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  isDarkMode = false;

  ngOnInit(): void {
    const stored = localStorage.getItem('darkmode');
    this.isDarkMode = stored === 'true';
    this.applyDarkMode();
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
