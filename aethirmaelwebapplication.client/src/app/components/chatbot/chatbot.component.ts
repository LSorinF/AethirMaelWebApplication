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

  messages: ChatMessage[] = [
    {
      text: "Salut! Sunt eu Maël, asistentul tău virtual. Cu ce te pot ajuta astăzi?",
      isUser: false,
      timestamp: new Date()
    }
  ];

  //Injectam serviciul
  constructor(public chatService: ChatService) { }

  ngOnInit() {
    // subscribe la starea chat-ului
    this.chatService.isOpen$.subscribe(state => {
      this.isOpen = state;
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

    this.messages.push({
      text: this.userMessage,
      isUser: true,
      timestamp: new Date()
    });

    const question = this.userMessage;
    this.userMessage = '';
    this.isLoading = true;

    setTimeout(() => {
      this.isLoading = false;
      this.messages.push({
        text: "Am înțeles întrebarea ta: '" + question + "'. Momentan sunt în modul demo.",
        isUser: false,
        timestamp: new Date()
      });
    }, 1500);
  }

  private scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }
}
