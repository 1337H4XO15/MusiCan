import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export interface MusicRequest {
  id?: string;
  title: string;
  author: string;
  releaseyear?: string;
  genre?: string;
  mimetype: string;
  file: File;
}

export interface MusicOwner {
  id: string;
  name: string;
  isComposer: boolean;
}

export interface DisplayMusic {
  id: string;
  title: string;
  composer: string;
  contentType: string;
  fileData: string; 
  publication?: string;
  genre?: string;  
  timestamp: string;
  owner?: MusicOwner;
}

export type MusicListResponse = DisplayMusic[];

@Injectable({
  providedIn: 'root'
})
export class MusicService {
  private apiUrl = 'https://localhost:7012/Music';

  constructor(private http: HttpClient,
    private router: Router,
  //  private authService: AuthService
  ) { }

  getRandomMusic(): Observable<MusicListResponse> {
    return this.http.get<MusicListResponse>(`${this.apiUrl}/randomMusic`)
      .pipe(
        tap(response => {
          console.log(`tap: ${response}`);
        })
      );
  }

  getOwnMusic(): Observable<MusicListResponse> {
    return this.http.get<MusicListResponse>(`${this.apiUrl}/ownMusic`)
      .pipe(
        tap(response => {
          console.log(`tap: ${response}`);
        })
      );
  }

  getMusic(): Observable<MusicListResponse> {
    return this.http.get<MusicListResponse>(`${this.apiUrl}/music`)
      .pipe(
        tap(response => {
          console.log(`tap: ${response}`);
        })
      );
  }

  postMusic(request: FormData): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/music`, request)
      .pipe(
        tap(response => {
          console.log(`tap: ${response}`);
        })
      );
  }

  updateMusic(request: MusicRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/music`, request)
      .pipe(
        tap(response => {
          console.log(`tap: ${response}`);
        })
      );
  }

  deleteMusic(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/music/${id}`)
      .pipe(
        tap(success => {
          if (success) {
            console.log('Deletion succeeded');
          }
        })
      );
  }
}
