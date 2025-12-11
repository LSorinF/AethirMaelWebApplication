import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

// Definim interfata
export interface AuthResponse {
  token: string;
  role: string;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7145/api/Auth'; 

  //Retine user curent
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // La pornirea aplicatiei, verificam daca avem un user salvat in browser
    this.loadUserFromStorage();
  }

  private loadUserFromStorage() {
    const savedUser = localStorage.getItem('user');
    if (savedUser) {
      this.currentUserSubject.next(JSON.parse(savedUser));
    }
  }

  register(userData: any): Observable<any> {
  return this.http.post(`${this.apiUrl}/register`, userData);
}

  login(loginData: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, loginData).pipe(
      tap(response => {
        localStorage.setItem('user', JSON.stringify(response));
        localStorage.setItem('token', response.token);
        
        //Anuntam toata aplicatia ca avem un user nou
        this.currentUserSubject.next(response);
      })
    );
  }

  logout() {
    // Stergem tot si anuntam ca nu mai avem user
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    return !!this.currentUserSubject.value;
  }
}
