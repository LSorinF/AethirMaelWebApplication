import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Doctor } from '../../models/doctor.interface';
import { DoctorService } from '../../services/doctor.service';

@Component({
  selector: 'app-appointment-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule
  ],
  templateUrl: './appointment-dialog.component.html'
})
export class AppointmentDialogComponent {
  selectedDate: Date | null = null;
  selectedTime: string = '';
  notes: string = '';
  isLoading = false;

  // 1. Definim data minimă ca fiind "Acum"
  minDate = new Date();

  availableHours = [
    '09:00', '10:00', '11:00', '12:00',
    '13:00', '14:00', '15:00', '16:00', '17:00'
  ];

  constructor(
    public dialogRef: MatDialogRef<AppointmentDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { doctor: Doctor },
    private doctorService: DoctorService,
    private snackBar: MatSnackBar
  ) { }

  weekendFilter = (d: Date | null): boolean => {
    const day = (d || new Date()).getDay();
    return day !== 0 && day !== 6;
  };

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (!this.selectedDate || !this.selectedTime) {
      this.snackBar.open('Selectează data și ora!', 'OK', { duration: 3000 });
      return;
    }

    this.isLoading = true;

    const finalDate = new Date(this.selectedDate);
    const [hours, minutes] = this.selectedTime.split(':').map(Number);
    finalDate.setHours(hours, minutes);

    // Validare suplimentară în Frontend: Dacă a ales ziua de azi, ora trebuie să fie în viitor
    if (finalDate < new Date()) {
      this.snackBar.open('Nu poți selecta o oră din trecut!', 'OK', { duration: 3000, panelClass: ['bg-red-500', 'text-white'] });
      this.isLoading = false;
      return;
    }

    const payload = {
      doctorId: this.data.doctor.doctorId,
      appointmentDate: finalDate,
      patientNotes: this.notes
    };

    this.doctorService.createAppointment(payload).subscribe({
      next: () => {
        this.isLoading = false;
        this.snackBar.open('Programare trimisă cu succes!', 'OK', { duration: 4000, panelClass: ['bg-green-600', 'text-white'] });
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
        // Afișăm mesajul de eroare venit de la Backend (ex: "Nu poți face programări în trecut")
        const msg = err.error || 'Eroare la programare.';
        this.snackBar.open(msg, 'Închide', { duration: 3000, panelClass: ['bg-red-500', 'text-white'] });
      }
    });
  }
}
