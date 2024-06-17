import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SearchFilter, TableColumn, TableRecord, TableRecordAction } from '../../table/table.component';
import { ClientStatus, ClientType } from 'src/app/models/data/Client';
import { GroupApiService } from 'src/app/services/group-api.service';
import { UserApiService } from 'src/app/services/user-api.service';

@Component({
  selector: 'app-group-member-list',
  templateUrl: './group-member-list.component.html',
  styleUrls: ['./group-member-list.component.css']
})
export class GroupMemberListComponent {

  public page: number = 1;
  public limit: number = 100;

  public groupId: string = '';
  public selectedUsers: string[] = [];
  public deleteUsers: string[] = [];
  public get currentLocation(): string {
    return window.location.pathname;
  }

  public columns: TableColumn[] = [
    { name: 'models.user.firstName', type: 'string' },
    { name: 'models.user.lastName', type: 'string' },
    { name: 'models.client.nickname', type: 'string' },
    { name: 'models.user.email', type: 'string' },
    { name: 'models.user.taxPayerId', type: 'string' },
    { name: 'models.client.lastOnline', type: 'date' },
    { name: 'models.client.accessLevel', type: 'number' },
    { name: 'models.client.status', type: 'string', translate: true }
  ];

  public actions: TableRecordAction[] = [
    {
      name: 'table.view',
      style: 'primary',
      type: 'button',
      event: this.onViewInfo.bind(this)
    },
    {
      name: 'buttons.delete',
      style: 'danger',
      type: 'button',
      event: this.deleteMember.bind(this)
    }
  ];

  public filters: SearchFilter[] = [
    { column: 'models.user.firstName', type: 'string', value: '' },
    { column: 'models.user.lastName', type: 'string', value: '' },
    { column: 'models.client.nickname', type: 'string', value: '' },
    { column: 'models.user.email', type: 'string', value: '' },
    { column: 'models.user.taxPayerId', type: 'string', value: '' },
    { column: 'models.client.accessLevel', type: 'number', value: 0, defaultValue: 0 },
    {
      column: 'models.client.type', defaultValue: 'models.clientTypes.any', type: 'select', value: '', selectOptions: [
        { value: undefined, label: 'models.clientTypes.any', selected: true },
        { value: ClientType.User, label: 'models.clientTypes.user' },
        { value: ClientType.Robot, label: 'models.clientTypes.robot' }
      ]
    },
    {
      column: 'models.client.status', defaultValue: 'models.clientStatuses.any', type: 'select', value: '', selectOptions: [
        { value: undefined, label: 'models.clientStatuses.any', selected: true },
        { value: ClientStatus.Active, label: 'models.clientStatuses.active' },
        { value: ClientStatus.Inactive, label: 'models.clientStatuses.inactive' },
        { value: ClientStatus.Limited, label: 'models.clientStatuses.limited' },
        { value: ClientStatus.Banned, label: 'models.clientStatuses.banned' }
      ]
    }
  ];

  public data: TableRecord[] = [];

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private groupApi: GroupApiService, private userApi: UserApiService) {
    let state = this.router.getCurrentNavigation()?.extras.state;
    if (state) {
      this.selectedUsers = state['selected'] as string[];
      this.deleteUsers = state['deleted'] as string[];
    }

    this.activatedRoute.params.subscribe(params => {
      this.groupId = params['id'];
      let deleteUserPromises = this.deleteUsers.map(user => this.groupApi.deleteMember(this.groupId, user));
      Promise.all(deleteUserPromises).finally(() => {
        let addSelectedUserPromises = this.selectedUsers.map(user => this.groupApi.addMember(this.groupId, user));
        Promise.all(addSelectedUserPromises).finally(() => {
          this.loadMembers();
        });
      });
    });
  }

  public addMember(): void {
    this.router.navigate(['/home/admin/users'], {
      queryParams: {
        selectMode: true,
        multiple: true,
        returnTo: this.currentLocation
      },
      state: {
        selected: this.data.map(user => user.id)
      }
    });
  }

  public onViewInfo(id: string): void {
    this.router.navigate(['/home/admin/users', id], { queryParams: { returnTo: this.currentLocation } });
  }

  public back(): void {
    this.router.navigate(['/home/group', this.groupId]);
  }

  public loadMembers(): void {
    let options = {
      page: this.page,
      limit: this.limit,
      firstName: this.filters.find(filter => filter.column == 'models.user.firstName')?.value as string,
      lastName: this.filters.find(filter => filter.column == 'models.user.lastName')?.value as string,
      nickname: this.filters.find(filter => filter.column == 'models.client.nickname')?.value as string,
      email: this.filters.find(filter => filter.column == 'models.user.email')?.value as string,
      taxPayerId: this.filters.find(filter => filter.column == 'models.user.taxPayerId')?.value as string,
      accessLevel: this.filters.find(filter => filter.column == 'models.client.accessLevel')?.value as number,
      type: this.filters.find(filter => filter.column == 'models.client.type')?.value as ClientType,
      status: this.filters.find(filter => filter.column == 'models.client.status')?.value as ClientStatus
    }
    this.groupApi.getMembers(this.groupId, options).then(response => {

      this.data = response.items.map(client => {
        let status = 'models.clientStatuses.' + ClientStatus[client.status].toLowerCase();
        return {
          id: client.id,
          values: [client.firstName || '-', client.lastName || '-', client.nickname || '-', client.email || '-', client.taxPayerId || '-', client.lastOnline || '-', client.accessLevel, status],
          data: {
            'buttons.delete': {
              disabled: client.id == this.userApi.user.id
            }
          }
        };
      });
    });
  }

  private deleteMember(id: string): void {
    this.groupApi.deleteMember(this.groupId, id).then(() => {
      this.loadMembers();
    });
  }
}
