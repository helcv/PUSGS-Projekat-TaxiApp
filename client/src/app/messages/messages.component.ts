import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { MessageService } from '../_services/message.service';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit{
  user: User | null = null;
  messages?: Message[];
  container = 'Unread';
  loading = false;
  
  constructor(private messageService: MessageService, 
    private accountService: AccountService,
    private router: Router) {
  }

  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
    this.loadMessages()
  }

  loadMessages() {
    this.loading = true;
    this.messageService.getMessages(this.container).subscribe({
      next: response => {
        this.messages = response;
        this.loading = false;
      }
    })
  }

  deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe({
      next: () => this.messages?.splice(this.messages.findIndex(m => m.id === id), 1)
    })
  }

  goToMessageThread(message: Message) {
    if (this.container === 'Unread' || this.container === 'Inbox')
    {
      this.router.navigate(['/messages/thread', message.senderUsername])
    }
    if (this.container === 'Outbox')
    {
      this.router.navigate(['/messages/thread', message.recipientUsername])
    }
  }
}
