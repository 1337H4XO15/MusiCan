import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { LucideAngularModule, Sun, Moon } from 'lucide-angular';
import { LoginComponent } from './login/login.component';
import { SigninComponent } from './login/signin/signin.component';
import { SignupComponent } from './login/signup/signup.component';
import { HomeComponent } from './home/home.component';
import { ProfilComponent } from './profil/profil.component';
import { ArtistsComponent } from './artists/artists.component';
import { UserprofileComponent } from './profil/userprofile/userprofile.component';
import { ArtistprofileComponent } from './profil/artistprofile/artistprofile.component';
import { NotesComponent } from './notes/notes.component';
import { AddNotesComponent } from './notes/add-notes/add-notes.component';
import { ShowNotesComponent } from './notes/show-notes/show-notes.component';

import { PdfViewerModule } from 'ng2-pdf-viewer';
import { AddButtonComponent } from './add-button/add-button.component';
import { AuthGuard } from './services/auth.guard';
import { AuthInterceptor } from './services/auth.interceptor';
import { BackbtnComponent } from './notes/show-notes/backbtn/backbtn.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: LoginComponent },
  //{ path: 'index', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    LoginComponent,
    SigninComponent,
    SignupComponent,
    HomeComponent,
    ProfilComponent,
    ArtistsComponent,
    UserprofileComponent,
    ArtistprofileComponent,
    NotesComponent,
    AddNotesComponent,
    ShowNotesComponent,
    AddButtonComponent,
    BackbtnComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    LucideAngularModule.pick({ Sun, Moon }),
    ReactiveFormsModule,
    PdfViewerModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forRoot(routes)
  ],
  providers: [
    AuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
