import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ProfileComponent } from './profile/profile.component';
import { CreateRideComponent } from './ride/create-ride/create-ride.component';
import { RideHistoryComponent } from './ride/ride-history/ride-history.component';
import { authGuard } from './_guards/auth.guard';

const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [authGuard],
    children: [
      {path: 'profile', component: ProfileComponent},
      {path: 'ride', component: CreateRideComponent},
      {path: 'ride-history', component: RideHistoryComponent}
    ]
  },
  {path: '**', component: HomeComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
