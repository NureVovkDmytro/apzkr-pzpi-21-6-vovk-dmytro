import { Component } from '@angular/core';
import { Alert, AlertsService } from 'src/app/services/alerts.service';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
})
export class NotificationsComponent {
  public get alerts() {
    return this.alertsService.list;
  }
  constructor(private alertsService: AlertsService) { }

  close(alert: Alert) {
    if(alert.onclose) {
      alert.onclose(alert);
    }
    this.alertsService.remove(alert);
  }

  click(alert: Alert, event: MouseEvent) {
    if(event.target instanceof HTMLElement && event.target.closest('.btn-close')) {
      return;
    }
    if (alert.onclick) {
      alert.onclick(alert);
    }
  }
}
