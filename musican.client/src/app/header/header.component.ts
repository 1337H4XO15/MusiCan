import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service'; 

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  isLoggedIn: boolean = false;


  constructor(private authService: AuthService) { }

  ngOnInit(): void {
    this.checkLoginStatus();
  }

  checkLoginStatus(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isLoggedIn = user !== null;
    }); //nicht ändern, this.authService.isLoggedIn() kann nicht subscibed werden, deshalb funktioniert es damit nicht
  }

  logout(): void {
    this.authService.logout();
  }
}
