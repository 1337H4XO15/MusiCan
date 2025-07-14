import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-notes',
  standalone: false,
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.css',
})
export class NotesComponent {
  @Input() random: boolean = false; // Standardwert
  @Input() own: boolean = false; //nur eigene Musikstücke anzeigen

  ngOnInit() {
    if (this.random) {
      //Musik laden
    } else if (this.own) {
      //Musik laden
    } else {
      //Musik laden
    }
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
}
