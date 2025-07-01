import { Component } from '@angular/core';

@Component({
  selector: 'app-artists',
  standalone: false,
  templateUrl: './artists.component.html',
  styleUrl: './artists.component.css',
})
export class ArtistsComponent {
  flippedCards: Set<number> = new Set();

  artists = [
    {
      name: 'Ludwig van Beethoven',
      genre: 'Klassik',
      birthYear: 1770,
      country: 'Deutschland',
      description:
        'Wegbereiter der Romantik. Bedeutend für Sinfonien und Klaviersonaten.',
      image:
        'https://upload.wikimedia.org/wikipedia/commons/6/6f/Beethoven.jpg',
    },
    {
      name: 'Wolfgang Amadeus Mozart',
      genre: 'Klassik',
      birthYear: 1756,
      country: 'Österreich',
      description:
        'Komponist mit mehr als 600 Werken: Opern, Sinfonien, Konzerte.',
      image:
        'https://upload.wikimedia.org/wikipedia/commons/1/1e/Wolfgang-amadeus-mozart_1.jpg',
    },
    {
      name: 'Clara Schumann',
      genre: 'Romantik',
      birthYear: 1819,
      country: 'Deutschland',
      description:
        'Virtuose Pianistin und Komponistin, starke Stimme der Romantik.',
      image:
        'https://www.breitkopf.com/assets/komponisten/903_Schumann_C.jpg',
    },
    {
      name: 'Frédéric Chopin',
      genre: 'Romantik',
      birthYear: 1810,
      country: 'Polen',
      description:
        'Meister der Klaviermusik, poetischer Stil, gefühlvolle Nocturnes.',
      image:
        'https://www.klassika.info/Komponisten/Chopin/Bild.png',
    },
    {
      name: 'Johann Sebastian Bach',
      genre: 'Barock',
      birthYear: 1685,
      country: 'Deutschland',
      description:
        'Vater der Kontrapunktik. Werke wie das „Wohltemperierte Klavier“.',
      image:
        'https://upload.wikimedia.org/wikipedia/commons/6/6a/Johann_Sebastian_Bach.jpg',
    },
    {
      name: 'Joseph Haydn',
      genre: 'Klassik',
      birthYear: 1732,
      country: 'Österreich',
      description: '„Vater der Sinfonie“, Lehrer von Beethoven, Hofkomponist.',
      image:
        'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ5svTXE1ibV9V42yM0bMynw0TnhzoxA_TZYQ&s',
    },
    {
      name: 'Franz Schubert',
      genre: 'Romantik',
      birthYear: 1797,
      country: 'Österreich',
      description:
        'Liedermeister und Schöpfer traumhafter Kammermusik und Symphonien.',
      image:
        'https://www.concerti.de/wp-content/uploads/2019/07/Franz-Schubert-by-Kriehuber-1846-c-gemeinfrei.jpg',
    },
    {
      name: 'Richard Wagner',
      genre: 'Romantik',
      birthYear: 1813,
      country: 'Deutschland',
      description:
        'Komponist monumentaler Opern wie „Der Ring des Nibelungen“.',
      image:
        'https://image.geo.de/30145136/t/1l/v3/w1440/r1/-/richard-wagner-f-183301637-zlatko-guzmic-jpg--82631-.jpg',
    },
    {
      name: 'Fanny Hensel (geb. Mendelssohn)',
      genre: 'Romantik',
      birthYear: 1805,
      country: 'Deutschland',
      description:
        'Hochbegabte Komponistin, Schwester von Felix Mendelssohn Bartholdy.',
      image:
        'https://klassik-begeistert.de/wp-content/uploads/Fanny-Mendelssohn.jpeg',
    },
  ];

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
