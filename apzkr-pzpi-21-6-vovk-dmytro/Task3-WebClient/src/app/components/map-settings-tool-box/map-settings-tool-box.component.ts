import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-map-settings-tool-box',
  templateUrl: './map-settings-tool-box.component.html',
  styleUrls: ['./map-settings-tool-box.component.css']
})
export class MapSettingsToolBoxComponent {
  @Input()
  public opened = false;
  @Input()
  public title = '';
  @Input()
  public position: Position = {};

  public get positionStyle(){
    return {
      left: this.position.left || 'auto',
      top: this.position.top || 'auto',
      bottom: this.position.bottom || 'auto',
      right: this.position.right || 'auto'
    };
  }
  toggleList(){
    this.opened = !this.opened;
  }
}

type Position = {
  left?: string;
  top?: string;
  bottom?: string;
  right?: string; 
}
