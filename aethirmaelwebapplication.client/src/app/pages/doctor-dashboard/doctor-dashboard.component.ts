import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { AppointmentService } from '../../services/appointment.service';

// Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { AddRecordDialogComponent } from '../../components/add-record-dialog/add-record-dialog.component';
import { HistoryDialogComponent } from '../../components/history-dialog/history-dialog.component';

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatButtonModule, MatIconModule,
    MatChipsModule, MatTabsModule, MatProgressSpinnerModule, MatDialogModule,
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
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.appService.getDoctorAppointments().subscribe({
      next: (data) => {
        const now = new Date();
        this.upcomingAppointments = data.filter(a => new Date(a.appointmentDate) >= now);
        this.pastAppointments = data.filter(a => new Date(a.appointmentDate) < now);

        this.upcomingAppointments.sort((a, b) => new Date(a.appointmentDate).getTime() - new Date(b.appointmentDate).getTime());
        this.pastAppointments.sort((a, b) => new Date(b.appointmentDate).getTime() - new Date(a.appointmentDate).getTime());

        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  changeStatus(id: number, status: string) {
    this.appService.updateStatus(id, status).subscribe({
      next: () => {
        this.snackBar.open(`Status actualizat: ${status}`, 'OK', { duration: 3000 });
        this.loadAppointments();
      },
      error: () => this.snackBar.open('Eroare.', 'X')
    });
  }

  openAddRecord(app: any) {
    const dialogRef = this.dialog.open(AddRecordDialogComponent, {
      width: '500px',
      data: {
        appointmentId: app.appointmentId,
        patientId: app.patientId,
        patientName: app.patientName
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadAppointments();
      }
    });
  }

  // Metoda de vazut instoricul medical
  viewHistory(app: any) {
    this.dialog.open(HistoryDialogComponent, {
      width: '600px',
      data: {
        patientId: app.patientId,
        patientName: app.patientName
      }
    });
  }
}
