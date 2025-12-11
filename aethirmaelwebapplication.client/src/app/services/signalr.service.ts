import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection | undefined;
  // Asigură-te că acest port (7145) este cel din profilul "https" din launchSettings.json al serverului C#
  private hubUrl = 'https://localhost:7145/notificationHub';

  constructor(private snackBar: MatSnackBar) { }

  public startConnection = () => {
    // Verificăm dacă există deja o conexiune activă pentru a evita erorile de pornire multiplă
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      console.log('SignalR este deja conectat.');
      return;
    }

    const token = localStorage.getItem('token');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        // Trimitem token-ul JWT pentru autentificare
        accessTokenFactory: () => token || '',
        // Dacă ai probleme cu certificatul self-signed în dezvoltare (uneori necesar, deși riscant)
        // transport: signalR.HttpTransportType.WebSockets, 
        // skipNegotiation: true
      })
      .withAutomaticReconnect()
      // ACTIVĂM LOGGING-UL DETALIAT PENTRU DEBUG
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Conexiune SignalR inițiată cu succes (ID: ' + this.hubConnection?.connectionId + ')'))
      .catch(err => {
        console.error('Eroare CRITICĂ la conectarea SignalR: ', err);
        // Putem afișa un mesaj discret utilizatorului că notificările nu merg
        // this.snackBar.open('Sistemul de notificări indisponibil momentan.', 'X', { duration: 5000 });
      });

    this.addNotificationListener();
  }

  private addNotificationListener = () => {
    // Ascultăm evenimentul "ReceiveNotification" definit în Backend
    this.hubConnection?.on('ReceiveNotification', (message) => {
      console.log('Mesaj SignalR primit:', message);

      // Afișăm mesajul folosind SnackBar-ul Material
      this.snackBar.open(message, 'Închide', {
        duration: 8000, // Durata mai lungă pentru a fi observat
        verticalPosition: 'top',
        horizontalPosition: 'right',
        panelClass: ['bg-mystic-accent', 'text-white']
      });
    });
  }

  public stopConnection = () => {
    if (this.hubConnection) {
      this.hubConnection.stop().then(() => console.log('Conexiune SignalR oprită.'));
    }
  }
}
