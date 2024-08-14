import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AccountService } from './account.service';
import { Message } from '../_models/message';
import { BehaviorSubject, Observable, take } from 'rxjs';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient, private accountService: AccountService, private toastr: ToastrService, private router: Router) { }

  createHubConnection(token: string, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start()
      .then(() => console.log('Connection started'))
      .catch(err => console.error('Error while starting connection: ' + err));

      this.hubConnection.on('ReceiveMessageThread', (messages: Message[]) => {
        console.log('Received messages from SignalR:', messages);
        this.messageThreadSource.next(messages);
      });

      this.hubConnection.on('ErrorMessage', error => {
        this.toastr.error('Error: ' + error);
        this.router.navigateByUrl('/messages');
      });

     this.hubConnection.on('NewMessage', message => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: messages => {
          this.messageThreadSource.next([...messages, message])
        }
      })
    })
  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().then(() => console.log('Connection ended'));
    }
    
  }

  getMessages(container: string): Observable<Message[]>{
    const headers = this.accountService.getAuthHeaders();
    const params = new HttpParams().set('container', container)
    return this.http.get<Message[]>(this.baseUrl + 'messages', {headers, params})
   }

   getMessageThread(username: string): Observable<Message[]>{
    const headers = this.accountService.getAuthHeaders();
    return this.http.get<Message[]>(`${this.baseUrl}messages/thread/${username}`, {headers});
   }

   async sendMessage(username: string, content: string) {
    const headers = this.accountService.getAuthHeaders();
    return this.hubConnection?.invoke('SendMessage', {recipientUsername: username, content})
      .catch(error => console.log(error));
   } 

   deleteMessage(id: number) {
    const headers = this.accountService.getAuthHeaders();
    return this.http.delete(this.baseUrl + 'messages/' + id, {headers});
   }
}
