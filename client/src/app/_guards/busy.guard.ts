import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BusyGuard {
  constructor(private accountService: AccountService, private toastr: ToastrService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user && user.busy === true) {
          return true; 
        } else {
          this.toastr.error('Not allowed.');
          this.router.navigateByUrl('/profile')
          return false; 
        }
      })
    );
  }
}
