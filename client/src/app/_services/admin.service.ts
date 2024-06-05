import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Driver } from '../_models/driver';
import { AccountService } from './account.service';
import { Ride } from '../_models/ride';
import { DetailedRide } from '../_models/detailedRide';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  user: User | null = null;

  constructor(private http: HttpClient, private accountService: AccountService) { }

  getDrivers(): Observable<Driver[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Driver[]>(this.baseUrl + 'admin/drivers', {headers})
  }

  getRides(): Observable<DetailedRide[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<DetailedRide[]>(this.baseUrl + 'admin/rides', {headers})
   }

  blockDriver(driverId: number): Observable<any> {
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}admin/${driverId}/block`, {}, {headers});
  }

  unBlockDriver(driverId: number): Observable<any> {
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}admin/${driverId}/unblock`, {}, {headers});
  }

  acceptVerification(driverId: number): Observable<any>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}admin/${driverId}/accept`, {}, {headers});
  }

  denyVerification(driverId: number): Observable<any>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.patch<any>(`${this.baseUrl}admin/${driverId}/deny`, {}, {headers});
  }

}
