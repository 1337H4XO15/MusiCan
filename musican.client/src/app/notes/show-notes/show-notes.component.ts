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
  error: boolean = false;

  constructor(private route: ActivatedRoute, private musicService: MusicService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error = true // TODO: Display Error
      return;
    }

    this.musicService.getMusic(id).subscribe({
      next: (response) => {
        this.musicPiece = response;
      },
      error: (error) => {
        this.error = true // TODO: Display Error
      }
    });
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['music'] && changes['music'].currentValue) {
      this.initializeForm();
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
