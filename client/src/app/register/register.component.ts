import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomValidators } from '../_validators/custom-validators';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;


  constructor(private accountService: AccountService, private router: Router, private toastr: ToastrService) {
  }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 16);
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required,
        Validators.minLength(6), Validators.maxLength(15), CustomValidators.passwordStrength]),
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')]),
      name: new FormControl('', Validators.required),
      lastname: new FormControl('', Validators.required),
      role: new FormControl('', Validators.required),
      address: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required)
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValues(matchTo: string) : ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value, dateOfBirth: dob}
    console.log('Form Submitted', dob);
    this.accountService.register(values).subscribe({
      next: response => {
        this.router.navigateByUrl('/profile')
      },
      error: error => {
        this.toastr.error(error.error),
        this.validationErrors = error
      }
    }) 
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;

    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset()))
      .toISOString().slice(0,10);
  }
}
