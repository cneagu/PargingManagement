import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { ProgressBarService } from './progress-bar.service';

@Component({
  selector: 'app-progress-bar',
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.scss']
})
export class ProgressBarComponent implements OnInit, OnDestroy {
  show: boolean = false;
  subscription: Subscription | undefined;

  constructor(private progressBarService: ProgressBarService) { }

  ngOnInit() {
    this.subscription = this.progressBarService.subject$.subscribe((value: boolean) => {
      this.show = value;
    });
  }

  ngOnDestroy() {
    if (this.subscription)
      this.subscription.unsubscribe();
  }
}
