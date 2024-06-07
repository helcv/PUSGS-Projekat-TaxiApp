import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Subscription, catchError, interval } from 'rxjs';
import { RideService } from 'src/app/_services/ride.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Time } from 'src/app/_models/time';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-countdown',
  templateUrl: './countdown.component.html',
  styleUrls: ['./countdown.component.css']
})
export class CountdownComponent implements OnInit{
  @ViewChild('rideModal') rideModal!: TemplateRef<any>; 
  modalRef: BsModalRef | undefined;
  time: Time | null = null;
  intervalSubscription: Subscription | null = null;
  user: User | null = null;
  
  constructor(private rideService: RideService,
    private router: Router,
    private toastr: ToastrService,
    private modalService: BsModalService,
    private accountService: AccountService) {

      this.accountService.currentUser$.subscribe({
        next: user => {
          this.user = user;
        }
      });
    
  }

  ngOnInit(): void {
    this.loadUserProfile();
    if(this.user?.busy)
      this.startUpdatingTime()
    
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template, {
      backdrop: 'static',
      keyboard: false
    });
  }

  startUpdatingTime() {
    interval(1000).subscribe(() => {
      this.rideService.getTime().pipe(
        catchError(error => {
          this.toastr.error('Failed to get time. Redirecting to profile.');
          throw error;
        })
      ).subscribe({
        next: (time: Time) => {
          this.time = time;
          if (!this.modalRef) {
            this.openModal(this.rideModal);
          }
        },
        error: (err) => {
          console.error('Error fetching time:', err);
          this.router.navigateByUrl('/profile')
        }
      });
    });
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

  ngOnDestroy() {
    if (this.intervalSubscription) {
      this.intervalSubscription.unsubscribe();
    }
  }
}
