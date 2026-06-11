import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { AppointmentService } from '../../services/appointment.service';
import { Route, RouterModule } from "@angular/router";

// Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatTabsModule } from '@angular/material/tabs'; 
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
    RouterModule,
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
        const now = new Date(); 

        //Filtrare viitoare și trecute
        this.upcomingAppointments = data.filter(a => new Date(a.appointmentDate) >= now);

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
