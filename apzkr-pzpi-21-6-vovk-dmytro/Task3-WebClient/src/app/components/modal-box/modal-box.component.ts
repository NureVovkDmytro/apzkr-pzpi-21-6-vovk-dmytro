import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-modal-box',
  templateUrl: './modal-box.component.html',
  styleUrls: ['./modal-box.component.css']
})
export class ModalBoxComponent {
  @Input() 
  public title: string = '';
  @Input()
  public isVisible: boolean = false;

  @Output()
  public onClosed: EventEmitter<void> = new EventEmitter<void>();

  public closeModal(): void {
    this.isVisible = false;
    this.onClosed.emit();
  }
}
