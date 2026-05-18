import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DoctorService } from '../../services/doctor.service';
import { Doctor } from '../../models/doctor.interface';
import { AuthService } from '../../services/auth.service'; 

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AppointmentDialogComponent } from '../../components/appointment-dialog/appointment-dialog.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-doctor-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatDialogModule
  ],
  templateUrl: './doctor-list.component.html',
  styleUrls: ['./doctor-list.component.css']
})
export class DoctorListComponent implements OnInit {
  doctors: Doctor[] = [];
  isLoading = true;
  errorMessage = '';
  isAdmin = false; // Flag pentru admin

  constructor(
    private doctorService: DoctorService,
    private authService: AuthService,
    private dialog: MatDialog,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadDoctors();

    // Verificam rolul utilizatorului curent
    this.authService.currentUser$.subscribe(user => {
      this.isAdmin = user?.role === 'Admin';
    });
  }

  loadDoctors() {
    this.doctorService.getDoctors().subscribe({
      next: (data) => {
        this.doctors = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error(error);
        this.errorMessage = 'Nu am putut contacta serverul.';
        this.isLoading = false;
      }
    });
  }

  bookAppointment(doctor: Doctor) {
    if (!this.authService.isLoggedIn()) {
      this.snackBar.open('Trebuie să te loghezi pentru a face o programare!', 'Login', { duration: 4000 })
        .onAction().subscribe(() => this.router.navigate(['/login']));
      return;
    }

    this.dialog.open(AppointmentDialogComponent, {
      width: '400px',
      data: { doctor: doctor }
    });
  }

  // --- FUNCȚIE NOUĂ: DELETE ---
  deleteDoctor(doctor: Doctor) {
    if (!confirm(`Sigur vrei să ștergi medicul Dr. ${doctor.lastName}?`)) return;

    this.doctorService.deleteDoctor(doctor.doctorId).subscribe({
      next: () => {
        this.snackBar.open('Medic șters cu succes.', 'OK', { duration: 3000 });
        this.loadDoctors(); // Reîmprospătăm lista
      },
      error: (err) => {
        console.error(err);
        const msg = err.error || 'Eroare la ștergere (poate are programări active).';
        this.snackBar.open(msg, 'X', { panelClass: ['bg-red-500', 'text-white'] });
      }
    });
  }
}
