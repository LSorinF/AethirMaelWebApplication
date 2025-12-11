import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { AppointmentService } from '../../services/appointment.service';

// Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatTabsModule } from '@angular/material/tabs'; // <--- IMPORT ESENȚIAL PENTRU HTML-ul NOU
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-my-appointments',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatTabsModule,
    MatProgressSpinnerModule,
    DatePipe
  ],
  templateUrl: './my-appointments.component.html'
})
export class MyAppointmentsComponent implements OnInit {
  upcomingAppointments: any[] = [];
  pastAppointments: any[] = [];
  isLoading = true;

  constructor(private appointmentService: AppointmentService) { }

  ngOnInit() {
    this.appointmentService.getMyAppointments().subscribe({
      next: (data) => {
        const now = new Date(); // Luăm timpul curent (ex: 10 Dec 12:04)

        // LOGICA DE FILTRARE:
        // Orice programare cu data >= acum merge la VIITOARE
        this.upcomingAppointments = data.filter(a => new Date(a.appointmentDate) >= now);

        // Orice programare cu data < acum merge la ISTORIC
        this.pastAppointments = data.filter(a => new Date(a.appointmentDate) < now);

        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }
}
