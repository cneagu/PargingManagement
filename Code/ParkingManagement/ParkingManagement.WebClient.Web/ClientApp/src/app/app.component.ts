import { Component, OnInit } from '@angular/core';
import { TextService } from './core/text.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  loaded: boolean = false;

  constructor(
    private textService: TextService
  ) { }

  ngOnInit() {

    this.textService.load().subscribe(loaded => {
      this.loaded = loaded;

    });
  }
}
