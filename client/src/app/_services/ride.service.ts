import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { AccountService } from './account.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { take, Observable, tap, switchMap } from 'rxjs';
import { Ride } from '../_models/ride';
import { Token } from '@angular/compiler';

@Injectable({
  providedIn: 'root'
})
export class RideService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private accountService: AccountService) {
    
   }

   createRide(model: any): Observable<Ride>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.post<Ride>(this.baseUrl + 'ride', model, {headers})
   }

   requestRide(rideId: number): Observable<any> {
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}ride/${rideId}/request-ride`, {}, {headers}).pipe(
      switchMap(() => {
        return this.accountService.getUserProfile();
      })
    );
  }

  declineRide(rideId: number): Observable<any> {
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}ride/${rideId}/deny-ride`, {}, {headers});
  }

   getCompletedRides(): Observable<Ride[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Ride[]>(this.baseUrl + 'ride/completed-rides', {headers})
   }
}
