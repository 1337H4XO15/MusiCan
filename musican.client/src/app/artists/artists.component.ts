import { Component, OnInit } from '@angular/core';
import { ComposerListResponse, ComposerService, DisplayComposer } from '../services/composer.service';

@Component({
  selector: 'app-artists',
  standalone: false,
  templateUrl: './artists.component.html',
  styleUrl: './artists.component.css',
})
export class ArtistsComponent implements OnInit {
  flippedCards: Set<number> = new Set();
  artists!: ComposerListResponse;
  defaultImage = '/Beethoven.jpg';
  error: string = '';

  constructor(
    private composerService: ComposerService 
  ) { }

  ngOnInit() {
    this.preloadDefaultImage();
    this.composerService.getComposers().subscribe({
      next: (response) => {
        this.artists = response;
        this.error = '';
      },
      error: (error) => {
        this.error = typeof error.error === 'string' ? error.error : error?.message || 'Laden von Komponisten fehlgeschlagen';
      }
    });
  }

  preloadDefaultImage(): void {
    const img = new Image();
    img.onload = () => console.log('Default image loaded');
    img.onerror = () => console.log('Default image failed to load');
    img.src = this.defaultImage;
  }

  getArtistImageSrc(artist: DisplayComposer): string {
    if (artist.profileImage) {
      return `data:${artist.profileImageContentType};base64,${artist.profileImage}`;
    }
    return this.defaultImage;
  }

  onImageError(event: any): void {
    event.target.src = this.defaultImage;
  }

  toggleCard(index: number) {
    if (this.flippedCards.has(index)) {
      this.flippedCards.delete(index);
    } else {
      this.flippedCards.add(index);
    }
  }

  isFlipped(index: number): boolean {
    return this.flippedCards.has(index);
  }
}
