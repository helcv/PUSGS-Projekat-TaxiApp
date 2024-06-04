import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, map, of, switchMap, tap } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Token } from '../_models/token';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currUserSource = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currUserSource.asObservable();
  

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post<Token>(this.baseUrl + 'account/login', model).pipe(
      switchMap((response: Token) => {
        const token = response;
        if (token) {
          localStorage.setItem('token', JSON.stringify(token));
          return this.getUserProfile(token.token).pipe(
            tap(user => {
              if (user) {
                localStorage.setItem('user', JSON.stringify(user));
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

  logout(){
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.currUserSource.next(null);
  }

  setCurrentUser(user: User){
    this.currUserSource.next(user);
  }


  private getUserProfile(token: string): Observable<User | null> {
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.get<User>(this.baseUrl + 'account', { headers }).pipe(
      catchError(error => {
        console.error('Error fetching user profile:', error);
        return of(null);
      })
    );
  }
}
