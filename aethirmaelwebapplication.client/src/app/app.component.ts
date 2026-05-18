import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from './services/auth.service';
import { SignalRService } from './services/signalr.service';
import { ChatbotComponent } from "./components/chatbot/chatbot.component"; 

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, ChatbotComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {

  constructor(
    public authService: AuthService,
    private signalRService: SignalRService 
  ) { }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.signalRService.startConnection();
    }

    this.authService.currentUser$.subscribe(user => {
      if (user) {
        this.signalRService.startConnection();
      } else {
        this.signalRService.stopConnection();
      }
    });
  }

  ngOnDestroy() {
    this.signalRService.stopConnection();
  }

  logout() {
    this.authService.logout();
  }
}
