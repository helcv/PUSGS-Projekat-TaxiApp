import { Component, Injector, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
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
  model: any = {}
  
  constructor(public accountService: AccountService, private router: Router, private toastr: ToastrService) {
    
  }

  ngOnInit(): void {
    
  }

  login(){
    console.log('login triggered');
    
    this.accountService.login(this.model).subscribe({
      next: _ => this.router.navigateByUrl('/profile'),
      error: error => {
        this.toastr.error(error.error)
        console.log(error);
        
      }
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/')
  }

  toggleNavbar(){
    this.isCollapsed = !this.isCollapsed;
  }
}
