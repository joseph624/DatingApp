import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {

  @Input() label: string;
  @Input() maxDate: Date;
  bsConfig: Partial<BsDatepickerConfig>; // Partial means every property inside type BsDatpickerConfig is optional

  //Self allows us to inject dependencies locally & doesn't try to get it somewhere else
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: 'theme-blue',
      dateInputFormat: 'DD MMMM YYYY'
    }
   }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }
  
  registerOnTouched(fn: any): void {
  }



}