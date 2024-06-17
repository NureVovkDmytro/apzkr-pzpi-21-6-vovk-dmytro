import { Component, OnInit, AfterViewInit, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as L from 'leaflet';
import { MapSector, MapSectorPoint } from 'src/app/models/data/MapSector';
import { User } from 'src/app/models/data/User';
import { SignalRService } from 'src/app/services/signal-r.service';
import { UserApiService } from 'src/app/services/user-api.service';
import { MapProperties } from '../map-settings/map-settings.component';
import { MapSignalREvents } from './map.signalrEvents';
import { GroupApiService } from 'src/app/services/group-api.service';
import { MarkerApiService } from 'src/app/services/marker-api.service';
import { Group } from 'src/app/models/data/Group';
import { UserResponseDetailed } from 'src/app/models/responses/user-response';

type Marker = { [key: string]: L.Marker };

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit, AfterViewInit {
  private debug = {
    enabled: false,
    coords: {
      lat: 0,
      lng: 0
    }
  };

  public confirmBox = {
    visible: false,
    title: 'map.confirmBox.title',
    message: 'map.confirmBox.message'
  };

  public returnTo = '';

  public user: UserResponseDetailed = new UserResponseDetailed();
  public init: boolean = false;
  public map!: L.Map;
  public mode: 'select' | 'view' = 'view';
  public type: 'area' | 'point' = 'point';
  public meLocation: L.Marker = L.marker([0, 0], {
    icon: L.icon({
      iconUrl: 'assets/icons/navigation-48.png',
      iconSize: [18, 18],
      className: 'me-location-icon'
    })
  });
  public compass: number = 0;
  nearClients: Marker = {};

  public config: MapProperties = {
    subscribeToMeLocation: false
  };

  newSector!: MapSector;

  constructor(private route: ActivatedRoute,
    private router: Router,
    private signalR: SignalRService,
    private userService: UserApiService,
    private groupService: GroupApiService,
    private markerService: MarkerApiService) {

    this.userService.getSelfDetailed().then(user => {
      this.user = user;
    });

    this.route.queryParams.subscribe(params => {
      this.returnTo = params['returnTo'];
      let mode = params['mode'];
      if (mode === 'select') {
        this.mode = 'select';
        this.confirmBox.visible = true;
      }
      let type = params['type'];
      if (type === 'area') {
        this.type = 'area';
      }

      let debug = params['debug'];
      if (debug === 'true') {
        this.debug.enabled = true;
        this.debug.coords.lat = params['lat'] || 50;
        this.debug.coords.lng = params['lng'] || 36;
      }
    });
  }

  ngOnInit() {
  }

  async ngAfterViewInit() {
    await this.signalR.connect();
    console.log('ngAfterViewInit');
    this.signalR.on('NearClients', this.handleNearClients.bind(this));
    this.signalR.on('LocationUpdated', this.handleLocationUpdated.bind(this));

    this.initializeMap();
    this.centerMap();
    this.signalR.eventHandler.setMap(this.map);
    this.signalR.eventHandler.init();

    this.groupService.getMyGroups().then(groups => {
      this.loadMarkers(groups);
    });

    this.getMeLocation();

    this.map.addEventListener('click', this.handleMouseClick.bind(this));
    this.map.addEventListener('keydown', this.handleKeyDown.bind(this));

  }

  public confirm() {
    if (this.returnTo) {
      this.router.navigate([this.returnTo], {
        state: {
          event: 'map.confirm',
          sector: this.newSector.getPoints()
        }
      });
      return;
    }

    this.confirmBox.visible = false;
  }

  public cancel() {
    if (this.returnTo) {
      console.log(this.returnTo);
      this.router.navigate([this.returnTo]);
      return;
    }

    this.confirmBox.visible = false;
    this.mode = 'view';
  }

  private initializeMap() {
    const baseMapURl = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png'
    // const baseMapURl = 'https://localhost:7223/{z}/{x}/{y}.png'
    this.map = L.map('map');
    L.tileLayer(baseMapURl).addTo(this.map);
    this.map.zoomControl.setPosition('bottomright');
    this.map.setMinZoom(3);

    this.newSector = new MapSector(this.map);
    this.newSector.supportEdit = true;
  }

  private centerMap() {
    this.map.setView(this.meLocation.getLatLng(), 13);
  }

  private getMeLocation() {
    this.meLocation.addTo(this.map);
    this.meLocation.bindPopup(`<b>${this.user.nickname}</b>`);
    this.meLocation.setLatLng([this.user.location.northing, this.user.location.easting])
    let zoom = this.map.getZoom();
    this.map.setView(this.meLocation.getLatLng(), zoom);
    this.init = true;
  }

  private handleNearClients(nearClients: NearClient[]) {
    Object.keys(this.nearClients).forEach(key => {
      if (nearClients.find(nearClient => nearClient.id === key) === undefined) {
        this.nearClients[key].removeFrom(this.map);
        delete this.nearClients[key];
      }
    });

    nearClients.forEach(nearClient => {
      let marker = this.nearClients[nearClient.id];

      if (this.user.id === nearClient.id) {
        return;
      }

      if (marker !== undefined) {
        marker.removeFrom(this.map);
        marker.setLatLng([nearClient.northing, nearClient.easting]);
        marker.addTo(this.map);
      } else {
        let markerNew = L.marker([nearClient.northing, nearClient.easting], {
          icon: L.icon({
            iconUrl: 'assets/icons/navigation-48.png',
            iconSize: [18, 18],
            className: 'near-client-icon'
          })
        });
        markerNew.bindPopup(`<b>${nearClient.nickname}</b><br>${nearClient.updateAt}`);
        markerNew.addTo(this.map);
        this.nearClients[nearClient.id] = markerNew;
      }
      // rotate marker
      let element = this.nearClients[nearClient.id].getElement();
      if (element) {
        element.style.transformOrigin = 'center';
        let parts = element.style.transform.match(/translate3d\(([\d-]+)px, ([\d-]+)px, ([\d-]+)px\)/);
        element.style.transform = `${parts?.[0]} rotate(${nearClient.compass}deg)`;
      }
    });
  }

  private handleLocationUpdated(selfPos: NearClient) {
    console.log('LocationUpdated', selfPos);
    this.meLocation.setLatLng([selfPos.northing, selfPos.easting]);
    if (this.config.subscribeToMeLocation) {
      let zoom = this.map.getZoom();
      this.map.setView(this.meLocation.getLatLng(), zoom);
    }

    let element = this.meLocation.getElement();
    if (element) {
      element.style.transformOrigin = 'center';
      let parts = element.style.transform.match(/translate3d\(([\d-]+)px, ([\d-]+)px, ([\d-]+)px\)/);
      element.style.transform = `${parts?.[0]} rotate(${selfPos.compass}deg)`;
    }
  }

  private handleMouseClick(e: L.LeafletMouseEvent) {
    if (this.mode === 'select') {
      if (this.type === 'point') {
        // TODO: Add point
      } else {
        if (this.newSector.hasSelectedPoint == false) {
          console.log(this.newSector.getSelectedPoint());
          this.newSector.addPoint(new MapSectorPoint(e.latlng.lat, e.latlng.lng));
        }
      }
    }
    this.newSector.handleMapClick(e);
  }

  private handleKeyDown(e: L.LeafletKeyboardEvent) {
    // Ctrl + Z
    if (e.originalEvent.ctrlKey && e.originalEvent.code === 'KeyZ') {
      if (this.type === 'area') {
        if (this.newSector !== null) {
          this.newSector.popPoint();
        }
      }
    }
  }

  private loadMarkers(groups: Group[]) {
    groups.forEach(group => {
      this.markerService.getMarkersForGroup(group.id);
    });
  }
}

type NearClient = {
  id: string;
  nickname: string;
  updateAt: Date;
  easting: number;
  northing: number;
  compass: number;
}
