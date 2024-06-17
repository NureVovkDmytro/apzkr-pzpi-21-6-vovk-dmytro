import { Marker } from "src/app/models/data/Marker";
import { BaseHandler, HandlerOptions } from "./BaseHandler";

export class MarkDeletedHandler extends BaseHandler {
    public get command(): string {
        return 'markDeleted';
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

        let signalR = options.signalR;
        let resourceService = signalR.resourceService;
        resourceService.markers.delete(response.id);
    }
}