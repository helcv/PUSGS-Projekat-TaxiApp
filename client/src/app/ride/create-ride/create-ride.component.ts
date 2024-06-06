/// <reference types="@types/googlemaps" />
import { AfterViewInit, Component, ElementRef, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators} from '@angular/forms';
import { Ride } from 'src/app/_models/ride';
import { RideService } from 'src/app/_services/ride.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-create-ride',
  templateUrl: './create-ride.component.html',
  styleUrls: ['./create-ride.component.css']
})
export class CreateRideComponent implements AfterViewInit {
  @ViewChild('startInput') startInput!: ElementRef;
  @ViewChild('finishInput') finishInput!: ElementRef;
  @ViewChild('rideModal') rideModal!: TemplateRef<any>;
  rideForm: FormGroup;
  createdRide?: Ride;
  startAddress: string = '';
  finalAddress: string = '';
  rideModalRef?: BsModalRef;

  constructor(private rideService: RideService, 
    private fb: FormBuilder, 
    private modalService: BsModalService,
    private toastr: ToastrService,
    private router: Router,
    private accountService: AccountService) {

    this.rideForm = this.fb.group({
      startAddress: ['', [Validators.required, this.addressValidator]],
      finalAddress: ['', [Validators.required, this.addressValidator]]
    });
  }

  ngAfterViewInit(): void {
    this.initAutocomplete(this.startInput.nativeElement);
    this.initAutocomplete(this.finishInput.nativeElement)
  }

  initAutocomplete(input: HTMLInputElement) {
    if (window.google && window.google.maps && window.google.maps.places) {
      const startAutocomplete = new google.maps.places.Autocomplete(this.startInput.nativeElement);
      const finishAutocomplete = new google.maps.places.Autocomplete(this.finishInput.nativeElement);

      startAutocomplete.addListener('place_changed', () => {
        const place = startAutocomplete.getPlace();
        this.startAddress = this.formatAddress(place);
      });

      finishAutocomplete.addListener('place_changed', () => {
        const place = finishAutocomplete.getPlace();
        this.finalAddress = this.formatAddress(place);
      });
    } else {
      console.error('Google Maps API or Places library not loaded.');
    }
  }

  private formatAddress(place: google.maps.places.PlaceResult): string {
    if (!place.address_components) {
      return '';
    }

    let streetNumber = '';
    let route = '';
    let locality = '';
    let country = '';

    for (const component of place.address_components) {
      const types = component.types;
      if (types.includes('street_number')) {
        streetNumber = component.long_name;
      }
      if (types.includes('route')) {
        route = component.long_name;
      }
      if (types.includes('locality')) {
        locality = component.long_name;
      }
      if (types.includes('country')) {
        country = component.long_name;
      }
    }

    return `${streetNumber}, ${route}, ${locality}, ${country}`;
  }

  createRide() {
    const startAddress = this.startInput.nativeElement.value;
    const finalAddress = this.finishInput.nativeElement.value;

    console.log(this.startAddress);
    console.log(this.finalAddress);

    const rideModel = {
      startAddress: this.startAddress,
      finalAddress: this.finalAddress
    }

    this.rideService.createRide(rideModel).subscribe({
      next: (ride) => {
        console.log('Ride created successfully:', ride)
        this.createdRide = ride
        this.rideModalRef = this.modalService.show(this.rideModal, {
          backdrop: 'static',
          keyboard: false
        });
      } ,
      error: (error) => {
        console.error('Error creating ride:', error),
        this.toastr.error('Failed to create ride. Please check the addresses you entered.', 'Error')
      }
    });
  }

  requestRide(): void {
    if (this.createdRide?.id) {
      this.rideService.requestRide(this.createdRide.id).subscribe({
        next: () => {
          this.toastr.success('Ride requested successfully!');
          this.router.navigateByUrl('/active');
          this.rideModalRef?.hide();
        },
        error: (err) => {
          this.toastr.error('Failed to request ride.');
        }
      });
    } else {
      this.toastr.error('No ride selected.');
    }
  }

  declineRide(): void {
    if (this.createdRide?.id) {
      this.rideService.declineRide(this.createdRide.id).subscribe({
        next: () => {
          this.toastr.success('Ride declined successfully!');
          this.rideModalRef?.hide();
        },
        error: (err) => {
          this.toastr.error('Failed to decline ride.');
        }
      });
    } else {
      this.toastr.error('No ride selected.');
    }
  }

  private addressValidator(control: { value: string }): { [key: string]: any } | null {
    const parts = control.value.split(',');
    const hasStreetNumber = /\d/.test(parts[0]);
    const hasEnoughParts = parts.length >= 3;
  
    if (!hasStreetNumber || !hasEnoughParts) {
      return { 'invalidInput': true };
    }
    return null;
  }

  onDropdownSelect(inputField: string, event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target) {
      const selectedValue = target.value;
      this.rideForm.controls[inputField].setValue(selectedValue);
      this.rideForm.controls[inputField].markAsDirty();
      this.rideForm.controls[inputField].updateValueAndValidity();
    }
  }
}

