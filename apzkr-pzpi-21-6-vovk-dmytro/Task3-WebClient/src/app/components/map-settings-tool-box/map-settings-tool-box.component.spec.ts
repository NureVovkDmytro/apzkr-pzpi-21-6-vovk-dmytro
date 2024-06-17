import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MapSettingsToolBoxComponent } from './map-settings-tool-box.component';

describe('MapSettingsToolBoxComponent', () => {
  let component: MapSettingsToolBoxComponent;
  let fixture: ComponentFixture<MapSettingsToolBoxComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MapSettingsToolBoxComponent]
    });
    fixture = TestBed.createComponent(MapSettingsToolBoxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
