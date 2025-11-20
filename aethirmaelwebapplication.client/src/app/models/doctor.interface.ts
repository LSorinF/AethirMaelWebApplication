export interface Specialization {
  specializationId: number;
  name: string;
}

export interface Doctor {
  doctorId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  specializationId: number;
  specialization?: Specialization;
}
