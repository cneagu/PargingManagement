import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProgressBarService {
  private subject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  subject$: Observable<boolean> = this.subject.asObservable();

  constructor() { }

  push(value: boolean) {
    this.subject.next(value);
  }
}
