import { Injectable, Injector } from "@angular/core";
import { ClientPermissions } from "./models/data/ClientPermission";
import { UserApiService } from "./services/user-api.service";
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from "@angular/router";

const DATA_VALIDATION_FUNCTION = '$permissionValidationFunction';

@Injectable({
    providedIn: 'root'
})
export class PermissionGuard implements CanActivate {

    constructor(private userApiService: UserApiService) {
    }

    async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean | UrlTree> {
        if(!this.userApiService.isAuthenticated){
            return false;
        }
        var user = await this.userApiService.getSelf();
        let func = route.data[DATA_VALIDATION_FUNCTION] as PermissionValidateFunction;
        return func(user.permissions);
    }

    static has(func: PermissionValidateFunction): any {
        let data: any = {};
        data[DATA_VALIDATION_FUNCTION] = func;
        return data;
    }

}
export type PermissionValidateFunction = (permission: ClientPermissions) => boolean;