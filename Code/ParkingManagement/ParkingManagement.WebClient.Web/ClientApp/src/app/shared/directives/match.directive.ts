import { Directive, Input } from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';

@Directive({
  selector: '[match]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MatchDirective, multi: true }]
})
export class MatchDirective implements Validator {
  @Input('match')
    matchTo: string = '';

  validate(control: AbstractControl): { [key: string]: any } | null {
    return this.matchTo !== control.value ? { match: { value: control.value } } : null;
  }
}
