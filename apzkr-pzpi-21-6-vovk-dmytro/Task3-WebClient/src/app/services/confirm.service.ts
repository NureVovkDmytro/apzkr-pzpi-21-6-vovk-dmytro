import { Injectable } from '@angular/core';
import { BaseApi } from './base-api';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService extends BaseApi {
  public override get apiUrl(): string {
    return '/confirm';
  }

  public async confirm(token: string, type: string): Promise<void> {
    await this.get(`?token=${token}&type=${type}`);
  }
}
