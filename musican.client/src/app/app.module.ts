import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';

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
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    LucideAngularModule.pick({ Sun, Moon }),
    ReactiveFormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
