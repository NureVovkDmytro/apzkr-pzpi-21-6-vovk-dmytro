import { EventEmitter } from '@angular/core';
import * as L from "leaflet";
import GeometryTools from 'src/app/tools/GeometryTools';

export class MapSector {

    private map: L.Map;
    private _visible: boolean = true;
    private _supportEdit: boolean = false;

    public id: string = '';
    public name: string = '';
    public get supportEdit() {
        return this._supportEdit;
    }

    public set supportEdit(value: boolean) {
        this._supportEdit = value;
    }

    public onInside: EventEmitter<boolean> = new EventEmitter<boolean>();

    public points: MapSectorPoint[] = [];
    private get selectedPoint() {
        return this.points.find(p => p.selected);
    }
    private addMarker: L.Marker;
    public get hasSelectedPoint() {
        return this.selectedPoint != undefined;
    }

    public get visible() {
        return this._visible;
    }

    public set visible(value: boolean) {
        this._visible = value;
        if (this.polygon) {
            if (value) {
                this.polygon.addTo(this.map);
            } else {
                this.polygon.remove();
            }
        }
        this.points.forEach(p => {
            if (value) {
                p.marker.addTo(this.map);
            } else {
                p.marker.remove();
            }
        });
    }

    public get area() {
        if (this.polygon) {
            var area = GeometryTools.area(this.polygon.getLatLngs()[0]);
            return Math.round(area * 100) / 100;
        }
        return 0;
    }

    public polygon: L.Polygon | null = null;

    constructor(map: L.Map) {
        this.map = map;
        document.addEventListener('keydown', (e: KeyboardEvent) => this.handleKeyDown(e));
        // this.map.addEventListener('mousemove', (e: L.LeafletMouseEvent) => this.handleMouseMove(e));
        this.addMarker = L.marker([0, 0], {
            icon: L.icon({
                iconUrl: 'assets/icons/add.png',
                iconSize: [18, 18],
                className: 'polygon-point-icon'
            })
        });
    }

    public setMap(map: L.Map) {
        this.map = map;
    }

    public getSelectedPoint() {
        return this.selectedPoint;
    }

    public getPoints() {
        return this.points.map(p => ({ lat: p.lat, lng: p.lng }));
    }

    public addPoints(points: { lat: number, lng: number }[]) {
        points.forEach(p => {
            this.addPoint(new MapSectorPoint(p.lat, p.lng));
        });
    }

    public addPoint(point: MapSectorPoint) {
        this.points.pop();
        this.points.push(point);
        
        let firstPoint = this.points[0];
        this.points.push(firstPoint);

        if (this.visible) {
            point.marker.addTo(this.map);
        }
        point.marker.addEventListener('click', (e: L.LeafletMouseEvent) => {
            if (!this.supportEdit) {
                return;
            }
            if (this.selectedPoint?.id === point.id) {
                point.selected = !point.selected;
                return;
            }
            if (this.selectedPoint !== undefined) {
                this.selectedPoint.selected = false;
            }
            point.selected = true;
        });

        if (this.polygon) {
            // rm last point
            this.polygon.setLatLngs(this.points.map(p => [p.lat, p.lng]));
        } else {
            this.polygon = L.polygon([[point.lat, point.lng]]);
            if (this.visible) {
                this.polygon.addTo(this.map);
            }
        }
    }

    public removePoint(point: MapSectorPoint) {
        let index = this.points.indexOf(point);
        if (index >= 0) {
            this.points.splice(index, 1);
            if (this.polygon) {
                this.polygon.setLatLngs(this.points.map(p => [p.lat, p.lng]));
            }
        }
    }

    public popPoint() {
        if (this.points.length > 0) {
            let point = this.points.pop();
            if (point) {
                point.marker.remove();
            }
            if (this.polygon) {
                this.polygon.setLatLngs(this.points.map(p => [p.lat, p.lng]));
            }
            return point;
        }
        return null;
    }

    public clear() {
        this.points = [];
        if (this.polygon) {
            this.polygon.remove();
            this.polygon = null;
        }
    }

    public handleMapClick(e: L.LeafletMouseEvent) {
        if (!this.supportEdit) {
            return;
        }

        if (this.selectedPoint != undefined) {
            this.selectedPoint.marker.setLatLng(e.latlng);
            this.selectedPoint.lat = e.latlng.lat;
            this.selectedPoint.lng = e.latlng.lng;
            if (this.polygon) {
                this.polygon.setLatLngs(this.points.map(p => [p.lat, p.lng]));
                console.log('polygon updated', this.polygon.getLatLngs());
            }
            this.selectedPoint.selected = false;
        }
    }

    private handleKeyDown(e: KeyboardEvent) {
        if (!this.supportEdit) {
            return;
        }
        if (this.selectedPoint == undefined) {
            return;
        }

        let speed = 0.0001;
        if (e.shiftKey) {
            speed = 0.001;
        }
        if (e.ctrlKey) {
            speed = 0.00001;
        }

        // arrow up
        if (e.code === 'ArrowUp') {
            this.selectedPoint.lat += speed;
            this.selectedPoint.marker.setLatLng([this.selectedPoint.lat, this.selectedPoint.lng]);
            if (this.polygon) {
                this.polygon.setLatLngs(this.points.map(p => [p.lat, p.lng]));
            }
        }

        // arrow down
        if (e.code === 'ArrowDown') {
            this.selectedPoint.lat -= speed;
            this.selectedPoint.marker.setLatLng([this.selectedPoint.lat, this.selectedPoint.lng]);
            if (this.polygon) {
                this.polygon.setLatLngs(this.points.map(p => [p.lat, p.lng]));
            }
        }

        // arrow left
        if (e.code === 'ArrowLeft') {
            this.selectedPoint.lng -= speed;
            this.selectedPoint.marker.setLatLng([this.selectedPoint.lat, this.selectedPoint.lng]);
            if (this.polygon) {
                this.polygon.setLatLngs(this.points.map(p => [p.lat, p.lng]));
            }
        }

        // arrow right
        if (e.code === 'ArrowRight') {
            this.selectedPoint.lng += speed;
            this.selectedPoint.marker.setLatLng([this.selectedPoint.lat, this.selectedPoint.lng]);
            if (this.polygon) {
                this.polygon.setLatLngs(this.points.map(p => [p.lat, p.lng]));
            }
        }
    }

    private handleMouseMove(e: L.LeafletMouseEvent) {
        if (this.polygon) {
            // if mouse is near to the polygon edge, show add marker in the middle of the edge
            let latlngs = this.polygon.getLatLngs()[0] as L.LatLng[];
            let onEdge = false;
            for (let i = 1; i < latlngs.length; i++) {
                let j = (i + 2) % latlngs.length;
                let point1 = latlngs[i];
                let point2 = latlngs[j];
                let edge = L.latLngBounds(point1, point2);
                if (edge.contains(e.latlng)) {
                    onEdge = true;
                    this.addMarker.setLatLng(edge.getCenter());
                    if (!this.map.hasLayer(this.addMarker)) {
                        this.addMarker.addTo(this.map);
                    }
                    return;
                }
            }
            if (!onEdge) {
                this.map.removeLayer(this.addMarker);
            }
        }
    }
}

export class MapSectorPoint {
    private _selected: boolean = false;

    public id: string;
    public lat: number;
    public lng: number;

    public marker: L.Marker;

    public get selected() {
        return this._selected;
    }

    public set selected(value: boolean) {
        this._selected = value;
        if (value) {
            this.marker.getElement()?.classList.add('selected');
        } else {
            this.marker.getElement()?.classList.remove('selected');
        }
    }

    constructor(lat: number, lng: number) {
        this.lat = lat;
        this.lng = lng;
        this.id = `polygon-point_${lat},${lng}`;
        this.marker = L.marker([lat, lng], {
            icon: L.icon({
                iconUrl: 'assets/icons/point.png',
                iconSize: [16, 16],
                className: 'polygon-point-icon'
            })
        });
    }
}