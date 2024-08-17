import { Component, OnInit } from '@angular/core';
import { Ride } from 'src/app/_models/ride';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { RatingService } from 'src/app/_services/rating.service';
import { RideService } from 'src/app/_services/ride.service';

@Component({
  selector: 'app-ride-history',
  templateUrl: './ride-history.component.html',
  styleUrls: ['./ride-history.component.css']
})
export class RideHistoryComponent implements OnInit {
  rides: Ride[] = [];
  isCollapsed: { [key: number]: boolean } = {};
  activeRide: number | null = null;
  user: User | null = null;

  constructor(private rideService: RideService, 
    private accountService: AccountService) {}

  ngOnInit(): void {
    this.accountService.currentUser$.subscribe(user => {
      this.user = user;
    });
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
    if (this.activeRide === rideId) {
      this.activeRide = null;
    } else {
      this.activeRide = rideId;
    }
  }

}
