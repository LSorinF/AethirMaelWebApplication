import { Component } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { RouterModule } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule], 
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  // Injectam AuthService si il facem public pentru a fi accesat in HTML
  constructor(public authService: AuthService) {}

  logout() {
    this.authService.logout();
  }
}
