
import { LatLng } from "leaflet"

export default class GeometryTools {
    public static area(latlngs: LatLng | LatLng[] | LatLng[][]): number {
        let area = 0;

        let points = (latlngs as LatLng[]).map(this.degreesToMeters);

        if (points.length > 2) {
            for (let i = 0; i < points.length; i++) {
                let j = (i + 1) % points.length;
                let point1 = points[i];
                let point2 = points[j];
                area += (point1.x - point2.x) * (point1.y - point2.y);
            }
            area = Math.abs(area / 2);
        }
        return area;
    }

    public static degreesToMeters(latlng: LatLng): { x: number, y: number } {
        var earthRadius = 6378137; // Earth's radius in meters
        var lat = latlng.lat * Math.PI / 180;
        return {
            x: latlng.lng * Math.PI * earthRadius / 180,
            y: Math.log(Math.tan((Math.PI / 4) + (lat / 2))) * earthRadius
        };
    }
    
}