import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { CustomValidators } from '../_validators/custom-validators';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css']
})
export class EditProfileComponent implements OnInit {
  user: User | null = null;
  editProfileForm: FormGroup;
  changePasswordForm: FormGroup;

  @HostListener('window:beforeunload', ['$event'])
  onBeforeUnload(event: any) {
    if( this.changePasswordForm.dirty || this.editProfileForm.dirty) {
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

    this.changePasswordForm = this.fb.group({
      oldPassword: ['', Validators.required],
      newPassword: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(15),
        CustomValidators.passwordStrength
      ]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    this.loadUserProfile();
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

  onSubmitProfile(): void {
    if (this.editProfileForm.valid) {
      this.accountService.editProfile(this.editProfileForm.value).subscribe({
        next: _ => {
          this.loadUserProfile();
          this.editProfileForm?.reset(this.user);
          this.toastr.success('Profile updated successfully.');
        },
        error: (error) => {
          this.toastr.error('Failed to update profile.');
          console.log(error);
        }
      });
    }
  }

  onSubmitPassword(): void {
    if (this.changePasswordForm.valid) {
      this.accountService.changePassword(this.changePasswordForm.value).subscribe({
        next: _ => {
          this.changePasswordForm.reset();
          this.toastr.success('Password changed successfully.');
        },
        error: (error) => {
          this.toastr.error('Failed to change password.');
          console.log(error);
        }
      });
    }
  }

  loadUserProfile(): void {
    if(this.user)
    this.accountService.getUserProfile().subscribe({
      next: (user: User | null) => {
        this.user = user;
        this.updateForm(this.user);
      },
      error: (error) => {
        console.error('Error fetching user profile:', error);
      }
    });
  }

  private passwordMatchValidator(form: FormGroup) {
    const password = form.get('newPassword');
    const confirmPassword = form.get('confirmPassword');
    return password && confirmPassword && password.value === confirmPassword.value ? null : { mismatch: true };
  }
}
