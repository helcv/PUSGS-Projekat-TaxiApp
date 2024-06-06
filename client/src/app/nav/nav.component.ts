import { Component, Injector, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of, take } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  isCollapsed = false;
  user: User | null = null;
  model: any = {}
  
  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) {
    this.accountService.currentUser$.subscribe({
      next: user => {
        this.user = user;
      }
    });
  }

  ngOnInit(): void {
    
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: (response: any) => {
        console.log(this.user?.busy);
        if (this.user?.busy)
          {
            this.router.navigateByUrl('/active');
          }
        else{
          this.router.navigateByUrl('/profile');
        }
      },
      error: error => {
        this.toastr.error(error.error);
        console.error(error.error);
      }
    });
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/')
  }

  toggleNavbar(){
    this.isCollapsed = !this.isCollapsed;
  }
}
