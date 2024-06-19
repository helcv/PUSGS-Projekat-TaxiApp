import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Ride } from 'src/app/_models/ride';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { RideService } from 'src/app/_services/ride.service';

@Component({
  selector: 'app-new-rides',
  templateUrl: './new-rides.component.html',
  styleUrls: ['./new-rides.component.css']
})
export class NewRidesComponent implements OnInit {
  rides: Ride[] = [];
  isCollapsed: { [key: number]: boolean } = {};
  activeRide: number | null = null;
  user: User | null = null;

  
  constructor(private rideService: RideService, private toastr: ToastrService, private router: Router, private accountService: AccountService) {
    this.accountService.currentUser$.subscribe({
      next: user => {
        this.user = user;
      }
    });
  }

  ngOnInit(): void {
    if (!this.user?.busy)
      this.loadRides();
  }

  loadRides(): void {
    this.rideService.getCreatedRides().subscribe({
      next: (rides: Ride[]) => {
        this.rides = rides;
      },
      error: (err) => {
        console.error('Error fetching rides', err);
      }
    });
  }

  acceptRide(rideId: number) {
    this.rideService.acceptRide(rideId).subscribe({
      next: () => {
        if (this.user) {
          this.user.busy = true;
          this.accountService.setCurrentUser(this.user);
        }
        this.toastr.success('Ride accepted successfully!');
        this.router.navigateByUrl('/countdown');
      }, 
      error: (err) => {
        this.toastr.error('Failed to accept ride.');
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
