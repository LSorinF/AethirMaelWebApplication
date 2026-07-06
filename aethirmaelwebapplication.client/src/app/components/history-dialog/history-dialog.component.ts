import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MedicalRecordService } from '../../services/medical-record.service';
import { MedicalRecord } from '../../models/medical-record.interface';

import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AddRecordDialogComponent } from '../add-record-dialog/add-record-dialog.component';

@Component({
  selector: 'app-history-dialog',
  standalone: true,
  imports: [
    CommonModule, MatDialogModule, MatButtonModule,
    MatProgressSpinnerModule, MatExpansionModule, MatIconModule, MatTooltipModule,
    DatePipe
  ],
  templateUrl: './history-dialog.component.html'
})
export class HistoryDialogComponent implements OnInit {
  records: MedicalRecord[] = [];
  isLoading = true;

  constructor(
    public dialogRef: MatDialogRef<HistoryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { patientId: number, patientName: string },
    private recordService: MedicalRecordService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar 
  ) { }

  ngOnInit() {
    this.loadHistory();
  }

  loadHistory() {
    this.isLoading = true;
    this.recordService.getPatientHistory(this.data.patientId).subscribe({
      next: (data) => {
        this.records = data;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  openEdit(record: MedicalRecord) {
    const dialogRef = this.dialog.open(AddRecordDialogComponent, {
      width: '500px',
      data: { existingRecord: record }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadHistory();
    });
  }


  deleteRecord(record: MedicalRecord) {
    if (!confirm('Sigur vrei să ștergi această fișă definitiv? Acțiunea este ireversibilă.')) return;

    this.isLoading = true;
    this.recordService.deleteRecord(record.medicalRecordId).subscribe({
      next: () => {
        this.snackBar.open('Fișă ștearsă cu succes.', 'OK', { duration: 3000 });
        this.records = this.records.filter(r => r.medicalRecordId !== record.medicalRecordId);
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);

        let errorMessage = 'A apărut o eroare la ștergere.';

        if (err.error && err.error.message) {
          errorMessage = err.error.message;
        } else if (typeof err.error === 'string') {
          errorMessage = err.error;
        }

        this.snackBar.open(errorMessage, 'X', { duration: 5000, panelClass: ['bg-red-500', 'text-white'] });
        this.isLoading = false;
      }
    });
  }
}
