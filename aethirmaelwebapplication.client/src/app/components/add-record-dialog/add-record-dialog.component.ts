import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MedicalRecordService } from '../../services/medical-record.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MedicalRecord, UpdateMedicalRecordDto } from '../../models/medical-record.interface';

// Material
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

@Component({
  selector: 'app-add-record-dialog',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatDialogModule,
    MatButtonModule, MatInputModule, MatFormFieldModule
  ],
  templateUrl: './add-record-dialog.component.html'
})
export class AddRecordDialogComponent implements OnInit {
  recordData = {
    symptoms: '',
    diagnosis: '',
    treatment: '',
    investigationResults: ''
  };
  isLoading = false;
  isEditMode = false;

  constructor(
    public dialogRef: MatDialogRef<AddRecordDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: {
      // Props pentru Create
      appointmentId?: number,
      patientId?: number,
      patientName?: string,
      // Props pentru Edit
      existingRecord?: MedicalRecord
    },
    private recordService: MedicalRecordService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit() {
    if (this.data.existingRecord) {
      this.isEditMode = true;
      const rec = this.data.existingRecord;
      // Pre-completăm formularul
      this.recordData = {
        symptoms: rec.symptoms,
        diagnosis: rec.diagnosis,
        treatment: rec.treatment,
        investigationResults: rec.investigationResults
      };
    }
  }

  onSubmit() {
    if (!this.recordData.symptoms || !this.recordData.diagnosis || !this.recordData.treatment) {
      this.snackBar.open('Completează câmpurile obligatorii!', 'OK', { duration: 3000 });
      return;
    }

    this.isLoading = true;

    const errorHandler = (err: any) => {
      console.error('Eroare detaliată:', err);
      let msg = 'A apărut o eroare la salvare/editare.';

      if (err.error && err.error.message) {
        msg = err.error.message;
      } else if (typeof err.error === 'string') {
        msg = err.error;
      } else if (err.error && err.error.title) {
        msg = err.error.title;
      }

      this.snackBar.open(msg, 'X', { duration: 5000, panelClass: ['bg-red-500', 'text-white'] });
      this.isLoading = false;
    };

    if (this.isEditMode && this.data.existingRecord) {
      // Logica de update
      const updatePayload: UpdateMedicalRecordDto = {
        medicalRecordId: this.data.existingRecord.medicalRecordId,
        ...this.recordData
      };

      this.recordService.updateRecord(updatePayload).subscribe({
        next: () => {
          this.snackBar.open('Fișă medicală actualizată!', 'OK', { duration: 3000 });
          this.dialogRef.close(true);
        },
        error: errorHandler
      });

    } else {
      //Logica de creare
      const createPayload = {
        patientId: this.data.patientId!,
        appointmentId: this.data.appointmentId,
        ...this.recordData
      };

      this.recordService.createRecord(createPayload).subscribe({
        next: () => {
          this.snackBar.open('Fișă medicală salvată!', 'OK', { duration: 3000 });
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error(err);
          this.snackBar.open('Eroare la salvare.', 'X');
          this.isLoading = false;
        }
      });
    }
  }
}
