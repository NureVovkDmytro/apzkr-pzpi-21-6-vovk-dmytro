import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from 'src/app/models/data/User';
import { UserApiService } from 'src/app/services/user-api.service';
import { UserResponse, UserResponseDetailed } from 'src/app/models/responses/user-response';
import { AuthApiService } from 'src/app/services/auth-api.service';

@Component({
  selector: 'app-user-administration',
  templateUrl: './user-administration.component.html',
  styleUrls: ['./user-administration.component.css']
})
export class UserAdministrationComponent {
  public displayMode: DisplayMode = 'VIEW';
  public buttonLabel: string = 'buttons.save';
  public returnTo: string = '/home/admin/users';
  public userModel: User = new User();

  public get isEditMode(): boolean {
    return this.displayMode === 'EDIT' && !this.isSelfUser;
  }

  public get isSelfUser(): boolean {
    return this.userModel.id == this.currentUser.id;
  }

  public get permissions(): any {
    return this.userModel.permissions as any;
  }

  public get currentUser(): UserResponse {
    return this.userApi.user;
  }

  private oldUser = {
    baseInformation: '',
    permissions: '',
    accessLevel: '',
    scanRadius: '',
    locationInterval: '',
    status: ''
  };

  constructor(protected translate: TranslateService, protected userApi: UserApiService, protected authApi: AuthApiService, protected router: Router, protected activatedRoute: ActivatedRoute) {
    if (this.activatedRoute.snapshot.data['mode'] == 'CREATE') {
      this.displayMode = 'CREATE';
      this.buttonLabel = 'buttons.create';
    } else {
      this.activatedRoute.params.subscribe(params => {
        this.userModel.id = params['id'];
        this.displayMode = 'EDIT';
        this.buttonLabel = 'buttons.save';
        this.loadUserData();
      });
    }
    this.activatedRoute.queryParams.subscribe(params => {
      this.returnTo = params['returnTo'] || this.returnTo;
    });
  }

  loadUserData() {
    this.userApi.getById(this.userModel.id)
    .then(user => {
      this.userModel = User.fromResponse(user);
      this.oldUser.baseInformation = JSON.stringify({
        firstName: this.userModel.firstName,
        lastName: this.userModel.lastName,
        nickname: this.userModel.nickname,
        taxPayerId: this.userModel.taxPayerId,
        email: this.userModel.email,
        description: this.userModel.description
      });
      this.oldUser.permissions = JSON.stringify(this.userModel.permissions);
      this.oldUser.accessLevel = JSON.stringify(this.userModel.accessLevel);
      this.oldUser.scanRadius = JSON.stringify(this.userModel.scanRadius);
      this.oldUser.locationInterval = JSON.stringify(this.userModel.updateLocationSecondsInterval);
      this.oldUser.status = JSON.stringify(this.userModel.status);
    }).catch(() => {
      this.router.navigate(['/home/admin/users']);
    });
  }

  changePermission(permission: string, event: any) {
    // permission can be 'users.canDelete', 'users.canUpdateSelf', etc.
    let parts = permission.split('.');
    let obj = this.permissions;
    for (let i = 0; i < parts.length - 1; i++) {
      obj = obj[parts[i]];
    }
    obj[parts[parts.length - 1]] = !obj[parts[parts.length - 1]];
    event.target.checked = obj[parts[parts.length - 1]];
  }

  submit() {
    if (this.displayMode == 'CREATE' && this.userApi.user.permissions.users.canCreate) {
      this.createUser();
    } else {
      this.updateUser();
    }
  }

  private createUser() {
    this.userApi.create(this.userModel).then(user => {
      this.router.navigate(['/home/admin/users', user.id]);
    });
  }

  private updateUser() {
    const taskStack = new Array<Promise<UserResponseDetailed>>();

    if (this.currentUser.permissions.users.canUpdateBaseInformation && this.oldUser.baseInformation != JSON.stringify({
      firstName: this.userModel.firstName,
      lastName: this.userModel.lastName,
      nickname: this.userModel.nickname,
      taxPayerId: this.userModel.taxPayerId,
      email: this.userModel.email,
      description: this.userModel.description
    })) {
      taskStack.push(this.userApi.update(this.userModel));
    }

    if (this.currentUser.permissions.users.canUpdatePermissions && this.oldUser.permissions != JSON.stringify(this.userModel.permissions)) {
      taskStack.push(this.userApi.updatePermissions(this.userModel.id, this.userModel.permissions));
    }

    if (this.currentUser.permissions.users.canUpdateAccessLevel && this.oldUser.accessLevel != JSON.stringify(this.userModel.accessLevel)) {
      taskStack.push(this.userApi.updateAccessLevel(this.userModel.id, this.userModel.accessLevel));
    }

    if (this.currentUser.permissions.users.canUpdateScanRadius && this.oldUser.scanRadius != JSON.stringify(this.userModel.scanRadius)) {
      taskStack.push(this.userApi.updateScanRadius(this.userModel.id, this.userModel.scanRadius));
    }

    if (this.currentUser.permissions.users.canUpdateScanRadius && this.oldUser.locationInterval != JSON.stringify(this.userModel.updateLocationSecondsInterval)) {
      taskStack.push(this.userApi.updateLocationInterval(this.userModel.id, this.userModel.updateLocationSecondsInterval));
    }

    if (this.currentUser.permissions.users.canUpdateStatus && this.oldUser.status != JSON.stringify(this.userModel.status)) {
      taskStack.push(this.userApi.updateStatus(this.userModel.id, this.userModel.status));
    }

    if (taskStack.length == 0) {
      this.router.navigate(['/home/admin/users', this.userModel.id]);
    } else {
      Promise.all(taskStack).then((results) => {
        let user = results[0];
        this.router.navigate(['/home/admin/users', user.id]);
      });
    }
  }

  public cancel() {
    this.router.navigate([this.returnTo]);
  }

  public resetPassword() {
    if (!this.currentUser.permissions.users.canResetPassword) {
      return;
    }
    this.userApi.resetPassword(this.userModel.id).then(() => {
      alert('Password reset');
    });
  }

  public deleteUser() {
    if (!this.currentUser.permissions.users.canDelete) {
      return;
    }
    this.userApi.deleteUser(this.userModel.id).then(() => {
      this.router.navigate(['/home/admin/users']);
    });
  }

  public onGetToken(){
    this.authApi.downloadDeviceConfig(this.userModel.id);
  }
}

type DisplayMode = 'EDIT' | 'VIEW' | 'CREATE';