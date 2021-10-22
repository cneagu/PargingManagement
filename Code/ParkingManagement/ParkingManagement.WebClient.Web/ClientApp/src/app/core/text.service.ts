import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TextService {
  private translations!: Translations;

  constructor(private httpClient: HttpClient) { }

  load(): Observable<boolean> {
    return this.httpClient.get<Translations>('assets/translations.json')
      .pipe(
        map(result => {
          this.translations = result;

          return !!result;
        })
      );
  }

  getText(key: string): string {
    return this.translations[key];
  }
}

export interface Translations {
  [key: string]: string
}
