import { Directive, Input, OnChanges, OnInit, SimpleChanges, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit, OnChanges {
  @Input() appHasRole: string[] = [];
  @Input('busy') busy: boolean = false;
  user: User = {} as User;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.accountService.currentUser$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: user => {
          if (user) {
            this.user = user;
            this.updateView();
          }
        }
      });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['busy']) {
      this.updateView();
    }
  }

  private updateView(): void {
    if (
      this.user.roles.some(r => this.appHasRole.includes(r)) &&
      this.user['busy'] === this.busy
    ) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
