import { Marker } from "src/app/models/data/Marker";
import { BaseHandler, HandlerOptions } from "./BaseHandler";
import * as L from 'leaflet';
import { SignalRService } from "src/app/services/signal-r.service";

export class MarkUpdatedHandler extends BaseHandler {
    public get command(): string {
        return 'markUpdated';
    }
    
    public execute(options: HandlerOptions): void {
        let response = options.request as Marker;
        let marker = options.map.getPane('markerPane')?.querySelector(`.marker-icon-${response.id}`);
        if(marker == null || marker == undefined){
            return;
        }
        let circle = options.map.getPane('overlayPane')?.querySelectorAll(`.marker-circle-${response.id}`);
        if(circle != null && circle != undefined){
            circle.forEach(c => c.remove());
        }
        marker.remove();
        
        this.createMarker(response, options.map, options.signalR);
    }

    private createMarker(response: Marker, map: L.Map, signalR: SignalRService){
        let marker = new L.Marker([response.location.northing, response.location.easting], {
            icon: L.icon({
                iconUrl: `assets/icons/marker-${response.icon ?? 'default'}.png`,
                iconSize: [18, 18],
                className: 'marker-icon marker-icon-' + response.id
            })
        });
        marker.addTo(map);
        marker.bindPopup(`<b>${response.name}</b><br>${response.description}`);

        if(response.radius > 0){
            let circle = new L.Circle([response.location.northing, response.location.easting], {
                radius: response.radius,
                className: 'marker-circle marker-circle-' + response.id
            });
            circle.addTo(map);
        }

        let mark = signalR.resourceService.getMarkerById(response.id);
        if(mark != null){
            mark.marker.description = response.description;
            mark.marker.icon = response.icon;
            mark.marker.location = response.location;
            mark.marker.name = response.name;
            mark.marker.radius = response.radius;
        }
    }
}