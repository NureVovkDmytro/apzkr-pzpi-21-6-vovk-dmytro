import { SignalRService } from "src/app/services/signal-r.service";
import { BaseHandler, MarkCreatedHandler, MarkDeletedHandler, MarkUpdatedHandler } from "./handlers";

export class MapSignalREvents {
    private map!: L.Map;
    private signalR: SignalRService;
    private handlers: BaseHandler[] = [];

    constructor(signalR: SignalRService) {
        this.signalR = signalR;
    }

    public setMap(map: L.Map) {
        this.map = map;
    }

    public init(){
        this.handlers.push(new MarkCreatedHandler());
        this.handlers.push(new MarkUpdatedHandler());
        this.handlers.push(new MarkDeletedHandler());

        this.registerHandlers();
    }

    public getHandler(command: string): BaseHandler | undefined {
        return this.handlers.find(handler => handler.command === command);
    }

    private registerHandlers(){
        this.handlers.forEach(handler => {
            this.signalR.on(handler.command, (request) => {
                handler.execute({
                    signalR: this.signalR,
                    map: this.map,
                    request: request
                });
            });
        });
    }
}