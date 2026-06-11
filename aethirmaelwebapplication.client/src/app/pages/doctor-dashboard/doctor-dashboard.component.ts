import { Component, OnInit, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

// Material Imports
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

// Servicii & Dialoguri existente
import { AppointmentService } from '../../services/appointment.service';
import { MedicalRecordService } from '../../services/medical-record.service';
import { HistoryDialogComponent } from '../../components/history-dialog/history-dialog.component';
import { AddRecordDialogComponent } from '../../components/add-record-dialog/add-record-dialog.component';

interface ChatMessage {
  text: string;
  isUser: boolean;
  time: Date;
}

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCardModule,
    MatTabsModule,
    MatSelectModule,
    MatFormFieldModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './doctor-dashboard.component.html'
})
export class DoctorDashboardComponent implements OnInit, AfterViewChecked {
  @ViewChild('scrollMe') private myScrollContainer!: ElementRef;

  isLoading = true;
  appointments: any[] = [];
  upcomingAppointments: any[] = [];
  pastAppointments: any[] = [];

  // --- STATISTICI LIVE TAB 3 ---
  uniquePatients: any[] = [];
  selectedPatientId: number | null = null;
  selectedPatientName = '';
  totalVisits = 0;
  latestDiagnosis = '-';
  firstVisitDate: Date | null = null;

  // --- CHAT AI TAB 3 ---
  currentInput = '';
  isAiLoading = false;
  messages: ChatMessage[] = [];

  constructor(
    private appointmentService: AppointmentService,
    private recordService: MedicalRecordService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private http: HttpClient
  ) { }

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.appointmentService.getDoctorAppointments().subscribe({
      next: (data) => {
        this.appointments = data;

        // Separăm programările
        this.upcomingAppointments = data.filter(a => a.status === 'Solicitata' || a.status === 'Confirmată');
        this.pastAppointments = data.filter(a => a.status === 'Finalizată' || a.status === 'Anulată');

        // Generăm lista unică de pacienți în mod dinamic pe baza programărilor primite
        const patientMap = new Map();
        data.forEach(a => {
          if (a.patientId && a.patientName) {
            patientMap.set(a.patientId, a.patientName);
          }
        });

        this.uniquePatients = Array.from(patientMap.entries()).map(([id, name]) => ({
          id: id,
          name: name
        }));

        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.snackBar.open('Eroare la încărcarea datelor.', 'Închide', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  // --- SCHIMBARE PACIENT (TAB 3 RAPOARTE) ---
  onPatientChange(patientId: number) {
    this.selectedPatientId = patientId;
    const patientObj = this.uniquePatients.find(p => p.id === patientId);
    this.selectedPatientName = patientObj ? patientObj.name : 'Pacient';

    // Resetăm conversația cu Maël
    this.messages = [
      {
        text: `Salut, doctore! Sunt Maël. Am acces la fișele medicale pentru ${this.selectedPatientName}. Întreabă-mă orice din istoricul său clinic.`,
        isUser: false,
        time: new Date()
      }
    ];

    // Încărcăm datele LIVE din istoricul real
    this.recordService.getPatientHistory(patientId).subscribe({
      next: (records) => {
        this.totalVisits = records.length;
        if (records.length > 0) {
          this.latestDiagnosis = records[0].diagnosis;
          this.firstVisitDate = records[records.length - 1].dateCreated;
        } else {
          this.latestDiagnosis = 'Nicio fișă salvată';
          this.firstVisitDate = null;
        }
      },
      error: (err) => {
        console.error(err);
        this.snackBar.open('Eroare la preluarea statisticilor pacientului.', 'Închide', { duration: 3000 });
      }
    });
  }

  // --- APEL NLP AI (TAB 3 CHAT) ---
  sendMessage() {
    if (!this.currentInput.trim() || this.isAiLoading || !this.selectedPatientId) return;

    const queryText = this.currentInput;
    // Injectăm automat un context explicit pentru asistentul backend
    const contextualizedPrompt = `[Medicul întreabă despre pacientul ${this.selectedPatientName}]: ${queryText}`;

    this.messages.push({ text: queryText, isUser: true, time: new Date() });
    this.currentInput = '';
    this.isAiLoading = true;
    this.scrollToBottom();

    this.http.post<{ response: string }>('https://localhost:7145/api/Mael/ask', { message: contextualizedPrompt })
      .subscribe({
        next: (res) => {
          this.messages.push({ text: res.response, isUser: false, time: new Date() });
          this.isAiLoading = false;
          this.scrollToBottom();
        },
        error: (err) => {
          console.error(err);
          this.messages.push({
            text: "🤖 Eroare: Nu m-am putut conecta la serverul AI. Verifică dacă API-ul C# rulează.",
            isUser: false,
            time: new Date()
          });
          this.isAiLoading = false;
          this.scrollToBottom();
        }
      });
  }

  // Aprobare / Anulare Programări
  acceptAppointment(app: any) {
    this.appointmentService.updateStatus(app.appointmentId, 'Confirmată').subscribe(() => {
      this.snackBar.open('Programare confirmată!', 'OK', { duration: 3000 });
      this.loadAppointments();
    });
  }

  cancelAppointment(app: any) {
    this.appointmentService.updateStatus(app.appointmentId, 'Anulată').subscribe(() => {
      this.snackBar.open('Programare anulată.', 'OK', { duration: 3000 });
      this.loadAppointments();
    });
  }

  openAddRecord(app: any) {
    const dialogRef = this.dialog.open(AddRecordDialogComponent, {
      width: '500px',
      data: { appointmentId: app.appointmentId, patientId: app.patientId, patientName: app.patientName }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadAppointments();
      }
    });
  }

  viewHistory(app: any) {
    this.dialog.open(HistoryDialogComponent, {
      width: '600px',
      data: { patientId: app.patientId, patientName: app.patientName }
    });
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    try {
      if (this.myScrollContainer) {
        setTimeout(() => {
          this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
        }, 50);
      }
    } catch (err) { }
  }
}
