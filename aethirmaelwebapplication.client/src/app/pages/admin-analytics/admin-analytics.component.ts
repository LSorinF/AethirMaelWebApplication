import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../services/admin.service';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-admin-analytics',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './admin-analytics.component.html'
})
export class AdminAnalyticsComponent implements OnInit {
  stats: any = null;
  isLoading = true;

  constructor(private adminService: AdminService) { }

  ngOnInit() {
    this.adminService.getStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  // Helper pentru a calcula inaltimea barei in grafic 
  getBarHeight(count: number, list: any[]): string {
    if (!list || list.length === 0) return '0%';
    const max = Math.max(...list.map(i => i.count));
    if (max === 0) return '0%';
    // Marime bara
    const percentage = (count / max) * 100;
    return `${Math.max(percentage, 5)}%`;
  }
}
