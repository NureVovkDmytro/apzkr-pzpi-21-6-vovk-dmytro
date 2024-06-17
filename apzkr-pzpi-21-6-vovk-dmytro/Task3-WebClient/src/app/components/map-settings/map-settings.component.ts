import { Component, Input } from '@angular/core';
import * as L from 'leaflet';
import { GeoCoordinates } from 'src/app/models/data/GeoCoordinates';
import { Group } from 'src/app/models/data/Group';
import { MapSector } from 'src/app/models/data/MapSector';
import { Marker } from 'src/app/models/data/Marker';
import { Alert, AlertsService } from 'src/app/services/alerts.service';
import { GroupApiService } from 'src/app/services/group-api.service';
import { MarkerApiService } from 'src/app/services/marker-api.service';
import { ResourceService } from 'src/app/services/resource.service';
import { SignalRService } from 'src/app/services/signal-r.service';

@Component({
  selector: 'app-map-settings',
  templateUrl: './map-settings.component.html',
  styleUrls: ['./map-settings.component.css']
})
export class MapSettingsComponent {

  @Input({ required: true })
  public map!: L.Map;

  @Input({ required: true })
  public meLocation!: L.Marker;

  @Input({ required: true })
  public config: MapProperties = {} as MapProperties;
  @Input({ required: true })
  public set init(value: boolean) {
    if (value) {
      this.onInit();
    }
  }

  isOpenMarker: boolean = false;
  public marker: Marker = new Marker();

  isOpenGroupList = false;
  isOpenAddMarker = false;
  canAddMarker = false;
  selectedGroup: Group | null = null;
  groups: GroupListItem[] = [];

  constructor(private groupService: GroupApiService, private markerService: MarkerApiService, private alertsService: AlertsService, private signalR: SignalRService, private resourceService: ResourceService) {
  }

  onInit(): void {
    this.map.addEventListener('move', (e: L.LeafletEvent) => {
      this.config.subscribeToMeLocation = false;
    });
    this.map.addEventListener('click', (e: L.LeafletMouseEvent) => {
      if (this.canAddMarker) {
        this.marker.location = { easting: e.latlng.lng, northing: e.latlng.lat, compass: 0 };
        this.isOpenMarker = true;
        L.DomUtil.removeClass(this.map.getContainer(), 'crosshair-cursor-enabled');
      }
    });
    this.groupService.getMyGroups().then(groups => {
      this.groups = groups.map(group => {
        const item = {
          item: group,
          checked: localStorage.getItem(`group-${group.id}-isShown`) === 'true',
          sector: new MapSector(this.map)
        };
        item.sector.visible = item.checked;
        item.sector.name = group.name;
        item.sector.addPoints(group.responsibilityArea.map(p => ({ lat: p.northing, lng: p.easting })));
        this.handleMarkerVisibility(item);
        return item;
      });
    });
  }

  toggleGroupList() {
    this.isOpenGroupList = !this.isOpenGroupList;
  }

  public toggleSubscribeToMeLocation() {
    this.config.subscribeToMeLocation = !this.config.subscribeToMeLocation;
    if (this.config.subscribeToMeLocation) {
      let zoom = this.map.getZoom();
      this.map.setView(this.meLocation.getLatLng(), zoom);
    }
  }

  public toggleGroup(group: GroupListItem) {
    group.checked = !group.checked;
    localStorage.setItem(`group-${group.item.id}-isShown`, group.checked.toString());
    group.sector.visible = group.checked;
    this.handleMarkerVisibility(group);
  }

  public openAddMarker() {
    this.isOpenAddMarker = true;
  }

  public closeAddMarker() {
    this.isOpenAddMarker = false;
    this.canAddMarker = false;
    this.selectedGroup = null;
    L.DomUtil.removeClass(this.map.getContainer(), 'crosshair-cursor-enabled');
  }

  public selectGroup(group: Group) {
    this.selectedGroup = group;
    this.marker.groupId = group.id;
  }

  public enableAddMarker() {
    this.canAddMarker = true;
    this.isOpenAddMarker = false;
    L.DomUtil.addClass(this.map.getContainer(), 'crosshair-cursor-enabled');
  }

  public saveMarker() {
    if (this.marker.id === '') {
      console.log('Add marker');
      this.markerService.createMarker(this.marker).then(marker => {
        this.marker = marker;
        this.isOpenMarker = false;
      }).catch(err => {
        this.alertsService.add(new Alert('danger', 'Failed to create marker. Please try again later.'));
        console.error(err);
      });
    } else {
      console.log('Edit marker');
    }
  }

  public closeMarker() {
    this.isOpenMarker = false;
    this.marker = new Marker();
  }

  private handleMarkerVisibility(group: GroupListItem) {
    let handlerName = group.checked ? 'markCreated' : 'markDeleted';
    console.log('handleMarkerVisibility', this.resourceService.getGroupById(group.item.id)?.markers);
    this.resourceService.getGroupById(group.item.id)?.markers.forEach(marker => {
      this.signalR.eventHandler.getHandler(handlerName)?.execute({
        map: this.map,
        signalR: this.signalR,
        request: marker
      });
    });
  }
}

export type MapProperties = {
  subscribeToMeLocation: boolean;
};

type GroupListItem = {
  item: Group;
  checked: boolean;
  sector: MapSector;
};