import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MusicService } from '../services/music.service';
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
      observable = this.musicService.getMusic();
    }
    observable.subscribe({
      next: (response) => {
        //this.router.navigate(['/home']); // TODO: routing
      },
      error: (error) => {
        this.error = true
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

  allPieces = [
    {
      title: 'Symphonie Nr. 5',
      composers: ['Ludwig van Beethoven'],
      genre: 'Klassik',
      year: 1808,
    },
    {
      title: 'Die Zauberflöte',
      composers: ['Wolfgang Amadeus Mozart'],
      genre: 'Oper',
      year: 1791,
    },
    {
      title: 'Rhapsody in Blue',
      composers: ['George Gershwin'],
      genre: 'Jazz',
      year: 1924,
    },
    {
      title: 'Boléro',
      composers: ['Maurice Ravel'],
      genre: 'Orchesterwerk',
      year: 1928,
    },
  ];

  musicPieces = [...this.allPieces];


  setSelectedPiece(id: number) {
    this.selectedPiece = this.musicPieces[id];
  }

  //müsste nochmal drübergeschaut werden
  deletePiece(): void {
    console.log('Lösche Stück mit ID:', this.selectedPiece);
    this.musicService.deleteMusic(this.selectedPiece.id);

    this.musicService.deleteMusic(this.selectedPiece.id).subscribe({
      next: (success) => {
        if (success) {
          console.log('Musikstück erfolgreich gelöscht');
        } else {
          console.warn('Löschen fehlgeschlagen');
        }
      },
      error: (err) => {
        console.error('Fehler beim Löschen:', err);
      }
    });
  }
}
