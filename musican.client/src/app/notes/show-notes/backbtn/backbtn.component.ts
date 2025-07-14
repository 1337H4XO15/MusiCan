import { Component } from '@angular/core';
import { Location } from '@angular/common';


@Component({
  selector: 'app-backbtn',
  standalone: false,
  templateUrl: './backbtn.component.html',
  styleUrl: './backbtn.component.css'
})
export class BackbtnComponent {

  constructor(private location: Location) { }

  navigateBack(): void {
    this.location.back();
  }
}
