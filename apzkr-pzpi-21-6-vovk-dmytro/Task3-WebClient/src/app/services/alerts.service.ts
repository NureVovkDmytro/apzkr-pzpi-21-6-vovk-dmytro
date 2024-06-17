import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AlertsService {
  private alerts: AlertRecord[] = [];
  constructor() { 
    console.warn('AlertsService created');
  }

  public get list(): Alert[] {
    return this.alerts.sort((a, b) => a.index - b.index);
  }

  public add(alert: Alert) {
    const alertRecord = new AlertRecord(alert);
    alertRecord.lifetime = alertRecord.lifetime ?? 5000;
    if (alertRecord.lifetime > 0) {
      alertRecord.timer = window.setTimeout(() => {
        this.remove(alertRecord);
      }, alertRecord.lifetime);
    }
    this.alerts.push(alertRecord);

    return alertRecord;
  }

  public get(id: string): Alert | undefined {
    return this.alerts.find(a => a.id === id);
  }

  public remove(alert: Alert | string) {
    const id = typeof alert === 'string' ? alert : alert.id;
    const index = this.alerts.findIndex(a => a.id === id);
    if (index >= 0) {
      const alertRecord = this.alerts[index];
      if (alertRecord.timer) {
        clearTimeout(alertRecord.timer);
      }
      if (alertRecord.onclose) {
        alertRecord.onclose(alertRecord);
      }
      this.alerts.splice(index, 1);
    }
  }

  public removeAll() {
    this.alerts.forEach(a => {
      if (a.timer) {
        clearTimeout(a.timer);
      }
    });
    this.alerts = [];
  }
}

export type AlertType = 'success' | 'info' | 'warning' | 'danger';

export class Alert {
  private _id: string = Math.random().toString(36).substring(7);
  private _created: Date = new Date();
  private _updateAt: Date = new Date();
  private _message: string = '';
  public get id(): string {
    return this._id;
  }
  public get created(): Date {
    return this._created;
  }
  public get updateAt(): Date {
    return this._updateAt;
  }
  public index: number = 0;
  public type: AlertType;
  public get message(): string {
    return this._message;
  }
  public set message(value: string) {
    this._message = value;
    this._updateAt = new Date();
  }
  /**
   * If the message is HTML.
   * @default false
   */
  isHtml: boolean = false;
  /**
   * Lifetime in milliseconds
   * @default 5000
   */
  lifetime: number = 5000;
  data: any = null;
  onclick?: (caller: Alert) => void;
  onclose?: (caller: Alert) => void;

  constructor(type: AlertType, message: string, options?: AlertOptions) {
    this.type = type;
    this.message = message;
    if (options) {
      this.lifetime = options.lifetime ?? 5000;
      this.data = options.data ?? null;
      this.index = options.index ?? 0;
      this.onclick = options.onclick;
      this.onclose = options.onclose;
    }
  }
}

export type AlertOptions = {
  lifetime?: number;
  data?: any;
  index?: number;
  onclick?: (caller: Alert) => void;
  onclose?: (caller: Alert) => void;
};

class AlertRecord extends Alert {
  public timer: number | null = null;
  constructor(alert: Alert) {
    super(alert.type, alert.message, {
      lifetime: alert.lifetime,
      data: alert.data,
      onclick: alert.onclick,
      onclose: alert.onclose
    });
  }
}