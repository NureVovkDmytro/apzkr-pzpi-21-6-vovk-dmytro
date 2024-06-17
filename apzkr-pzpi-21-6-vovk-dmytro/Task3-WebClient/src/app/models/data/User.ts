import { UserResponse, UserResponseDetailed } from "../responses/user-response";
import { Client } from "./Client";

export class User extends Client {
    firstName: string = '';
    lastName: string = '';
    taxPayerId: string = '';
    email: string = '';
    password: string = '';
    needPasswordChange: boolean = true;
    updateLocationSecondsInterval: number = 0;

    static fromResponse(response: UserResponseDetailed | UserResponse): User {
        let user = new User();
        user.id = response.id;
        user.firstName = response.firstName;
        user.lastName = response.lastName;
        user.nickname = response.nickname;
        user.email = response.email;
        user.taxPayerId = response.taxPayerId;
        user.createdAt = response.createdAt;
        user.type = response.type;
        user.permissions = response.permissions;
        user.status = response.status;
        user.updateLocationSecondsInterval = response.updateLocationSecondsInterval;
        let detailedResponse = response as UserResponseDetailed;
        user.groups = detailedResponse.groups || [];
        user.lastOnline = detailedResponse.lastOnline || undefined;
        user.accessLevel = detailedResponse.accessLevel || 0;
        user.scanRadius = detailedResponse.scanRadius || 0;
        user.description = detailedResponse.description || '';

        return user;
    }
}