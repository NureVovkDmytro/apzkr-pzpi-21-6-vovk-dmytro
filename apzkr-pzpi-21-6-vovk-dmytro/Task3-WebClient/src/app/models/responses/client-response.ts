import { ClientStatus, ClientType } from "../data/Client";
import { ClientPermissions } from "../data/ClientPermission";

export class ClientResponse {
    public id: string = '';
    public firstName: string | undefined;
    public lastName: string | undefined;
    public nickname: string | undefined;
    public email: string | undefined;
    public taxPayerId: string | undefined;
    public createdAt: Date = new Date();
    public lastOnline: Date = new Date();
    public type: ClientType = ClientType.User;
    public status: ClientStatus = ClientStatus.Active;
    public accessLevel: number = 0;
    public permissions: ClientPermissions = new ClientPermissions();
}