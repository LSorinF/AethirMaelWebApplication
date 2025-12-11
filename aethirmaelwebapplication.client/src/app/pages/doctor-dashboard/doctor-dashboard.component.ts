import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { AppointmentService } from '../../services/appointment.service';

// Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs'; // <--- IMPORT NOU
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatTabsModule, // <--- ADAUGAT
    MatProgressSpinnerModule,
    DatePipe
  ],
  templateUrl: './doctor-dashboard.component.html'
})
export class DoctorDashboardComponent implements OnInit {
  upcomingAppointments: any[] = [];
  pastAppointments: any[] = [];
  isLoading = true;

  constructor(
    private appService: AppointmentService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.appService.getDoctorAppointments().subscribe({
      next: (data) => {
        const now = new Date();

        // LOGICA DE FILTRARE:
        // 1. Viitoare: Data >= Acum (Indiferent de status, aici vedem ce avem de munca)
        this.upcomingAppointments = data.filter(a => new Date(a.appointmentDate) >= now);

        // 2. Istoric: Data < Acum
        this.pastAppointments = data.filter(a => new Date(a.appointmentDate) < now);

        // Sortare: Cele viitoare sa fie cronologic (cea mai apropiata prima)
        this.upcomingAppointments.sort((a, b) => new Date(a.appointmentDate).getTime() - new Date(b.appointmentDate).getTime());

        // Sortare: Cele vechi sa fie invers cronologic (cea mai recenta prima)
        this.pastAppointments.sort((a, b) => new Date(b.appointmentDate).getTime() - new Date(a.appointmentDate).getTime());

        this.isLoading = false;
      },
      error: () => {
        this.snackBar.open('Eroare la încărcarea datelor.', 'X');
        this.isLoading = false;
      }
    });
  }

  changeStatus(id: number, status: string) {
    this.appService.updateStatus(id, status).subscribe({
      next: () => {
        const msg = status === 'Confirmată' ? 'Programare acceptată!' : 'Programare respinsă.';
        this.snackBar.open(msg, 'OK', { duration: 3000, panelClass: status === 'Confirmată' ? ['bg-green-600', 'text-white'] : ['bg-red-500', 'text-white'] });
        this.loadAppointments(); // Reincarcam listele pentru a actualiza UI-ul
      },
      error: () => this.snackBar.open('Eroare la actualizare.', 'X')
    });
  }
}
