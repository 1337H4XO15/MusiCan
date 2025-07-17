import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DisplayMusic, MusicService } from '../../services/music.service';

@Component({
  selector: 'app-show-notes',
  standalone: false,
  templateUrl: './show-notes.component.html',
  styleUrl: './show-notes.component.css',
})
export class ShowNotesComponent implements OnInit, OnChanges {
  musicPiece!: DisplayMusic;
  error: string = '';

  constructor(private route: ActivatedRoute, private musicService: MusicService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = 'Kein Komponist gefunden.'
      return;
    }

    this.musicService.getMusic(id).subscribe({
      next: (response) => {
        this.musicPiece = response;
        this.error = '';
      },
      error: (error) => {
        this.error = typeof error.error === 'string' ? error.error : error?.message || 'Laden von Komponisten fehlgeschlagen';
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['music'] && changes['music'].currentValue) {
    }
  }

  getDownloadFilename(): string {
    if (!this.musicPiece?.contentType || !this.musicPiece?.title) return 'download';

    const extension = this.musicPiece.contentType === 'application/pdf' ? '.pdf' : '.jpg';
    return this.musicPiece.title + extension;
  }

  getDownloadText(): string {
    if (!this.musicPiece?.contentType) return 'Herunterladen';
    return this.musicPiece.contentType === 'application/pdf' ? 'PDF herunterladen' : 'Bild herunterladen';
  }
}
