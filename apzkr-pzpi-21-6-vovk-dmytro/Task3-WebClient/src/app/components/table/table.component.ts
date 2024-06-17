import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css']
})
export class TableComponent {
  @Input()
  columns: TableColumn[] = [];
  @Input()
  searchFilters: SearchFilter[] = [];
  @Input()
  hasNextPage: boolean = false;
  @Input()
  isLoading: boolean = false;
  @Input()
  data: TableRecord[] = [];
  @Input()
  enableSearch: boolean = true;
  @Input()
  actions: TableRecordAction[] = [
    {
      name: 'table.view',
      style: 'primary',
      type: 'button',
      event: this.viewInfoClick.bind(this)
    }
  ];
  @Output()
  onSearch: EventEmitter<SearchFilter[]> = new EventEmitter<SearchFilter[]>();
  @Output()
  onLoadMore: EventEmitter<void> = new EventEmitter<void>();
  @Output()
  onViewInfo: EventEmitter<string> = new EventEmitter<string>();

  public searchClick() {
    this.onSearch.emit(this.searchFilters);
  }

  public resetClick() {
    this.searchFilters.forEach(filter => {
      if (filter.defaultValue !== undefined) {
        filter.value = filter.defaultValue;
      } else {
        filter.value = '';
      }
    });
    this.onSearch.emit(this.searchFilters);
  }

  public loadMoreClick() {
    this.isLoading = true;
    this.onLoadMore.emit();
  }

  public viewInfoClick(id: string) {
    this.onViewInfo.emit(id);
  }

  getColumnById(id: number): TableColumn | undefined {
    return this.columns[id];
  }
}

export type TableColumn = {
  name: string;
  type: 'string' | 'number' | 'date' | 'boolean';
  translate?: boolean;
}

export type TableRecord = {
  id: string;
  values: (string | number | Date | boolean)[];
  data: any;
}

export type SelectOption = {
  value: any;
  label: string;
  selected?: boolean;
}

export type SearchFilter = {
  column: string;
  type: 'string' | 'number' | 'date' | 'boolean' | 'select' | 'checkbox' | 'radio';
  value: string | number | Date | boolean;
  selectOptions?: SelectOption[];
  defaultValue?: string | number | boolean;
  placeholder?: string;
}

export type TableRecordAction = {
  name: string;
  type: 'button' | 'checkbox' | 'radio';
  style: 'primary' | 'secondary' | 'danger' | 'warning' | 'info' | 'light' | 'dark' | undefined;
  event: (id: string, event: any) => void;
}