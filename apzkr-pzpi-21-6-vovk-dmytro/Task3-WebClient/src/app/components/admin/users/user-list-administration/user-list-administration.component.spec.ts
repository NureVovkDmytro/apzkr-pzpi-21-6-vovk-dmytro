import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserListAdministrationComponent } from './user-list-administration.component';

describe('UserListAdministrationComponent', () => {
  let component: UserListAdministrationComponent;
  let fixture: ComponentFixture<UserListAdministrationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [UserListAdministrationComponent]
    });
    fixture = TestBed.createComponent(UserListAdministrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
