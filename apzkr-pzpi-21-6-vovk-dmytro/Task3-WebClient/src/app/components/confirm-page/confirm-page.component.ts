import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Alert, AlertsService } from 'src/app/services/alerts.service';
import { ConfirmService } from 'src/app/services/confirm.service';

@Component({
  selector: 'app-confirm-page',
  templateUrl: './confirm-page.component.html',
  styleUrls: ['./confirm-page.component.css']
})
export class ConfirmPageComponent implements AfterViewInit{
  public token: string = '';
  public type: string = '';

  constructor(private route: ActivatedRoute, private router: Router, private confirmService: ConfirmService, private alertsService: AlertsService) {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      this.type = params['type'];
    });
  }

  ngAfterViewInit() {
    this.confirmService.confirm(this.token, this.type).then(() => {
      this.alertsService.add(new Alert('success', 'alert.confirm.success', {
        lifetime: 3000
      }));
      this.router.navigate(['/login']);
    }).catch(() => {
      this.alertsService.add(new Alert('danger', 'alert.confirm.error', {
        lifetime: 3000
      }));
      this.router.navigate(['/login']);
    });
  }
}
