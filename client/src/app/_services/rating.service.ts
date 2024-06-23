import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountService } from './account.service';
import { HttpClient } from '@angular/common/http';
import { Rating } from '../_models/rating';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RatingService {
  baseUrl = environment.apiUrl;

  constructor(private accountService: AccountService, private http: HttpClient) { }

  createRating(rating: Rating): Observable<string>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.post<string>(this.baseUrl + 'ratings', rating, {headers})
   }
}
