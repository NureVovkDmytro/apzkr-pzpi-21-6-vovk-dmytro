import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  isMapPage: boolean = false;

  constructor(private router: Router) {
    this.router.events.subscribe((event) => {
      let splitUrl = this.router.url.split('/');
      if(splitUrl.length > 1 && splitUrl[1] == 'home'){
        this.isMapPage = splitUrl[2].split('?')[0] == 'map';
      }else {
        this.isMapPage = false;
      }
    });
  }
}
