import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupMemberListComponent } from './group-member-list.component';

describe('GroupMemberListComponent', () => {
  let component: GroupMemberListComponent;
  let fixture: ComponentFixture<GroupMemberListComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GroupMemberListComponent]
    });
    fixture = TestBed.createComponent(GroupMemberListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
