import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicalRecord, CreateMedicalRecordDto, UpdateMedicalRecordDto } from '../models/medical-record.interface';

@Injectable({
  providedIn: 'root'
})
export class MedicalRecordService {
  private apiUrl = 'https://localhost:7145/api/MedicalRecords';

  constructor(private http: HttpClient) { }

  // Doctor: Adauga o fisa noua
  createRecord(data: CreateMedicalRecordDto): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  updateRecord(data: UpdateMedicalRecordDto): Observable<any> {
    return this.http.put(this.apiUrl, data);
  }
  // Doctor: Sterge o fisa medicala
  deleteRecord(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  // Pacient: Isi vede propriul istoric
  getMyHistory(): Observable<MedicalRecord[]> {
    return this.http.get<MedicalRecord[]>(`${this.apiUrl}/my-history`);
  }

  // Doctor: Vede istoricul unui pacient specific (Optional, pentru viitor)
  getPatientHistory(patientId: number): Observable<MedicalRecord[]> {
    return this.http.get<MedicalRecord[]>(`${this.apiUrl}/patient/${patientId}`);
  }
}
