import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { User } from 'src/app/_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-active-ride',
  templateUrl: './active-ride.component.html',
  styleUrls: ['./active-ride.component.css']
})
export class ActiveRideComponent implements OnInit{
  user: User | null = null;
  ride : any;
  pageReloaded: boolean = false;
  
  constructor(private accountService: AccountService, private router: Router) {    
    this.accountService.currentUser$.subscribe({
      next: user => {
        this.user = user;
      }
    });
  }

  ngOnInit(): void {
    this.loadUserProfile();
    if (this.user)
      {
        this.accountService.setCurrentUser(this.user)
      }
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
}
