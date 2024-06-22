import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { CustomValidators } from '../_validators/custom-validators';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';

@Component({
  selector: 'app-google-register',
  templateUrl: './google-register.component.html',
  styleUrls: ['./google-register.component.css']
})
export class GoogleRegisterComponent implements OnInit {
  registrationForm: FormGroup;
  googleToken: string | null = null;
  maxDate: Date = new Date();
  user: User | null = null;
  isLoading: boolean = true;
 
  constructor(private router: Router, private accountService: AccountService, private toastr: ToastrService) {
    this.accountService.currentUser$.subscribe({
      next: user => {
        this.user = user;
      }
    });

    this.registrationForm = new FormGroup({
      password: new FormControl('', [Validators.required,
        Validators.minLength(6), Validators.maxLength(15), CustomValidators.passwordStrength]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')]),
      role: new FormControl('', Validators.required),
      address: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required),
    });
    this.registrationForm.controls['password'].valueChanges.subscribe({
      next: () => this.registrationForm.controls['confirmPassword'].updateValueAndValidity()
    });
  }

  ngOnInit(): void {
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 16);
    this.googleToken = history.state.googleToken;
    if (!this.googleToken) {
      this.router.navigate(['']);
    } else {
      this.login()
    }
  }

  register() {
    const dob = this.getDateOnly(this.registrationForm.controls['dateOfBirth'].value);
    const values = { ...this.registrationForm.value, dateOfBirth: dob, googleToken: this.googleToken };

    const formData: FormData = new FormData();
    for (const key in values) {
      formData.append(key, values[key]);
    }

    this.accountService.googleRegister(formData).subscribe({
      next: response => {
        this.toastr.success('Registration successful');
        this.router.navigateByUrl('/profile');
      },
      error: error => {
        console.error('Registration error:', error);
        this.toastr.error('Registration failed');
      }
    });
  }

  login() {
    this.accountService.googleLogin({ googleToken: this.googleToken }).subscribe({
      next: (response: any) => {
        if (this.user?.busy)
          {
            this.router.navigateByUrl('/active');
          }
        else{
          this.router.navigateByUrl('/profile');
        }
      },
      error: error => {
        this.isLoading = false;
        this.router.navigateByUrl('/complete-registration');
        console.error(error.error);
      }
    });
  }

  matchValues(matchTo: string) : ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
    }
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;

    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset()))
      .toISOString().slice(0, 10);
  }
}
