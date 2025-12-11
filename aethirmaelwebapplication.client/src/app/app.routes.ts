import { Routes } from '@angular/router';
import { DoctorListComponent } from './pages/doctors-list/doctor-list.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { MyAppointmentsComponent } from './pages/my-appointments/my-appointments.component';
import { DoctorDashboardComponent } from './pages/doctor-dashboard/doctor-dashboard.component';
import { AddDoctorComponent } from './pages/add-doctor/add-doctor.component';


export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent, title: 'AethirMael - Acasă' },
  { path: 'medici', component: DoctorListComponent, title: 'Lista Medici' },
  { path: 'login', component: LoginComponent, title: 'Autentificare' },
  { path: 'register', component: RegisterComponent, title: 'Creaza-ti cont' },
  { path: 'programarile-mele', component: MyAppointmentsComponent, title: 'Istoric Programări' },
  { path: 'doctor-dashboard', component: DoctorDashboardComponent, title: 'Dashboard Medic' },
  { path: 'add-doctor', component: AddDoctorComponent, title: 'Adaugare Doctor' },
  
  { path: '**', redirectTo: 'home' }
];
