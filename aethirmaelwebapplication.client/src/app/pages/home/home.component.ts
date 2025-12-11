import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
// 1. Importam serviciul
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatButtonModule, MatIconModule, MatCardModule
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'] // (daca ai fisier css gol, e ok)
})
export class HomeComponent {

  // 2. Injectam serviciul
  constructor(private chatService: ChatService) { }

  // 3. Metoda care va fi apelata de buton
  openChatBot() {
    this.chatService.open();
  }
}
