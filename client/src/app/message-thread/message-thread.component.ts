import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';
import { User } from '../_models/user';
import { Message } from '../_models/message';
import { MessageService } from '../_services/message.service';
import { ActivatedRoute } from '@angular/router';
import { NgForm } from '@angular/forms';
import { PresenceService } from '../_services/presence.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-message-thread',
  templateUrl: './message-thread.component.html',
  styleUrls: ['./message-thread.component.css']
})
export class MessageThreadComponent implements OnInit, OnDestroy{
  @ViewChild('messageForm', { static: false }) messageForm!: NgForm;
  user: User | null = null;
  messages: Message[] = [];
  username: string = '';
  messageContent = '';
  recipientPhoto = '';

  constructor(private accountService: AccountService, 
    public messageService: MessageService, 
    private route: ActivatedRoute,
    public presenceService: PresenceService) {
  }

  ngOnInit(): void {
    const authToken = this.accountService.getAuthToken() ?? ''
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })

    this.route.paramMap.subscribe(params => {
      this.username = params.get('username') || '';
      this.messageService.createHubConnection(authToken, this.username);
    });
    this.storePhoto()
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  sendMessage() {
    if (!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm?.reset();
    })
  } 

  storePhoto(): void {
    this.messageService.messageThread$.subscribe(messages => {
      if (!messages || messages.length === 0) return;
  
      for (let i = messages.length - 1; i >= 0; i--) {
        const message = messages[i];
        if (message.senderUsername === this.username) {
          this.recipientPhoto = message.senderPhotoUrl;
          break;
        }
        if (message.recipientUsername === this.username) {
          this.recipientPhoto = message.recipientPhotoUrl;
          break;
        }
      }
    });
  }
}
