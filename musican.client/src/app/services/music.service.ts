import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
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

  constructor(private http: HttpClient) { }

  getRandomMusic(): Observable<MusicListResponse> {
    return this.http.get<MusicListResponse>(`${this.apiUrl}/randomMusic`);
  }

  getOwnMusic(): Observable<MusicListResponse> {
    return this.http.get<MusicListResponse>(`${this.apiUrl}/ownMusic`);
  }

  getMusics(): Observable<MusicListResponse> {
    return this.http.get<MusicListResponse>(`${this.apiUrl}/music`);
  }

  getMusic(id: string): Observable<DisplayMusic> {
    return this.http.get<DisplayMusic>(`${this.apiUrl}/music/${id}`);
  }

  postMusic(request: FormData): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/music`, request);
  }

  updateMusic(request: MusicRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/music`, request);
  }

  deleteMusic(id: string): Observable<MusicListResponse> {
    return this.http.delete<MusicListResponse>(`${this.apiUrl}/music/${id}`);
  }
}
