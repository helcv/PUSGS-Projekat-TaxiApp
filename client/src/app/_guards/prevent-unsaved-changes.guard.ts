import { CanDeactivateFn } from '@angular/router';
import { EditProfileComponent } from '../edit-profile/edit-profile.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<EditProfileComponent> = (component) => {
  if (component.editProfileForm.dirty || component.changePasswordForm.dirty) {
    return confirm('Are you sure you want to continue? Any unsaved changes will be lost')
  }

  return true;
};
