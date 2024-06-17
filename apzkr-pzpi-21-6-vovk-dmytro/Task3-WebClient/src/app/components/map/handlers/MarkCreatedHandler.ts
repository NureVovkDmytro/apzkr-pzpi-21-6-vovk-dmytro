import { Marker } from "src/app/models/data/Marker";
import { BaseHandler, HandlerOptions } from "./BaseHandler";
import * as L from 'leaflet';

export class MarkCreatedHandler extends BaseHandler {
    public get command(): string {
        return 'markCreated';
    }
    
    public execute(options: HandlerOptions): void {
        console.debug('MarkCreatedHandler.execute', options);
        let response = options.request as Marker;
        let marker = new L.Marker([response.location.northing, response.location.easting], {
            icon: L.icon({
                iconUrl: `assets/icons/marker-${response.icon ?? 'default'}.png`,
                iconSize: [18, 18],
                className: 'marker-icon marker-icon-' + response.id
            })
        });
        marker.addTo(options.map);
        marker.bindPopup(`<b>${response.name}</b><br>${response.description}`);
        marker.on('click', () => {
            marker.openPopup();
        });

        if(response.radius > 0){
            let circle = new L.Circle([response.location.northing, response.location.easting], {
                radius: response.radius,
                className: 'marker-circle marker-circle-' + response.id
            });
            circle.addTo(options.map);
        }

        let signalR = options.signalR;
        let resourceService = signalR.resourceService;
        resourceService.markers.set(response.id, response);
    }
}