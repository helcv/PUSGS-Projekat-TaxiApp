import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ProfileComponent } from './profile/profile.component';
import { CreateRideComponent } from './ride/create-ride/create-ride.component';
import { RideHistoryComponent } from './ride/ride-history/ride-history.component';
import { authGuard } from './_guards/auth.guard';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { adminGuard } from './_guards/admin.guard';
import { RidesComponent } from './admin/rides/rides.component';
import { homeAccessGuard } from './_guards/home-access.guard';
import { ActiveRideComponent } from './ride/active-ride/active-ride.component';
import { BusyGuard } from './_guards/busy.guard';
import { NotBusyGuard } from './_guards/not-busy.guard';
import { NewRidesComponent } from './ride/_driver/new-rides/new-rides.component';
import { CountdownComponent } from './ride/countdown/countdown.component';
import { RatingsComponent } from './ratings/ratings.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { GoogleRegisterComponent } from './google-register/google-register.component';


const routes: Routes = [
  { path: '', component: HomeComponent, canActivate: [homeAccessGuard] },
  { path: 'complete-registration', component: GoogleRegisterComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      { path: 'active', component: ActiveRideComponent, canActivate: [BusyGuard] },
      { path: 'countdown', component: CountdownComponent, canActivate: [BusyGuard] },
      { path: 'profile', component: ProfileComponent, canActivate: [NotBusyGuard] },
      { path: 'profile/edit', component: EditProfileComponent, 
        canActivate: [NotBusyGuard], 
        canDeactivate: [preventUnsavedChangesGuard] },
      { path: 'ride', component: CreateRideComponent, canActivate: [NotBusyGuard] },
      { path: 'new-rides', component: NewRidesComponent, canActivate: [NotBusyGuard] },
      { path: 'ride-history', component: RideHistoryComponent, canActivate: [NotBusyGuard] },
      { path: 'drivers', component: AdminPanelComponent, canActivate: [adminGuard] },
      { path: 'rides', component: RidesComponent, canActivate: [adminGuard] },
      { path: 'rating/:driverUsername', component: RatingsComponent }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
