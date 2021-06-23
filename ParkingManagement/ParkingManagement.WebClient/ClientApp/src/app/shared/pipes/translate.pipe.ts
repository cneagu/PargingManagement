import { Pipe, PipeTransform } from '@angular/core';

import { TextService } from '../../core/text.service';

@Pipe({
  name: 'translate'
})
export class TranslatePipe implements PipeTransform {

  constructor(private textService: TextService) { }

  transform(key: any): string {
    return this.textService.getText(key);
  }
}
