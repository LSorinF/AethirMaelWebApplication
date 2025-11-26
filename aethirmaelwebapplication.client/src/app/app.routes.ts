import { Routes } from '@angular/router';
import { DoctorListComponent } from './pages/doctors-list/doctor-list.component';
import { HomeComponent } from './pages/home/home.component';
// Importam Login
import { LoginComponent } from './pages/login/login.component';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent, title: 'AethirMael - Acasă' },
  { path: 'medici', component: DoctorListComponent, title: 'Lista Medici' },
  // Ruta noua
  { path: 'login', component: LoginComponent, title: 'Autentificare' },
  
  { path: '**', redirectTo: 'home' }
];
