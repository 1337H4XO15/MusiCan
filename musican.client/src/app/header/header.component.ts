import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { SearchService } from '../services/search.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  isLoggedIn: boolean = false;
  searchTerm: string = '';

  @Output() search = new EventEmitter<string>();

  constructor(private authService: AuthService, public router: Router, private searchService: SearchService) { }

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

  onSearch() {
    console.log('Suchbegriff:', this.searchTerm);
    this.searchService.setSearchTerm(this.searchTerm);
  }

  onSearchIfCleared(term: string) {
    if (term === '') {
      this.onSearch();
    }
  }
}
