import { Component, ElementRef, ViewChild, AfterViewChecked, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
// 1. Importam serviciul
import { ChatService } from '../../services/chat.service';
// 2. Importam HttpClient pentru apelul catre serverul AI
import { HttpClient } from '@angular/common/http';
// 3. ADAUGAT: Importam AuthService pentru a detecta schimbarile de cont
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

  // MODIFICARE: Punem mesajul de salut separat ca sa-l putem refolosi la resetare
  private readonly initialMessage: ChatMessage = {
    text: "Salut! Sunt eu Maël, asistentul tău virtual. Cu ce te pot ajuta astăzi?",
    isUser: false,
    timestamp: new Date()
  };

  messages: ChatMessage[] = [{ ...this.initialMessage }];

  // Injectam AuthService alaturi de celelalte servicii
  constructor(
    public chatService: ChatService,
    private http: HttpClient,
    private authService: AuthService
  ) { }

  ngOnInit() {
    // subscribe la starea chat-ului
    this.chatService.isOpen$.subscribe(state => {
      this.isOpen = state;
    });

    // ADAUGAT: Ascultăm schimbările de cont (Login / Logout / Schimbare User)
    this.authService.currentUser$.subscribe(user => {
      // 1. Curățăm mesajele vechi și îl punem doar pe cel de salut, cu ora actualizată
      this.messages = [{ ...this.initialMessage, timestamp: new Date() }];

      // 2. Dacă utilizatorul s-a delogat complet, închidem și fereastra
      if (!user) {
        this.chatService.close();
      }
    });
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  toggleChat() {
    // schimbam starea chat-ului folosind serviciul
    this.chatService.toggle();
  }

  sendMessage() {
    if (!this.userMessage.trim()) return;

    const question = this.userMessage;

    // Afișăm mesajul utilizatorului
    this.messages.push({
      text: question,
      isUser: true,
      timestamp: new Date()
    });

    this.userMessage = '';
    this.isLoading = true;
    this.scrollToBottom();

    // Apelăm endpoint-ul de Backend
    this.http.post<{ response: string }>('https://localhost:7145/api/Mael/ask', { message: question })
      .subscribe({
        next: (res) => {
          this.isLoading = false;
          this.messages.push({
            text: res.response, // Răspunsul primit de la OpenAI prin intermediul C#
            isUser: false,
            timestamp: new Date()
          });
          this.scrollToBottom();
        },
        error: (err) => {
          this.isLoading = false;
          console.error('Eroare AI:', err);

          let errorMessage = "Oups! S-a produs o eroare la conectare.";

          // Tratăm eroarea 401 (Neautorizat) specific pentru a îndruma pacientul
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
