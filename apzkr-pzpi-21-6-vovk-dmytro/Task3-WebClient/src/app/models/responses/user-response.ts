import { ClientStatus, ClientType } from "../data/Client";
import { ClientPermissions } from "../data/ClientPermission";
import { GeoCoordinates } from "../data/GeoCoordinates";

export class UserResponse {
    id: string = '';
    firstName: string = '';
    lastName: string = '';
    nickname: string = '';
    email: string = '';
    taxPayerId: string = '';
    updateLocationSecondsInterval: number = 0;
    createdAt: Date = new Date();
    type: ClientType = ClientType.User;
    permissions: ClientPermissions = new ClientPermissions();
    status: ClientStatus = ClientStatus.Inactive
}

export class UserResponseDetailed extends UserResponse {
    groups: string[] = [];
    lastOnline: Date = new Date();
    accessLevel: number = 0;
    scanRadius: number = 0;
    description: string = '';
    location: GeoCoordinates = new GeoCoordinates();
}
