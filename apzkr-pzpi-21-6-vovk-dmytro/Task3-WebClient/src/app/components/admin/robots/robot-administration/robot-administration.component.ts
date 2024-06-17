import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Group } from 'src/app/models/data/Group';
import { Robot } from 'src/app/models/data/Robot';
import { RobotResponse } from 'src/app/models/responses/robot-response';
import { UserResponse } from 'src/app/models/responses/user-response';
import { GroupApiService } from 'src/app/services/group-api.service';
import { RobotApiService } from 'src/app/services/robot-api.service';
import { UserApiService } from 'src/app/services/user-api.service';

@Component({
  selector: 'app-robot-administration',
  templateUrl: './robot-administration.component.html',
  styleUrls: ['./robot-administration.component.css']
})
export class RobotAdministrationComponent {
  public displayMode: DisplayMode = 'VIEW';
  public buttonLabel: string = 'buttons.save';
  public returnTo: string = '/home/admin/robots';
  public robotModel: Robot = new Robot();

  public groupSearch: string = '';
  public groups: Group[] = [];
  public selectedGroup: Group | null = null;

  public get isEditMode(): boolean {
    return this.displayMode === 'EDIT' && !this.isSelfUser;
  }

  public get isSelfUser(): boolean {
    return this.robotModel.id == this.currentUser.id;
  }
  
  public get currentUser(): UserResponse {
    return this.userApi.user;
  }

  private oldRobot = {
    baseInformation: '',
    accessLevel: '',
    scanRadius: '',
    status: ''
  };

  constructor(protected translate: TranslateService, protected userApi: UserApiService, protected robotApi: RobotApiService, protected groupApi: GroupApiService, protected router: Router, protected activatedRoute: ActivatedRoute) {
    if (this.activatedRoute.snapshot.data['mode'] == 'CREATE') {
      this.displayMode = 'CREATE';
      this.buttonLabel = 'buttons.create';
    } else {
      this.activatedRoute.params.subscribe(params => {
        this.robotModel.id = params['id'];
        this.displayMode = 'EDIT';
        this.buttonLabel = 'buttons.save';
        this.loadUserData();
      });
    }
    this.activatedRoute.queryParams.subscribe(params => {
      this.returnTo = params['returnTo'] || this.returnTo;
    });
    this.groupApi.getAllGroups({ page: 1, limit: 10, name: undefined, accessLevel: undefined }).then(groups => {
      this.groups = groups.items;
    });
  }

  loadUserData() {
    this.robotApi.getById(this.robotModel.id)
      .then(robot => {
        this.robotModel = Robot.fromResponse(robot);
        this.oldRobot.baseInformation = JSON.stringify({
          nickname: this.robotModel.nickname,
          description: this.robotModel.description
        });
        this.oldRobot.accessLevel = JSON.stringify(this.robotModel.accessLevel);
        this.oldRobot.scanRadius = JSON.stringify(this.robotModel.scanRadius);
        this.oldRobot.status = JSON.stringify(this.robotModel.status);
        if(this.robotModel.groupId != ''){
          this.groupApi.getGroup(this.robotModel.groupId).then(group => {
            this.selectedGroup = group;
          });
        }
      }).catch(() => {
        this.router.navigate([this.returnTo]);
      });
  }

  searchGroups(){
    this.groupApi.getAllGroups({ page: 1, limit: 10, name: this.groupSearch, accessLevel: undefined }).then(groups => {
      this.groups = groups.items;
    });
  }

  public selectGroup(group: Group) {
    this.selectedGroup = group;
    this.robotModel.groupId = group.id;
  }

  submit() {
    if (this.displayMode == 'CREATE' && this.userApi.user.permissions.users.canCreate) {
      this.createRobot();
    } else {
      this.updateRobot();
    }
  }

  private createRobot() {
    this.robotApi.create(this.robotModel).then(user => {
      this.router.navigate(['/home/admin/robots', user.id]);
    });
  }

  private updateRobot() {
    const taskStack = new Array<Promise<RobotResponse>>();
    // Need fix API.

    if (this.currentUser.permissions.users.canUpdateBaseInformation && this.oldRobot.baseInformation != JSON.stringify({
      nickname: this.robotModel.nickname,
      description: this.robotModel.description
    })) {
      taskStack.push(this.robotApi.update(this.robotModel));
    }

    if (this.currentUser.permissions.users.canUpdateAccessLevel && this.oldRobot.accessLevel != JSON.stringify(this.robotModel.accessLevel)) {
      taskStack.push(this.robotApi.updateAccessLevel(this.robotModel.id, this.robotModel.accessLevel));
    }

    if (this.currentUser.permissions.users.canUpdateScanRadius && this.oldRobot.scanRadius != JSON.stringify(this.robotModel.scanRadius)) {
      taskStack.push(this.robotApi.updateScanRadius(this.robotModel.id, this.robotModel.scanRadius));
    }

    if (this.currentUser.permissions.users.canUpdateStatus && this.oldRobot.status != JSON.stringify(this.robotModel.status)) {
      taskStack.push(this.robotApi.updateStatus(this.robotModel.id, this.robotModel.status));
    }

    if (taskStack.length == 0) {
      this.router.navigate(['/home/admin/robots', this.robotModel.id]);
    } else {
      Promise.all(taskStack).then((results) => {
        let user = results[0];
        this.router.navigate(['/home/admin/robots', user.id]);
      });
    }
  }

  public cancel() {
    this.router.navigate([this.returnTo]);
  }

  public deleteRobot() {
    if (!this.currentUser.permissions.users.canDelete) {
      return;
    }
    this.robotApi.deleteRobot(this.robotModel.id).then(() => {
      this.router.navigate(['/home/admin/robots']);
    });
  }

  public onGetToken(){
    this.robotApi.getRobotToken(this.robotModel.id);
  }
}

type DisplayMode = 'EDIT' | 'VIEW' | 'CREATE';
