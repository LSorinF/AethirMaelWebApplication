import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection | undefined;
  private hubUrl = 'https://localhost:7145/notificationHub';

  constructor(private snackBar: MatSnackBar) { }

  public startConnection = () => {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      console.log('SignalR este deja conectat.');
      return;
    }

    const token = localStorage.getItem('token');

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => token || '',
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Conexiune SignalR inițiată cu succes (ID: ' + this.hubConnection?.connectionId + ')'))
      .catch(err => {
        console.error('Eroare CRITICĂ la conectarea SignalR: ', err);

      });

    this.addNotificationListener();
  }

  private addNotificationListener = () => {
    this.hubConnection?.on('ReceiveNotification', (message) => {
      console.log('Mesaj SignalR primit:', message);

      this.snackBar.open(message, 'Închide', {
        duration: 8000, 
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
