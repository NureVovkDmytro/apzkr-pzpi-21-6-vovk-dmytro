import { Injectable } from "@angular/core";
import { CanActivate, CanActivateChild, CanDeactivate, CanLoad, Router } from "@angular/router";
import { AuthApiService } from "./services/auth-api.service";

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild, CanLoad {
    constructor(private authService: AuthApiService, private router: Router) { }

    canActivate(): boolean {
        return this.checkAuth();
    }

    canActivateChild(): boolean {
        return this.checkAuth();
    }

    canLoad(): boolean {
        return this.checkAuth();
    }

    private checkAuth(): boolean {
        if (this.authService.isAuthenticatedUser) {
            return true;
        } else {
            this.router.navigate(['/login']);
            return false;
        }
    }

}