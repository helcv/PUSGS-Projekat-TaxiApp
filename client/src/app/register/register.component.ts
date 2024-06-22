import { Component, EventEmitter, NgZone, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { AbstractControl, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomValidators } from '../_validators/custom-validators';
import { ToastrService } from 'ngx-toastr';

declare const google: any;

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
  selectedFile: File | null = null;
  googleToken: string | null = null;

  constructor(private accountService: AccountService, 
    private router: Router, 
    private toastr: ToastrService,
    private ngZone: NgZone) {}

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 16);

    google.accounts.id.initialize({
      client_id: '1004079564257-u861m67mkdlcoqar7drc1hflub6oevpc.apps.googleusercontent.com',
      callback: (response: any) => this.handleCredentialResponse(response)
    });
    google.accounts.id.renderButton(
      document.getElementById('google-button'),
      { theme: 'outline', size: 'large' }
    );
  }

  handleCredentialResponse(response: any) {
    this.googleToken = response.credential;
    this.ngZone.run(() => {
      this.router.navigate(['/complete-registration'], { state: { googleToken: this.googleToken } });
    });
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
      dateOfBirth: new FormControl('', Validators.required),
      photo: new FormControl(null, Validators.required)
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    });
  }

  matchValues(matchTo: string) : ValidatorFn {
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : { notMatching: true };
    }
  }

  onFileChange(event: any) {
    if (event.target.files.length > 0) {
      this.selectedFile = event.target.files[0];
      this.registerForm.controls['photo'].setValue(this.selectedFile);
    }
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = { ...this.registerForm.value, dateOfBirth: dob };

    const formData: FormData = new FormData();
    for (const key in values) {
      formData.append(key, values[key]);
    }

    if (this.selectedFile) {
      formData.append('photo', this.selectedFile, this.selectedFile.name);
    } else {
      this.toastr.error('Photo is required.');
      return;
    }

    this.accountService.register(formData).subscribe({
      next: response => {
        this.toastr.success('Registration successful');
        this.router.navigateByUrl('/profile');
      },
      error: error => {
        console.error('Registration error:', error);
        this.validationErrors = error;
        this.toastr.error('Registration failed');
      }
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if (!dob) return;

    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset()))
      .toISOString().slice(0, 10);
  }
}
