import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-show-notes',
  standalone: false,
  templateUrl: './show-notes.component.html',
  styleUrl: './show-notes.component.css'
})
export class ShowNotesComponent implements OnInit{
  musicPiece: any;

  musicPieces = [
    {
      title: 'Symphonie Nr. 5',
      composers: ['Ludwig van Beethoven'],
      genre: 'Klassik',
      year: 1808,
      description: 'Eines der berühmtesten klassischen Werke der Musikgeschichte.',
      sheetMusicUrl: 'sheet-music/beethoven5.png'
    },
    {
      title: 'Die Zauberflöte',
      composers: ['Wolfgang Amadeus Mozart'],
      genre: 'Oper',
      year: 1791,
      description: 'Eine der bekanntesten Opern mit märchenhaften Elementen.',
      sheetMusicUrl: 'assets/sheet-music/zauberfloete.png'
    }
  ];

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.musicPiece = this.musicPieces[+id!];
  }
}
