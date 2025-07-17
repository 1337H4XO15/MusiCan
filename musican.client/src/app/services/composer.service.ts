import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface DisplayComposer {
  id: string;
  artistName: string;
  genre: string;
  birthYear: string;
  country: string;
  description?: string;
  profileImage?: string;
  profileImageContentType?: string;
}

export type ComposerListResponse = DisplayComposer[];


@Injectable({
  providedIn: 'root'
})
export class ComposerService {
  private apiUrl = 'https://localhost:7012/Profile';

  constructor(private http: HttpClient) { }

  getComposers(): Observable<ComposerListResponse> {
    return this.http.get<ComposerListResponse>(`${this.apiUrl}/composers`);
  }

  getComposer(id: string): Observable<DisplayComposer> {
    return this.http.get<DisplayComposer>(`${this.apiUrl}/composer/${id}`);
  }

}
