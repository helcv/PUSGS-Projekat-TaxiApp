import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AccountService } from './account.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { take, Observable, tap, switchMap } from 'rxjs';
import { Ride } from '../_models/ride';
import { Time } from '../_models/time';
import { DetailedRide } from '../_models/detailedRide';

@Injectable({
  providedIn: 'root'
})
export class RideService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private accountService: AccountService) {
    
   }

   getTime(): Observable<Time>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Time>(this.baseUrl + 'rides/remaining-time', {headers});
   }

   getCreatedRide(): Observable<Ride>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Ride>(this.baseUrl + 'rides/created-ride', {headers});
   }

  //User
   createRide(model: any): Observable<Ride>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.post<Ride>(this.baseUrl + 'rides', model, {headers})
   }

   //User
   requestRide(rideId: number): Observable<any> {
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}rides/${rideId}/request`, {}, {headers}).pipe(
      switchMap(() => {
        return this.accountService.getUserProfile();
      })
    );
  }

  //User
  declineRide(rideId: number): Observable<any> {
    const headers = this.accountService.getAuthHeaders();
    return this.http.delete<any>(`${this.baseUrl}rides/${rideId}/decline`, {headers});
  }

  //Driver
  acceptRide(rideId: number): Observable<Ride>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<Ride>(`${this.baseUrl}rides/${rideId}/accept`, {}, { headers });
   }

   getCompletedRides(): Observable<Ride[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Ride[]>(this.baseUrl + 'rides/completed', {headers})
   }

   getCreatedRides(): Observable<Ride[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Ride[]>(this.baseUrl + 'rides/created', {headers})
   }

   getRideInProgress(): Observable<DetailedRide>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<DetailedRide>(this.baseUrl + 'rides/in-progress', {headers})
   }
}
