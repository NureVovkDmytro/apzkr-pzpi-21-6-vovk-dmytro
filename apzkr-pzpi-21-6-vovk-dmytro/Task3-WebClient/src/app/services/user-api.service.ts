import { Injectable } from '@angular/core';
import { BaseApi } from './base-api';
import { UserResponse, UserResponseDetailed } from '../models/responses/user-response';
import { User } from '../models/data/User';
import { TokenResponse } from './auth-api.service';
import { ClientStatus } from '../models/data/Client';
import { PageResponse } from '../models/responses/page-response';
import { ClientPermissions } from '../models/data/ClientPermission';

@Injectable({
  providedIn: 'root'
})
export class UserApiService extends BaseApi {
  private _user: UserResponse = new UserResponse();

  public override get apiUrl(): string {
    return '/users';
  }

  public get user(): UserResponse {
    return this._user;
  }

  constructor() {
    super();
  }

  async getSelf(): Promise<UserResponse> {
    if (this._user.id) {
      return this._user;
    }
    let user = await this._getSelf({ detailed: false });
    this._user = user;
    return user;
  }

  async getSelfDetailed(): Promise<UserResponseDetailed> {
    let user = await this._getSelf({ detailed: true });
    this._user = user as UserResponse;
    return user;
  }

  getById(id: string): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.get<UserResponseDetailed>(id));
  }

  create(user: User): Promise<UserResponseDetailed> {
    user.status = Number(user.status);
    return this.toDataResponse(this.post<UserResponseDetailed>('', user));
  }

  async update(user: User): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>(user.id, user));
  }

  async updateSelfEmail(email: string): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>('self/email', email));
  }

  async updateStatus(id: string, status: ClientStatus): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>(`${id}/status`, Number(status)));
  }

  async updatePassword(id: string, password: string): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>(`${id}/password`, password));
  }

  async changePassword(oldPassword: string, newPassword: string): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>('self/password', { oldPassword, newPassword }));
  }

  async updatePermissions(id: string, permissions: ClientPermissions): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>(`${id}/permissions`, permissions));
  }

  async updateAccessLevel(id: string, accessLevel: number): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>(`${id}/accessLevel`, accessLevel));
  }

  async updateScanRadius(id: string, scanRadius: number): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>(`${id}/scanRadius`, scanRadius));
  }

  async updateLocationInterval(id: string, locationInterval: number): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.put<UserResponseDetailed>(`${id}/updateLocationInterval`, locationInterval));
  }

  async deleteUser(id: string): Promise<void> {
    return this.toDataResponse<any>(this.delete<void>(id));
  }

  async resetPassword(id: string): Promise<void> {
    return this.toDataResponse<any>(this.put<void>(`${id}/resetPassword`, ""));
  }

  async activate(options: ActivateOptions): Promise<TokenResponse> {
    var response = await this.post<TokenResponse>('activate', options);
    var promise = new Promise<TokenResponse>((resolve, reject) => {
      if (response.status !== 200) {
        this.token = null;
        reject(response);
      }
      if (response.body != null) {
        this.token = response.body.token;
        this.tokenExpiresAt = response.body.expiresAt;
        resolve(response.body);
      }
    });
    return promise;
  }

  async getUsers(options: GetUsersOptions) {
    let url = `?Page=${options.page}&Limit=${options.limit}&` +
      `Status=${options.status ?? ''}&FirstName=${options.firstName ?? ''}&`+
      `LastName=${options.lastName ?? ''}&TaxPayerId=${options.taxPayerId ?? ''}&`+
      `Email=${options.email ?? ''}&AccessLevel=${options.accessLevel ?? ''}`;
    return this.toDataResponse(this.get<PageResponse<UserResponseDetailed>>(url));
  }

  private async _getSelf(options: GetSelfOptions): Promise<UserResponseDetailed> {
    return this.toDataResponse(this.get<UserResponseDetailed>(`self?detailed=${options.detailed}`));
  }
}

interface GetSelfOptions {
  detailed: boolean | 'true' | 'false'
}

interface ActivateOptions {
  email: string;
  newPassword: string;
  oldPassword: string;
}

type GetUsersOptions = {
  page: number;
  limit: number;
  status: ClientStatus | undefined | null;
  firstName: string | undefined | null;
  lastName: string | undefined | null;
  taxPayerId: string | undefined | null;
  accessLevel: number | undefined | null;
  email: string | undefined | null;
}