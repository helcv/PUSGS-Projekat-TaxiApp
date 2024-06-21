import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup} from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';


@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css']
})
export class EditProfileComponent implements OnInit {
  user: User | null = null;
  editProfileForm: FormGroup;
  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: any) {
    if(this.editProfileForm.dirty) {
      event.returnValue = true;
    }
  }
  
  constructor(private fb: FormBuilder, private accountService: AccountService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })

    this.editProfileForm = this.fb.group({
      name: ['',],
      lastname: ['',],
      address: ['',]
    });
  }

  ngOnInit(): void {
    this.loadUserProfile()
  }

  private updateForm(user: User | null): void {
    if (user && this.editProfileForm) {
      this.editProfileForm.patchValue({
        name: user.name,
        lastname: user.lastname,
        address: user.address
      });
    }
  }

  onSubmit(): void {
    if (this.editProfileForm.valid) {
      this.accountService.editProfile(this.editProfileForm.value).subscribe({
        next: _ => {
          this.loadUserProfile()
          this.editProfileForm?.reset(this.user)
          this.toastr.success('Profile updated successfully.')
        },
        error: (error) => {
          this.toastr.error('Failed to update profile.')
          console.log(error);
        }
      });
    }
  }

  loadUserProfile(): void {
    if(this.user)
    this.accountService.getUserProfileById(this.user?.id).subscribe({
      next: (user: User | null) => {
        this.user = user;
        this.updateForm(this.user)
      },
      error: (error) => {
        console.error('Error fetching user profile:', error);
      }
    });
  }
}
