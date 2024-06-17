import { ClientStatus, ClientType } from "../data/Client";
import { GeoCoordinates } from "../data/GeoCoordinates";

export class RobotTokenResponse {
    id: string = '';
    key: string = '';
    token: string = '';
}

export class RobotResponse {
    id: string = '';
    nickname: string = '';
    createdAt: Date = new Date();
    type: ClientType = ClientType.Robot;
    updateLocationSecondsInterval: number = 0;
    status: ClientStatus = ClientStatus.Inactive;
    scanRadius: number = 0;
    location?: GeoCoordinates = new GeoCoordinates();
    accessLevel: number = 0;
    groupId: string = '';
}