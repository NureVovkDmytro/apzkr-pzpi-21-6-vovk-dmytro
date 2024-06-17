import { SignalRService } from "src/app/services/signal-r.service";

export type HandlerOptions = {
    signalR: SignalRService;
    map: L.Map;
    request: any;
};

export abstract class BaseHandler {
    public abstract get command(): string;
    public abstract execute(options: HandlerOptions): void;
}