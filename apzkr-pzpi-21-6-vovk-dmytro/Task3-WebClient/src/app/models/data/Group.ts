import { GeoCoordinates } from "./GeoCoordinates";

export class Group {
    id: string = '';
    name: string = '';
    description: string = '';
    createAt: Date = new Date();
    accessLevel: number = 0;
    memberCount: number = 0;
    responsibilityArea: GeoCoordinates[] = [];
    markers: string[] = [];
}

export class GroupDetail extends Group {
    members: string[] = [];
    admins: string[] = [];
}