import { Component, OnInit } from '@angular/core';
import { SearchFilter, TableColumn, TableRecord, TableRecordAction } from '../../../table/table.component';
import { ActivatedRoute, Router } from '@angular/router';
import { UserApiService } from 'src/app/services/user-api.service';
import { ClientStatus } from 'src/app/models/data/Client';
import { AuthApiService } from 'src/app/services/auth-api.service';
import { Alert, AlertsService } from 'src/app/services/alerts.service';

@Component({
  selector: 'app-user-list-administration',
  templateUrl: './user-list-administration.component.html',
  styleUrls: ['./user-list-administration.component.css']
})
export class UserListAdministrationComponent implements OnInit {
  public selectMode = {
    enabled: false,
    selected: [] as string[],
    deleted: [] as string[],
    defaultSelected: [] as string[],
    multiple: false
  };

  public returnTo: string = '';

  public page: number = 1;
  public limit: number = 10;
  public columns: TableColumn[] = [
    { name: 'models.user.firstName', type: 'string' },
    { name: 'models.user.lastName', type: 'string' },
    { name: 'models.client.nickname', type: 'string' },
    { name: 'models.user.email', type: 'string' },
    { name: 'models.user.taxPayerId', type: 'string' },
    { name: 'models.client.accessLevel', type: 'number' },
    { name: 'models.client.status', type: 'string', translate: true }
  ];

  public searchFilters: SearchFilter[] = [
    { column: 'models.user.firstName', type: 'string', value: '' },
    { column: 'models.user.lastName', type: 'string', value: '' },
    { column: 'models.user.email', type: 'string', value: '' },
    { column: 'models.user.taxPayerId', type: 'string', value: '' },
    { column: 'models.client.accessLevel', type: 'number', value: 0 },
    {
      column: 'models.client.status', type: 'select', value: -1, selectOptions: [
        { value: -1, label: 'models.clientStatuses.any', selected: true },
        { value: ClientStatus.Active, label: 'models.clientStatuses.active' },
        { value: ClientStatus.Inactive, label: 'models.clientStatuses.inactive' },
        { value: ClientStatus.Limited, label: 'models.clientStatuses.limited' },
        { value: ClientStatus.Banned, label: 'models.clientStatuses.banned' }
      ]
    }
  ];

  public tableActions: TableRecordAction[] = [
    {
      name: 'table.view',
      style: 'primary',
      type: 'button',
      event: this.onViewInfo.bind(this)
    }
  ];

  public data: TableRecord[] = [];

  constructor(private router: Router, private userApi: UserApiService, public authApi: AuthApiService, private activatedRoute: ActivatedRoute, private alertsService: AlertsService) {
    this.activatedRoute.queryParams.subscribe(param => {
      this.selectMode.enabled = param['selectMode'] == 'true';
      this.selectMode.multiple = param['multiple'] == 'true';
      this.returnTo = param['returnTo'] || '';
      if (this.selectMode.enabled) {
        this.tableActions = [];
        this.tableActions.push({
          name: 'table.select',
          style: 'primary',
          type: 'checkbox',
          event: this.onSelect.bind(this)
        });
      }
    });
    let state = this.router.getCurrentNavigation()?.extras.state;
    let selectedUsers = state?.['selected'] as string[];
    if (selectedUsers) {
      this.selectMode.defaultSelected = selectedUsers;
    }
    this.userApi.getSelf().then(response => {
      if(response.permissions.users.canCreateDeviceToken){
        this.tableActions.push(    {
          name: 'userList.downloadDeviceConfig',
          style: 'info',
          type: 'button',
          event: this.downloadDeviceConfig.bind(this)
        });
      }
    });
  }

  ngOnInit(): void {
    this.load();
  }

  public onViewInfo(id: string) {
    this.router.navigate(['/home/admin/users', id]);
  }

  public onSearch(filters: SearchFilter[]) {
    this.load();
  }

  public onSelect(id: string, event: any) {
    if (event.target.checked) {
      if (this.selectMode.deleted.includes(id)) {
        this.selectMode.defaultSelected.push(id);
        this.selectMode.deleted = this.selectMode.deleted.filter(s => s != id);
      } else {
        this.selectMode.selected.push(id);
      }
    } else {
      if (this.selectMode.defaultSelected.includes(id)) {
        this.selectMode.deleted.push(id);
        this.selectMode.defaultSelected = this.selectMode.defaultSelected.filter(s => s != id);
      }
      this.selectMode.selected = this.selectMode.selected.filter(s => s != id);
    }
  }

  public load() {
    let status: number | undefined = this.searchFilters.find(f => f.column == 'models.client.status')?.value as number;
    if (status == -1) {
      status = undefined;
    }
    let query = {
      page: this.page,
      limit: this.limit,
      firstName: this.searchFilters.find(f => f.column == 'models.user.firstName')?.value as string,
      lastName: this.searchFilters.find(f => f.column == 'models.user.lastName')?.value as string,
      email: this.searchFilters.find(f => f.column == 'models.user.email')?.value as string,
      taxPayerId: this.searchFilters.find(f => f.column == 'models.user.taxPayerId')?.value as string,
      accessLevel: this.searchFilters.find(f => f.column == 'models.client.accessLevel')?.value as number,
      status: status
    };

    this.userApi.getUsers(query).then(users => {
      this.data = users.items.map(user => {
        let status = 'models.clientStatuses.' + ClientStatus[user.status].toLowerCase();
        return {
          id: user.id,
          values: [user.firstName, user.lastName, user.nickname, user.email, user.taxPayerId, user.accessLevel, status],
          data: {
            'table.select': {
              checked: this.selectMode.selected.includes(user.id) || this.selectMode.defaultSelected.includes(user.id)
            }
          }
        };
      });
    });
  }

  public cancel() {
    this.router.navigate([this.returnTo]);
  }

  public confirm() {
    this.router.navigate([this.returnTo], { state: { selected: this.selectMode.selected, deleted: this.selectMode.deleted } });
  }

  private downloadDeviceConfig(id: string){
    this.authApi.downloadDeviceConfig(id).then(() => {
    }).catch(() => {
      this.alertsService.add(new Alert('danger', 'userList.downloadDeviceConfigFailed'));
    });
  }
}
