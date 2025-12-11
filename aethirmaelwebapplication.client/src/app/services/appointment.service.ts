import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private apiUrl = 'https://localhost:7145/api/appointments';

  constructor(private http: HttpClient) { }

  // Pacient
  getMyAppointments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my-appointments`);
  }

  // Doctor: Obtine lista cererilor
  getDoctorAppointments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/doctor-appointments`);
  }

  // Doctor: Actualizeaza statusul
  updateStatus(id: number, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/status`, { status });
  }
}
