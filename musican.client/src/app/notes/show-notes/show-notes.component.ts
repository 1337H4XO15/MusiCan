import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-show-notes',
  standalone: false,
  templateUrl: './show-notes.component.html',
  styleUrl: './show-notes.component.css',
})
export class ShowNotesComponent implements OnInit, OnChanges {
  musicPiece: any;
  pdfSrc = 'https://vadimdez.github.io/ng2-pdf-viewer/assets/pdf-test.pdf'; //im Test hat es mit einer localen Datei nicht funktioniert, muss dann an Datenbank angepasst werden

  musicPieces = [
    {
      title: 'Symphonie Nr. 5',
      composers: ['Ludwig van Beethoven'],
      genre: 'Klassik',
      year: 1808,
      description:
        'Eines der berühmtesten klassischen Werke der Musikgeschichte.',
      sheetMusicUrl: 'sheet-music/beethoven5.pdf',
    },
    {
      title: 'Die Zauberflöte',
      composers: ['Wolfgang Amadeus Mozart'],
      genre: 'Oper',
      year: 1791,
      description: 'Eine der bekanntesten Opern mit märchenhaften Elementen.',
      sheetMusicUrl: 'assets/sheet-music/zauberfloete.png',
    },
  ];

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    //const id = this.route.snapshot.paramMap.get('id');
    //this.musicPiece = this.musicPieces[+id!];
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['profile'] && changes['profile'].currentValue) {
      this.initializeForm();
    }
  }

  private initializeForm(): void {
    //if (this.profile && JSON.stringify(this.profile) !== '{}') {
    //  this.profileForm = this.fb.group({
    //    username: [this.profile.name, Validators.required],
    //    email: [this.profile.mail, [Validators.required, Validators.email]],
    //    password: ['', [Validators.required, Validators.minLength(6)]],
    //    role: [this.profile.role, Validators.required],
    //  });
    //  this.error = false;
    //} else {
    //  this.error = true;
    //}
  }
}
