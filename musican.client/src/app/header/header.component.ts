import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { SearchService } from '../services/search.service';
import { Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

declare var bootstrap: any;

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  isLoggedIn: boolean = false;
  searchTerm: string = '';
  private routerSubscription?: Subscription;


  @Output() search = new EventEmitter<string>();

  constructor(private authService: AuthService, public router: Router, private searchService: SearchService) { }

  ngOnInit(): void {
    this.checkLoginStatus();

    //bei dieser Funktion hat KI unterstützt
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        if (event.urlAfterRedirects !== '/noten') {
          this.searchTerm = '';
          this.searchService.setSearchTerm('');
        }
      });
  }

  checkLoginStatus(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isLoggedIn = user !== null;
    }); //this.authService.isLoggedIn() kann nicht subscibed werden, deshalb funktioniert es damit nicht
  }

  logout(): void {
    this.authService.logout();
  }

  onSearch() {
    this.searchService.setSearchTerm(this.searchTerm);
  }

  onSearchIfCleared(term: string) {
    if (term === '') {
      this.onSearch();
    }
  }

  //bei dieser Funktion hat KI unterstützt
  closeNavbar(navbarCollapse: HTMLElement): void {
    if (window.innerWidth < 992) { // Nur bei kleinen Geräten
      const bsCollapse = bootstrap.Collapse.getInstance(navbarCollapse)
        || new bootstrap.Collapse(navbarCollapse, { toggle: false });
      bsCollapse.hide();
    }
  }
}
