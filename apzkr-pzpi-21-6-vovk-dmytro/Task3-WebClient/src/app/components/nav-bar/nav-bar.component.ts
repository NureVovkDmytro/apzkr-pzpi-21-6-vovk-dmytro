import { Component, Input, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { UserResponse } from 'src/app/models/responses/user-response';
import { AuthApiService } from 'src/app/services/auth-api.service';
import { UserApiService } from 'src/app/services/user-api.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent implements OnInit {

  private _user: UserResponse = new UserResponse();

  @Input()
  public isFixed: boolean = false;

  public get isAuthenticated(): boolean {
    return this.authService.isAuthenticatedUser;
  }

  public get user() {
    return this._user;
  }

  public get userFullName() {
    return `${this.user.firstName} ${this.user.lastName}`;
  }

  public get languageList() {
    return environment.languageList;
  }
  public get currentLanguage() {
    let lang = environment.languageList.find(l => l.code == this.translate.currentLang);
    if (lang) {
      return lang;
    }
    return environment.languageList.find(l => l.code == this.translate.defaultLang) || environment.languageList[0];
  }

  constructor(private translate: TranslateService, private authService: AuthApiService, private userService: UserApiService) { }
  ngOnInit(): void {
    if (this.isAuthenticated) {
      this.getUser();
    }
    let lang = localStorage.getItem('language');
    if (lang) {
      this.translate.use(lang);
    }
  }

  public logout() {
    this.authService.logout();
  }

  private getUser() {
    this.userService.getSelf().then(user => this._user = user);
  }

  changeLanguage(language: string): void {
    this.translate.use(language);
    localStorage.setItem('language', language);
  }
}
