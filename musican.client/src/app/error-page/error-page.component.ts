import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-error-page',
  standalone: false,
  templateUrl: './error-page.component.html',
  styleUrl: './error-page.component.css'
})
export class ErrorPageComponent {
  @Input() error: String = '';
}
