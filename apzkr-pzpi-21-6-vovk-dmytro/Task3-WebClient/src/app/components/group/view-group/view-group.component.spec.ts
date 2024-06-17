import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewGroupComponent } from './view-group.component';

describe('ViewGroupComponent', () => {
  let component: ViewGroupComponent;
  let fixture: ComponentFixture<ViewGroupComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ViewGroupComponent]
    });
    fixture = TestBed.createComponent(ViewGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
