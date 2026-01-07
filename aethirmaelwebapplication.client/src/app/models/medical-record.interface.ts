export interface MedicalRecord {
  medicalRecordId: number;
  dateCreated: Date;
  doctorName: string;
  specialization: string;
  symptoms: string;
  diagnosis: string;
  treatment: string;
  investigationResults: string;
}

export interface CreateMedicalRecordDto {
  patientId: number;
  symptoms: string;
  diagnosis: string;
  treatment: string;
  investigationResults?: string;
  appointmentId?: number;
}

export interface UpdateMedicalRecordDto {
  medicalRecordId: number;
  symptoms: string;
  diagnosis: string;
  treatment: string;
  investigationResults?: string;
}
