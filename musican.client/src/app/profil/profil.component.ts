import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-profil',
  standalone: false,
  templateUrl: './profil.component.html',
  styleUrl: './profil.component.css'
})
export class ProfilComponent {
  role: 'artist' | 'user' | 'admin' | null= 'user';

  constructor(private authService: AuthService) {
    //this.role = this.authService.getCurrentUser()?.role;
  }
}
