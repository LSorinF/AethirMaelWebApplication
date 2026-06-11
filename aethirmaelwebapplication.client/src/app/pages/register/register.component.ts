import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  userData = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    cnp: '',
    dateOfBirth: null 
  };
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  onRegister() {
    if (this.userData.password !== this.userData.confirmPassword) {
      this.snackBar.open('Parolele nu coincid!', 'OK', { duration: 3000, panelClass: ['bg-red-500', 'text-white'] });
      return;
    }

    if(!this.userData.cnp || !this.userData.phone || !this.userData.dateOfBirth) {
        this.snackBar.open('Toate câmpurile sunt obligatorii!', 'OK', { duration: 3000 });
        return;
    }

    this.isLoading = true;

    // Construim payload-ul complet pentru backend
    const payload = {
      firstName: this.userData.firstName,
      lastName: this.userData.lastName,
      email: this.userData.email,
      password: this.userData.password,
      cnp: this.userData.cnp,
      phone: this.userData.phone,
      dateOfBirth: this.userData.dateOfBirth
    };

    this.authService.register(payload).subscribe({
      next: () => {
        this.isLoading = false;
        this.snackBar.open('Cont creat cu succes! Te rog să te loghezi.', 'OK', { duration: 5000 });
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
        // Afisam mesajul de eroare server
        const errorMsg = err.error || 'A apărut o eroare la înregistrare.';
        this.snackBar.open(errorMsg, 'Închide', { duration: 3000, panelClass: ['bg-red-500', 'text-white'] });
      }
    });
  }
}
