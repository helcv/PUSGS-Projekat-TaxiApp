import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { User } from 'src/app/_models/user';
import { ActivatedRoute, Router } from '@angular/router';
import { Ride } from 'src/app/_models/ride';
import { RideService } from 'src/app/_services/ride.service';

@Component({
  selector: 'app-active-ride',
  templateUrl: './active-ride.component.html',
  styleUrls: ['./active-ride.component.css']
})
export class ActiveRideComponent implements OnInit{
  user: User | null = null;
  rideId : any;
  pageReloaded: boolean = false;
  ride: Ride | null = null;
  
  constructor(private accountService: AccountService, 
    private router: Router, 
    private route: ActivatedRoute,
    private rideService: RideService) { 

    this.accountService.currentUser$.subscribe({
      next: user => {
        this.user = user;
      }
    });
  }

  ngOnInit(): void {
    this.loadUserProfile();
    this.getRideDetails();
  }

  loadUserProfile(): void {
    if(this.user)
    this.accountService.getUserProfileById(this.user?.id).subscribe({
      next: (user: User | null) => {
        this.user = user;
        localStorage.setItem('user', JSON.stringify(user));
        
      },
      error: (error) => {
        console.error('Error fetching user profile:', error);
      }
    });
  }

  getRideDetails(): void {
    this.rideService.getCreatedRide().subscribe({
      next: (ride) => {
        this.ride = ride;
      },
      error: (err) => {
        console.error('Failed to load ride details', err);
        this.router.navigateByUrl('/countdown')
      }
    });
  }
}
