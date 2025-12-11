import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DoctorService } from '../../services/doctor.service';

// Material
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-add-doctor',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatInputModule, MatButtonModule, MatSelectModule, MatIconModule
  ],
  templateUrl: './add-doctor.component.html'
})
export class AddDoctorComponent implements OnInit {
  doctorData = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    phone: '',
    specializationId: null
  };

  specializations: any[] = [];
  isLoading = false;

  constructor(
    private doctorService: DoctorService,
    private snackBar: MatSnackBar,
    private router: Router
  ) { }

  ngOnInit() {
    // Incarcam specializarile pentru dropdown
    this.doctorService.getSpecializations().subscribe(data => {
      this.specializations = data;
    });
  }

  onSubmit() {
    this.isLoading = true;

    this.doctorService.addDoctor(this.doctorData).subscribe({
      next: () => {
        this.snackBar.open('Medic adăugat cu succes!', 'OK', { duration: 3000 });
        this.router.navigate(['/medici']); // Ne intoarcem la lista
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
        this.snackBar.open('Eroare la adăugare.', 'X', { panelClass: ['bg-red-500', 'text-white'] });
      }
    });
  }
}
