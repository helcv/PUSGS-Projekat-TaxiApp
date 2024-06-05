/// <reference types="@types/googlemaps" />
import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-create-ride',
  templateUrl: './create-ride.component.html',
  styleUrls: ['./create-ride.component.css']
})
export class CreateRideComponent implements AfterViewInit {
  @ViewChild('startInput') startInput!: ElementRef;
  @ViewChild('finishInput') finishInput!: ElementRef;

  constructor() {}

  ngAfterViewInit(): void {
    this.initAutocomplete();
  }

  initAutocomplete() {
    const startAutocomplete = new google.maps.places.Autocomplete(this.startInput.nativeElement);
    const finishAutocomplete = new google.maps.places.Autocomplete(this.finishInput.nativeElement);
  }

  createRide() {
    const startAddress = this.startInput.nativeElement.value;
    const finalAddress = this.finishInput.nativeElement.value;
    console.log('Start Address:', startAddress);
    console.log('Final Address:', finalAddress);
  }
}

