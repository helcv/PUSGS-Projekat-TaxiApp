import { AbstractControl, ValidationErrors } from '@angular/forms';

export class CustomValidators {
  static passwordStrength(control: AbstractControl): ValidationErrors | null {
    const value: string = control.value || '';
    if (!value) {
      return null;
    }

    const hasUpperCase = /[A-Z]+/.test(value);
    const hasNumber = /[0-9]+/.test(value);

    const errors: ValidationErrors = {};

    if (!hasUpperCase) {
      errors['uppercase'] = 'Passwords must have at least one uppercase letter (A-Z).';
    }

    if (!hasNumber) {
      errors['number'] = 'Passwords must have at least one digit (0-9).';
    }

    return Object.keys(errors).length > 0 ? errors : null;
  }
}