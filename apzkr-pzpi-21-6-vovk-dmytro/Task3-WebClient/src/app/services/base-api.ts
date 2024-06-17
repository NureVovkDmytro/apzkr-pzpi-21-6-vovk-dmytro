import { environment } from "src/environments/environment";
import { StorageService } from "./storage.service";

export abstract class BaseApi extends StorageService {
    public abstract get apiUrl(): string;
    public get baseUrl(): string {
        return environment.apiUrl + this.apiUrl;
    }
    
    protected get<T>(url: string) {
        let request = fetch(`${this.baseUrl}/${url}`, {
            headers: this.getHeaders()
        });
        return this.toPromise<T>(request);
    }

    protected post<T>(url: string, body: any) {
        let request = fetch(`${this.baseUrl}/${url}`, {
            method: 'POST',
            headers: this.getHeaders(),
            body: JSON.stringify(body)
        });
        return this.toPromise<T>(request);
    }

    protected put<T>(url: string, body: any) {
        let request = fetch(`${this.baseUrl}/${url}`, {
            method: 'PUT',
            headers: this.getHeaders(),
            body: JSON.stringify(body)
        });
        return this.toPromise<T>(request);
    }

    protected delete<T>(url: string) {
        let request = fetch(`${this.baseUrl}/${url}`, {
            method: 'DELETE',
            headers: this.getHeaders()
        });
        return this.toPromise<T>(request);
    }

    private toPromise<T>(func: Promise<Response>): Promise<HttpResponse<T>> {
        return new Promise<HttpResponse<T>>((resolve, reject) => {
            func.then(response => {
                if (response.ok) {
                    response.json().then(body => {
                        resolve({ ok: response.ok, status: response.status, body: body });
                    });
                } else {
                    if (response.headers.get('Content-Type') === 'application/json') {
                        response.json().then(body => {
                            resolve({ ok: response.ok, status: response.status, body: body });
                        });
                    } else {
                        response.text().then(body => {
                            resolve({ ok: response.ok, status: response.status, body: {} as T });
                        });
                    }
                }
                if(response.status === 401) {
                    this.token = null;
                }
            });
        });
    }

    protected toDataResponse<T>(response: Promise<HttpResponse<T>>): Promise<T> {
        return new Promise<T>((resolve, reject) => {
            response.then(res => {
                if (res.ok) {
                    resolve(res.body as T);
                } else {
                    reject(res.body);
                }
            });
        });
    }


    private getHeaders() {
        let headers: [string, string][] = [];
        headers.push(['Content-Type', 'application/json']);
        if (this.token) {
            headers.push(['Authorization', `Bearer ${this.token}`]);
        }
        return headers;
    }
}

export interface HttpResponse<T> {
    ok: boolean;
    status: number;
    body: T | null;
}