import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { AuthApiService } from 'src/app/services/auth-api.service';
import { Alert, AlertsService } from 'src/app/services/alerts.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit{
  public email: string = "";
  public password: string = "";

  constructor(private authApi: AuthApiService, private router: Router, private alertsService: AlertsService){}
  ngOnInit(): void {
    if(this.authApi.isAuthenticatedUser){
      this.router.navigate(['/home']);
    }
  }

  public onLoginKeyDown(event: KeyboardEvent){
    if(event.key === 'Enter'){
      this.onLogin();
    }
  }

  public onLogin(){
    this.authApi.login({email: this.email, password: this.password})
      .then(response => {
        if(response.ok){
          this.router.navigate(['/home']);
        } else {
          this.alertsService.add(new Alert('danger', 'login.error'));
        }
      }).catch(() => {
        this.alertsService.add(new Alert('danger', 'login.error'));
      });
  }
}
