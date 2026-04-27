import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MedicalRecordService } from '../../services/medical-record.service';
import { MedicalRecord } from '../../models/medical-record.interface';

import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from "@angular/material/button"; 

@Component({
  selector: 'app-patient-history',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatExpansionModule,
    MatButtonModule,
    DatePipe
  ],
  templateUrl: './patient-history.component.html'
})
export class PatientHistoryComponent implements OnInit {
  records: MedicalRecord[] = [];
  isLoading = true;
  isGeneratingPdf = false;
  datePipe = new DatePipe('en-US');

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

  downloadPDF(record: MedicalRecord) {
    this.isGeneratingPdf = true;

    // Gasim HTML-ul pentru fisa medicala
    const element = document.getElementById(`pdf-record-${record.medicalRecordId}`);

    if (element) {
      html2canvas(element, { scale: 2 }).then((canvas) => {
        //Dimensiuni pentru foaie
        const imgData = canvas.toDataURL('image/png');
        const pdf = new jsPDF('p', 'mm', 'a4');

        const pdfWidth = pdf.internal.pageSize.getWidth();
        const pdfHeight = (canvas.height * pdfWidth) / canvas.width;

        // Imagine 
        pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);

        // Nume fisier
        const formattedDate = this.datePipe.transform(record.dateCreated, 'dd-MM-yyyy');
        const fileName = `Fisa_Medicala_${formattedDate}.pdf`;

        pdf.save(fileName);

        this.isGeneratingPdf = false;
      }).catch(err => {
        console.error('Eroare la generarea PDF:', err);
        this.isGeneratingPdf = false;
      });
    }
  }
}

