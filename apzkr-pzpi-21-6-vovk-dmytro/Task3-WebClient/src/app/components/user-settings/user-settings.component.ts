import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ClientStatus } from 'src/app/models/data/Client';
import { User } from 'src/app/models/data/User';
import { Alert, AlertsService } from 'src/app/services/alerts.service';
import { UserApiService } from 'src/app/services/user-api.service';

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent {
  userModel: User = new User();

  oldEmail: string = '';

  oldPassword: string = '';
  newPassword: string = '';

  constructor(private userApi: UserApiService, private router: Router, private alertsService: AlertsService) {
    this.userApi.getSelfDetailed().then(user => {
      this.userModel = User.fromResponse(user);
      this.oldEmail = user.email;
    });
  }
  statusToText(): string {
    switch (this.userModel.status) {
      case ClientStatus.Active:
        return 'active';
      case ClientStatus.Inactive:
        return 'inactive';
      case ClientStatus.Banned:
        return 'blocked';
      case ClientStatus.Limited:
        return 'limited';
      default:
        return 'unknown';
    }
  }

  save() {
    if(this.userApi.user.permissions.users.canUpdateSelfEmail){
      if (this.userModel.email !== this.oldEmail) {
        this.userApi.updateSelfEmail(this.userModel.email).then(() => {
          this.alertsService.add(new Alert('info', 'userSettings.emailChanged',{
            lifetime: 10_000
          }));
        });
      }
    }
  }

  cancel() {
    this.router.navigate(['/home/map']);
  }
}
