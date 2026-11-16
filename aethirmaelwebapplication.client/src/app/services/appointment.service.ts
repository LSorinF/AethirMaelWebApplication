import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private apiUrl = `${environment.apiUrl}/api/appointments`;

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
