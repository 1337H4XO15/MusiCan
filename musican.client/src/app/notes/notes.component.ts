import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MusicService } from '../services/music.service';


@Component({
  selector: 'app-notes',
  standalone: false,
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.css',
})
export class NotesComponent implements OnInit {
  @Input() random: boolean = false; // Standardwert
  @Input() own: boolean = false; //nur eigene Musikstücke anzeigen
  error: boolean = false;
  selectedPiece: any;

  constructor(
    private musicService: MusicService,
    private router: Router
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
  }

  get showPublicPieces(): boolean {
    return !(this.random || this.own);
  }

  musicPieces = [
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
