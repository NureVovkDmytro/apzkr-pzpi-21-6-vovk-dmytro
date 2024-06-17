import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RobotListAdministrationComponent } from './robot-list-administration.component';

describe('RobotListAdministrationComponent', () => {
  let component: RobotListAdministrationComponent;
  let fixture: ComponentFixture<RobotListAdministrationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RobotListAdministrationComponent]
    });
    fixture = TestBed.createComponent(RobotListAdministrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
