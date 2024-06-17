import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RobotAdministrationComponent } from './robot-administration.component';

describe('RobotAdministrationComponent', () => {
  let component: RobotAdministrationComponent;
  let fixture: ComponentFixture<RobotAdministrationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RobotAdministrationComponent]
    });
    fixture = TestBed.createComponent(RobotAdministrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
