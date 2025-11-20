import { Routes } from '@angular/router';

export const routes: Routes = [
  // Ruta default: cand intri pe site, te duce la /home
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent),
    data: { title: 'AethirMael - home' }
  },
  {
    path: 'medici',
    loadComponent: () => import('./pages/doctors-list/doctor-list.component').then(m => m.DoctorListComponent),
    data: { title: 'Lista Medici' }
  },
  // Orice alta ruta gresita te duce inapoi la home
  {
    path: '**',
    redirectTo: 'home'
  }
];
