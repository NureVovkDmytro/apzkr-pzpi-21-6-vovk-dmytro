import { Injectable } from '@angular/core';
import { BaseApi } from './base-api';
import { Marker } from '../models/data/Marker';
import { ResourceService } from './resource.service';

@Injectable({
  providedIn: 'root'
})
export class MarkerApiService extends BaseApi {
  public override get apiUrl(): string {
    return '/mark'
  }

  constructor(private resourceService: ResourceService){
    super();
  }

  public getMarkersForGroup(groupId: string): Promise<Marker[]> {
    return this.toDataResponse(this.get<Marker[]>(`group/${groupId}`)).then(markers => {
      markers.forEach(marker => {
        this.resourceService.markers.set(marker.id, marker);
      });
      return markers;
    });
  }

  public getMarkerById(markerId: string): Promise<Marker> {
    return this.toDataResponse(this.get<Marker>(`/${markerId}`));
  }

  public createMarker(marker: Marker): Promise<Marker> {
    return this.toDataResponse(this.post<Marker>('', marker));
  }

  public updateMarker(marker: Marker): Promise<Marker> {
    return this.toDataResponse(this.put<Marker>(`/${marker.id}`, marker));
  }

  public deleteMarker(markerId: string): Promise<void> {
    return this.toDataResponse(this.delete<void>(`/${markerId}`));
  }
}
