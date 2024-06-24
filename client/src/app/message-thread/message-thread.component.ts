import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';
import { User } from '../_models/user';
import { Message } from '../_models/message';
import { MessageService } from '../_services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-message-thread',
  templateUrl: './message-thread.component.html',
  styleUrls: ['./message-thread.component.css']
})
export class MessageThreadComponent implements OnInit{
  @ViewChild('messageForm', { static: false }) messageForm!: NgForm;
  @ViewChild('messageContainer') messageContainer?: ElementRef;
  user: User | null = null;
  messages: Message[] = [];
  username: string = '';
  messageContent = '';
  recipientPhoto = '';

  constructor(private accountService: AccountService, 
    private messageService: MessageService, 
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService) {
    
  }

  ngOnInit(): void {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })

    this.route.paramMap.subscribe(params => {
      this.username = params.get('username') || '';
      this.loadMessages();
    });
  }

  ngAfterViewInit() {
    setTimeout(() => this.scrollToBottom(), 0);
  }

  loadMessages() {
    if (this.user && this.username) {
      this.messageService.getMessageThread(this.username).subscribe({
        next: messages => {
          this.messages = messages
          this.storePhoto();
        },
        error: err => {
          this.toastr.error('Unable to load page')
          this.router.navigateByUrl('/messages')
        }
      });
    }
  }

  sendMessage() {
    if (!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        this.messages.push(message)
        this.messageForm?.reset()
        setTimeout(() => this.scrollToBottom(), 0);
      }
    })
  }

  scrollToBottom(): void {
    if (this.messageContainer)
    try {
        this.messageContainer.nativeElement.scrollTop = this.messageContainer.nativeElement.scrollHeight;
    } catch(err) {}
  }

  storePhoto(): void {
    if (!this.messages || this.messages.length === 0) return;

    for (let i = this.messages.length - 1; i >= 0; i--) {
      const message = this.messages[i];
      if (message.senderUsername === this.username) {
        this.recipientPhoto = message.senderPhotoUrl;
        break;
      }
      if (message.recipientUsername === this.username) {
        this.recipientPhoto = message.recipientPhotoUrl;
        break;
      }
    }
  }
}
