import { Injectable } from '@angular/core';
import { BaseApi } from './base-api';
import { UserResponse } from '../models/responses/user-response';
import { ClientPermissions } from '../models/data/ClientPermission';

@Injectable({
    providedIn: 'root'
})
export class AuthApiService extends BaseApi {
    public override get apiUrl(): string {
        return '/auth';
    }

    public get isAuthenticatedUser(): boolean {
        return this.isAuthenticated;
    }

    constructor() {
        super();
    }

    async login(data: LoginRequest) {
        let response = await this.post<TokenResponse>('login', data);
        if (response.ok && response.body != null) {
            this.token = response.body.token;
            this.tokenExpiresAt = response.body.expiresAt;
        } else {
            localStorage.removeItem(this.authSecretKey);
        }
        return response;
    }

    logout() {
        this.token = null;
    }

    async downloadDeviceConfig(id: string) {
        let response = await this.get(`device/token?userId=${id}`);
        if (response.ok) {
            let a = document.createElement('a');
            a.style.display = 'none';
            let blob = new Blob([JSON.stringify(response.body)], { type: 'application/json' });
            a.href = URL.createObjectURL(blob);
            a.download = `device_${id}.json`;

            document.body.appendChild(a);
            a.click();

            document.body.removeChild(a);
            return Promise.resolve();
        } else {
            return Promise.reject(response);
        }
    }

    refreshToken() {
        return this.post<UserResponse>('refresh', {});
    }
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    permissions: ClientPermissions;
    groupId: string;
}

export interface TokenResponse {
    token: string;
    expiresAt: number;
    createAt: number;
}