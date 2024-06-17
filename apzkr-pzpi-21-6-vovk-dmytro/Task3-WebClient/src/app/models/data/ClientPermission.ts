export class ClientPermissions {
    public users: UsersInteractionPermissions = new UsersInteractionPermissions();
    public robots: RobotsInteractionPermissions = new RobotsInteractionPermissions();
    public groups: GroupsInteractionPermissions = new GroupsInteractionPermissions();
}

export class UsersInteractionPermissions{
    public canDelete: boolean = false;
    public canUpdateSelfEmail: boolean = false;
    public canUpdateBaseInformation: boolean = false;
    public canResetPassword: boolean = false;
    public canUpdateStatus: boolean = false;
    public canUpdateAccessLevel: boolean = false;
    public canUpdateScanRadius: boolean = false;
    public canUpdatePermissions: boolean = false;
    public canRead: boolean = false;
    public canCreate: boolean = false;
    public canCreateDeviceToken: boolean = false;
}

export class RobotsInteractionPermissions{
    public canRead: boolean = false;
    public canCreate: boolean = false;
    public canUpdate: boolean = false;
    public canUpdateStatus: boolean = false;
    public canUpdateScanRadius: boolean = false;
    public canUpdateAccessLevel: boolean = false;
    public canDelete: boolean = false;
}

export class GroupsInteractionPermissions{
    public canRead: boolean = false;
    public canCreate: boolean = false;
    public canUpdate: boolean = false;
    public canDelete: boolean = false;
}