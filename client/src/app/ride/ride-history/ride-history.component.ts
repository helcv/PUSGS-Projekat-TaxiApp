import { Component, OnInit } from '@angular/core';
import { Ride } from 'src/app/_models/ride';
import { RideService } from 'src/app/_services/ride.service';

@Component({
  selector: 'app-ride-history',
  templateUrl: './ride-history.component.html',
  styleUrls: ['./ride-history.component.css']
})
export class RideHistoryComponent implements OnInit {
  rides: Ride[] = [];
  isCollapsed: { [key: number]: boolean } = {};

  constructor(private rideService: RideService) {}

  ngOnInit(): void {
    this.loadRides();
  }

  loadRides(): void {
    this.rideService.getCompletedRides().subscribe({
      next: (rides: Ride[]) => {
        this.rides = rides;
      },
      error: (err) => {
        console.error('Error fetching rides', err);
      }
    });
  }

  toggleCollapse(rideId: number): void {
    this.isCollapsed[rideId] = !this.isCollapsed[rideId];
  }
}
