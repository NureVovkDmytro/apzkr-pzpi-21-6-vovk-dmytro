import { GeoCoordinates } from "./GeoCoordinates";

export class Marker {
    public id: string = '';
    public name: string = '';
    public description: string = '';
    public icon: string | null = null;
    public location: GeoCoordinates = new GeoCoordinates();
    public radius: number = 0;
    public groupId: string = '';
}