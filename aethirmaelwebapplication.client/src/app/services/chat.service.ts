import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  // Folosim BehaviorSubject pentru a retine starea curenta 
  private isOpenSubject = new BehaviorSubject<boolean>(false);

  public isOpen$ = this.isOpenSubject.asObservable();

  constructor() { }

  toggle() {
    this.isOpenSubject.next(!this.isOpenSubject.value);
  }

  open() {
    this.isOpenSubject.next(true);
  }

  close() {
    this.isOpenSubject.next(false);
  }
}
