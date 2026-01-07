import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MedicalRecordService } from '../../services/medical-record.service';
import { MedicalRecord } from '../../models/medical-record.interface';

import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion'; // Pentru a extinde detaliile

@Component({
  selector: 'app-patient-history',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatExpansionModule,
    DatePipe
  ],
  templateUrl: './patient-history.component.html'
})
export class PatientHistoryComponent implements OnInit {
  records: MedicalRecord[] = [];
  isLoading = true;

  constructor(private recordService: MedicalRecordService) { }

  ngOnInit() {
    this.recordService.getMyHistory().subscribe({
      next: (data) => {
        this.records = data;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }
}
