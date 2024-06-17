import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Group, GroupDetail } from 'src/app/models/data/Group';
import { GroupApiService } from 'src/app/services/group-api.service';
import { SearchFilter, TableColumn, TableRecord } from '../../table/table.component';

@Component({
  selector: 'app-group-list',
  templateUrl: './group-list.component.html',
  styleUrls: ['./group-list.component.css']
})
export class GroupListComponent {
  public page = 1;
  public pageSize = 10;
  public total = 0;

  public columns: TableColumn[] = [
    { name: 'models.group.name', type: 'string' },
    { name: 'models.group.description', type: 'string' },
    { name: 'models.group.members', type: 'number' },
    { name: 'models.group.accessLevel', type: 'number' }
  ];

  public searchFilters: SearchFilter[] = [
    { column: 'models.group.name', type: 'string', value: '' },
    { column: 'models.group.accessLevel', type: 'number', value: '' }
  ];

  public data: TableRecord[] = [];

  constructor(private groupApi: GroupApiService, private route: ActivatedRoute, private router: Router) {
    this.route.data.subscribe(data => {
      if (data['mode'] === 'MY') {
        this.loadMyGroups();
      } else {
        this.loadAllGroups();
      }
    });
  }

  public loadMyGroups() {
    this.groupApi.getMyGroups().then(groups => {
      this.data = groups.map(this.groupToRecord);
    });
  }

  public loadAllGroups() {
    this.groupApi.getAllGroups({
      page: this.page,
      limit: this.pageSize,
      name: this.searchFilters.find(f => f.column === 'models.group.name')?.value as string,
      accessLevel: this.searchFilters.find(f => f.column === 'models.group.accessLevel')?.value as number
    }).then(groupsPage => {
      this.data = groupsPage.items.map(this.groupToRecord);
      this.page = groupsPage.page;
      this.pageSize = groupsPage.limit;
      this.total = groupsPage.total;
    });
  }

  public search(filters: SearchFilter[]) {
    this.loadAllGroups();
  }

  private groupToRecord(group: Group): TableRecord {
    return {
      id: group.id,
      values: [
        group.name,
        group.description || '-', // If description is empty, show a dash
        group.memberCount,
        group.accessLevel
      ],
      data: {}
    };
  }

  public onViewInfo(id: string) {
    this.router.navigate(['/home/group', id]);
  }
}
