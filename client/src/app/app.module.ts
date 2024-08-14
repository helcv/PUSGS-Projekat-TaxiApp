import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http'

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DatePickerComponent } from './_forms/date-picker/date-picker.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { CreateRideComponent } from './ride/create-ride/create-ride.component';
import { RideHistoryComponent } from './ride/ride-history/ride-history.component';
import { ProfileComponent } from './profile/profile.component';
import { ToastrModule } from 'ngx-toastr';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/has-role.directive';
import { RidesComponent } from './admin/rides/rides.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { ActiveRideComponent } from './ride/active-ride/active-ride.component';
import { NewRidesComponent } from './ride/_driver/new-rides/new-rides.component';
import { CountdownComponent } from './ride/countdown/countdown.component';
import { RatingsComponent } from './ratings/ratings.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { GoogleRegisterComponent } from './google-register/google-register.component';
import { MessagesComponent } from './messages/messages.component';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { TimeagoModule } from 'ngx-timeago';
import { MessageThreadComponent } from './message-thread/message-thread.component';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    TextInputComponent,
    DatePickerComponent,
    CreateRideComponent,
    RideHistoryComponent,
    ProfileComponent,
    AdminPanelComponent,
    HasRoleDirective,
    RidesComponent,
    ActiveRideComponent,
    NewRidesComponent,
    CountdownComponent,
    RatingsComponent,
    EditProfileComponent,
    GoogleRegisterComponent,
    MessagesComponent,
    MessageThreadComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    BsDropdownModule.forRoot(),
    BsDatepickerModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }),
    CollapseModule.forRoot(),
    ModalModule.forRoot(),
    ButtonsModule.forRoot(),
    TimeagoModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
