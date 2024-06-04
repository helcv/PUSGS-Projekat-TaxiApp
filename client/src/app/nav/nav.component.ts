import { Component, Injector, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  isCollapsed = false;
  model: any = {}
  
  constructor(public accountService: AccountService, private injector: Injector) {
    
  }

  ngOnInit(): void {
    
  }

  login(){
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
      },
      error: error => console.log(error)
    })
  }

  logout(){
    this.accountService.logout();
  }

  toggleNavbar(){
    this.isCollapsed = !this.isCollapsed;
  }
}
