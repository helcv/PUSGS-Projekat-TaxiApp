import { Component, OnInit } from '@angular/core';
import { RatingService } from '../_services/rating.service';
import { Rating } from '../_models/rating';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-ratings',
  templateUrl: './ratings.component.html',
  styleUrls: ['./ratings.component.css']
})
export class RatingsComponent implements OnInit {
  ratingForm: FormGroup;
  rating: number = 0;
  stars: boolean[] = Array(5).fill(false);
  ratingTouched: boolean = false;
  driverUsername: string = '';

  constructor(
    private fb: FormBuilder,
    private ratingService: RatingService,
    private toastr: ToastrService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.ratingForm = this.fb.group({
      message: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.driverUsername = params.get('driverUsername') || '';
    });
   }

  rate(stars: number) {
    this.rating = stars;
    this.ratingTouched = true;
  }

  submitForm() {
    if (this.ratingForm.valid && this.rating > 0) {
      const ratingData: Rating = {
        driverUsername: this.driverUsername,
        message: this.ratingForm.value.message,
        stars: this.rating
      };
      console.log(ratingData);


      this.ratingService.createRating(ratingData).subscribe(
        response => {
          console.log('Response from backend:', response);
          this.toastr.success("Rating added successfully!")
          this.router.navigateByUrl('/profile')
        },
        error => {
          console.error('Error occurred:', error);
          this.toastr.error("Failed to add a rating.")
        }
      );
    }
  }
}
