import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';


import { TranslatePipe } from './pipes/translate.pipe';
import { MatchDirective } from './directives/match.directive';


@NgModule({
  imports: [
    CommonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatNativeDateModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    FormsModule
  ],
  declarations: [
    TranslatePipe,
    MatchDirective
  ],
  exports: [
    TranslatePipe,
    MatchDirective
  ]
})
export class SharedModule { }
