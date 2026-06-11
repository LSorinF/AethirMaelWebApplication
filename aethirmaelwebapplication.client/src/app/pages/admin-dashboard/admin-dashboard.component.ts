import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../services/admin.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RouterModule } from '@angular/router'; 

import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatCardModule } from '@angular/material/card';
import { FormsModule } from "@angular/forms";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule, 
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatCardModule,
    FormsModule,
    MatInputModule,
    MatFormFieldModule
  ],
  templateUrl: './admin-dashboard.component.html'
})
export class AdminDashboardComponent implements OnInit {
  users: any[] = [];
  filteredUsers: any[] = []; 
  searchQuery: string = '';
  displayedColumns: string[] = ['id', 'name', 'email', 'role', 'actions'];

  constructor(
    private adminService: AdminService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.adminService.getAllUsers().subscribe(data => {
      this.users = data;
      this.applyFilter(); 
    });
  }

  applyFilter() {
    const q = this.searchQuery.toLowerCase();
    this.filteredUsers = this.users.filter(u =>
      u.fullName?.toLowerCase().includes(q) ||
      u.email?.toLowerCase().includes(q) ||
      u.role?.toLowerCase().includes(q)
    );
  }

  deleteUser(user: any) {
    if (user.role === 'Admin') {
      this.snackBar.open('Nu poți șterge un admin!', 'X');
      return;
    }

    if (!confirm(`ATENȚIE! Ești pe cale să ștergi utilizatorul ${user.fullName}.\n\nSe vor șterge PERMANENT:\n- Contul de login\n- Toate programările\n- Toate fișele medicale (dacă e pacient)\n\nContinui?`)) {
      return;
    }

    this.adminService.deleteUser(user.userId).subscribe({
      next: () => {
        this.snackBar.open('Utilizator șters cu succes.', 'OK', { duration: 3000 });
        this.loadUsers();
      },
      error: (err) => {
        console.error(err);
        this.snackBar.open('Eroare la ștergere.', 'X', { panelClass: ['bg-red-500', 'text-white'] });
      }
    });
  }
}
