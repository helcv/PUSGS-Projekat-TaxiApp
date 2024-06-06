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


const routes: Routes = [
  {path: '', component: HomeComponent, canActivate: [homeAccessGuard]},
  {path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      {path: 'active', component: ActiveRideComponent, canActivate: [BusyGuard]},
      {path: 'profile', component: ProfileComponent, canActivate: [NotBusyGuard]},
      {path: 'ride', component: CreateRideComponent, canActivate: [NotBusyGuard]},
      {path: 'ride-history', component: RideHistoryComponent, canActivate: [NotBusyGuard]},
      {path: 'drivers', component: AdminPanelComponent, canActivate: [adminGuard]},
      {path: 'rides', component: RidesComponent, canActivate: [adminGuard]}
    ]
  },
  {path: '**', redirectTo: ''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
