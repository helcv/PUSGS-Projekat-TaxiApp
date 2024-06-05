import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Driver } from 'src/app/_models/driver';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {
  drivers: Driver[] = [];
  activeDriverId: number | null = null;
  showRatingsId: number | null = null;
  
  constructor(private adminService: AdminService,  private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.loadDrivers()
  }

  loadDrivers(): void{
    this.adminService.getDrivers().subscribe({
      next: (drivers: Driver[]) => {
        this.drivers = drivers;
      },
      error: (err) => {
        console.error('Error fetching drivers', err);
      }
    })
  }

  toggleDriverDetails(driverId: number): void {
    this.activeDriverId = this.activeDriverId === driverId ? null : driverId;
  }
  
  toggleRatings(driverId: number): void {
    this.showRatingsId = this.showRatingsId === driverId ? null : driverId;
  }

  acceptVerification(driverId: number): void {
    this.adminService.acceptVerification(driverId).subscribe(
      () => {
        this.toastr.success('Verification successfully accepted', 'Success');
        this.loadDrivers();
      },
      (error) => {
        console.error('Verification error:', error);
        this.toastr.error('Verification error', 'Error');
      }
    );
  }

  denyVerification(driverId: number): void {
    this.adminService.denyVerification(driverId).subscribe(
      () => {
        this.toastr.success('Verification successfully denied', 'Success');
        this.loadDrivers();
      },
      (error) => {
        console.error('Verification error:', error);
        this.toastr.error('Verification error', 'Error');
      }
    );
  }

  blockDriver(driverId: number): void {
    this.adminService.blockDriver(driverId).subscribe(
      () => {
        this.toastr.success('Driver successfully blocked', 'Success');
        this.loadDrivers();
      },
      (error) => {
        console.error('Error blocking driver:', error);
        this.toastr.error('Failed to block driver', 'Error');
      }
    );
  }

  unBlockDriver(driverId: number): void {
    this.adminService.unBlockDriver(driverId).subscribe(
      () => {
        this.toastr.success('Driver successfully unblocked', 'Success');
        this.loadDrivers();
      },
      (error) => {
        console.error('Error unblocking driver:', error);
        this.toastr.error('Failed to unblock driver', 'Error');
      }
    );
  }
}
