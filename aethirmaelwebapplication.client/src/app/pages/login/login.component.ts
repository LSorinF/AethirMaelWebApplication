import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // Pentru ngModel
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

// Angular Material
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule,
    RouterModule,
    MatCardModule, 
    MatInputModule, 
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  credentials = {
    email: '',
    password: ''
  };
  isLoading = false;

  constructor(
    private authService: AuthService, 
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  onLogin() {
    if (!this.credentials.email || !this.credentials.password) return;

    this.isLoading = true;
    
    this.authService.login(this.credentials).subscribe({
      next: (res) => {
        // Succes
        this.isLoading = false;
        this.snackBar.open(`Bine ai venit, ${res.name}!`, 'OK', { duration: 3000 });
        this.router.navigate(['/home']); // Redirectionam acasa
      },
      error: (err) => {
        // Eroare
        this.isLoading = false;
        console.error(err);
        this.snackBar.open('Email sau parolă incorectă.', 'Închide', { 
          duration: 3000, 
          panelClass: ['bg-red-500', 'text-white'] 
        });
      }
    });
  }
}
