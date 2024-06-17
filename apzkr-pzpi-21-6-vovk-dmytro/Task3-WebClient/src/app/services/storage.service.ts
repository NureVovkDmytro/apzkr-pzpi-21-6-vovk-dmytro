import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class StorageService {
    private _isAuthenticated: boolean | null = null;
    private storage: Storage = localStorage;

    public get authSecretKey(): string {
        return 'BearerToken';
    }
    
    public get tokenExpiresAtKey(): string {
        return 'TokenExpiresAt';
    }

    public get token(): string | null {
        return this.getItem(this.authSecretKey);
    }

    protected set token(value: string | null) {
        if (value) {
            this._isAuthenticated = true;
            this.setItem(this.authSecretKey, value);
        } else {
            this._isAuthenticated = false;
            this.removeItem(this.authSecretKey);
        }
    }

    public get tokenExpiresAt(): number | null {
        let value = this.getItem(this.tokenExpiresAtKey);
        if (value) {
            return Number(value)
        }
        return null;
    }

    protected set tokenExpiresAt(value: number | null) {
        if (value) {
            this.setItem(this.tokenExpiresAtKey, value.toString());
        } else {
            this.removeItem(this.tokenExpiresAtKey);
        }
    }
    public get isAuthenticated(): boolean {
        if (this._isAuthenticated === null) {
            this._isAuthenticated = this.token !== null && this.tokenExpiresAt !== null && this.tokenExpiresAt > Math.floor((new Date()).getTime() / 1000)
        }
        return this._isAuthenticated;
    }

    public setItem(key: string, value: string) {
        this.storage.setItem(key, value);
    }

    public getItem(key: string): string | null {
        return this.storage.getItem(key);
    }

    public removeItem(key: string) {
        this.storage.removeItem(key);
    }

    public clear() {
        this.storage.clear();
    }
}