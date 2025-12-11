import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Doctor } from '../models/doctor.interface';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  // MODIFICARE: URL-ul de bază se oprește la /api (fără /doctors)
  private baseUrl = 'https://localhost:7145/api';

  constructor(private http: HttpClient) { }

  // 1. Obtine lista de specializari (pentru dropdown)
  getSpecializations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/doctors/specializations`);
  }

  // 2. Adauga medic nou
  addDoctor(doctorData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/doctors`, doctorData);
  }

  getDoctors(): Observable<Doctor[]> {
    // Aici adăugăm /doctors
    return this.http.get<Doctor[]>(`${this.baseUrl}/doctors`);
  }

  createAppointment(appointmentData: any): Observable<any> {
    // Aici adăugăm /appointments (NU /doctors/appointments)
    return this.http.post(`${this.baseUrl}/appointments`, appointmentData);
  }

  deleteDoctor(doctorId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/doctors/${doctorId}`);
  }
}
