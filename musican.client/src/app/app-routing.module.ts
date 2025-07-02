import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home.component';
import { ProfilComponent } from './profil/profil.component';
import { Home } from 'lucide-angular';
import { ArtistsComponent } from './artists/artists.component';
import { NotesComponent } from './notes/notes.component';
import { ShowNotesComponent } from './notes/show-notes/show-notes.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent },
  { path: 'profil', component: ProfilComponent },
  { path: 'komponisten', component: ArtistsComponent },
  { path: 'noten', component: NotesComponent },
  { path: 'noten/:id', component: ShowNotesComponent },
  { path: '', component: HomeComponent },
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
