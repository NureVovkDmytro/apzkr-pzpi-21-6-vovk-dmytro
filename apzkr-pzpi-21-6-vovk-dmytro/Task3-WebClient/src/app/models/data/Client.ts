import { ClientPermissions } from "./ClientPermission";
import { GeoCoordinates } from "./GeoCoordinates";

export enum ClientStatus{
    Inactive = 0,
    Active = 0x1,
    Limited = 0x2,
    Banned = 0x4,
}

export enum ClientType{
    User = 0,
    Robot = 0x2,
}

export class ClientSession {
    id: string = '';
    lastUpdated: string = '';
    lastAccessed: string = '';
    expiresAt: string = '';
}

export class Client{
    id: string = '';
    nickname: string = '';
    description: string = '';
    createdAt: Date = new Date();
    lastOnline: Date = new Date();
    location: GeoCoordinates = new GeoCoordinates();
    type: ClientType = ClientType.User;
    status: ClientStatus = ClientStatus.Inactive;
    scanRadius: number = 0;
    sessions: ClientSession[] = [];
    groups: string[] = [];
    accessLevel: number = 0;
    permissions: ClientPermissions = new ClientPermissions();
}