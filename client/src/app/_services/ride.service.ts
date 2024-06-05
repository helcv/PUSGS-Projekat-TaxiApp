import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { AccountService } from './account.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { take, Observable } from 'rxjs';
import { Ride } from '../_models/ride';
import { Token } from '@angular/compiler';

@Injectable({
  providedIn: 'root'
})
export class RideService {
  baseUrl = environment.apiUrl;
  user: User | null = null;

  constructor(private http: HttpClient, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user){
          this.user = user;
        }
      }
    })
   }

   getCompletedRides(): Observable<Ride[]>{
    const headers = this.getAuthHeaders();
    console.log(headers);
    
    return this.http.get<Ride[]>(this.baseUrl + 'ride/completed-rides', {headers})
   }

   private getAuthToken(): string | null {
    const tokenString = localStorage.getItem('token');
    if (tokenString) {
      const tokenObject = JSON.parse(tokenString);
      return tokenObject.token;
    }
    return null;
  }

   private getAuthHeaders(): HttpHeaders {
    const token = this.getAuthToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }
}
