import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AccountService } from './account.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { take, Observable, tap, switchMap } from 'rxjs';
import { Ride } from '../_models/ride';
import { Time } from '../_models/time';

@Injectable({
  providedIn: 'root'
})
export class RideService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private accountService: AccountService) {
    
   }

   getTime(): Observable<Time>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Time>(this.baseUrl + 'ride/remaining-time', {headers});
   }

   getCreatedRide(): Observable<Ride>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Ride>(this.baseUrl + 'ride/created', {headers});
   }

  //User
   createRide(model: any): Observable<Ride>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.post<Ride>(this.baseUrl + 'ride', model, {headers})
   }

   //User
   requestRide(rideId: number): Observable<any> {
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}ride/${rideId}/request-ride`, {}, {headers}).pipe(
      switchMap(() => {
        return this.accountService.getUserProfile();
      })
    );
  }

  //User
  declineRide(rideId: number): Observable<any> {
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}ride/${rideId}/deny-ride`, {}, {headers});
  }

  //Driver
  acceptRide(rideId: number): Observable<Ride>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<Ride>(`${this.baseUrl}ride/${rideId}/accept-ride`, {}, { headers });
   }

   getCompletedRides(): Observable<Ride[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Ride[]>(this.baseUrl + 'ride/completed-rides', {headers})
   }

   getCreatedRides(): Observable<Ride[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Ride[]>(this.baseUrl + 'ride/created-rides', {headers})
   }
}
