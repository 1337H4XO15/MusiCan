import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MusicListResponse, MusicService } from '../services/music.service';
import { SearchService } from '../services/search.service';


@Component({
  selector: 'app-notes',
  standalone: false,
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.css',
})
export class NotesComponent implements OnInit {
  @Input() random: boolean = false; // Standardwert
  @Input() own: boolean = false; //nur eigene Musikstücke anzeigen
  searchTerm: string = '';
  error: boolean = false;
  selectedPiece: any;
  allPieces!: MusicListResponse;
  musicPieces!: MusicListResponse;

  constructor(
    private musicService: MusicService,
    private router: Router,
    private searchService: SearchService
  ) { }

  ngOnInit() {
    let observable;
    if (this.random) {
      observable = this.musicService.getRandomMusic();
    } else if (this.own) {
      observable = this.musicService.getOwnMusic();
    } else {
      observable = this.musicService.getMusics();
    }
    observable.subscribe({
      next: (response) => {
        this.allPieces = response;
        this.musicPieces = [...this.allPieces];
      },
      error: (error) => {
        this.error = true // TODO: Display Error
      }
    });


    this.searchService.searchTerm$.subscribe(term => {
      this.searchTerm = term;
      console.log('Suchbegriff empfangen:', term);
      if (!this.random && !this.own) {
        this.filterPieces(this.searchTerm);
      }
    });
  }

  filterPieces(term: string) {
    const lowerTerm = term.toLowerCase().trim();

    if (lowerTerm) {
      this.musicPieces = this.allPieces.filter(piece =>
        piece.title.toLowerCase().includes(lowerTerm)
      );
    } else {
      this.musicPieces = this.allPieces;
    }
  }

  get showPublicPieces(): boolean {
    return !(this.random || this.own);
  }

  setSelectedPiece(id: number) {
    this.selectedPiece = this.musicPieces[id];
  }

  deletePiece(): void {
    this.musicService.deleteMusic(this.selectedPiece.id).subscribe({
      next: (response) => {
        this.allPieces = response;
        this.musicPieces = [...this.allPieces]; // TODO: display Success
      },
      error: (err) => {
        console.error('Fehler beim Löschen:', err);// TODO: display Error
      }
    });
  }
}
