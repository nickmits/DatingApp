import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { map } from "rxjs/operators";
import {JwtHelperService} from "@auth0/angular-jwt";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  baseUrl = "http://localhost:5000/api/auth/";
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  constructor(private http: HttpClient) {}

  login(dataToSend: any) {
    return this.http.post(this.baseUrl + "login", dataToSend).pipe(
      map((response: any) => {
        if (response) {
          localStorage.setItem("token", response.token);
          this.decodeToken(response.token); //this.decodedToken=this.jwtHelper.decodedToken(response.Token)
          console.log(this.decodedToken);
        }
      })
    );
  }

  register(dataToSend: any) {
    return this.http.post(this.baseUrl + "register", dataToSend);
  }

  decodeToken(token: any){
    this.decodedToken = this.jwtHelper.decodeToken(token);
  }

  loggedIn(){
    const token = localStorage.getItem("token");
    return !this.jwtHelper.isTokenExpired(token);
  }
}
