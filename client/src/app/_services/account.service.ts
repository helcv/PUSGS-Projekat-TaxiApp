import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, map, of, switchMap, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Token } from '../_models/token';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currUserSource.asObservable();
  token: string | null = null;

  constructor(private http: HttpClient, private presenceService: PresenceService) {
   }

  login(model: any) {
    return this.http.post<Token>(this.baseUrl + 'account/login', model).pipe(
      switchMap((response: Token) => {
        const token = response;
        if (token) {
          localStorage.setItem('token', JSON.stringify(token));
          return this.getUserProfile().pipe(
            tap(user => {
              if (user) {
                localStorage.setItem('user', JSON.stringify(user));
                this.setCurrentUser(user);
              }
            })
          );
        }
        return of(null);
      }),
      tap(user => {  
        this.currUserSource.next(user);
      })
    );
  }

  googleLogin(model: any) {
    return this.http.post<Token>(this.baseUrl + 'account/login-google', model).pipe(
      switchMap((response: Token) => {
        const token = response;
        if (token) {
          localStorage.setItem('token', JSON.stringify(token));
          return this.getUserProfile().pipe(
            tap(user => {
              if (user) {
                localStorage.setItem('user', JSON.stringify(user));
                this.setCurrentUser(user);
              }
            })
          );
        }
        return of(null);
      }),
      tap(user => {
        this.currUserSource.next(user);
      })
    );
  }

  register(model: any){
    return this.http.post<Token>(this.baseUrl + "account/register", model).pipe(
      switchMap((response: Token) => {
        const token = response;
        if (token) {
          localStorage.setItem('token', JSON.stringify(token));
          return this.getUserProfile().pipe(
            tap(user => {
              if (user) {
                localStorage.setItem('user', JSON.stringify(user));
                this.setCurrentUser(user);
              }
            })
          );
        }
        return of(null);
      }),
      tap(user => {
        this.currUserSource.next(user);
      })
    )
  }

  googleRegister(model: any){
    return this.http.post<Token>(this.baseUrl + "account/signin-google", model).pipe(
      switchMap((response: Token) => {
        const token = response;
        if (token) {
          localStorage.setItem('token', JSON.stringify(token));
          return this.getUserProfile().pipe(
            tap(user => {
              if (user) {
                localStorage.setItem('user', JSON.stringify(user));
                this.setCurrentUser(user);
              }
            })
          );
        }
        return of(null);
      }),
      tap(user => {
        this.currUserSource.next(user);
      })
    )
  }

  logout(){
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.currUserSource.next(null);
    this.presenceService.stopHubConnection();
  }

  editProfile(model: any): Observable<string>{
    const headers = this.getAuthHeaders();
    return this.http.put<string>(this.baseUrl + 'account', model, {headers})
   }

  changePassword(model: any): Observable<string>{
    const headers = this.getAuthHeaders();
    return this.http.put<string>(this.baseUrl + 'account/password', model, {headers})
   }

  setCurrentUser(user: User) {
    const tokenString = localStorage.getItem('token');
    if (tokenString) {
      const tokenObject = JSON.parse(tokenString);
      this.token = tokenObject.token;
    }
  
    if (!user.roles) {
      user.roles = []; 
    }
  
    if (this.token) {
      const roles = this.getDecodedToken(this.token).role;
      if (Array.isArray(roles)) {
        user.roles = [...user.roles, ...roles];
      } else {
        user.roles.push(roles);
      }
    }
    this.currUserSource.next(user);
    if (this.token)
      this.presenceService.createHubConnection(this.token);
  }
  
  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]))
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getAuthToken();
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getAuthToken(): string | null {
    const tokenString = localStorage.getItem('token');
    if (tokenString) {
      const tokenObject = JSON.parse(tokenString);
      return tokenObject.token;
    }
    return null;
  }

  getUserProfile(): Observable<User | null> {
    const headers = this.getAuthHeaders()
    return this.http.get<User>(this.baseUrl + 'account', {headers}).pipe(
      catchError(error => {
        console.error('Error fetching user profile:', error);
        return of(null);
      })
    );
  }
}
