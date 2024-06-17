import { RobotResponse } from "../responses/robot-response";
import { ClientStatus, ClientType } from "./Client";
import { GeoCoordinates } from "./GeoCoordinates";

export class Robot {
    id: string = '';
    nickname: string = '';
    description: string = '';
    createdAt: Date = new Date();
    lastOnline: Date = new Date();
    location: GeoCoordinates = new GeoCoordinates();
    type: ClientType = ClientType.Robot;
    status: ClientStatus = ClientStatus.Inactive;
    scanRadius: number = 0;
    accessLevel: number = 0;
    groupId: string = '';
    static fromResponse(response: RobotResponse): Robot {
        let robot = new Robot();
        robot.id = response.id;
        robot.nickname = response.nickname;
        robot.createdAt = new Date(response.createdAt);
        robot.location = response.location ?? new GeoCoordinates();
        robot.type = response.type;
        robot.status = response.status;
        robot.scanRadius = response.scanRadius;
        robot.accessLevel = response.accessLevel;
        robot.groupId = response.groupId;
        return robot;
    }
}