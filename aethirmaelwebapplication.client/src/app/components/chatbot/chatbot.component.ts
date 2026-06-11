import { Component, ElementRef, ViewChild, AfterViewChecked, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatService } from '../../services/chat.service';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

interface ChatMessage {
  text: string;
  isUser: boolean;
  timestamp: Date;
}

@Component({
  selector: 'app-chatbot',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatButtonModule, MatIconModule, MatCardModule, MatInputModule, MatProgressSpinnerModule
  ],
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.css']
})
export class ChatbotComponent implements OnInit, AfterViewChecked {
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;

  isOpen = false;
  isLoading = false;
  userMessage = '';

  private readonly initialMessage: ChatMessage = {
    text: "Salut! Sunt eu Maël, asistentul tău virtual. Cu ce te pot ajuta astăzi?",
    isUser: false,
    timestamp: new Date()
  };

  messages: ChatMessage[] = [{ ...this.initialMessage }];

  constructor(
    public chatService: ChatService,
    private http: HttpClient,
    private authService: AuthService
  ) { }

  ngOnInit() {
    this.chatService.isOpen$.subscribe(state => {
      this.isOpen = state;
    });

    this.authService.currentUser$.subscribe(user => {
      // Curatam mesajele vechi daca utilizatorul s-a delogat
      this.messages = [{ ...this.initialMessage, timestamp: new Date() }];

      // Inchidem fereastra
      if (!user) {
        this.chatService.close();
      }
    });
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  toggleChat() {
    this.chatService.toggle();
  }

  sendMessage() {
    if (!this.userMessage.trim()) return;

    const question = this.userMessage;

    this.messages.push({
      text: question,
      isUser: true,
      timestamp: new Date()
    });

    this.userMessage = '';
    this.isLoading = true;
    this.scrollToBottom();

    // Apelam endpoint-ul de Backend
    this.http.post<{ response: string }>('https://localhost:7145/api/Mael/ask', { message: question })
      .subscribe({
        next: (res) => {
          this.isLoading = false;
          this.messages.push({
            text: res.response, 
            isUser: false,
            timestamp: new Date()
          });
          this.scrollToBottom();
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Eroare AI:', err);

          let errorMessage = "Oups! S-a produs o eroare la conectare.";

          // Tratam eroarea 401 (Neautorizat) specific pentru a îndruma pacientul
          if (err.status === 401) {
            errorMessage = "Trebuie să te loghezi pentru a discuta cu mine. Am nevoie de acces la dosarul tău medical pentru a-ți oferi sfaturi personalizate!";
          }

          this.messages.push({
            text: errorMessage,
            isUser: false,
            timestamp: new Date()
          });
          this.scrollToBottom();
        }
      });
  }

  private scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }
}
