import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { StorageService } from './storage.service';
import { environment } from 'src/environments/environment';
import { GeoCoordinates } from '../models/data/GeoCoordinates';
import { Alert, AlertsService } from './alerts.service';
import { MapSignalREvents } from '../components/map/map.signalrEvents';
import { ResourceService } from './resource.service';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private connection: signalR.HubConnection | null = null;
  private warnMessage: Alert | null = null;
  private _eventHandler: MapSignalREvents = new MapSignalREvents(this);

  public get eventHandler(): MapSignalREvents {
    return this._eventHandler;
  }

  public get isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }

  constructor(private storage: StorageService, private alertsService: AlertsService, public resourceService: ResourceService) { }

  public async connect() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalRUrl, {
        accessTokenFactory: () => {
          return this.storage.token ?? '';
        }
      })
      .build();
    try {
      await this.connection.start();
      console.log('SignalR connection started');
    } catch (error) {
      console.error('SignalR connection failed', error);
    }
    this.connection?.on('warning', (response: { message: string, details: string }) => {
      if (this.warnMessage == null) {
        this.warnMessage = this.alertsService.add(new Alert('warning', `SignalR: ${response.message}`, {
          lifetime: 0,
          index: 999_999_999,
          onclose: (alert: Alert) => {
            this.warnMessage = null;
            console.info('Message removed', this.warnMessage);
          }
        }));
        console.info('Message added', this.warnMessage);
      }
      this.warnMessage.message = `SignalR: ${response.message}`;
    });

    this.connection.onclose((err) => {
      console.log('SignalR connection closed', err);
    });
  }
  public on(methodName: string, newMethod: (...args: any[]) => any) {
    this.connection?.on(methodName, newMethod);
  }

  public off(methodName: string, method: (...args: any[]) => any) {
    this.connection?.off(methodName, method);
  }

  public invoke(methodName: string, ...args: any[]) {
    if (!this.isConnected) {
      return Promise.reject('SignalR is not connected');
    }
    return this.connection?.invoke(methodName, ...args);
  }
}
