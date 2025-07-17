import { Component, Input, OnInit, OnChanges, SimpleChanges, EventEmitter, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Profile, ProfileResponse } from '../profil.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-artistprofile',
  standalone: false,
  templateUrl: './artistprofile.component.html',
  styleUrl: './artistprofile.component.css'
})
export class ArtistprofileComponent implements OnInit, OnChanges {
  artistForm!: FormGroup; // Non-null assertion
  isEditing: boolean = false;
  showPassword: boolean = false;
  selectedProfileImage: File | null = null;
  profileImagePreview: string | null = null;
  defaultImage = '/Beethoven.jpg';

  constructor(private fb: FormBuilder) {

  }

  @Input() profile!: Profile;
  @Input() postProfileFn!: (formData: FormData) => Observable<ProfileResponse>;
  @Input() edit!: boolean;
  @Output() switchToArtist = new EventEmitter<{ isArtist: boolean, isEdit: boolean }>();
  @Output() errorChange = new EventEmitter<string>();

  ngOnInit() {
    this.isEditing = this.edit;
    this.preloadDefaultImage();
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['edit']) {
      this.isEditing = changes['edit'].currentValue;
    }
    if (changes['profile'] && changes['profile'].currentValue) {
      this.initializeForm();
    }
  }

  private initializeForm(): void {
    if (this.profile && JSON.stringify(this.profile) !== '{}') {
      this.artistForm = this.fb.group({
        name: [this.profile.name, Validators.required],
        email: [this.profile.mail, [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        isComposer: [true],
        profileImage: null,
        birthYear: [this.profile.birthYear, Validators.required],
        genre: [this.profile.genre, Validators.required],
        country: [this.profile.country, Validators.required],
        description: [this.profile.description]
      });
      setTimeout(() => this.errorChange.emit(''), 0);
    } else {
      setTimeout(() => this.errorChange.emit('Kein gültiges Profil geladen'), 0);
    }
  }

  toggleEdit(): void {
    if (this.isEditing) {
      this.initializeForm();
      this.selectedProfileImage = null;
      this.profileImagePreview = null;
    }
    this.isEditing = !this.isEditing;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  toggleRole(event: any): void {
    this.switchToArtist.emit({ isArtist: false, isEdit: this.isEditing });
  }

  preloadDefaultImage(): void {
    const img = new Image();
    img.onload = () => console.log('Default image loaded');
    img.onerror = () => console.log('Default image failed to load');
    img.src = this.defaultImage;
  }

  getProfileImageSrc(): string {
    if (this.profileImagePreview) {
      return this.profileImagePreview;
    }
    if (this.profile?.profileImage) {
      return this.profile.profileImage;
    }
    return this.defaultImage;
  }

  onImageError(event: any): void {
    event.target.src = this.defaultImage;
  }

  onProfileImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input?.files?.length) {
      this.selectedProfileImage = input.files[0];

      const reader = new FileReader();
      reader.onload = (e) => {
        this.profileImagePreview = e.target?.result as string;
      };
      reader.readAsDataURL(this.selectedProfileImage);

      this.artistForm.patchValue({ profileImage: this.selectedProfileImage });
      this.artistForm.get('profileImage')?.updateValueAndValidity();
    } else {
      this.selectedProfileImage = null;
      this.profileImagePreview = null;
    }
  }

  saveArtist(): void {
    if (this.artistForm.invalid) {
      this.artistForm.markAllAsTouched();
      return;
    }

    this.errorChange.emit('');

    const formData = new FormData();
    const formValue = this.artistForm.value;

    formData.append('name', formValue.name);
    formData.append('email', formValue.email);
    formData.append('password', formValue.password);
    formData.append('isComposer', formValue.isComposer);

    if (this.selectedProfileImage) {
      formData.append('mimetype', this.selectedProfileImage.type);
      formData.append('profileImage', this.selectedProfileImage);
    }

    if (formValue.birthYear) formData.append('birthYear', formValue.birthYear);
    if (formValue.genre) formData.append('genre', formValue.genre);
    if (formValue.country) formData.append('country', formValue.country);
    if (formValue.description) formData.append('description', formValue.description);

    // Pass formData, not this.artistForm
    this.postProfileFn(formData).subscribe({
      next: (response) => {
        this.errorChange.emit('');
        console.log('Profile saved successfully'); // TODO: 
      },
      error: (error) => {
        this.errorChange.emit(typeof error.error === 'string' ? error.error : error?.message || 'Profil bearbeiten fehlgeschlagen');
      }
    });

    this.toggleEdit();
  }


  // Helper-Methoden für Template
  get password() {
    return this.artistForm.get('password');
  }

  get birthYear() {
    return this.artistForm.get('birthYear');
  }

  get genre() {
    return this.artistForm.get('genre');
  }

  get country() {
    return this.artistForm.get('country');
  }

  get email() {
    return this.artistForm.get('email');
  }

  get name() {
    return this.artistForm.get('name');
  }
}
