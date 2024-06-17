import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SearchFilter, TableColumn, TableRecord, TableRecordAction } from 'src/app/components/table/table.component';
import { ClientStatus } from 'src/app/models/data/Client';
import { RobotApiService } from 'src/app/services/robot-api.service';
import { UserApiService } from 'src/app/services/user-api.service';

@Component({
  selector: 'app-robot-list-administration',
  templateUrl: './robot-list-administration.component.html',
  styleUrls: ['./robot-list-administration.component.css']
})
export class RobotListAdministrationComponent implements OnInit {
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
    { name: 'models.client.nickname', type: 'string' },
    { name: 'models.client.accessLevel', type: 'number' },
    { name: 'models.client.status', type: 'string', translate: true }
  ];

  public searchFilters: SearchFilter[] = [
    { column: 'models.client.nickname', type: 'string', value: '' },
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
    },
    {
      name: 'admin.robot.getToken',
      style: 'primary',
      type: 'button',
      event: this.onGetToken.bind(this)
    }
  ];

  public data: TableRecord[] = [];

  constructor(private router: Router, private userApi: UserApiService, private robotApi: RobotApiService, private activatedRoute: ActivatedRoute) {
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
  }

  ngOnInit(): void {
    this.load();
  }

  public onViewInfo(id: string) {
    this.router.navigate(['/home/admin/robots', id]);
  }

  public onGetToken(id: string) {
    this.robotApi.getRobotToken(id);
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
      accessLevel: this.searchFilters.find(f => f.column == 'models.client.accessLevel')?.value as number,
      nickname: this.searchFilters.find(f => f.column == 'models.client.nickname')?.value as string,
      status: status
    };

    this.robotApi.getRobots(query).then(robots => {
      this.data = robots.items.map(robot => {
        let status = 'models.clientStatuses.' + ClientStatus[robot.status].toLowerCase();
        return {
          id: robot.id,
          values: [robot.nickname, robot.accessLevel, status],
          data: {
            'table.select': {
              checked: this.selectMode.selected.includes(robot.id) || this.selectMode.defaultSelected.includes(robot.id)
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
}
