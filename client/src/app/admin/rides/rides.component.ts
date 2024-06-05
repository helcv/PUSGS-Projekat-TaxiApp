import { Component, OnInit } from '@angular/core';
import { DetailedRide } from 'src/app/_models/detailedRide';
import { Ride } from 'src/app/_models/ride';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-rides',
  templateUrl: './rides.component.html',
  styleUrls: ['./rides.component.css']
})
export class RidesComponent implements OnInit {
  isCollapsed: { [key: number]: boolean } = {};
  activeRide: number | null = null;
  rides: DetailedRide[] = [];
   
  constructor(private adminService: AdminService) {
    
  }

  ngOnInit(): void {
    this.loadRides()
  }

  loadRides(): void {
    this.adminService.getRides().subscribe({
      next: (rides: DetailedRide[]) => {
        this.rides = rides;
      },
      error: (err) => {
        console.error('Error fetching rides', err);
      }
    });
  }

  toggleCollapse(rideId: number): void {
    if (this.activeRide === rideId) {
      this.activeRide = null;
    } else {
      this.activeRide = rideId; 
    }
    this.isCollapsed[rideId] = !this.isCollapsed[rideId];
  }
}
