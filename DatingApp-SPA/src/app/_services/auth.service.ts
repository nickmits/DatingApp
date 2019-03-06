import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import {BehaviorSubject} from 'rxjs';
import { map } from "rxjs/operators";
import {JwtHelperService} from "@auth0/angular-jwt";
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: "root"
})
export class AuthService {
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {}

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login(dataToSend: any) {
    return this.http.post(environment.apiUrlLogin, dataToSend).pipe(
      map((response: any) => {
        if (response) {
          localStorage.setItem("token", response.token);
          localStorage.setItem('user', JSON.stringify(response.user))
          this.decodeToken(response.token); //this.decodedToken=this.jwtHelper.decodedToken(response.Token)
          this.currentUser = response.user;
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  register(dataToSend: any) {
    return this.http.post(environment.apiUrlRegister, dataToSend);
  }

  decodeToken(token: any){
    this.decodedToken = this.jwtHelper.decodeToken(token);
  }

  loggedIn(){
    const token = localStorage.getItem("token");
    return !this.jwtHelper.isTokenExpired(token);
  }
}
