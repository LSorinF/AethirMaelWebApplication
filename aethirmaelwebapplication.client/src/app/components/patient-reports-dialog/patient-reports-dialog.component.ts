import { Component, Inject, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpClient } from '@angular/common/http';

interface ChatMessage {
  text: string;
  isUser: boolean;
  time: Date;
}

@Component({
  selector: 'app-patient-reports-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './patient-reports-dialog.component.html'
})
export class PatientReportsDialogComponent implements AfterViewChecked {
  @ViewChild('scrollMe') private myScrollContainer!: ElementRef;

  currentInput = '';
  isLoading = false;
  messages: ChatMessage[] = []; 

  constructor(
    public dialogRef: MatDialogRef<PatientReportsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { patientId: number, patientName: string },
    private http: HttpClient
  ) {
    this.messages = [
      {
        text: `Salutari! Daca doresti pot analiza istoricul si rapoartele pacientului ${this.data.patientName}. Ce doresti sa afli?`,
        isUser: false,
        time: new Date()
      }
    ];
  }

  sendMessage() {
    if (!this.currentInput.trim() || this.isLoading) return;

    // Adaugam context pentru ca AI sa foloseasca datele pacientului specific
    const userText = this.currentInput;
    const contextualizedPrompt = `(Pentru pacientul ${this.data.patientName}): ${userText}`;

    this.messages.push({ text: userText, isUser: true, time: new Date() });
    this.currentInput = '';
    this.isLoading = true;
    this.scrollToBottom();

    this.http.post<{ response: string }>('https://localhost:7145/api/Mael/ask', { message: contextualizedPrompt })
      .subscribe({
        next: (res) => {
          this.messages.push({ text: res.response, isUser: false, time: new Date() });
          this.isLoading = false;
          this.scrollToBottom();
        },
        error: (err) => {
          this.messages.push({ text: "Eroare la conectarea cu serverul AI.", isUser: false, time: new Date() });
          this.isLoading = false;
          this.scrollToBottom();
        }
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
