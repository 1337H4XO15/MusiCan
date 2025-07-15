import { Component, OnInit } from '@angular/core';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { FormGroup } from '@angular/forms';

export enum UserRole {
  Admin = 0,
  Artist = 1,
  User = 2,
  Banned = 3
}

export interface ProfileResponse {
  name: string;
  mail: string;
  role: number;
  birtyear: string;
  genre: string;
  country: string;
  description: string;
}

export interface Profile {
  name: string;
  mail: string;
  role: UserRole;
  birtyear?: Date;
  genre?: string;
  country?: string;
  description?: string;
  error?: string;
}

@Component({
  selector: 'app-profil',
  standalone: false,
  templateUrl: './profil.component.html',
  styleUrl: './profil.component.css'
})
export class ProfilComponent implements OnInit {
  private apiUrl = 'https://localhost:7012/Profile';
  profile!: Profile; // Non-null assertion
  isArtist: boolean = false;
  edit: boolean = false;
  postProfileFn = this.postProfile.bind(this); // Forwarding von http

  constructor(private http: HttpClient,
    private router: Router,
    private authService: AuthService) {
  }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    this.getProfile().subscribe({
      next: (response) => {
        console.log('Profile loaded successfully');
      },
      error: (error) => {
        console.error('Failed to load profile:', error);
      }
    });
  }

  onSwitch(event: { isArtist: boolean, isEdit: boolean }) {
    this.isArtist = event.isArtist;
    this.edit = event.isEdit;
  }

  getProfile(): Observable<ProfileResponse> {
    // Token über Interceptor gesetzt
    return this.http.get<ProfileResponse>(`${this.apiUrl}/profile`)
      .pipe(
        tap(response => {
          this.handleProfileResponse(response);
        })
      );
  }

  postProfile(profileGroup: FormGroup): Observable<ProfileResponse> {

    return this.http.post<ProfileResponse>(`${this.apiUrl}/profile`, profileGroup.value)
      .pipe(
        tap(response => {
          this.handleProfileResponse(response);
        })
    )
  }

  private handleProfileResponse(response: ProfileResponse): void {
    console.log("Profile response:", response);
    let profile: Profile = {
      name: response.name,
      mail: response.mail,
      role: response.role as UserRole
    };

    if (profile.role == UserRole.Artist) {
      this.isArtist = true;
      profile.birtyear = new Date(response.birtyear);
      profile.genre = response.genre;
      profile.country = response.country;
      profile.description = response.description;
    }

    this.profile = profile;
  }
}
