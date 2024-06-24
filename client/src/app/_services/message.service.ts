import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AccountService } from './account.service';
import { Message } from '../_models/message';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient, private accountService: AccountService) { }

  getMessages(container: string): Observable<Message[]>{
    const headers = this.accountService.getAuthHeaders();
    const params = new HttpParams().set('container', container)
    return this.http.get<Message[]>(this.baseUrl + 'messages', {headers, params})
   }

   getMessageThread(username: string): Observable<Message[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Message[]>(`${this.baseUrl}messages/thread/${username}`, {headers});
   }

   sendMessage(username: string, content: string) {
    const headers = this.accountService.getAuthHeaders();
    return this.http.post<Message>(this.baseUrl + 'messages', {recipientUsername: username, content}, {headers})
   }

   deleteMessage(id: number) {
    const headers = this.accountService.getAuthHeaders();
    return this.http.delete(this.baseUrl + 'messages/' + id, {headers});
   }
}
