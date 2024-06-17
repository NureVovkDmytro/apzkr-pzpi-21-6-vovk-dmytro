import { Injectable } from "@angular/core";
import { Group } from "../models/data/Group";
import { Marker } from "../models/data/Marker";
import { ProxyArray } from "../models/data/ProxyArray";
import { ProxyMap } from "../models/data/ProxyMap";

@Injectable({
    providedIn: 'root'
})
export class ResourceService {
    private _groups: ProxyMap<string, Group> = new ProxyMap<string, Group>();
    private _markers: ProxyMap<string, Marker> = new ProxyMap<string, Marker>();
    private _markerByGroup: ProxyMap<string, string[]> = new ProxyMap<string, string[]>();
    private _groupResources: GroupResource[] = [];
    private _markerResources: MarkerResource[] = [];

    public get groups(): ProxyMap<string, Group> {
        return this._groups;
    }

    public get markers(): ProxyMap<string, Marker> {
        return this._markers;
    }

    public set groups(groups: ProxyMap<string, Group> | Group[]) {
        this._groups.clear();
        if (groups instanceof Array) {
            this._groups = new ProxyMap<string, Group>(groups.map(group => [group.id, group]));
        } else {
            this._groups = groups;
        }
        this._markerByGroup.clear();
        this._groupResources = [];

        for (const group of groups) {
            const id = (group instanceof Array) ? group[0] : group.id;
            const entity = (group instanceof Array) ? group[1] : group;
            console.log('ResourceService.set groups', group);

            this._markerByGroup.set(id, entity.markers);
            this._groupResources.push({
                group: entity,
                markers: this._markerByGroup.get(id)?.map(markerId => this._markers.get(markerId) as Marker) ?? ([] as Marker[])
            });
        }

        this.initGroupHandlers();
    }

    public set markers(markers: ProxyMap<string, Marker> | Marker[]) {
        this._markers.clear();
        if (markers instanceof Array) {
            this._markers = new ProxyMap<string, Marker>(markers.map(marker => [marker.id, marker]));
        } else {
            this._markers = markers;
        }
        this._markerResources = [];

        for (const marker of markers) {
            const entity = (marker instanceof Array) ? marker[1] : marker;
            console.log('ResourceService.set markers', entity);

            this._markerResources.push({
                marker: entity,
                group: this._groups.get(entity.groupId)
            });
        }

        this.initMarkerHandlers();
    }

    public get groupResources(): GroupResource[] {
        return this._groupResources;
    }

    public get markerResources(): MarkerResource[] {
        return this._markerResources;
    }

    public getGroupById(groupId: string): GroupResource | undefined {
        return this._groupResources.find(group => group.group.id == groupId);
    }

    public getMarkerById(markerId: string): MarkerResource | undefined {
        return this._markerResources.find(marker => marker.marker.id == markerId);
    }

    constructor() {
        console.log('ResourceService.constructor');
        (document as any)['resourceReport'] = this.resourceReport.bind(this);
    }

    private initMarkerHandlers() {
        this._markers.on('set', (key: string, value: Marker, map: ProxyMap<string, Marker>) => {
            let markerResource = this._markerResources.find(marker => marker.marker.id == value.id);
            if (markerResource != undefined) {
                markerResource.group = this._groups.get(value.groupId);
                markerResource.marker = value;
            } else {
                this._markerResources.push({
                    marker: value,
                    group: this._groups.get(value.groupId)
                });
            }
            let groupResource = this._groupResources.find(group => group.group.id == value.groupId);
            if (groupResource != undefined) {
                groupResource.markers.push(value);
            } else {
                let group = this._groups.get(value.groupId);
                if (group == null) {
                    group = new Group();
                    group.id = value.groupId;
                    group.name = 'Unknown';
                    this._groups.set(group.id, group);
                }
                this._groupResources.push({
                    group: group,
                    markers: [value]
                });
            }
        });

        this._markers.on('delete', (key: string, map: ProxyMap<string, Marker>) => {
            let markerResource = this._markerResources.find(marker => marker.marker.id == key);
            if (markerResource != null) {
                let index = this._markerResources.indexOf(markerResource);
                this._markerResources.splice(index, 1);
            }

            let marker = markerResource?.marker as Marker;
            if (!marker) {
                return;
            }

            let groupResource = this._groupResources.find(group => group.group.id == marker.groupId);
            if (groupResource != null) {
                let index = groupResource.markers.indexOf(marker);
                groupResource.markers.splice(index, 1);
            }
        });

        this._markers.on('clear', (map: ProxyMap<string, Marker>) => {
            this._markerResources = [];
            this._groupResources.forEach(groupResource => {
                groupResource.markers = [];
            });
        });
    }

    private initGroupHandlers() {
        this._groups.on('set', (key: string, value: Group, map: ProxyMap<string, Group>) => {
            let groupResource = this._groupResources.find(group => group.group.id == value.id);
            if (groupResource != undefined) {
                groupResource.group = value;
            } else {
                let markersIds = this._markerByGroup.get(value.id);
                let markers = markersIds?.map(markerId => this._markers.get(markerId) as Marker) ?? [];
                this._groupResources.push({
                    group: value,
                    markers: markers
                });
            }
        });

        this._groups.on('delete', (key: string, map: ProxyMap<string, Group>) => {
            let groupResource = this._groupResources.find(group => group.group.id == key);
            if (groupResource != null) {
                let index = this._groupResources.indexOf(groupResource);
                this._groupResources.splice(index, 1);
            }
        });

        this._groups.on('clear', (map: ProxyMap<string, Group>) => {
            this._groupResources = [];
        });
    }

    private resourceReport() {
        console.log('ResourceService.resourceReport\nGroups:', this._groups,
            '\nMarkers:', this._markers,
            '\nGroupResources:', this._groupResources,
            '\nMarkerResources:', this._markerResources);
    }
}

export type GroupResource = {
    group: Group;
    markers: Marker[];
}

export type MarkerResource = {
    marker: Marker;
    group: Group | undefined;
}