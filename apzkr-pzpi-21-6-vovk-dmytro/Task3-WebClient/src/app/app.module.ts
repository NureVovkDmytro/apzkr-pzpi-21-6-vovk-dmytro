import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { MapComponent } from './components/map/map.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AuthGuard } from './auth.guard';
import { TableComponent } from './components/table/table.component';
import { NavBarComponent } from './components/nav-bar/nav-bar.component';
import { UserSettingsComponent } from './components/user-settings/user-settings.component';
import { UserListAdministrationComponent } from './components/admin/users/user-list-administration/user-list-administration.component';
import { PermissionGuard } from './permission.guard';
import { UserAdministrationComponent } from './components/admin/users/user-administration/user-administration.component';
import { NgxTranslateModule } from './translate.module';
import { ActivationPageComponent } from './components/activation-page/activation-page.component';
import { ViewGroupComponent } from './components/group/view-group/view-group.component';
import { GroupListComponent } from './components/group/group-list/group-list.component';
import { GroupMemberListComponent } from './components/group/group-member-list/group-member-list.component';
import { ConfirmPageComponent } from './components/confirm-page/confirm-page.component';
import { MapSettingsComponent } from './components/map-settings/map-settings.component';
import { MapSettingsToolBoxComponent } from './components/map-settings-tool-box/map-settings-tool-box.component';
import { ModalBoxComponent } from './components/modal-box/modal-box.component';
import { NotificationsComponent } from './components/notifications/notifications.component';
import { NgbAlertModule, NgbModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { AlertsService } from './services/alerts.service';
import { AuthApiService } from './services/auth-api.service';
import { GroupApiService } from './services/group-api.service';
import { MarkerApiService } from './services/marker-api.service';
import { UserApiService } from './services/user-api.service';
import { ResourceService } from './services/resource.service';
import { SignalRService } from './services/signal-r.service';
import { RobotListAdministrationComponent } from './components/admin/robots/robot-list-administration/robot-list-administration.component';
import { RobotAdministrationComponent } from './components/admin/robots/robot-administration/robot-administration.component';

@NgModule({
  declarations: [
    AppComponent,
    ModalBoxComponent,
    NotificationsComponent,
    LoginComponent,
    HomeComponent,
    MapComponent,
    MapSettingsToolBoxComponent,
    MapSettingsComponent,
    TableComponent,
    NavBarComponent,
    UserSettingsComponent,
    UserListAdministrationComponent,
    UserAdministrationComponent,
    RobotListAdministrationComponent,
    RobotAdministrationComponent,
    ActivationPageComponent,
    ViewGroupComponent,
    GroupListComponent,
    GroupMemberListComponent,
    ConfirmPageComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    AppRoutingModule,
    NgxTranslateModule,
    NgbModule,
    NgbPaginationModule, NgbAlertModule
  ],
  providers: [AuthGuard, PermissionGuard, AlertsService, ResourceService, SignalRService,
    AuthApiService, GroupApiService, MarkerApiService, UserApiService],
  bootstrap: [AppComponent]
})
export class AppModule { }
